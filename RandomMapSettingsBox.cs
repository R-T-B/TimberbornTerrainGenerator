using UnityEngine.UIElements;
using UnityEngine;
using static TimberbornTerrainGenerator.Statics;
using Timberborn.CoreUI;
using System.IO;
using Timberborn.MapEditorSceneLoading;
using TimberApi.DependencyContainerSystem;
using TimberApi.UiBuilderSystem;

namespace TimberbornTerrainGenerator
{
    public class RandomMapSettingsBox : IPanelController
    {
        //BEGIN EXTERNAL LOADABLE INI SETTINGS
        public static int MapSizeX = 128;
        public static int MapSizeY = 128;
        public static int Seed = -1;
        public static int TerrainMinHeight = 10;
        public static int TerrainMaxHeight = 20;
        public static FastNoiseLite.NoiseType TerrainNoiseType = FastNoiseLite.NoiseType.Perlin;
        public static float TerrainAmplitude = 2.5f;
        public static int TerrainFrequencyMult = 10;
        public static bool TerrainSlopeEnabled = false;
        public static float TerrainSlopeLevel = 0.8f;
        public static bool RiverSlopeEnabled = true;
        public static float RiverSlopeLevel = 0.3725f;
        public static int RiverNodes = 2;
        public static float RiverSourceStrength = 1.5f;
        public static float RiverWindiness = 0.4125f;
        public static int RiverWidth = 4;
        public static float RiverElevation = -0.45f;
        public static int RiverMapWeight = 5;
        public static int MaxMineCount = 4;
        public static int MinMineCount = 0;
        public static int RuinCount = 500;
        public static int PineTreeCount = 3600;
        public static int BirchTreeCount = 1200;
        public static int ChestnutTreeCount = 1500;
        public static int MapleTreeCount = 900;
        public static int BlueberryBushCount = 500;
        public static int DandelionBushCount = 250;
        public static int SlopeCount = 128;
        //END EXTERNAL LOADABLE INI SETTINGS
        public static int lastRadioButtonIndex = -1;
        private readonly UIBuilder builder;
        public static NineSliceTextField seedBox;
        public static NineSliceTextField mapSizeBox;
        public static TimberApi.UiBuilderSystem.CustomElements.LocalizableToggle perlinToggle;
        public static TimberApi.UiBuilderSystem.CustomElements.LocalizableToggle openSimplex2Toggle;
        public static TimberApi.UiBuilderSystem.CustomElements.LocalizableToggle cellularToggle;
        public static NineSliceTextField minHeightBox;
        public static NineSliceTextField maxHeightBox;
        public static NineSliceTextField terrainAmplitudeBox;
        public static NineSliceTextField terrainFrequencyMultBox;
        public static TimberApi.UiBuilderSystem.CustomElements.LocalizableToggle terrainSlopeEnabledToggle;
        public static TimberApi.UiBuilderSystem.CustomElements.LocalizableToggle riverSlopeEnabledToggle;
        public static NineSliceTextField terrainSlopeLevelBox;
        public static NineSliceTextField riverSlopeLevelBox;
        public static NineSliceTextField riverNodesBox;
        public static NineSliceTextField riverSourceStrengthBox;
        public static NineSliceTextField riverWindinessBox;
        public static NineSliceTextField riverWidthBox;
        public static NineSliceTextField riverElevationBox;
        public static NineSliceTextField riverMapWeightBox;
        public static NineSliceTextField maxMineCountBox;
        public static NineSliceTextField minMineCountBox;
        public static NineSliceTextField ruinCountBox;
        public static NineSliceTextField pineTreeCountBox;
        public static NineSliceTextField birchTreeCountBox;
        public static NineSliceTextField chestnutTreeCountBox;
        public static NineSliceTextField mapleTreeCountBox;
        public static NineSliceTextField blueberryBushCountBox;
        public static NineSliceTextField dandelionBushCountBox;
        public static NineSliceTextField slopeCountBox;
        
        private readonly PanelStack _panelStack;
        private readonly MapEditorSceneLoader _mapEditorSceneLoader;

        public RandomMapSettingsBox(UIBuilder uiBuilder, PanelStack panelStack, MapEditorSceneLoader mapEditorSceneLoader)
        {
            builder = uiBuilder;
            _panelStack = panelStack;
            _mapEditorSceneLoader = mapEditorSceneLoader;
        }

