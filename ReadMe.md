## Battle Gym
Unity RPG style ML agent training ground

original Battle system : http://roystanross.wordpress.com/

# Setup After import

- Edit->Project Settings
- input manager -> click the preset button and load input preset
- tags and layers -> same


# Todo
### Basic Training
- Touch control (simple goto spot)
    - ~~add basic controls~~
    - ~~hit detection~~
    - ~~Detect spot~~
    - ~~create Reward System~~
    - ~~reset~~
    - ~~build agent~~
    - ~~build config x2~~
    - ~~train~~
    - implement curriculum
        - control spot count
        - max episode frames
        - divider
    - change active goal value to float
    - more tensorboard kpi
- Implement Marskmen-h    
- Pathing (some maze work)
    - Drop random obstacles


### Battle Engine 
Implements targeting and self play
- Pray Predator
- Target Con (stat broadcasting)

### Battle System (one vs one)
- Implement hitpoints
- Stats
- raycast for fighting range
- Start with no weapons, basic movement
- sheath un-sheath (movement decrease and energy)

## Ideas
- Check points
    -leave trail on floor (path finding)
    -add vision to find un-touched 
- Perks
    -vision
    -
- Priority Actions (classes)
- Combo Actions 
- Attack Range
- In view
- Potential Max Goal for area 

Down the Road
- Path Finding
- Treasure Hunting
- Classes
    - Scouting
    - War
    - Mage
    - Commander
        - Team Work

