
# Random map generator

A work in progress project for generating random maps for the game video game Timberborn.  Minimum supported map size is 32x32, maximum is 384x384.  All maps are squares and the Y parameter on "new map" is used as a seed.  If the Y parameter is the same as x, it is completely random and unseeded from the user.
It once used several open source projects, including the works of Caps_Errors (initial python code), reddit user u/savanik (updated with river handling), MSterczewski (some plugin modeling), but as of the C# rewrite (version 0.9.0 release) the plugin is completely comprised of independent code, other than the use of one OSS C# noise library, known as FastNoiseLite.  FastNoise is very fast at producing noise, and well, it was helpful.

It used to be that if you were on a Mac, Python was needed for this thing to work.  That is no longer the case.  This is now a pure C# plugin with the only dependencies being it, the game, Bepinex, and TimberAPI.

# Generating a map
Generating a map is simple!  Install the plugin, and generate a new map!  Be aware all maps will be square due a (hopefully temporary) limitation of the generator, and all maps generated will by default use the settings profile in settings.ini, codenamed "PlentifulPlains."  They are a good starting point but other map types and presets are possible with all the options.  Please look in the included ini files for examples (such as HardyHills, our other "hard mode" preset), and if you want to use them, just copy and paste the values into the actual settings.ini.  You can also edit the values individually.  If you want to do that, you'll probably want to look at the settings readme here: https://github.com/R-T-B/TimberbornTerrainGenerator/blob/main/SettingsReadme.md

Once the map is generated, feel free to run the water sim, drop a starting location somewhere nice, and load in.  That's really all there is to it, and most of the maps it makes are playable (at least on the presets, especially on PlentifulPlains). though you may want to generate a few times to "optimze" your start.  I won't judge you... heh.

# Performance
A word about performance...  The C# version we just realeased as of version 0.9.0 is both massively more powerful and more flexible...  and massively slower than the python script version that preceeded it.  This isn't due to the language.  Frankly, python is slow, and the script had no business being that fast.  The reason it was that fast was only because it was a list of hardcoded assumptions and as such, did almost no thinking.  This obviously made customizing map parameters hard however, and the C# version is the infinitely better way forward.  But yes, when you click to generate your map, be advised it MAY APPEAR FROZEN for a bit.  This is normal!  Some numbers on expected load times, on PCs ranging from Supreme to Potato, is available here: https://github.com/R-T-B/TimberbornTerrainGenerator/blob/main/timings.md

That said yes we consider performance a priority issue.  See this ticket:  https://github.com/R-T-B/TimberbornTerrainGenerator/issues/3

# Installation
In order to install the mod follow the instructions from BepInEx website:
https://docs.bepinex.dev/articles/user_guide/installation/index.html

Changelog:

v0.10.0:  Fixed a major bug that could cause infinite loops in the slope logic, as well as several severe flaws in the terrain generator itself that could cause crashes, map holes, and other issues.  Also added a parameter to disable map sloping (on by default, because slopes were kind of potato).  FYI, there were no releases planned until "1.0" (the release where I add an in game GUI) but I thought it best to fix these critical bugs.  Unfortunately, this IS a seed breaking release, but there really was no other way to fix it.  As always your old maps will still load fine.  Presets have furthermore been updated to compensate for the new logic.  There should be no performance differences (more to come there).

v1.0.0:  Added an ingame GUI and validation logic to allow you to edit your settings in game.  Despite the major release, it should be seed compatible too!  Enjoy!

v1.0.1:  Minor bugfix for the generator not saving or obeying the ingame noise setting.

v1.0.2:  Fixed the wonky natural slope rotation that was often wrong or improper.  Seed compatible other than slopes being spun the right way, heh. Also stopped overwriting old settings with this release (sorry if this happened to you).

v1.0.3:  A (hopefully final) hotfix for pathetically low blueberry and dandelion spawns.

v1.0.4:  Some minor cosmetic upgrades and fixes to the GUI, most noticable for low res users. No generation or seed changes.

As of 08/24/2022, we no longer track releases beyond two major subversions back in the main readme.  See the elder release notes here: https://github.com/R-T-B/TimberbornTerrainGenerator/blob/main/HistoricalReleases.md

# Feedback

If you have any feedback, please reach out to me at github or on Timberborn Discord in the modding channel.

# Authors
R-T-B

  
