# Look around

To make an agent look more life like it can automatically look at items in front of him, swerve between the items, idle in beetween and pick up on sudden interest changes, like items that start to move.


## Algorithm
The agent checks if there are any items in front of him and then checks if they are seeable. For calculating the value of Interest to the agent, the distance to the item, the importance defined by the item, the time that the agent already look at the item and the perceived novelty is used. 

``timeLookedAt`` is modified in two ways:
- ``timeLookedAt`` increases every time that the item is looked at
- ``timeLookedAt`` decreases every time that the item is not looked at

``novelty`` is modified in two ways:
- ``novelty`` increases by 5 if the item has not been looked at before
- ``novelty`` increases by 10 if the importance of the item increased since last seeing it
- ``novelty`` decreases by 1 every time that the item is looked at
## Requirements

- Agent needs the ``LookAroundController`` component
  - Items that can be looked at need to be on specific layers, specified in ``seeLayers``
  - Objects that should obstruct the view of the agent need to be on specific layers, specified in ``occlisonLayers``
- Objects that should be looked at need the ``Item`` component
- The agent prefab has an Animation Rigging ``Multi-Aim Constraint`` where the head or other parts of the agent can be defined to look after the current item target



## Options
The ``LookAroundController`` has several options that can be modified to fit the agents purpose or personality:
- ``detectionRadius`` defines how big the detection cube in which items can be seen in front of the agent is [^1]
- ``maxNumberOfItemsInRange`` should be an upper limit of how many items can be in front of the agent, otherwise undefined behavior 
- ``detectionIntervalWhenWalking`` and ``detectionIntervalWhenIdle`` is the detection Interval in which the detection cube is checked for items in seconds. Longer Intervals allow the agent to look after items that move, even when they move outside the detection cube.
- ``chanceFirstItem``, ``chanceSecondItem``, ``chanceThirdItem`` define the chances for looking at the most interesting, second interesting and third interesting item based on the calculated interest value
- ``chanceRandomItem`` defines the chance for looking at a random item in sight
- ``chanceIdealTime`` defines the chance for the agent to not look at anything specific and to play the ideal animation instead
- ``maxWeight`` defines the weight of the head movement over the animation, a weight of 1 disregards the normal animation

[^1]: Use Window > Analysis > Physics Debug > Queries to see the detection radius, decrease detection Interval to see the cube on every frame