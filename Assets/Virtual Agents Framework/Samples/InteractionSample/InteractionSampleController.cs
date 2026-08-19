using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using UnityEngine;
using UnityEngine.AI;

namespace i5.VirtualAgents.Examples
{
    public class InteractionSampleController : SampleScheduleController
    {
        /// <summary>
        /// The button which the agent should interact with.
        /// </summary>
        public Interactable button1;
        public Interactable button2;
        public Interactable plant;

        public GameObject water;

        public GameObject waypoint;
        public GameObject waypoint2;
        protected override void Start()
        {
            base.Start();
            AgentMovementTask walkTask1 = (AgentMovementTask) taskSystem.Tasks.GoTo(waypoint);
            
            //AgentRotationTask rotationTask = new AgentRotationTask(180f,true,30);
            
            //AgentBaseTask task = taskSystem.Tasks.PointAt(button.gameObject, false, true);
            
            AgentBaseTask taskButton1 = taskSystem.Tasks.Interact(button1);
            taskButton1.DependsOnTasks.Add(walkTask1);
            AgentBaseTask waitTask = taskSystem.Tasks.WaitForSeconds(0.25f,default,"Left Arm");
            waitTask.DependsOnTasks.Add(taskButton1);
            AgentBaseTask taskButton2 = taskSystem.Tasks.Interact(button2);
            taskButton2.DependsOnTasks.Add(waitTask);
            
            taskButton2.OnTaskFinished += () =>
            {
                AgentBaseTask itemPick = taskSystem.Tasks.GoToAndPickUp(water);
                AgentMovementTask walkTask2 = (AgentMovementTask) taskSystem.Tasks.GoTo(waypoint2);
                walkTask2.DependsOnTasks.Add(itemPick);
            
                AgentRotationTask rotationTask = new AgentRotationTask(plant.gameObject);
                rotationTask.DependsOnTasks.Add(walkTask2);
                taskSystem.ScheduleTask(rotationTask);
            
                AgentBaseTask taskPlant = taskSystem.Tasks.Interact(plant);
                taskPlant.DependsOnTasks.Add(rotationTask);
            };
           
            
            
            
            
            walkTask1.OnTaskFinished += () =>
            {
                Debug.Log("Task finished");
                //taskSystem.ScheduleTask(rotationTask);
                
            };
            
            
        }
    }
}