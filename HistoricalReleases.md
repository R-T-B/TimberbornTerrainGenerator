Historical Releases changelog:

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

v0.10.0:  Fixed a major bug that could cause infinite loops in the slope logic, as well as several severe flaws in the terrain generator itself that could cause crashes, map holes, and other issues. Also added a parameter to disable map sloping (on by default, because slopes were kind of potato). FYI, there were no releases planned until "1.0" (the release where I add an in game GUI) but I thought it best to fix these critical bugs. Unfortunately, this IS a seed breaking release, but there really was no other way to fix it. As always your old maps will still load fine. Presets have furthermore been updated to compensate for the new logic. There should be no performance differences (more to come there).

v1.0.0:  Added an ingame GUI and validation logic to allow you to edit your settings in game. Despite the major release, it should be seed compatible too!  Enjoy!

v1.0.1:  Minor bugfix for the generator not saving or obeying the ingame noise setting.

v1.0.2:  Fixed the wonky natural slope rotation that was often wrong or improper. Seed compatible other than slopes being spun the right way, heh. Also stopped overwriting old settings with this release (sorry if this happened to you).

v1.0.3:  A (hopefully final) hotfix for pathetically low blueberry and dandelion spawns.

v1.0.4:  Some minor cosmetic upgrades and fixes to the GUI, most noticable for low res users. No generation or seed changes.

v1.0.5:  Added support for reading euro decimals (appologies to our euro users). We really need full localization support, though. That is forthcoming.

v1.0.6:  Squashed a pretty major bug, if you didn't already have a settings.ini the mod failed to make a new one, leading to settings not being saved. Thanks to Discord user Todor Alin for the last few reports. :)

v1.1.0:  Another optimization run with very good improvements, thanks to github user @hytonhan! See this doc for latest benchmarks: https://github.com/R-T-B/TimberbornTerrainGenerator/blob/main/timings.md

v1.1.1:  Fix an edge case involving the custom dialog getting a bit fiesty and taking over the main menu, and then crashing, when using the cancel button too many times.

v1.1.2:  Instead of using static time and version in the save file header, we now fetch the actual time and game version. This should help as the game version updates through the future. No end user changes.

v1.1.3:  Tweaked how version number of save was reported to include build number.  Again, this is unlikely to effect end users, and is more a forward facing change.

v1.2.0:  Added buttons to original NewMapBox instead of overwriting it's VisualElement. Add settings as a separate dialog box. Now you can make the old blank maps too. Thanks to developer pmduda for help and community outreach here! More to come, including settings profiles, soon!

v1.3.0:  A new release that adds a two settings related to riverbed sloping, feel free to check them out in the GUI!  Presets have also been updated to use these new settings, where applicable.  The river sloping logic is on by default,  so this is technically seedbreaking, but you can get the old flat beds by simply setting RiverSlopeEnabled=False.

v1.3.1:  Fixed a bug in entity placement that was making entities (trees, ruins etc) not as dense as they should be.  This is not seedbreaking, except of course for entities, which are now more dense than before. Also updated PlentifulPlains preset to take advantage of river sloping.

v1.4.0: Just TimberAPI 0.5.x support.  Big things are still planned (tm), but I just got a puppy so it may be a few weeks until they are done, just FYI. :)

v1.5.0:  Just Update 3 stable support.  Needs a compatible TimberAPI.

v1.5.1:  Just some dependency fixes for the mod.  Makes sure you have the required stuff (you probably already do, but helps fresh installs).