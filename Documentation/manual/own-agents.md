# Adding Own Agent Models and Animations

The framework already provides a standard agent which can be added as a prefab.
However, if you want to add your own character, for example from a 3D scan, this is also possible. The following explains how to create a new agent from scratch or as an alternative example how to [import models created with the service called Ready Player Me.](own-agents.md#importing-custom-models-from-ready-player-me)

## Preparing the Character

To create a new agent from scratch, first add a humanoid character to your project.
The character needs to be rigged.
So, it requires a skeleton that was set up in a 3D software like Blender.
The rig is what drives the movements of the character and it defines which parts of the mesh are deformed during an animation.

## Import Into Unity

In Unity's import settings, set the rig type to "humanoid" so that animations can be transferred to the new character.
Check the configuration of the rig mapping to make sure that bones are correctly transferred to Unity's default humanoid rig.
In this configuration, there are also more advanced settings for the muscles.
Usually, you do not need to worry about them but if you want your agent to be more or less flexible than defined in the animations, you can accomplish this with the muscle settings.

## Setup in the Scene

After finishing the setup, you can add the character to your scene. To configure the character correctly the Virtual Agents Framework provides an automatic import functionality.

Select the parent GameObject of your character in the scene and in the top menu of Unity click `Virtual Agents Framework` > `Custom Model Agent Import` > `Create Agent from Humanoid Model`.
Now a GameObject called `AgentBasedOnCharacterObjectName` should be selected in the scene. The character is now ready to be used as an agent with all functionalities.

The automatic import function uses an agent prefab without a model found at `Packages/com.i5.virtualagents/Runtime/Prefabs/AgentWithoutModel.prefab`. To make the import of multiple agents that all need the same changes easier, the `AgentWithoutModel.prefab` can be copied and named ``CustomAgentWithoutModel.prefab``. If that prefab is anywhere in the project the import function always uses the ``CustomAgentWithoutModel.prefab`` to configurate the new agents.

## Optional: Adjust Animation Controller

Usually, it suffices to take the existing standard controller as a basis.
T extend the animation range of the agent, it is recommended to copy the existing controller and to extend it rather than starting with a blank controller.
However, to create a controller from scratch, it needs to follow these guidelines:
There needs to be a blend tree that mixes an idle and a walking animation so that the agent can walk.
The blend tree is driven by an input parameter called "Speed".
If you choose a different name, also adapt the parameter name on the AnimationSynchronizer component.

### Replacing Existing Animations with Own Ones

If you just want to substitute the standard animations but keep the general structure of the controller, you can also copy the controller and substitute the animations with your own imported ones.

If you choose a different walking animation for the blend tree, make sure to recalculate the threshold values according to the speed value of the animation.
If the threshold value of the blend tree is incorrect, the agent's feet will slide along the ground during walking.
Also, make sure to set up the NavMeshAgent component by determining the corresponding speed and rotation values.
The speed of the NavMeshAgent component should correspond to the speed threshold value of the blend tree in the animator component.
With a lower value, the agent will not be able to walk at the full speed as intended by the animation.
A higher value will cause foot sliding as the animation is too slow to keep up with the movement speed.

## Importing Custom Models from Ready Player Me
[Ready Player Me](readyplayer.me) is a service that provides easy assess to custom avatars that can be used for rapid prototyping or as an avatar system. As an example we will show here how an avatar created on [Ready Player Me](readyplayer.me) can be turned into an agent.
1. Create a Ready Player Me avatar [here](https://readyplayer.me/en/hub/avatars)
To easily import the avatar, the Ready Player Me SDK for Unity can be used. Optionally the Avatar can also be downloaded as a gbl file and turned into a fbx file with programs like blender to import the avatar normally as in the steps above.
2. Copy the provided .gbl URL after avatar creation
3. Follow the first step [here](https://docs.readyplayer.me/ready-player-me/integration-guides/unity/quickstart) to import the Ready Player Me Unity SDK into your package. Optionally, the other steps of the installation guide can be followed but there are not needed here. Close the Setup Guide menu.
4. In the top menu of Unity click ``Ready Player Me`` > ``Avatar Loaded``. In the new window copy the .gbl URL of step 2 and load the avatar.
5. Select the loaded avatar in the scene and in the top menu of Unity click `Virtual Agents Framework` > `Custom Model Agent Import` > ` Create Agent from Humanoid Model`.
The avatar should now be ready to function as an agent.

