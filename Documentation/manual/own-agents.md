# Adding Own Agent Models and Animations

The framework already provides a standard agent which can be added as a prefab.
However, if you want to add your own character, for example from a 3D scan, this is also possible. The following explains how to create a new agent from scratch or as an alternative example how to [import models created with the service called Ready Player Me.](own-agents.md#importing-custom-models-from-ready-player-me) Own characters can also be modeled using the [MPFB Blender Extension.](own-agents.md#creating-custom-models-from-mpfb)

## Preparing the Character

To create a new agent from scratch, first add a humanoid character to your project.
The character needs to be rigged.
So, it requires a skeleton that was set up in a 3D software like Blender.
The rig is what drives the movements of the character, and it defines which parts of the mesh are deformed during an animation.

## Import Into Unity

In Unity's import settings, set the rig type to "humanoid" so that animations can be transferred to the new character.
Check the configuration of the rig mapping to make sure that bones are correctly transferred to Unity's default humanoid rig.
In this configuration, there are also more advanced settings for the muscles.
Usually, you do not need to worry about them but if you want your agent to be more or less flexible than defined in the animations, you can accomplish this with the muscle settings.

## Setup in the Scene

After finishing the setup, you can add the character to your scene. The character should now be similar to the standard agent's 3D model that the framework provides in `Virtual Agents Framework/Runtime/3D Models/AgentStandard.fbx`. To configure the character correctly to work as an agent the Virtual Agents Framework provides an automatic import functionality. After that your character should be similar to the prefab of the working standard agent that can be found in `Assets/Virtual Agents Framework/Runtime/Prefabs/AgentStandard.prefab`.

Select the parent GameObject of your character in the scene and in the top menu of Unity click `Virtual Agents Framework` > `Custom Model Agent Import` > `Create Agent from Humanoid Model`.
Now a GameObject called `AgentBasedOnCharacterObjectName` should be selected in the scene. The character is now ready to be used as an agent with all functionalities.

The automatic import function uses an agent prefab without a model found at `Packages/com.i5.virtualagents/Runtime/Prefabs/AgentWithoutModel.prefab`. To make the import of multiple agents that all need the same changes easier, the `AgentWithoutModel.prefab` can be copied and named ``CustomAgentWithoutModel.prefab``. If that prefab is anywhere in the project the import function always uses the ``CustomAgentWithoutModel.prefab`` to configure the new agents.

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

## Creating Custom Models from MPFB
To use the MPFB Blender extension for creating avatars, you need to have at least Blender version 4.2.0.
1. `Edit` > `Preferences` > `Add-ons` > Search for mpfb > enable
   1. `System` > `Network` > `Allow Online Access` has to be checked for this to work
2. Restart Blender
3. You can find MPFB on the right side of the Viewport after pressing "n"
4. In the [create a human](https://static.makehumancommunity.org/mpfb/docs/getting_started.html#create-a-human) section of the startup guide you can find the steps to create your avatar
Further tips for getting started with MPFB can be found [here.](https://static.makehumancommunity.org/mpfb/docs/getting_started.html)
### Clothing
1. Additional [asset packs](https://static.makehumancommunity.org/assets/assetpacks.html) including clothes are also available. Make sure to check the licence of each pack and correctly attribute the creator if necessary.
2. When wearing a top, the mesh of the avatar gets replaced by the top. When grabbing and removing the top there are holes in the mesh. You can fix this two ways:
   1. Add the mesh back in: `Apply assets` > `Topologies library` > find the correct mesh for your avatar
   2. Create a preset for your avatar before adding clothing: First select the avatar in Blender, then select `Manage save files` > `Human save files` > and choose a name for your preset and save it. After that you can load the preset under `New human` > `From save file` and add the preset avatar to the scene. Then you can choose clothes for the preset avatar and remove them to put on the original avatar. You can then delete the preset avatar again.

## Importing Custom Models from MPFB
1. Export the avatar as an fbx file and import it into Unity by selecting `File` > `Export` > `FBX (.fbx)`. If you have multiple avatars, make sure to select them with clothes and bones and check `Limit to Selected Objects` in the export settings.
2. Before clicking on `Export FBX`, make sure to set `Path Mode` to `Copy` and check `Embed Textures` (the small box icon right next to the Path Mode dropdown) to include the textures in the fbx file.
2. In Unity right-click the project window and import the fbx file as a new asset. 
3. In the inspector select the `Rig` tab and set the `Animation type` to `Humanoid`. Then click on `Apply`.
4. In the `Materials` tab, click on `Extract Textures` and choose a folder to save the textures. Repeat this with `Extract Materials` to save the materials as well.
5. Now you need to reassign the textures to the corresponding materials. To do this, drag each texture on to the `Albedo` slot of the right material. For eyebrows, eyelashes and hair you likely have to change the `Rendering Mode` of the material to `Cutout`.
6. If you drag the model into the scene, it should now be correctly textured and ready to be set up as an agent by following the steps described in the previous sections.

## Importing Custom Models from Ready Player Me
**Ready Player Me has shut down its services in January 2026, so this section is only relevant for users who have already created and saved avatars with Ready Player Me and want to use them as agents in their project.** The steps might not work anymore.
[Ready Player Me](https://readyplayer.me/) was a service that provided easy access to custom avatars that could be used for rapid prototyping or as an avatar system. As an example we will show here how an avatar created on [Ready Player Me](https://readyplayer.me/) can be turned into an agent.
1. ~~Create a Ready Player Me avatar [here](https://readyplayer.me/en/hub/avatars)~~
   To easily import the avatar, the Ready Player Me SDK for Unity can be used. Optionally the Avatar can also be downloaded as a gbl file and turned into a fbx file with programs like blender to import the avatar normally as in the steps above.
2. Copy the provided .gbl URL after avatar creation
3. Follow the first step [here](https://docs.readyplayer.me/ready-player-me/integration-guides/unity/quickstart) to import the Ready Player Me Unity SDK into your package. Optionally, the other steps of the installation guide can be followed but there are not needed here. Close the Setup Guide menu.
4. In the top menu of Unity click ``Ready Player Me`` > ``Avatar Loaded``. In the new window copy the .gbl URL of step 2 and load the avatar.
5. Select the loaded avatar in the scene and in the top menu of Unity click `Virtual Agents Framework` > `Custom Model Agent Import` > ` Create Agent from Humanoid Model`.
   The avatar should now be ready to function as an agent.