# Adding Own Agent Models and Animations

The framework already provides a standard agent which can be added as a prefab.
However, if you want to add your own character, for example from a 3D scan, this is also possible. The following explains how to create a new agent from scratch or as an alternative example how to [import models created with the service called Ready Player Me.](own-agents.md#importing-custom-models-from-ready-player-me) Own characters can also be modeled using the [MPFB Blender Extension.](own-agents.md#creating-custom-models-from-mpfb)
If you already have a rigged character or use one from an online library you can follow the steps in the section ["Importing Custom Models"](own-agents.md#importing-custom-models).


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

## Creating Custom Models from MPFB
To use the MPFB Blender extension for creating avatars, you need to have at least Blender version 4.2.0.
1. `Edit` > `Preferences` > `Add-ons` > Search for mpfb > enable
   1. `System` > `Network` > `Allow Online Access` has to be checked for this to work
2. Restart Blender
3. You can find MPFB on the right side of the Viewport after pressing "n"
4. In the [create a human](https://static.makehumancommunity.org/mpfb/docs/getting_started.html#create-a-human) section of the startup guide you can find the steps to create your avatar
   1. As a rig please choose GameEngine (with or without breasts), this works better with Unity than the standard rig.

Further tips for getting started with MPFB can be found [here.](https://static.makehumancommunity.org/mpfb/docs/getting_started.html)

### Clothing
1. Additional [asset packs](https://static.makehumancommunity.org/assets/assetpacks.html) including clothes are also available. Make sure to check the licence of each pack and correctly attribute the creator if necessary.
2. When wearing a top, the mesh of the avatar gets replaced by the top. When grabbing and removing the top there are holes in the mesh. This should be fine, if you do not want or need to remove the clothes, f.e. to swap them in Unity or modify them in Blender. Otherwise, you can fix this two ways:
   1. Add the mesh back in: `Apply assets` > `Topologies library` > find the correct mesh for your avatar
   2. Create a preset for your avatar before adding clothing: First select the avatar in Blender, then select `Manage save files` > `Human save files` > and choose a name for your preset and save it. After that you can load the preset under `New human` > `From save file` and add the preset avatar to the scene. Then you can choose clothes for the preset avatar and remove them to put on the original avatar. You can then delete the preset avatar again. If you do this, the clothes may not react to the movements of the avatar. To fix this, you need to create a new piece of clothing from them within MPFB, see [the bottom instructions at section Custom Clothing](#custom-clothing).
#### Custom Clothing
To create custom clothing, you need to have a basic understanding of 3D modelling in Blender, i.e. be comfortable with scaling, extruding, modifiers, the edit mode etc. We will follow and summarise the process described in [this video](https://www.youtube.com/watch?v=v_WMJLudpvg).
1. Create a cube (or other fitting base mesh) and scale it roughly to fit the size of the avatar at the position of the item of clothing you want to create.
2. Enter edit mode and further fit the mesh to the avatar. Add loop cuts to add geometry, especially where the clothing should later deform.
3. Delete one half of the mesh and add a mirror modifier to create the other half of the clothing. This way you only have to model one half of the clothing.
4. Add a shrinkwrap modifier to the mesh and select the avatar as the target. This will make the clothing fit to the avatar. In the modifier's settings add a small offset, also depending on the desired fit. In places where the clothing clips through the avatar, you can move and/or add more geometry to the clothing to make it fit better.
5. Make sure that no vertices of the half you are modelling are behind the mirror axis, to avoid problems with the mirror modifier. Then apply the mirror modifier.
6. Use sculpting to loosen up the clothing and to add details to it, like for example folds.
7. Add a material. This can be a texture or simply a fitting colour.

If you are happy with your new piece of clothing, you need to make it a piece of clothing within MPFB. Otherwise, the clothing will not react to the movements of the avatar and will just stay in place.
The official video tutorial can be found [here](https://www.youtube.com/watch?v=b5AA5nlelxc).
1. Select your clothes and mark them as such in MPFB using `Create assets` > `MakeClothes` > `Change type`, making sure to select "Clothes" as object type in the dropdown menu.
2. In the same menu further below, fill out the properties of the clothing ("Clothes props"), and generate a new unique UUID. 
3. In edit mode, select all vertices of the clothing (press `a`) and assign them to a new vertex group called exactly "body". Vertex groups can be found in the properties panel on the right side of the viewport, under the tab with the green triangle icon.
4. Back in object mode use the `Check clothes` function in the MPFB `MakeClothes` menu to check if the clothing is correctly set up.
   1. If not all faces have the same number of vertices, go into edit mode, select all vertices and use `Face` > `Triangulate Faces` to convert all faces to polygons with three vertices.
   2. If not all vertices belong to a face, go into edit mode, select all vertices, go into the vertex select mode (the leftmost icon to the right of the mode selection dropdown) and use `Select` > `Select All by Trait` > `Loose Geometry` to select all vertices that do not belong to a face. Then you can delete them.
5. Click `Store in Library` or `Save as files` respectively to save the clothing in the library or as a .MHCLO file. You should now be able to find the clothing in the library or load it from the file and add it to your avatar. You can delete or move the original clothing mesh at this point. Check that everything works by selecting the rig, going into pose mode and moving the avatar around. The clothing should move with the avatar.
   1. If you add the clothes to a different model to the one you used to create them, you might have to use sculpt mode with a low brush strength to smoothen things out.
   2. If the clothing moves when moving bones that it should not move with, you may need to select the clothing, go into `Weight Paint` mode and redraw the influence of the offending bone. You select the bone in the dropdown menu at the top of the viewport. To lower the influence of the bone on a vertex, hold `Ctrl` while painting.

## Importing Custom Models
If you already have a rigged character or use one from an online library, you can follow the steps below to import it as an agent. While the general workflow is similar for both .fbx and .glb files, specific import settings will vary depending on your file type and the model's source. We have provided general instructions below, followed by step-by-step examples for importing Rocketbox and Ready Player Me avatars.

### For .fbx Files

1. Move or copy the `.fbx` asset files into your project's Assets folder or subfolder.
2. Select the `.fbx` file in the project window.
3. In the Inspector, under `Rig` > `Animation Type`, change `Generic` to `Humanoid` and click `Apply`.
4. Drag and drop the asset file from the project window into a scene.
5. Select the loaded asset in the scene and in the top menu of Unity click `Virtual Agents Framework` > `Custom Model Agent Import` > `Create Agent from Humanoid Model`.
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

### Importing Custom Models from MPFB
1. In the MPFB menu select `Operations` > `Export copy` > `Create Export copy`. This creates a copy of the avatar with helpers removed. You find it in the Blender collection `export copy` and it is probably standing inside the original model. 
2. Export the export copy as an fbx file and import it into Unity by selecting `File` > `Export` > `FBX (.fbx)`. Make sure to select them with clothes and bones and check `Limit to Selected Objects` in the export settings. 
3. Before clicking on `Export FBX`, make sure to set `Path Mode` to `Copy` and check `Embed Textures` (the small box icon right next to the Path Mode dropdown) to include the textures in the fbx file.
4. In Unity right-click the project window and import the fbx file as a new asset. 
5. If you have multiple avatars you might have to set the position of the model to the origin, as Blender exports the model at the position where it is in the scene. To do this, select the model in the scene and set the position to (0,0,0) in the inspector. Alternatively you can move the model in Blender to the origin before exporting it. 
6. In the inspector select the `Rig` tab and set the `Animation type` to `Humanoid`. Then click on `Apply`.
7. In the `Materials` tab, click on `Extract Textures` and choose a folder to save the textures. Repeat this with `Extract Materials` to save the materials as well.
8. Now you need to reassign the textures to the corresponding materials. To do this, drag each texture on to the `Albedo` slot of the right material. For eyebrows, eyelashes and hair you likely have to change the `Rendering Mode` of the material to `Cutout`.
9. If you drag the model into the scene, it should now be correctly textured and ready to be set up as an agent by following the steps described in the previous sections.

### Importing Custom Models from Ready Player Me
**Ready Player Me has shut down its services in January 2026, so this section is only relevant for users who have already created and saved avatars with Ready Player Me and want to use them as agents in their project.** The steps might not work anymore.
[Ready Player Me](https://readyplayer.me/) was a service that provided easy access to custom avatars that could be used for rapid prototyping or as an avatar system. As an example we will show here how an avatar created on [Ready Player Me](https://readyplayer.me/) can be turned into an agent.
1. ~~Create a Ready Player Me avatar [here](https://readyplayer.me/en/hub/avatars)~~
   To easily import the avatar, the Ready Player Me SDK for Unity can be used. Optionally the Avatar can also be downloaded as a glb file and turned into a fbx file with programs like Blender to import the avatar normally as in the steps above.
2. Copy the provided .gbl URL after avatar creation
3. Follow the first step [here](https://docs.readyplayer.me/ready-player-me/integration-guides/unity/quickstart) to import the Ready Player Me Unity SDK into your package. Optionally, the other steps of the installation guide can be followed but there are not needed here. Close the Setup Guide menu.
4. In the top menu of Unity click ``Ready Player Me`` > ``Avatar Loaded``. In the new window copy the .glb URL of step 2 and load the avatar.
5. Select the loaded avatar in the scene and in the top menu of Unity click `Virtual Agents Framework` > `Custom Model Agent Import` > ` Create Agent from Humanoid Model`.
   The avatar should now be ready to function as an agent.

