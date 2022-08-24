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