
# Random map generator

A work in progress project for generating random maps for timberborn.  Minimum supported map size is 32x32, maximum is 384x384.  All maps are squares and the Y parameter on "new map" is used as a seed.  If the Y parameter is the same as x, it is completely random.
It uses several open source projects, including the works of Caps_Errors (initial python code), reddit user u/savanik (updated with river handling) and myself, MSterczewski (some plugin modeling), R-T-B (additions of new plant life, porting to the game as a plugin, and such).

Before you proceed, you should be aware mac users will need to install Python for this to work.  Google it, it should be easy, but you may need admin.
# Generating a map
Generating a map is simple!  Install the plugin, and generate a new map!  Be aware all maps will be square due a limitation of the generator, and all will be based roughly around a single river system.  I am open to expanding this in the future.

Once the map is generated, feel free to run the water sim, drop a starting location somewhere nice, and load in.  That's really all there is to it, and most of the maps it makes are playable, though you may want to generate a few times to "optimze" your start.  I won't judge you... heh.

Please note that because of how this mod hooks into another dev language (python) it may trigger some antiviruses.  This is a false positive.

# Installation
In order to install the mod follow the instructions from BepInEx website:
https://docs.bepinex.dev/articles/user_guide/installation/index.html

Changelog:

v0.1.0: Initial Release.

v0.1.1: Make rivers more shallow and integrate water errorsion to smooth them so flow is not impeded.

v0.1.2: Hotfix to prevent floodplains being way too common.

v0.1.3: Reduce spawn of mines to roughly same as stock, also add second dimension to be usable as a seed parameter since maps are always square anyways.  If you don't want a seed, having the dimensions exactly square behaves as before.  Also unlocked generating larger maps, but unsure if it works well.

v0.1.4: Add autoslope generation for better survivability out of the box.

v0.2.0: Improve errosion handling and added mac support.  The new errosion handling may change the outcome of seeds slightly (as well as slow down mapgen a little), but old maps will still load fine obviously, and the river banks will be much better.  About Mac support:  Mac users will also need to install the latest supported Python app for this to work.

v0.2.1: added map sealing to the water sources start, because water was sometimes (often) running out the back of the map.  Also some speed improvements on mapgen.  Seeds should remain consistent.

v0.2.2: Allow inputting negative values (as well as values <4) as seeds.

v0.2.3: Fix infinite loop on generation of very small maps (64x64 or less, 32x32 remains the smallest the map generator can reliably produce without errors).

v0.3.0: Slightly improved generation of hills/mountains (still a bit plains like, but more varied).  This is a seed breaking release, your old seeds will not work and need an older release.  Maps you have saved will still work, as always. You may have noticed the v0.x.0 version change often coincides indicates a seed breakage, this is intentional and can be used as a quick reference to know if your old seed will work.  FYI, I intend to support this branch for a while with seed compatability.
    
v0.3.1: Improved water sealing logic slightly.  No impact on seeds.

v0.3.2: Fix support on macs with Python 2 as default.

v0.3.3: More mac fixes.

# Feedback

If you have any feedback, please reach out to me at github

  
# Authors

- [@MSterczewski (aka Informati)](https://github.com/MSterczewski) (initial plugin code), Caps_Errors (initial python code), reddit user u/savanik (updated with river handling), R-T-B (additions of new plant life, interation to the game as a plugin, and such).

  