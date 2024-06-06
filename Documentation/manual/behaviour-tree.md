# Behaviour Tree

### Scene Setup
1. Insert Agent Prefab,follow standard guide or import guide
2. Remove Schadule Based Task System
3. Replace with Behaviour Tree Runner

### Creating Tree
4. Create Tree
5. Double Click to open Tree Editor
6. Add nodes, make general changes
7. Save Tree

### Connecting Tree with Scene
8. Add tree to Tree Runner Component of Agent
9. Do agent or scene specific scenes. Any changes in the inspector will overwrite the values from the file.

# Technical overview
![Behaviour Tree](~/resources/BehaviourTreeClassDiagram.png)

Diagram was generated with [Visual Studio Class Designer](https://learn.microsoft.com/en-us/visualstudio/ide/class-designer/designing-and-viewing-classes-and-types?view=vs-2022). Open Assets\Virtual Agents Framework\Runtime\Scripts\BehaviourTree\~BehaviourTreeClassDiagram.cd in Visual Studio for an interactive version. 