        public VisualElement GetPanel()
        {
            LoadINISettings();
            seedBox = builder.Presets().TextFields().InGameTextField(100, 25);
            mapSizeBox = builder.Presets().TextFields().InGameTextField(100, 25);
            perlinToggle = builder.Presets().Toggles().Circle("perlinCheckbox", default, null, default, default, FontStyle.Normal, new StyleColor(Color.white), "Perlin");
            openSimplex2Toggle = builder.Presets().Toggles().Circle("openSimplex2Checkbox", default, null, default, default, FontStyle.Normal, new StyleColor(Color.white), "OpenSimplex2");
            cellularToggle = builder.Presets().Toggles().Circle("cellularCheckbox", default, null, default, default, FontStyle.Normal, new StyleColor(Color.white), "Cellular");
            minHeightBox = builder.Presets().TextFields().InGameTextField(100, 25);
            maxHeightBox = builder.Presets().TextFields().InGameTextField(100, 25);
            terrainAmplitudeBox = builder.Presets().TextFields().InGameTextField(100, 25);
            terrainFrequencyMultBox = builder.Presets().TextFields().InGameTextField(100, 25);
            terrainSlopeEnabledToggle = builder.Presets().Toggles().Checkbox("terrainSlopeCheckbox", default, null, default, default, FontStyle.Normal, new StyleColor(Color.white));
            riverSlopeEnabledToggle = builder.Presets().Toggles().Checkbox("riverSlopeCheckbox", default, null, default, default, FontStyle.Normal, new StyleColor(Color.white));
            terrainSlopeLevelBox = builder.Presets().TextFields().InGameTextField(100, 25);
            riverSlopeLevelBox = builder.Presets().TextFields().InGameTextField(100, 25);
            riverNodesBox = builder.Presets().TextFields().InGameTextField(100, 25);
            riverSourceStrengthBox = builder.Presets().TextFields().InGameTextField(100, 25);
            riverWindinessBox = builder.Presets().TextFields().InGameTextField(100, 25);
            riverWidthBox = builder.Presets().TextFields().InGameTextField(100, 25);
            riverElevationBox = builder.Presets().TextFields().InGameTextField(100, 25);
            riverMapWeightBox = builder.Presets().TextFields().InGameTextField(100, 25);
            maxMineCountBox = builder.Presets().TextFields().InGameTextField(100, 25);
            minMineCountBox = builder.Presets().TextFields().InGameTextField(100, 25);
            ruinCountBox = builder.Presets().TextFields().InGameTextField(100, 25);
            pineTreeCountBox = builder.Presets().TextFields().InGameTextField(100, 25);
            birchTreeCountBox = builder.Presets().TextFields().InGameTextField(100, 25);
            chestnutTreeCountBox = builder.Presets().TextFields().InGameTextField(100, 25);
            mapleTreeCountBox = builder.Presets().TextFields().InGameTextField(100, 25);
            blueberryBushCountBox = builder.Presets().TextFields().InGameTextField(100, 25);
            dandelionBushCountBox = builder.Presets().TextFields().InGameTextField(100, 25);
            slopeCountBox = builder.Presets().TextFields().InGameTextField(100, 25);
            seedBox.text = Seed.ToString();
            mapSizeBox.text = MapSizeX.ToString();
            TimberApi.UiBuilderSystem.CustomElements.LocalizableButton acceptButton = builder.Presets().Buttons().ButtonGame(name: "acceptButton", text: "Accept");
            TimberApi.UiBuilderSystem.CustomElements.LocalizableButton cancelButton = builder.Presets().Buttons().ButtonGame(name: "cancelButton", 
                text: "Cancel");
            acceptButton.clicked += () => OnUIConfirmed();
            cancelButton.clicked += OnUICancelled;
            minHeightBox.text = TerrainMinHeight.ToString();
            maxHeightBox.text = TerrainMaxHeight.ToString();
            terrainAmplitudeBox.text = TerrainAmplitude.ToString();
            terrainFrequencyMultBox.text = TerrainFrequencyMult.ToString();
            terrainSlopeLevelBox.text = TerrainSlopeLevel.ToString();
            riverSlopeLevelBox.text = RiverSlopeLevel.ToString();
            riverNodesBox.text = RiverNodes.ToString();
            riverSourceStrengthBox.text = RiverSourceStrength.ToString();
            riverWindinessBox.text = RiverWindiness.ToString();
            riverWidthBox.text = RiverWidth.ToString();
            riverElevationBox.text = RiverElevation.ToString();
            riverMapWeightBox.text = RiverMapWeight.ToString();
            maxMineCountBox.text = MaxMineCount.ToString();
            minMineCountBox.text = MinMineCount.ToString();
            ruinCountBox.text = RuinCount.ToString();
            pineTreeCountBox.text = PineTreeCount.ToString();
            birchTreeCountBox.text = BirchTreeCount.ToString();
            chestnutTreeCountBox.text = ChestnutTreeCount.ToString();
            mapleTreeCountBox.text = MapleTreeCount.ToString();
            blueberryBushCountBox.text = BlueberryBushCount.ToString();
            dandelionBushCountBox.text = DandelionBushCount.ToString();
            slopeCountBox.text = SlopeCount.ToString();
            if (TerrainNoiseType.Equals(FastNoiseLite.NoiseType.Perlin))
            {
                perlinToggle.value = true;
                openSimplex2Toggle.value = false;
                cellularToggle.value = false;
            }
            else if (TerrainNoiseType.Equals(FastNoiseLite.NoiseType.OpenSimplex2))
            {
                perlinToggle.value = false;
                openSimplex2Toggle.value = true;
                cellularToggle.value = false;
            }
            else if (TerrainNoiseType.Equals(FastNoiseLite.NoiseType.Cellular))
            {
                perlinToggle.value = false;
                openSimplex2Toggle.value = false;
                cellularToggle.value = true;
            }
            if (TerrainSlopeEnabled)
            {
                terrainSlopeEnabledToggle.value = true;
            }
            else
            {
                terrainSlopeEnabledToggle.value = false;
            }
            if (RiverSlopeEnabled)
            {
                riverSlopeEnabledToggle.value = true;
            }
            else
            {
                riverSlopeEnabledToggle.value = false;
            }
            VisualElement dialogBox = builder.CreateBoxBuilder()
                .AddHeader(text: "Timberborn Terrain Generator Settings")
                .AddComponent(builder => builder
                    .SetWidth(new Length(960, Length.Unit.Pixel))
                    .SetHeight(new Length(610, Length.Unit.Pixel))
                    .SetFlexDirection(FlexDirection.Row)
                    .SetBackgroundColor(new StyleColor(new Color(0.33f, 0.31f, 0.18f, 0.5f)))
                    .SetAlignItems(Align.FlexStart)
                    .SetAlignContent(Align.FlexStart)
                    .SetFlexWrap(Wrap.Wrap)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("Seed:")))
                    .AddPreset(factory => seedBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|A seed of -1 means a completely random map, any other integer will be the same each run     " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("MapSizeXY:")))
                    .AddPreset(factory => mapSizeBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|The size of the map.  Only square maps are supported, between 32x32 and 384x384             " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("TerrainNoiseType:")))
                    .AddPreset(factory => perlinToggle)
                    .AddPreset(factory => openSimplex2Toggle)
                    .AddPreset(factory => cellularToggle)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|Determines the noise used by the generator. Experts only.                                   " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("TerrainMinHeight:")))
                    .AddPreset(factory => minHeightBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|Bottom of gen'd terrain. Must be integer greater than 0 and < TerrainMaxHeight.             " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("TerrainMaxHeight:")))
                    .AddPreset(factory => maxHeightBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|Top of gen'd terrain.  Must be integer > TerrainMinHeight and < 22.                         " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("TerrainAmplitude:")))
                    .AddPreset(factory => terrainAmplitudeBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|A decimal between 0.0 and 10.0 describing how extreme the terrain is (pits&hills).    " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("TerrainFrequencyMult:")))
                    .AddPreset(factory => terrainFrequencyMultBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|An integer describing how 'zoomed in' features are generated.  See readme.                  " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("TerrainSlopeEnabled:")))
                    .AddPreset(factory => terrainSlopeEnabledToggle)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|Whether or not Terrain is generated with a builtin slope angle.                                      " + '\u2800')))//needs more spaces because whitepsace is not the same kerning as letters
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("TerrainSlopeLevel:")))
                    .AddPreset(factory => terrainSlopeLevelBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|The angle of the generated terrain slope, a decimal between 0.0 and 1.0.                    " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("RiverSlopeEnabled:")))
                    .AddPreset(factory => riverSlopeEnabledToggle)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|Whether or not Riverbed is generated with a builtin slope angle.                                     " + '\u2800')))//needs more spaces because whitepsace is not the same kerning as letters
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("RiverSlopeLevel:")))
                    .AddPreset(factory => riverSlopeLevelBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|The angle of the generated river slope, a decimal between 0.0 and 1.0.                      " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("RiverNodes:")))
                    .AddPreset(factory => riverNodesBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|An integer describing the number of bends in the river.                                              " + '\u2800'))) //needs more spaces because whitepsace is not the same kerning as letters
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("RiverSourceStrength:")))
                    .AddPreset(factory => riverSourceStrengthBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|A decimal value describing the strength of the water sources (flow rate)                    " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("RiverWindiness:")))
                    .AddPreset(factory => riverWindinessBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|A decimal value between 0.0 and 1.0 describing how much the river 'wanders.'                " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("RiverWidth:")))
                    .AddPreset(factory => riverWidthBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|An Integer describing how wide the river is. Is scaled to map size, but always > 2.         " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("RiverElevation:")))
                    .AddPreset(factory => riverElevationBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("|A decimal between -1.0 and 1.0 describing the map height of the riverbed.                   " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("RiverMapWeight:")))
                    .AddPreset(factory => riverMapWeightBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("| An integer describing the strength of the rivermap vs the terrain heightmap.               " + '\u2800')))
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("MaxMineCount:")))
                    .AddPreset(factory => maxMineCountBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("MinMineCount:")))
                    .AddPreset(factory => minMineCountBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("RuinCount:")))
                    .AddPreset(factory => ruinCountBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("PineTreeCount:")))
                    .AddPreset(factory => pineTreeCountBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("BirchTreeCount:")))
                    .AddPreset(factory => birchTreeCountBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("ChestnutTreeCount:")))
                    .AddPreset(factory => chestnutTreeCountBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("MapleTreeCount:")))
                    .AddPreset(factory => mapleTreeCountBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("                      " + '\u2800'))) //Spacer needed for proper element placement
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("BlueberryBushCount:")))
                    .AddPreset(factory => blueberryBushCountBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("DandelionBushCount:")))
                    .AddPreset(factory => dandelionBushCountBox)
                    .AddPreset(factory => factory.Labels().DefaultBig(text: ("SlopeCount:")))
                    .AddPreset(factory => slopeCountBox)
                    .AddPreset(factory => factory.Labels().Label(text: ("All counts are scaled to a 256^2 map.")))
                    .AddPreset(factory => acceptButton)
                    .AddPreset(factory => factory.Labels().DefaultBold(text: ('\u2800' + "   REMEMBER MAPGEN CAN TAKE BETWEEN 1-2 MINUTES DEPENDING ON MAPSIZE   " + '\u2800'))) //Larger Spacer needed for proper button seperation
                    .AddPreset(factory => cancelButton)
                )
                .BuildAndInitialize();
            
            var wrapper = new VisualElement 
            {
                style = 
                {
                    flexGrow = 1,
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                    flexDirection = FlexDirection.Row,
                }
            };
            wrapper.Add(dialogBox);

            dialogBox.RegisterCallback<ChangeEvent<bool>>(UIInputValidation.OnBoolChangedEvent); ;
            dialogBox.RegisterCallback<FocusOutEvent>(UIInputValidation.OnFocusOutEvent);

            return wrapper;
        }
        
        public bool OnUIConfirmed() 
        {
            if (UIInputValidation.Validate())
            {
                TerrainGen.customMapEnabled = true;
                SaveINISettings();
                var size = new Vector2Int(MapSizeX, MapSizeY);
                _mapEditorSceneLoader.StartNewMap(size);
                return true;
            }
            return false;
        }
        
        public void OnUICancelled() 
        {
            _panelStack.Pop(this);
        }
        
        public void LoadINISettings()
        {
            //Try load .ini settings
            try
            {
                if (!File.Exists(PluginPath + "/settings.ini"))
                {
                    File.Copy(PluginPath + "/PlentifulPlains.ini", PluginPath + "/settings.ini");
                }
                IniParser iniParser = iniParser = new IniParser(PluginPath + "/settings.ini");
                MapSizeX = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "MapSizeX"));
                MapSizeY = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "MapSizeY"));
                Seed = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "Seed"));
                TerrainMinHeight = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainMinHeight"));
                TerrainMaxHeight = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainMaxHeight"));
                if (iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainNoiseType").ToLower().Equals("perlin"))
                {
                    TerrainNoiseType = FastNoiseLite.NoiseType.Perlin;
                }
                else if (iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainNoiseType").ToLower().Equals("opensimplex2"))
                {
                    TerrainNoiseType = FastNoiseLite.NoiseType.OpenSimplex2;
                }
                else if (iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainNoiseType").ToLower().Equals("cellular"))
                {
                    TerrainNoiseType = FastNoiseLite.NoiseType.Cellular;
                }
                else
                {
                    TerrainNoiseType = FastNoiseLite.NoiseType.Perlin;
                    Debug.LogWarning("Unable to determine noise settings, proceeding with Perlin Noise.");
                }
                TerrainAmplitude = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainAmplitude"));
                TerrainFrequencyMult = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainFrequencyMult"));
                TerrainSlopeEnabled = bool.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainSlopeEnabled"));
                TerrainSlopeLevel = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainSlopeLevel"));
                RiverSlopeEnabled = bool.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverSlopeEnabled"));
                RiverSlopeLevel = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverSlopeLevel"));
                RiverNodes = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverNodes"));
                RiverSourceStrength = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverSourceStrength"));
                RiverWindiness = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverWindiness"));
                RiverWidth = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverWidth"));
                RiverElevation = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverElevation"));
                RiverMapWeight = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverMapWeight"));
                MaxMineCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "MaxMineCount"));
                MinMineCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "MinMineCount"));
                RuinCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RuinCount"));
                PineTreeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "PineTreeCount"));
                BirchTreeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "BirchTreeCount"));
                ChestnutTreeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "ChestnutTreeCount"));
                MapleTreeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "MapleTreeCount"));
                BlueberryBushCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "BlueberryBushCount"));
                DandelionBushCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "DandelionBushCount"));
                SlopeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "SlopeCount"));
            }
            catch
            {
                Debug.LogWarning("Unable to load settings file, using default parameters!"); //Fail?
            }
        }
        public void SaveINISettings()
        {
            //Try save .ini settings
            try
            {
                if (!File.Exists(PluginPath + "/settings.ini"))
                {
                    File.Copy(PluginPath + "/PlentifulPlains.ini", PluginPath + "/settings.ini");
                }
                IniParser iniParser = new IniParser(PluginPath + "/settings.ini");
                iniParser.AddSetting("TimberbornTerrainGenerator", "MapSizeX", MapSizeX.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "MapSizeY", MapSizeY.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "Seed", Seed.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "TerrainMinHeight", TerrainMinHeight.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "TerrainMaxHeight", TerrainMaxHeight.ToString());

                if (TerrainNoiseType.ToString().ToLower().Equals("perlin"))
                {
                    iniParser.AddSetting("TimberbornTerrainGenerator", "TerrainNoiseType", "Perlin");
                }
                else if (TerrainNoiseType.ToString().ToLower().Equals("opensimplex2"))
                {
                    iniParser.AddSetting("TimberbornTerrainGenerator", "TerrainNoiseType", "OpenSimplex2");
                }
                else if (TerrainNoiseType.ToString().ToLower().Equals("cellular"))
                {
                    iniParser.AddSetting("TimberbornTerrainGenerator", "TerrainNoiseType", "Cellular");
                }
                else
                {
                    iniParser.AddSetting("TimberbornTerrainGenerator", "TerrainNoiseType", "Perlin");
                    Debug.LogWarning("Unable to determine noise settings, proceeding to save with default Perlin Noise.");
                }
                iniParser.AddSetting("TimberbornTerrainGenerator", "TerrainAmplitude", TerrainAmplitude.ToString().Replace(",", "."));
                iniParser.AddSetting("TimberbornTerrainGenerator", "TerrainFrequencyMult", TerrainFrequencyMult.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "TerrainSlopeEnabled", TerrainSlopeEnabled.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "TerrainSlopeLevel", TerrainSlopeLevel.ToString().Replace(",", "."));
                iniParser.AddSetting("TimberbornTerrainGenerator", "RiverSlopeEnabled", RiverSlopeEnabled.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "RiverSlopeLevel", RiverSlopeLevel.ToString().Replace(",", "."));
                iniParser.AddSetting("TimberbornTerrainGenerator", "RiverNodes", RiverNodes.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "RiverSourceStrength", RiverSourceStrength.ToString().Replace(",", "."));
                iniParser.AddSetting("TimberbornTerrainGenerator", "RiverWindiness", RiverWindiness.ToString().Replace(",", "."));
                iniParser.AddSetting("TimberbornTerrainGenerator", "RiverWidth", RiverWidth.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "RiverElevation", RiverElevation.ToString().Replace(",", "."));
                iniParser.AddSetting("TimberbornTerrainGenerator", "RiverMapWeight", RiverMapWeight.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "MaxMineCount", MaxMineCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "MinMineCount", MinMineCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "RuinCount", RuinCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "PineTreeCount", PineTreeCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "BirchTreeCount", BirchTreeCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "ChestnutTreeCount", ChestnutTreeCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "MapleTreeCount", MapleTreeCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "BlueberryBushCount", BlueberryBushCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "DandelionBushCount", DandelionBushCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "SlopeCount", SlopeCount.ToString());
                iniParser.SaveSettings();
            }
            catch
            {
                Debug.LogWarning("Unable to save entirety of settings file, parameters may be lost!");
            }
        }
    }
}