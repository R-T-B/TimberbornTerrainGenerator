
# Random map generator

A work in progress project for generating random maps for the game video game Timberborn. Minimum supported map size is 32x32, maximum is 384x384. All maps are squares and the Y parameter on "new map" is used as a seed. If the Y parameter is the same as x, it is completely random and unseeded from the user.
It once used several open source projects, including the works of Caps_Errors (initial python code), reddit user u/savanik (updated with river handling), MSterczewski (some plugin modeling), but as of the C# rewrite (version 0.9.0 release) the plugin is completely comprised of independent code, other than the use of one OSS C# noise library, known as FastNoiseLite. FastNoise is very fast at producing noise, and well, it was helpful.

It used to be that if you were on a Mac, Python was needed for this thing to work. That is no longer the case. This is now a pure C# plugin with the only dependencies being it, the game, Bepinex, and TimberAPI.

# Generating a map
Generating a map is simple!  Install the plugin, and generate a new map!  Be aware all maps will be square due a (hopefully temporary) limitation of the generator, and all maps generated will by default use the settings profile in settings.ini, codenamed "PlentifulPlains."  They are a good starting point but other map types and presets are possible with all the options. Please look in the included ini files for examples (such as HardyHills, our other "hard mode" preset), and if you want to use them, just copy and paste the values into the actual settings.ini. You can also edit the values individually. If you want to do that, you'll probably want to look at the settings readme here: https://github.com/R-T-B/TimberbornTerrainGenerator/blob/main/SettingsReadme.md

Once the map is generated, feel free to run the water sim, drop a starting location somewhere nice, and load in. That's really all there is to it, and most of the maps it makes are playable (at least on the presets, especially on PlentifulPlains). though you may want to generate a few times to "optimze" your start. I won't judge you... heh.

# Installation
In order to install the mod follow the instructions from BepInEx website:
https://docs.bepinex.dev/articles/user_guide/installation/index.html

As of 08/24/2022, we no longer track releases beyond two major subversions back in the main readme. See the elder release notes here: https://github.com/R-T-B/TimberbornTerrainGenerator/blob/main/HistoricalReleases.md

Changelog:

v1.3.0:  A new release that adds a two settings related to riverbed sloping, feel free to check them out in the GUI!  Presets have also been updated to use these new settings, where applicable.  The river sloping logic is on by default,  so this is technically seedbreaking, but you can get the old flat beds by simply setting RiverSlopeEnabled=False.

v1.3.1:  Fixed a bug in entity placement that was making entities (trees, ruins etc) not as dense as they should be.  This is not seedbreaking, except of course for entities, which are now more dense than before. Also updated PlentifulPlains preset to take advantage of river sloping.

v1.4.0: Just TimberAPI 0.5.x support.  Big things are still planned (tm), but I just got a puppy so it may be a few weeks until they are done, just FYI. :)

v1.4.1: Just some annoying GUI fixes.

v1.4.2: Just getting back to coding.  Fixed the annoying inverted depth map on the river sloping. :)

# Feedback

If you have any feedback, please reach out to me at github or on Timberborn Discord in the modding channel.

# Authors
R-T-B

  
