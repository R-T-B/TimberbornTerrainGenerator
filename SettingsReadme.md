TerrainNoiseType Options = Perlin,OpenSimplex2,Cellular = one of these values. You really should not mess with this unless you are an expert though. It will dramatically change the generation. See the FastNoise github for details. There is a generator program there that you may find handy in visualizing a lot of this: https://github.com/Auburn/FastNoiseLite

TerrainMinHeight,TerrainMaxHeight, these values are self-explanatory integers but they must respect game limits. Furthermore, be aware that MinHeight is now the actual map floor for generated terrain, not "sea level" as it used to be.  Also do not set max height above 32, it is unknown what that may do.  Or set min height at or below 0, because that may create holes.

TerrainAmplitude = a decimal/floating point number that makes things more hilly and extreme as it rises.  It also makes deep canyons more common. Don't raise it too high, can cause clipping. 10.0 is probably a reasonable ceiling.  If you just want hills, consider raising the differnce between TerrainMinHeight and TerrainMaxHeight.  That helps.

TerrainFrequencyMult = a integer that describes the noise frequency used to generate the terrain. This basically determines how far zoomed in terrain features are. The best way to describe it is if you are looking at a hill from a sky scraper vs from space, the same hill will appear much smaller. To achieve that with the generator, you'd raise this number.

TerrainSlopeLevel is a confusing value, but basically, lower values equal less slope but also lower the overall terrain height (you may need to adjust your river elevation for this, see below).

RiverNodes = a integer describing how many signifigant bends the river posseses. I know I know, you were hoping it was for extra water sources. Some day, maybe?

RiverSourceStrength = a floating point decimal that describes the power of the water current from each spawned water source.

RiverWindiness = a floating point decimal describing how much the river wanders from the center of the map. 0= a straight line, 1=wanders all over the place.

RiverWidth = an integer describing how wide the river is on a 256x256 map. It WILL be scaled down accordingly on smaller maps, but never smaller than 2.

RiverElevation = a floating point decimal that describes the elevation of the river bed. It is a value between negative 1.0 and positive 1.0. Negative 1.0 would be the map floor, and positive 1.0 would be the sky limit. Be aware this is not a forced elevation, but rather heavily weighted into the base terrain map. See below:

RiverMapWeight = a decimal describing how the rivermap is merged with the base terrain. Basically, when the river is generated, it is an independent map of just the river bed and it's altitude (RiverElevation, above). It needs to be merged with the standard terrain, but if we just average the numbers it will get weird because the river will likely not form solid borders and result in harsh flooding. Thus, we 'weight' it in the average, making it more important than the features underneath it. The end result of this is a higher RiverMapWeight will make the river more likely to be at RiverElevation and not get blocked by the TerrainMap. It will also make your river deeper and more extreme in places, so YMMV.

All entity (trees, bushes, ruins, etc) "counts" are scaled to a 256x256 map size, except mines which can have a minimum no matter how small. All the rest should be self-explanatory.
