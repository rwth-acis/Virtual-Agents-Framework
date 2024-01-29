# Changelog

This document keeps track of the changes between versions of the framework.

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