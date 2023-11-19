# Adding Own Agent Models and Animations

The framework already provides a standard agent which can be added as a prefab.
However, if you want to add your own character, for example from a 3D scan, this is also possible.

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

After finishing the setup, you can add the character to your scene.

Next, add the Agent component to the character in the scene.
This will automatically add the NavMeshAgent component and the AnimationSynchronizer component to the character.
Because of its humanoid type, there should already be an Animator component on your agent.
You need to assign an AnimationController to the Animator.
It is recommended to choose the default "StandardAnimationController" which is provided by the framework.

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
