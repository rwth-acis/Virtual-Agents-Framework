using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.Common.WebApi.WebApiEndpoints;
using UnityEngine.Animations.Rigging;
using System.Linq;

namespace i5.VirtualAgents
{
    public class ItemInfo
    {
        public Item item = null;
        public float distance = 0;
        public float importance = 0;
        public float timeLookedAt = 0;
        public float calcValueOfInterest = 0;
        public bool isCurrentlyNearby = false;
    }


    public class LookAroundController : MonoBehaviour
    {
        public int maxNumberOfItemsInRange = 50;
        public float detectionRadius = 10f;

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

        [SerializeField]
        private List<ItemInfo> nearbyItems = new List<ItemInfo>();
        private float timer = 0f;
        public float lookSpeed = 2f;
        [Range(0f, 1f)]
        public float maxWeight = 0.8f;
        ItemInfo currentlyMostInterestingItem;

        MultiAimConstraint headAimConstraint;

        public LayerMask seeLayer;
        public LayerMask occlusionLayers = default;

        // Start is called before the first frame update
        void Start()
        {

            headAimConstraint = GetComponentInChildren<MultiAimConstraint>();

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

        void Update()
        {
            //Check if the agent is walking
            adjustIntervalBasedOnWalkingSpeed();

            //Position of the Target is updated every frame in cases where it moves 
            updatePositionOfTarget();

            //Every second check which items are nearby and invoke the function to calculate the most interesting item
            checkWitchItemsAreNearbyAndSeeable();

        }
        private void checkWitchItemsAreNearbyAndSeeable()
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

            int count = Physics.OverlapBoxNonAlloc(center, halfExtents, colliders, rotation, seeLayer);
            //Use Window > Analysis > Physics Debug > Queries to see the detection radius, decrease detection Interval to see the qube on every frame

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
                    if(itemInfo.importance < item.importance)
                    {
                        itemInfo.timeLookedAt = 0f;
                    }
                    itemInfo.importance = item.importance;
                    itemInfo.isCurrentlyNearby = true;
                    //Decrease time looked at by the detection interval
                    itemInfo.timeLookedAt -= detectionInterval;
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
            calculateInterestInItemsAndSelectOne();

        }

        private void adjustIntervalBasedOnWalkingSpeed()
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


        public void calculateInterestInItemsAndSelectOne()
        {
            foreach (ItemInfo itemInfo in nearbyItems)
            {
                itemInfo.calcValueOfInterest = itemInfo.importance - itemInfo.distance - itemInfo.timeLookedAt;
            }
            nearbyItems.Sort((x, y) => y.calcValueOfInterest.CompareTo(x.calcValueOfInterest));

            ItemInfo newItemOfInterest = selectFromListWithProbability();

            if (currentlyMostInterestingItem != newItemOfInterest && newItemOfInterest != null)
            {
                newItemOfInterest.timeLookedAt -= 5;
            }

            currentlyMostInterestingItem = newItemOfInterest;
            if (currentlyMostInterestingItem != null)
            {
                //Because the sourceObjects is a struct, we need to get the array, change it and then overwrite it again
                var a = headAimConstraint.data.sourceObjects;
                a.SetWeight(0, maxWeight);
                headAimConstraint.data.sourceObjects = a;
                currentlyMostInterestingItem.timeLookedAt += detectionInterval * 2;
            }

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

        private ItemInfo selectFromListWithProbability()
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

        private void updatePositionOfTarget()
        {
            Vector3 targetPosition;
            if (currentlyMostInterestingItem != null && headAimConstraint != null)
            {
                targetPosition = currentlyMostInterestingItem.item.transform.position;
            }
            else
            {
                //Return to standard look ahead
                targetPosition = transform.position + transform.forward * 2f + new Vector3(0f, 1.5f, 0f);

                //When target position of the standard look is reached, set weight to 0, so that other animations can take over
                if (headAimConstraint.data.sourceObjects[0].transform.position == targetPosition)
                {
                    var a = headAimConstraint.data.sourceObjects;
                    a.SetWeight(0, 0);
                    headAimConstraint.data.sourceObjects = a;
                }

            }
            headAimConstraint.data.sourceObjects[0].transform.position = Vector3.Lerp(headAimConstraint.data.sourceObjects[0].transform.position, targetPosition, Time.deltaTime * lookSpeed);
        }
    }
}
