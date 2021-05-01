# GridFramework
Advanced Grid Framework on Unity Tilemaps for making turn based sokabon-style games.

Requires the following repos. I haven't included them as submodules because I find editing them messy and the file structure non-ideal. These may eventually become unity packages, in which case the other repos would become dependencies.
But for now, just add all these:

- https://github.com/hunterdyar/BloopsUnityUtility
- https://github.com/hunterdyar/SimpleLevelManager
- https://github.com/hunterdyar/SOStateMachine

Thats my simple level manager, a state machine made of scriptable objects, and some utility scripts. The state machine is easily removed, but you may want to leave ICondition, or implement something similar.
