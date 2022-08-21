# Changelog

This document keeps track of the changes between versions of the framework.

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