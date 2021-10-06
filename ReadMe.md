## Battle Gym
Unity RPG style ML agent training ground

original Battle system : http://roystanross.wordpress.com/

# Setup After import

- Edit->Project Settings
- input manager -> click the preset button and load input preset
- tags and layers -> same


# Todo

Import RPG battle system
- ~~get it to work~~
- ~~test controls~~
- ~~review code~~

Code Updates
- ~~Remove manual control~~
- ~~control basic movements~~
       
Add Sensors
- ~~raycast for vision~~

Basic Training
- Touch control (simple goto spot)
    - ~~add basic controls~~
    - ~~ hit detection ~~
    - Detect spot
    - create Reward System
    - reset
    - build agent
    - build nn
    - train
- Pathing (some maze work)

Battle Engine (cat and mouse)
- Add agents on the stage
- control resets 
- stat casting (mob conning)

Battle System (one on one)
- Depending on existing one
    - Implement hitpoints
    - Random Stats
    - raycast for fighting range
- Start with no weapons, basic movement

Create Neural Network
- Features
    - Check points
        -leave trail on floor (path finding)
        -add vision to find un-touched 
    - Stats
    - Actions (moves)
    - Attack Range
    - In view
    - Potential Max Goal

Down the Road
- Path Finding
- Treasure Hunting
- Classes
    - Scouting
    - War
    - Mage
    - Commander
        - Team Work

