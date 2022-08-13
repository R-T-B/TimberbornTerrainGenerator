
# Random map generator

A work in progress project for generating random maps for timberborn.
It uses several open source projects, including the works of Caps_Errors (initial python code), reddit user u/savanik (updated with river handling) and myself, MSterczewski (some plugin modeling), R-T-B (additions of new plant life, porting to the game as a plugin, and such).

Before you proceed, you should be aware this plugin will likely only work on windows installs of the game, due to the included python script it has to fire as an exe.  Mac users might be able to make use of the python script in it's raw form on the source repo, but no promises.
# Generating a map
Generating a map is simple!  Install the plugin, and generate a new map!  Be aware all maps will be square due a limitation of the generator, using the largest dimension, and all will be based roughly around a single river system.  I am open to expanding this in the future.

Once the map is generated, feel free to run the water sim, drop a starting location somewhere nice, and load in.  That's reall all there is to it, and most of the maps it makes are playable, though you may want to load a few times to "optimze" your start.  I won't judge you... heh.

Please note that because of how this mod hooks into another dev language (python) it may trigger some antiviruses.  This is a false positive.

# Installation
In order to install the mod follow the instructions from BepInEx website:
https://docs.bepinex.dev/articles/user_guide/installation/index.html

Changelog:

v0.1.0: Initial Release.

v0.1.1: Make rivers more shallow and integrate water errorsion to smooth them so flow is not impeded.
    
# Feedback

If you have any feedback, please reach out to me at github

  
# Authors

- [@MSterczewski (aka Informati)](https://github.com/MSterczewski) (initial plugin code), Caps_Errors (initial python code), reddit user u/savanik (updated with river handling), R-T-B (additions of new plant life, interation to the game as a plugin, and such).

  