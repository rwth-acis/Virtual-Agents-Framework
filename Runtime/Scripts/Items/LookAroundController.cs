using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace i5.VirtualAgents
{
    public class ItemInfo
    {
        public Item item = null;
        public float distance = 0;
        public float importance = 0;
        public float timeLookedAt = 0;
        public float novelty = 0;
        public float calcValueOfInterest = 0;
        public bool isCurrentlyNearby = false;
    }


    public class LookAroundController : MonoBehaviour
    {


        public float detectionRadius = 10f;
        public int maxNumberOfItemsInRange = 50;

        public float detectionIntervalWhenIdle = 3f;
        public float detectionIntervalWhenWalking = 0.5f;
        private float detectionInterval = 3f;



        //Define the chances for item selection, e.g. chance for the second most interesting item in the list to be looked at
        [Range(0f, 1f)]
        public float chanceFirstItem = 0.5f;
        [Range(0f, 1f)]
        public float chanceSecondItem = 0.1f;
        [Range(0f, 1f)]
        public float chanceThirdItem = 0.1f;
        [Range(0f, 1f)]
        public float chanceRandomItem = 0.05f;
        [Range(0f, 1f)]
        public float chanceIdealTime = 0.25f;



        public float lookSpeed = 2f;

        ItemInfo currentItemOfInterest;

        public LayerMask seeLayers;
        public LayerMask occlusionLayers = default;

        private List<ItemInfo> nearbyItems = new List<ItemInfo>();
        private float timer = 0f;
        [Range(0f, 1f)]
        private float maxWeight = 0.8f;

        private AimAtSomething aimScript;

        // Start is called before the first frame update
        void Start()
        {

            aimScript = this.gameObject.AddComponent<AimAtSomething>();

            aimScript.SetBonePreset("Head");
            aimScript.SetShouldDestroyItself(false);
            aimScript.lookSpeed = lookSpeed;

            //Normalize chances
            float sum = chanceFirstItem + chanceSecondItem + chanceThirdItem + chanceRandomItem + chanceIdealTime;
            chanceFirstItem /= sum;
            chanceSecondItem /= sum;
            chanceThirdItem /= sum;
            chanceRandomItem /= sum;
            chanceIdealTime /= sum;
            chanceSecondItem += chanceFirstItem;
            chanceThirdItem += chanceSecondItem;
            chanceRandomItem += chanceThirdItem;
            chanceIdealTime += chanceRandomItem;
            if (chanceIdealTime != 1f)
            {
                Debug.LogWarning("Normalisation went wrong");
            }

        }
        public void Activate()
        {
            if (aimScript != null)
                aimScript.enabled = true;

            this.enabled = true;
        }
        public void Deactivate()
        {
            if (aimScript != null)
                aimScript.enabled = false;

            this.enabled = false;
        }

        //If changes are made to the lookSpeed in the inspector, update the aim script
        private void OnValidate()
        {
            if (aimScript != null)
                aimScript.lookSpeed = lookSpeed;
        }

        void Update()
        {
            //Check if the agent is walking
            AdjustIntervalBasedOnWalkingSpeed();

            //Every second check which items are nearby and invoke the function to calculate the most interesting item
            CheckWitchItemsAreNearbyAndSeeable();

            //Position of the Target is updated every frame in cases where it moves 
            UpdatePositionOfTarget();

        }
        private void AdjustIntervalBasedOnWalkingSpeed()
        {
            if (GetComponent<UnityEngine.AI.NavMeshAgent>().velocity.magnitude > 0.1f)
            {
                detectionInterval = detectionIntervalWhenWalking;
            }
            else
            {
                detectionInterval = detectionIntervalWhenIdle;
            }
        }

        private void UpdatePositionOfTarget()
        {
            if (currentItemOfInterest != null)
            {
                aimScript.SetTargetTransform(currentItemOfInterest.item.transform);
            }
            else
            {
                aimScript.Stop();
            }
        }
        private void CheckWitchItemsAreNearbyAndSeeable()
        {
            timer += Time.deltaTime;
            if (timer < detectionInterval)
            {
                return;
            }
            timer = 0f;

            // Old nearby items are marked as not nearby
            foreach (ItemInfo itemInfo in nearbyItems)
            {
                itemInfo.isCurrentlyNearby = false;
            }


            // Check for nearby items
            Collider[] colliders = new Collider[maxNumberOfItemsInRange];
            //center is calcualted so that is cube is corner is at the position of the agent
            Vector3 center = transform.position + transform.forward * Mathf.Sqrt(2 * detectionRadius * detectionRadius);

            Vector3 halfExtents = new Vector3(detectionRadius, 2, detectionRadius);
            Quaternion rotation = transform.rotation * Quaternion.Euler(0, 45, 0);

            int count = Physics.OverlapBoxNonAlloc(center, halfExtents, colliders, rotation, seeLayers);
            //Use Window > Analysis > Physics Debug > Queries to see the detection radius, decrease detection Interval to see the cube on every frame

            for (int i = 0; i < count; i++)
            {
                Item item = colliders[i].GetComponent<Item>();
                //Check that the object has an item component and is not picked up
                if (item == null || item.isPickedUp)
                {
                    continue;
                }
                //Check if the item is in sight
                if (!IsInSight(item.gameObject))
                {
                    continue;
                }
                //If the item is not in the array yet, add it
                if (nearbyItems.Find(x => x.item == item) == null)
                {
                    ItemInfo itemInfo = new ItemInfo
                    {
                        item = item,
                        distance = Vector3.Distance(transform.position, item.transform.position),
                        importance = item.importance,
                        timeLookedAt = 0f,
                        novelty = 0f,
                        calcValueOfInterest = 0f,
                        isCurrentlyNearby = true
                    };
                    nearbyItems.Add(itemInfo);
                }
                else
                {
                    //If the item is already in the array, update info
                    ItemInfo itemInfo = nearbyItems.Find(x => x.item == item);
                    itemInfo.distance = Vector3.Distance(transform.position, item.transform.position);

                    //If importance of the item increased, reset time looked at
                    if (itemInfo.importance < item.importance)
                    {
                        itemInfo.novelty += (10 / detectionInterval);
                    }
                    itemInfo.importance = item.importance;
                    itemInfo.isCurrentlyNearby = true;
                    //Decrease time looked at by the detection interval
                    itemInfo.timeLookedAt = Mathf.Max(0f, itemInfo.timeLookedAt - detectionInterval);

                }
            }
            //Remove items that are not in the detection radius anymore
            foreach (ItemInfo itemInfo in nearbyItems.ToList())
            {
                if (itemInfo.isCurrentlyNearby == false)
                {
                    nearbyItems.Remove(itemInfo);
                }
            }

            //Calculate the most interesting item and select one by chance from the list
            CalculateInterestInItemsAndSelectOne();

        }


        public void CalculateInterestInItemsAndSelectOne()
        {
            foreach (ItemInfo itemInfo in nearbyItems)
            {
                itemInfo.calcValueOfInterest = itemInfo.importance - itemInfo.distance - itemInfo.timeLookedAt + itemInfo.novelty;
            }
            nearbyItems.Sort((x, y) => y.calcValueOfInterest.CompareTo(x.calcValueOfInterest));

            ItemInfo newItemOfInterest = SelectFromListWithProbability();



            if (newItemOfInterest != null)
            {
                aimScript.weight = maxWeight;
                //Increase time looked at by the detection interval
                newItemOfInterest.timeLookedAt += detectionInterval * 2;
                //Decrease novelty over time
                newItemOfInterest.novelty = Mathf.Max(0f, newItemOfInterest.novelty - (1 / detectionInterval));
                //Increase novalty if the item changed
                if (currentItemOfInterest != newItemOfInterest)
                {
                    newItemOfInterest.novelty += (5 / detectionInterval);
                }
            }

            currentItemOfInterest = newItemOfInterest;
        }

        public bool IsInSight(GameObject obj)
        {
            Vector3 origin = transform.position + transform.forward;
            origin.y += 1.5f;
            Vector3 dest = obj.transform.position;
            if (Physics.Linecast(origin, dest, occlusionLayers))
            {
                //Debug.DrawLine(origin, dest, Color.red, 2f);
                return false;
            }
            //Debug.DrawLine(origin, dest, Color.green, 2f);
            return true;
        }

        private ItemInfo SelectFromListWithProbability()
        {
            if (nearbyItems.Count == 0)
            {
                // No objects available
                return null;
            }
            else
            {

                double randomValue = Random.value;

                if (randomValue <= chanceFirstItem)
                {
                    // Select the first item
                    return nearbyItems[0];
                }
                else if (chanceFirstItem < randomValue && randomValue <= chanceSecondItem)
                {
                    // Select the second item or first item when second item is not available
                    if (nearbyItems.Count > 1)
                        return nearbyItems[1];
                    else
                        return nearbyItems[0];
                }
                else if (chanceSecondItem < randomValue && randomValue <= chanceThirdItem)
                {
                    // Select the third item or first item when second item is not available
                    if (nearbyItems.Count > 2)
                        return nearbyItems[2];
                    else
                        return nearbyItems[0];
                }
                else if (chanceThirdItem < randomValue && randomValue <= chanceRandomItem)
                {
                    // Select a random item
                    int randomIndex = Random.Range(0, nearbyItems.Count);
                    return nearbyItems[randomIndex];
                }
                else if (chanceRandomItem < randomValue && randomValue <= chanceIdealTime)
                {
                    // Select no item and idle
                    return null;
                }
                return null;
            }
        }


    }
}
