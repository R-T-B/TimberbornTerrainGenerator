
# Random map generator

A work in progress project for generating random maps for the game video game Timberborn.  Minimum supported map size is 32x32, maximum is 384x384.  All maps are squares and the Y parameter on "new map" is used as a seed.  If the Y parameter is the same as x, it is completely random and unseeded from the user.
It once used several open source projects, including the works of Caps_Errors (initial python code), reddit user u/savanik (updated with river handling), MSterczewski (some plugin modeling), but as of the C# rewrite (version 0.9.0 release) the plugin is completely comprised of independent code, other than the use of one OSS C# noise library, known as FastNoiseLite.  FastNoise is very fast at producing noise, and well, it was helpful.

It used to be that if you were on a Mac, Python was needed for this thing to work.  That is no longer the case.  This is now a pure C# plugin with the only dependencies being it, the game, and Bepinex.

# Generating a map
Generating a map is simple!  Install the plugin, and generate a new map!  Be aware all maps will be square due a (hopefully temporary) limitation of the generator, and all maps generated will by default use the settings profile in settings.ini, codenamed "PlentifulPlains."  They are a good starting point but other map types and presets are possible with all the options.  Please look in the included ini files for examples (such as HardyHills, our other "hard mode" preset), and if you want to use them, just copy and paste the values into the actual settings.ini.  You can also edit the values individually.  If you want to do that, you'll probably want to look at the settings readme here: https://github.com/R-T-B/TimberbornTerrainGenerator/blob/main/SettingsReadme.md

Once the map is generated, feel free to run the water sim, drop a starting location somewhere nice, and load in.  That's really all there is to it, and most of the maps it makes are playable (at least on the presets, especially on PlentifulPlains). though you may want to generate a few times to "optimze" your start.  I won't judge you... heh.

# Performance
A word about performance...  The C# version we just realeased as of version 0.9.0 is both massively more powerful and more flexible...  and massively slower than the python script version that preceeded it.  This isn't due to the language.  Frankly, python is slow, and the script had no business being that fast.  The reason it was that fast was only because it was a list of hardcoded assumptions and as such, did almost no thinking.  This obviously made customizing map parameters hard however, and the C# version is the infinitely better way forward.  But yes, when you click to generate your map, be advised it MAY APPEAR FROZEN for a bit.  This is normal!  Some numbers on expected load times, on PCs ranging from Supreme to Potato, is available here: https://github.com/R-T-B/TimberbornTerrainGenerator/blob/main/timings.md

That said yes we consider performance a priority issue.  See this ticket:

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

v0.3.4: Improved erosion processing. Still supports same seeds.

v0.9.0: Ported mod to C#.  Obviously a seed breaking release, be advised.  Old maps will still load fine, as always.

v0.9.1: Seed compatible optimization run.  Up to 180% generation time improvements.  No changes or new features otherwise. Will continue to work on speed!

v0.9.2: Hotfix for entities not spawning due to a debug flag.  Performance improvements are still present, but may be less than anticipated because of this, sorry.  We are reevaluating the benchmarks and will update them soon.

v0.9.3: Small slope bugfix (they were experiencing reduced spawns in one orientation).  Also, performance rebenched properly.  We don't QUITE have a 180% increase but the increase is still healthy average of ~72% improvement.  See the timings doc listed above under "Performance" for the benches.  We will of course always be optimizing.

v0.9.4: Fixed a bug where slope placement would occasionally cause a crash.  No seed or performance impacts.

v0.9.5: Stopped terrain from being allowed to generate beneath map bottom ("holes").  No other changes, yet.

# Feedback

If you have any feedback, please reach out to me at github or on Timberborn Discord in the modding channel.

# Authors
R-T-B

  
