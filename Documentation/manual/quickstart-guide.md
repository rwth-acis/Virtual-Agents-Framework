# Quickstart-guide

After importing the package this page is meant to get you going and introduce basic functionality.

## Setting up the scene
1. Have or add a ground to your scene that the agent will walk on, i.e. by right-clicking in the hierarchy and selecting ``3D Object > Plane``.
2. Add the standard agent prefab from ``Packages/com.i5.virtualagents/Runtime/Prefabs/AgentStandard.prefab`` to the scene.
3. Add a NavMesh Surface to the scene by right-clicking the hierarchy and selecting ``AI > NavMesh Surface``. 
4. Select the NavMesh Surface and press on `Bake` to create the NavMesh Data. The NavMesh represents where the agent can walk, so this has to be repeated after every change to the possible walking space of the agent.
## Adding Tasks to an agent
1. Add an empty GameObject to the scene that will function as a controller object.
2. Select the controller object and add a new script. This script will add different tasks to the agents schedule. We will call it ``ScheduleController``. To start, the script should look like this:

```C#
        using UnityEngine;
        using i5.VirtualAgents;
        using i5.VirtualAgents.ScheduleBasedExecution;
        public class ScheduleController : MonoBehaviour
        {
            // The agent which is controlled by this controller, set in the inspector
            public Agent agent;
            // The taskSystem of the agent
            protected ScheduleBasedTaskSystem taskSystem;

            protected void Start()
            {
                // Get the task system of the agent
                taskSystem = (ScheduleBasedTaskSystem)agent.TaskSystem;

                //Add tasks below
                //...
            }
        }
```

4. To add tasks for the agent multiple TaskActions are available, that can be used like this:
    ```taskSystem.Tasks.GoTo(Vector3.zero);```
This line can be added in the ``Start()`` function to call it once or anywhere else in the Script.
With ``GoTo(Vector3.zero)`` a walking Task is added to the agent, that will make him walk to the origin of the scene.
To find more explanations of different TaskActions and how tasks work generally see [here](task-system.md#shortcuts).

## Samples
At this point it is recommended to take a look at the samples provided with the package. To access them, follow these steps:
1. In Unity, go to "Window > Package Manager".
2. Make sure that the dropdown menu in the top left of the package manager window is set to "Packages: In Project".
3. Click on the entry for the Virtual Agents Framework in the list.
4. On the right side, expand the samples section.
   Here, you can find a list of available examples.
5. Click the import button to download the samples.
   They are saved in your Assets folder in a folder ``Samples/Virtual Agents Framework/1.2.0``. Every sample provides one or more documentation object that explains how the scene and the functions behind that scene work, and also links to recommended manual pages here.

Going from least to most complex, it is recommended to look at the samples in the following order: 
1. Navigation Sample
2. Wait Sample
3. Dynamic Navigation Sample
4. TaskBundle Sample ([manual page](task-bundle.md))
5. Parallel Task Sample ([manual page](parallel-tasks.md))
6. Aiming Sample ([manual page](aiming.md))
7. Adaptive Gaze Sample ([manual page](adaptive-gaze.md))
8. Item Pickup Sample ([manual page](items.md))

## Customization of agents
To make the application look more appealing from the beginning custom agent models can be used, see [Adding Own Agent Models and Animations](own-agents.md) for that.
