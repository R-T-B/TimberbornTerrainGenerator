using UnityEngine.UIElements;
using UnityEngine;
using static TimberbornTerrainGenerator.Statics;
using Timberborn.CoreUI;
using System.IO;
using Timberborn.MapEditorSceneLoading;
using TimberApi.DependencyContainerSystem;
using TimberApi.UIBuilderSystem;
using Timberborn.SettingsSystem;
using System.Collections.Generic;
using TimberApi.UIBuilderSystem.ElementBuilders;
using TimberApi.UIPresets.TextFields;
using TimberApi.UIPresets.Buttons;
using TimberApi.UIPresets.Builders;
using TimberApi.UIBuilderSystem.StyleSheetSystem.PropertyEnums;
using System.Reflection.Emit;

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
        public static int BadWaterCount = 4;
        public static int RuinCount = 500;
        public static int PineTreeCount = 3600;
        public static int BirchTreeCount = 2400;
        public static int OakTreeCount = 1800;
        public static int BlueberryBushCount = 500;
        public static int SlopeCount = 128;
        //END EXTERNAL LOADABLE INI SETTINGS
        public static int lastRadioButtonIndex = -1;
        private readonly UIBuilder builder;
        public static NineSliceTextField seedBox;
        public static NineSliceTextField mapSizeBox;
        public static Toggle perlinToggle;
        public static Toggle openSimplex2Toggle;
        public static Toggle cellularToggle;
        public static NineSliceTextField minHeightBox;
        public static NineSliceTextField maxHeightBox;
        public static NineSliceTextField terrainAmplitudeBox;
        public static NineSliceTextField terrainFrequencyMultBox;
        public static Toggle terrainSlopeEnabledToggle;
        public static Toggle riverSlopeEnabledToggle;
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
        public static NineSliceTextField badWaterCountBox;
        public static NineSliceTextField ruinCountBox;
        public static NineSliceTextField pineTreeCountBox;
        public static NineSliceTextField birchTreeCountBox;
        public static NineSliceTextField oakTreeCountBox;
        public static NineSliceTextField blueberryBushCountBox;
        public static NineSliceTextField slopeCountBox;
        public static ListView filenameListBox;
        public static NineSliceTextField newFileNameBox;
        public static string[] fileList;
        public static VisualElement wrapper = new VisualElement
        {
            style =
                {
                    flexGrow = 1,
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                    flexDirection = UnityEngine.UIElements.FlexDirection.Row,
                }
        };

        private readonly PanelStack _panelStack;
        private readonly MapEditorSceneLoader _mapEditorSceneLoader;
        private static DialogBoxShower _dialogBoxShower = DependencyContainer.GetInstance<DialogBoxShower>();

        public RandomMapSettingsBox(UIBuilder uiBuilder, PanelStack panelStack, MapEditorSceneLoader mapEditorSceneLoader)
        {
            builder = uiBuilder;
            _panelStack = panelStack;
            _mapEditorSceneLoader = mapEditorSceneLoader;
        }
        public void refreshGUI()
        {
            seedBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            mapSizeBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            perlinToggle = builder.Create<ToggleBuilder>().SetText("Perlin").Build();
            openSimplex2Toggle = builder.Create<ToggleBuilder>().SetText("OpenSimplex2").Build();
            cellularToggle = builder.Create<ToggleBuilder>().SetText("Cellular").Build();
            minHeightBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            maxHeightBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            terrainAmplitudeBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            terrainFrequencyMultBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            terrainSlopeEnabledToggle = builder.Create<ToggleBuilder>().Build();
            riverSlopeEnabledToggle = builder.Create<ToggleBuilder>().Build();
            terrainSlopeLevelBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            riverSlopeLevelBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            riverNodesBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            riverSourceStrengthBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            riverWindinessBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            riverWidthBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            riverElevationBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            riverMapWeightBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            maxMineCountBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            minMineCountBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            badWaterCountBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            ruinCountBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            pineTreeCountBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            birchTreeCountBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            oakTreeCountBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            blueberryBushCountBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            slopeCountBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            SetupFirstTimeConfigPresets();
            PopulateFileList();
            filenameListBox = builder.Create<ListViewBuilder>().SetItemSource(fileList).SetSelectionType(SelectionType.Single).SetHeight(new Length(515, LengthUnit.Pixel)).SetWidth(new Length(125, LengthUnit.Pixel)).Build();
            newFileNameBox = builder.Create<TextFieldBuilder>().SetBackgroundColor(Color.white).Build();
            LoadINISettings("stocksettings");
            TimberApi.UIBuilderSystem.CustomElements.LocalizableButton acceptButton = builder.Create<MenuButton>().Build();
            acceptButton.text = "Accept";
            TimberApi.UIBuilderSystem.CustomElements.LocalizableButton cancelButton = builder.Create<MenuButton>().Build();
            cancelButton.text = "Cancel";
            TimberApi.UIBuilderSystem.CustomElements.LocalizableButton saveButton = builder.Create<MenuButton>().Build();
            saveButton.text = "Save Profile";
            TimberApi.UIBuilderSystem.CustomElements.LocalizableButton newSaveButton = builder.Create<MenuButton>().Build();
            newSaveButton.text = "Save New Profile";
            TimberApi.UIBuilderSystem.CustomElements.LocalizableButton loadButton = builder.Create<MenuButton>().Build();
            loadButton.text = "Load Profile";
            TimberApi.UIBuilderSystem.CustomElements.LocalizableButton deleteButton = builder.Create<MenuButton>().Build();
            deleteButton.text = "Delete Profile";
            acceptButton.clicked += () => OnUIConfirmed();
            cancelButton.clicked += OnUICancelled;
            saveButton.clicked += () => saveButtonMethod();
            deleteButton.clicked += () => deleteButtonMethod();
            newSaveButton.clicked += () => newSaveButtonMethod();
            loadButton.clicked += () => loadButtonMethod();

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
            VisualElement settingsDialogBox = builder.Create<VisualElementBuilder>("Timberborn Terrain Generator Settings")
        .SetName("Timberborn Terrain Generator Settings")
        .SetWidth(new Length(960, LengthUnit.Pixel))
        .SetHeight(new Length(630, LengthUnit.Pixel))
        .SetFlexDirection(UnityEngine.UIElements.FlexDirection.Row)
        .SetBackgroundColor(new StyleColor(new Color(0.33f, 0.31f, 0.18f, 1.0f)))
        .SetAlignItems(Align.FlexStart)
        .SetAlignContent(Align.FlexStart)
        .SetFlexWrap(Wrap.Wrap)
        .AddComponent<LabelBuilder>(factory => factory.SetText("Seed:"))
        .AddComponent(seedBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|A seed of -1 means a completely random map, any other integer will be the same each run     " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("MapSizeXY:"))
        .AddComponent(mapSizeBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|The size of the map.  Only square maps are supported, between 32x32 and 384x384             " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("TerrainNoiseType:"))
        .AddComponent(perlinToggle)
        .AddComponent(openSimplex2Toggle)
        .AddComponent(cellularToggle)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|Determines the noise used by the generator. Experts only.     " + '\u2800'))//needs less spaces because whitepsace is not the same kerning as letters
        .AddComponent<LabelBuilder>(factory => factory.SetText("TerrainMinHeight:"))
        .AddComponent(minHeightBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|Bottom of gen'd terrain. Must be integer greater than 0 and < TerrainMaxHeight.             " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("TerrainMaxHeight:"))
        .AddComponent(maxHeightBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|Top of gen'd terrain.  Must be integer > TerrainMinHeight and < 22.                         " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("TerrainAmplitude:"))
        .AddComponent(terrainAmplitudeBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|A decimal between 0.0 and 10.0 describing how extreme the terrain is (pits&hills).    " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("TerrainFrequencyMult:"))
        .AddComponent(terrainFrequencyMultBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|An integer describing how 'zoomed in' features are generated.  See readme.                  " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("TerrainSlopeEnabled:"))
        .AddComponent(terrainSlopeEnabledToggle)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|Whether or not Terrain is generated with a builtin slope angle.                                      " + '\u2800'))//needs more spaces because whitepsace is not the same kerning as letters
        .AddComponent<LabelBuilder>(factory => factory.SetText("TerrainSlopeLevel:"))
        .AddComponent(terrainSlopeLevelBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|The angle of the generated terrain slope, a decimal between 0.0 and 1.0.                    " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("RiverSlopeEnabled:"))
        .AddComponent(riverSlopeEnabledToggle)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|Whether or not Riverbed is generated with a builtin slope angle.                                     " + '\u2800'))//needs more spaces because whitepsace is not the same kerning as letters
        .AddComponent<LabelBuilder>(factory => factory.SetText("RiverSlopeLevel:"))
        .AddComponent(riverSlopeLevelBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|The angle of the generated river slope, a decimal between 0.0 and 1.0.                      " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("RiverNodes:"))
        .AddComponent(riverNodesBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|An integer describing the number of bends in the river.                                              " + '\u2800')) //needs more spaces because whitepsace is not the same kerning as letters
        .AddComponent<LabelBuilder>(factory => factory.SetText("RiverSourceStrength:"))
        .AddComponent(riverSourceStrengthBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|A decimal value describing the strength of the water sources (flow rate)                    " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("RiverWindiness:"))
        .AddComponent(riverWindinessBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|A decimal value between 0.0 and 1.0 describing how much the river 'wanders.'                " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("RiverWidth:"))
        .AddComponent(riverWidthBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|An Integer describing how wide the river is. Is scaled to map size, but always > 2.         " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("RiverElevation:"))
        .AddComponent(riverElevationBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("|A decimal between -1.0 and 1.0 describing the map height of the riverbed.                   " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("RiverMapWeight:"))
        .AddComponent(riverMapWeightBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("| An integer describing the strength of the rivermap vs the terrain heightmap.               " + '\u2800'))
        .AddComponent<LabelBuilder>(factory => factory.SetText("MaxMineCount:"))
        .AddComponent(maxMineCountBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("MinMineCount:"))
        .AddComponent(minMineCountBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("RuinCount:"))
        .AddComponent(ruinCountBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("PineTreeCount:"))
        .AddComponent(pineTreeCountBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("BirchTreeCount:"))
        .AddComponent(birchTreeCountBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("OakTreeCount:"))
        .AddComponent(oakTreeCountBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("BlueberryBushCount:"))
        .AddComponent(blueberryBushCountBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("SlopeCount:"))
        .AddComponent(slopeCountBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("BadWaterCount:"))
        .AddComponent(badWaterCountBox)
        .AddComponent<LabelBuilder>(factory => factory.SetText("                                                                                             " + '\u2800')) //Spacer needed for proper element placement
        .AddComponent<LabelBuilder>(factory => factory.SetText("All counts are scaled to a 256^2 map.").SetColor(new Color(0.5f, 0.5f, 0.5f)))
        .AddComponent(acceptButton)
        .AddComponent<LabelBuilder>(factory => factory.SetText('\u2800' + " REMEMBER GEN CAN TAKE BETWEEN 1-2 MINUTES DEPENDING ON MAPSIZE " + '\u2800')) //Larger Spacer needed for proper button seperation
        .AddComponent(cancelButton)
        .Build();
         VisualElement loadSaveDialogBox = builder.Create<VisualElementBuilder>("Load/Save Profiles")
                .SetName("Load/Save Profiles")
                    .SetWidth(new Length(200, LengthUnit.Pixel))
                    .SetHeight(new Length(620, LengthUnit.Pixel))
                    .SetFlexDirection(UnityEngine.UIElements.FlexDirection.Row)
                    .SetBackgroundColor(new StyleColor(new Color(0.5f, 0.5f, 0.5f, 1f)))
                    .SetAlignItems(Align.FlexStart)
                    .SetAlignContent(Align.FlexStart)
                    .SetFlexWrap(Wrap.Wrap)
                    .AddComponent(filenameListBox)
                    .AddComponent(saveButton)
                    .AddComponent(loadButton)
                    .AddComponent(deleteButton)
                    .AddComponent<LabelBuilder>(factory => factory.SetText("FileName:"))
                    .AddComponent(newFileNameBox)
                    .AddComponent(newSaveButton)
                .Build();

            wrapper.Clear();
            wrapper.Add(settingsDialogBox);
            wrapper.Add(loadSaveDialogBox);

            settingsDialogBox.RegisterCallback<ChangeEvent<bool>>(UIInputValidation.OnBoolChangedEvent);
            settingsDialogBox.RegisterCallback<FocusOutEvent>(UIInputValidation.OnFocusOutEvent);
            loadSaveDialogBox.RegisterCallback<ChangeEvent<bool>>(UIInputValidation.OnBoolChangedEvent);
            loadSaveDialogBox.RegisterCallback<FocusOutEvent>(UIInputValidation.OnFocusOutEvent);
        }

        public VisualElement GetPanel()
        {
            refreshGUI();
            return wrapper;
        }
        
        public bool OnUIConfirmed() 
        {
            if (UIInputValidation.Validate())
            {
                TerrainGen.customMapEnabled = true;
                SaveINISettings("stocksettings");
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
        public bool saveButtonMethod()
        {
            try
            {
                SaveINISettings("stocksettings");
                return SaveINISettings(filenameListBox.selectedItem.ToString());
            }
            catch
            {
                //any file save fails caught
                return false;
            }
        }
        public bool deleteButtonMethod()
        {
            SaveINISettings("stocksettings"); //first make sure the old settings are recorded from the GUI.
            _dialogBoxShower.Create().SetMessage("Are you sure you want to delete the profile you just highlighted?").SetConfirmButton(delegate () { deleteHighlightedFile(); }, "Yes").SetCancelButton(delegate () { }, "No").Show();
            return true;
        }
        public void deleteHighlightedFile()
        {
            try
            {
                File.Delete(GetTrueFilePath(filenameListBox.selectedItem.ToString()));
                PopulateFileList();
                refreshGUI();
            }
            catch
            {
                //if no file was highlighted abort.
            }
        }
        public bool newSaveButtonMethod()
        {
            if (newFileNameBox.text.Equals(""))
            {
                return false;
            }
            else
            {
                try
                {
                    SaveINISettings("stocksettings");
                    SaveINISettings(newFileNameBox.text);
                    PopulateFileList();
                    refreshGUI();
                    return true;
                }
                catch
                {
                    //any file save fails caught
                    return false;
                }
            }

        }
        public bool loadButtonMethod()
        {
            try
            {
                return LoadINISettings(filenameListBox.selectedItem.ToString());
            }
            catch
            {
                return false;
                //if no file was highlighted abort.
            }
        }

        public string GetTrueFilePath(string filename)
        {
            return ConfigPath + "/" + filename + ".ini";
        }

        public void SetupFirstTimeConfigPresets()
        {
            if (!File.Exists(ConfigPath + "/HardyHills.ini"))
            {
                File.Copy(PluginPath + "/HardyHills.ini", ConfigPath + "/HardyHills.ini");
            }
            if (!File.Exists(ConfigPath + "/MegaMesas.ini"))
            {
                File.Copy(PluginPath + "/MegaMesas.ini", ConfigPath + "/MegaMesas.ini");
            }
            if (!File.Exists(ConfigPath + "/PlentifulPlains.ini"))
            {
                File.Copy(PluginPath + "/PlentifulPlains.ini", ConfigPath + "/PlentifulPlains.ini");
            }
        }

        public void PopulateFileList()
        {
            List<string> arrayFileList = new List<string>(Directory.GetFiles(ConfigPath));
            //fileList =
            //now trim out extensions and stocksettings file
            int x = 0;
            while (x < arrayFileList.Count)
            {
                arrayFileList[x] = Path.GetFileNameWithoutExtension(arrayFileList[x]);
                x++;
            }
            if (arrayFileList.Contains("stocksettings"))
            {
                arrayFileList.Remove("stocksettings");
            }
            fileList = arrayFileList.ToArray();
        }

        public bool LoadINISettings(string origFilename)
        {
            string filename = GetTrueFilePath(origFilename);
            //Try load .ini settings
            try
            {
                if (!File.Exists(filename))
                { 
                    File.Copy(PluginPath + "/PlentifulPlains.ini", filename);
                }
                IniParser iniParser = iniParser = new IniParser(filename);
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
                BadWaterCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "BadWaterCount"));
                RuinCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RuinCount"));
                PineTreeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "PineTreeCount"));
                BirchTreeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "BirchTreeCount"));
                OakTreeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "OakTreeCount"));
                BlueberryBushCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "BlueberryBushCount"));
                SlopeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "SlopeCount"));
                seedBox.text = Seed.ToString();
                mapSizeBox.text = MapSizeX.ToString();
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
                badWaterCountBox.text = BadWaterCount.ToString();
                ruinCountBox.text = RuinCount.ToString();
                pineTreeCountBox.text = PineTreeCount.ToString();
                birchTreeCountBox.text = BirchTreeCount.ToString();
                oakTreeCountBox.text = OakTreeCount.ToString();
                blueberryBushCountBox.text = BlueberryBushCount.ToString();
                slopeCountBox.text = SlopeCount.ToString();
                SaveINISettings("stocksettings"); //Make sure the new settings are recorded from the GUI.
                return true;
            }
            catch
            {
                Debug.LogWarning("Unable to load settings file, using default parameters!"); //Fail?
                return false;
            }
        }
        public bool SaveINISettings(string origFilename)
        {
            string filename = GetTrueFilePath(origFilename);
            //Try save .ini settings
            try
            {
                if (!File.Exists(filename))
                {
                    File.Copy(PluginPath + "/PlentifulPlains.ini", filename);
                }
                IniParser iniParser = new IniParser(filename);
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
                iniParser.AddSetting("TimberbornTerrainGenerator", "BadWaterCount", BadWaterCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "RuinCount", RuinCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "PineTreeCount", PineTreeCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "BirchTreeCount", BirchTreeCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "OakTreeCount", OakTreeCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "BlueberryBushCount", BlueberryBushCount.ToString());
                iniParser.AddSetting("TimberbornTerrainGenerator", "SlopeCount", SlopeCount.ToString());
                iniParser.SaveSettings();
                PopulateFileList();
                return true;
            }
            catch
            {
                Debug.LogWarning("Unable to save entirety of settings file, parameters may be lost!");
                return false;
            }
        }
    }
}