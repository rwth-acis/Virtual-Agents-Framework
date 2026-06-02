# Adding Own Agent Models and Animations

The framework already provides a standard agent which can be added as a prefab.
However, if you want to add your own character, for example from a 3D scan, this is also possible. The following explains how to create a new agent from scratch. If you already have a rigged character or use one from an online library you can follow the steps in the section ["Importing Custom Models"](own-agents.md#importing-custom-models).

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

The automatic import function uses an agent prefab without a model found at `Packages/com.i5.virtualagents/Runtime/Prefabs/AgentWithoutModel.prefab`. To make the import of multiple agents that all need the same changes easier, the `AgentWithoutModel.prefab` can be copied and named `CustomAgentWithoutModel.prefab`. If that prefab is anywhere in the project the import function always uses the `CustomAgentWithoutModel.prefab` to configure the new agents.

## Optional: Adjust Animation Controller

Usually, it suffices to take the existing standard controller as a basis.
To extend the animation range of the agent, it is recommended to copy the existing controller and to extend it rather than starting with a blank controller.
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

## Importing Custom Models
If you already have a rigged character or use one from an online library, you can follow the steps below to import it as an agent. While the general workflow is similar for both .fbx and .glb files, specific import settings will vary depending on your file type and the model's source. We have provided general instructions below, followed by step-by-step examples for importing Rocketbox and Ready Player Me avatars.

### For .fbx Files

1. Move or copy the `.fbx` asset files into your project's Assets folder or subfolder.
2. Select the `.fbx` file in the project window.
3. In the Inspector, under `Rig` > `Animation Type`, change `Generic` to `Humanoid` and click `Apply`.
4. Drag and drop the asset file from the project window into a scene.
5. Select the loaded asset in the scene and in the top menu of Unity click `Virtual Agents Framework` > `Custom Model Agent Import` > ` Create Agent from Humanoid Model`.
   An agent called AgentBasedOnAssetName will appear in the scene next to the original asset. This avatar should now be ready to function as an agent.
6. Remove the original asset from the scene.

### For .glb Files

1. Install the [Unity GLTF Importer](https://docs.unity3d.com/Packages/com.unity.cloud.gltfast@6.16//manual/index.html) package from the Unity Package Manager by following the instructions [here](https://docs.unity3d.com/Packages/com.unity.cloud.gltfast@6.16//manual/installation.html).
2. Move or copy the `.glb` asset files into your project's Assets folder or subfolder.
3. Drag and drop the asset file from the project window into a scene.
4. Select the loaded asset in the scene and in the top menu of Unity click `Virtual Agents Framework` > `Custom Model Agent Import` > ` Create Agent from Humanoid Model`.
   An agent called AgentBasedOnAssetName will appear in the scene next to the original asset. This avatar should now be ready to function as an agent.
5. Remove the original asset from the scene.


### Importing Rocketbox Avatars

The [Microsoft Rocketbox Avatar library](https://github.com/microsoft/Microsoft-Rocketbox) consists of 115  high definition avatars in several profession categories. Rocketbox avatars can be imported and converted into agents with a small additional editor helper.

1. Copy `FixRocketboxMaxImport` into your project at `Assets/Editor` from the Rocketbox repository: https://github.com/microsoft/Microsoft-Rocketbox/tree/master/Assets/Editor
2. Copy one of the avatar folders (for example `Adults/Female_Adult_01`) into any `Assets` folder from [here](https://github.com/microsoft/Microsoft-Rocketbox/tree/master/Assets/Avatars).
3. Open the `Export` folder inside the avatar folder (for example `Female_Adult_01/Export`) and select the `.fbx` file (for example `Female_Adult_01.fbx`).
4. In the Inspector, under `Rig` > `Animation Type`, change `Generic` to `Humanoid` and click `Apply`. The setting may revert to `Generic` afterward, but it needs to be set to `Humanoid` once for the import to work, as that creates the necessary humanoid animation avatar for the model.
5. Drag and drop the `.fbx` into your scene.
6. Select the model in your scene and in the menu ribbon select `Virtual Agents Framework` > `Custom Model Agent Import` > `Create Agent from Humanoid Model`.
7. A warning about "Wrong rigging" appears, click `Continue anyway`.
8. If a "Manual Avatar Mapping" window opens, close it and repeat step 4.

You should now have an `AgentBasedOnFemale_Adult_01` (or equivalent) in the scene that can be used like the standard agent.

### Importing Ready Player Me Avatars
[!IMPORTANT] The services offered by Ready Player Me have become unavailable as of January 31, 2026. Already downloaded `.glb` files of avatars can be imported by using the steps provided [above](own-agents.md#importing-custom-models).

#### Legacy instructions
[Ready Player Me](https://readyplayer.me/) is a service that provides easy access to custom avatars that can be used for rapid prototyping or as an avatar system. As an example we will show here how an avatar created on [Ready Player Me](https://readyplayer.me/) can be turned into an agent.
1. Create a Ready Player Me avatar [here](https://readyplayer.me/en/hub/avatars)
To easily import the avatar, the Ready Player Me SDK for Unity can be used. Optionally the Avatar can also be downloaded as a glb file and turned into a fbx file with programs like Blender to import the avatar normally as in the steps above.
2. Copy the provided .glb URL after avatar creation
3. Follow the first step [here](https://docs.readyplayer.me/ready-player-me/integration-guides/unity/quickstart) to import the Ready Player Me Unity SDK into your package. Optionally, the other steps of the installation guide can be followed but they are not needed here. Close the Setup Guide menu.
4. In the top menu of Unity click `Ready Player Me` > `Avatar Loaded`. In the new window copy the .glb URL of step 2 and load the avatar.
5. Select the loaded avatar in the scene and in the top menu of Unity click `Virtual Agents Framework` > `Custom Model Agent Import` > ` Create Agent from Humanoid Model`.
The avatar should now be ready to function as an agent.