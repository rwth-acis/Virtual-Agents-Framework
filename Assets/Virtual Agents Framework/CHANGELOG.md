# Changelog

This document keeps track of the changes between versions of the framework.

## 1.3.0 (2024-11-18)

### Added
- The task queue can be cleared and tasks can be aborted
- Events to keep track of important events in the task queue, e.g., if it is empty
- Rotation task: Rotate the agent towards a point of interest, e.g., as a preparation for other tasks
- Sitting task: The agent can now sit down on chairs and surfaces
- Audio task: Play audio clips of pre-recorded speech on the agent
- Task bundles which combine multiple tasks into one composite task

## 1.2.1 (2024-03-18)

### Added
- Quality of life enhancements regarding the AdaptiveGaze feature
	- Start and Stop tasks
	- Autonomous gaze behavior can be overwritten with a specific target object
- Item pickup system now offers additional generic mesh sockets for less common item storage positions
- Automatic import feature for humanoid 3D models to turn them into fully functional agents
- Quick start guide introducing the first concepts and further documentation
- Documentation example for importing ReadyPlayerMe avatars and converting them to agents

### Changes
- Streamlined the setup process of a new agent
- AI navigation package is now correctly defined in the package dependencies
- Temporarily removed the behavior tree implementation so that compilations for devices work
- Aiming during animation tasks now considers the animation length
- Bone presets for aiming are not organized into own classes instead of string comparisons
- Unit tests now ensure the functionality of the sample scenes
- Documentation and sample scenes were updated

## 1.2.0 (2023-11-19)

### Added
- Inverse kinematic (IK) system which allows the agent to reach for objects or to target them
- IK targets for the head and hands
- IK support for animations, e.g., to point at objects
- Pointing animation
- Item interaction system which enables the agent to pick up objects, carry them around, and to drop them again
- Adaptive gaze system: an autonomous sub-system which lets the agent look around naturally based on calculated interest factors for items in the environment
- Support for layered animation combinations using body part masks
- Behavior tree runner as an alternative to the task-queue scheduler
- Visual editor for the behavior tree definition in the Unity editor

### Changes
- Project development now uses Unity 2022.3
- Bumped dependency of i5 Toolkit for Unity to 1.9.1

## 1.1.0 (2022-07-26)

### Added
- Parallel task structure which allows the agent to perform multiple animations
- Animation task
- Waiting task
- Task dependency implementation so that tasks on different body parts wait for another
- Waving and head shaking animations
- Example scenes demonstrating the parallel tasks

### Changes
- Task shortcuts now return the created and scheduled task object so that the code can add dependencies to it
- Agent now has one task manager for each relevant body region; the scheduling method can specify which body region is addressed

## 1.0.0 (2022-02-22)

### Added
- Project and package structure
- Sample agent 3D model
- Sample idle and walk animation
- Core task management system and task queue implementation
- Walking task
- Example scene demonstrating the walking tasks on a NavMesh