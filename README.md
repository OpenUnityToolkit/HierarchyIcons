# Hierarchy Icons
Replaces the standard cubes in the Scene Hierarchy with more informative icons.

![Image](.DocRef/hierarchy-beforeafter.png "before-after")

## Features
- Icons are sourced from:
    - The Game Object scene assigned icon 
    - The first component with an icon on a Game Object
    - Folders icons for Game Objects with children objects but no components
    - A representative mesh icons based on vertex count
    - Light icons with indicative colouring
- Prefab Game Objects with changes are flagged with overlays
- Vertex count for meshes are displayed
- Tree lines are shown to help make sense the object hierarchy
- Features can be toggled in preferences


## Preferences
The plugin can be configured in Preferences Open Toolkit > Hierarchy Icons

![Image](.DocRef/hierarchy-config.png "config")


## Compatibility
Minimum version Unity 2019.4
Tested with Unity 2020.3 and 2021.1

## Installing
Copy the git URL from GitHub

![Image](.DocRef/hierarchy-copyurl.png)


Open `Window > Package Manager` and select `+ > Add page from git URL...`

![Image](.DocRef/hierarchy-addpackage.png)


Paste the URL and click `Add`

![Image](.DocRef/hierarchy-urlpaste.png)