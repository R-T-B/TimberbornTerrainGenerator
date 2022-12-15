using HarmonyLib;
using Timberborn.MapSystemUI;
using UnityEngine.UIElements;
using static TimberbornTerrainGenerator.RandomMapSettingsBox;
using Timberborn.CoreUI;
using System;
using TimberApi.DependencyContainerSystem;

namespace TimberbornTerrainGenerator
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(NewMapBox), "TryParseSize")]
    public static class UIInputValidation
    {
        public static DialogBoxShower _dialogBoxShower = DependencyContainer.GetInstance<DialogBoxShower>();

        static int resultX = 0;
        static int resultY = 0;
        public static bool Validate()
        {
            var result = ValidateStrings();
            ValidateBools();
            return result;
        }
        public static void OnBoolChangedEvent(ChangeEvent<bool> evt)
        {
            ValidateBools();
        }
        public static void OnFocusOutEvent(FocusOutEvent evt)
        {
            ValidateStrings();
        }
        private static void ValidateBools()
        {
            if ((perlinToggle.value) && (lastRadioButtonIndex != 0))
            {
                openSimplex2Toggle.value = false;
                cellularToggle.value = false;
                lastRadioButtonIndex = 0;
                TerrainNoiseType = FastNoiseLite.NoiseType.Perlin;
            }
            else if ((openSimplex2Toggle.value) && (lastRadioButtonIndex != 1))
            {
                perlinToggle.value = false;
                cellularToggle.value = false;
                lastRadioButtonIndex = 1;
                TerrainNoiseType = FastNoiseLite.NoiseType.OpenSimplex2;
            }
            else if ((cellularToggle.value) && (lastRadioButtonIndex != 2))
            {
                perlinToggle.value = false;
                openSimplex2Toggle.value = false;
                lastRadioButtonIndex = 2;
                TerrainNoiseType = FastNoiseLite.NoiseType.Cellular;
            }
            TerrainSlopeEnabled = terrainSlopeEnabledToggle.value;
            RiverSlopeEnabled = riverSlopeEnabledToggle.value;
        }
        private static bool ValidateStrings()
        {
            try
            {
                Seed = int.Parse(seedBox.text);
            }
            catch
            {
                seedBox.text = Seed.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
            }
            try
            {
                int evalMapSizeInt = int.Parse(mapSizeBox.text);
                if ((evalMapSizeInt >= 32) && (evalMapSizeInt <= 384))
                {
                    MapSizeX = evalMapSizeInt;
                    MapSizeY = MapSizeX;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                mapSizeBox.text = MapSizeX.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalMinHeight = int.Parse(minHeightBox.text);
                int evalMaxHeight = int.Parse(maxHeightBox.text);
                if (((evalMinHeight > 0) && (evalMinHeight < evalMaxHeight)) && ((evalMaxHeight < 22) && (evalMaxHeight > evalMinHeight)))
                {
                    TerrainMinHeight = evalMinHeight;
                    TerrainMaxHeight = evalMaxHeight;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                minHeightBox.text = TerrainMinHeight.ToString();
                maxHeightBox.text = TerrainMaxHeight.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                float evalAmplitude = float.Parse(terrainAmplitudeBox.text.Replace(",", "."));
                if ((evalAmplitude > 0.0f) && (evalAmplitude < 10.0f))
                {
                    TerrainAmplitude = evalAmplitude;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                terrainAmplitudeBox.text = TerrainAmplitude.ToString().Replace(",", ".");
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalFrequencyMult = int.Parse(terrainFrequencyMultBox.text);
                if (evalFrequencyMult >= 1)
                {
                    TerrainFrequencyMult = evalFrequencyMult;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                terrainFrequencyMultBox.text = TerrainFrequencyMult.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                float evalSlopeLevel = float.Parse(terrainSlopeLevelBox.text.Replace(",", "."));
                if ((evalSlopeLevel > 0.0f) && (evalSlopeLevel <= 1.0f))
                {
                    TerrainSlopeLevel = evalSlopeLevel;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                terrainSlopeLevelBox.text = TerrainSlopeLevel.ToString().Replace(",", ".");
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                float evalSlopeLevel = float.Parse(riverSlopeLevelBox.text.Replace(",", "."));
                if ((evalSlopeLevel > 0.0f) && (evalSlopeLevel <= 1.0f))
                {
                    RiverSlopeLevel = evalSlopeLevel;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                riverSlopeLevelBox.text = RiverSlopeLevel.ToString().Replace(",", ".");
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalRiverNodes = int.Parse(riverNodesBox.text);
                if (evalRiverNodes > 0)
                {
                    RiverNodes = evalRiverNodes;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                riverNodesBox.text = RiverNodes.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                float evalRiverSourceStrength = float.Parse(riverSourceStrengthBox.text.Replace(",", "."));
                if (evalRiverSourceStrength > 0.0f)
                {
                    RiverSourceStrength = evalRiverSourceStrength;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                riverSourceStrengthBox.text = RiverSourceStrength.ToString().Replace(",", ".");
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                float evalRiverWindiness = float.Parse(riverWindinessBox.text.Replace(",", "."));
                if ((evalRiverWindiness >= 0.0f) && (evalRiverWindiness <= 1.0f))
                {
                    RiverWindiness = evalRiverWindiness;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                riverWindinessBox.text = RiverWindiness.ToString().Replace(",", ".");
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalRiverWidth = int.Parse(riverWidthBox.text);
                if (evalRiverWidth > 0)
                {
                    RiverWidth = evalRiverWidth;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                riverWidthBox.text = RiverWidth.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                float evalRiverElevation = float.Parse(riverElevationBox.text.Replace(",", "."));
                if ((evalRiverElevation >= -1.0f) && (evalRiverElevation <= 1.0f))
                {
                    RiverElevation = evalRiverElevation;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                riverElevationBox.text = RiverElevation.ToString().Replace(",", ".");
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalRiverMapWeight = int.Parse(riverMapWeightBox.text);
                if (evalRiverMapWeight > 0)
                {
                    RiverMapWeight = evalRiverMapWeight;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                riverMapWeightBox.text = RiverMapWeight.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalMaxMineCount = int.Parse(maxMineCountBox.text);
                if ((evalMaxMineCount > -1) && (evalMaxMineCount >= MinMineCount))
                {
                    MaxMineCount = evalMaxMineCount;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                maxMineCountBox.text = MaxMineCount.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalMinMineCount = int.Parse(minMineCountBox.text);
                if ((evalMinMineCount > -1) && (evalMinMineCount <= MaxMineCount))
                {
                    MinMineCount = evalMinMineCount;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                minMineCountBox.text = MinMineCount.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalRuinCount = int.Parse(ruinCountBox.text);
                if (evalRuinCount > -1)
                {
                    RuinCount = evalRuinCount;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                ruinCountBox.text = RuinCount.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalPineTreeCount = int.Parse(pineTreeCountBox.text);
                if (evalPineTreeCount > -1)
                {
                    PineTreeCount = evalPineTreeCount;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                pineTreeCountBox.text = PineTreeCount.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalBirchTreeCount = int.Parse(birchTreeCountBox.text);
                if (evalBirchTreeCount > -1)
                {
                    BirchTreeCount = evalBirchTreeCount;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                birchTreeCountBox.text = BirchTreeCount.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalChestnutTreeCount = int.Parse(chestnutTreeCountBox.text);
                if (evalChestnutTreeCount > -1)
                {
                    ChestnutTreeCount = evalChestnutTreeCount;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                chestnutTreeCountBox.text = ChestnutTreeCount.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalMapleTreeCount = int.Parse(mapleTreeCountBox.text);
                if (evalMapleTreeCount > -1)
                {
                    MapleTreeCount = evalMapleTreeCount;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                mapleTreeCountBox.text = MapleTreeCount.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalBlueberryBushCount = int.Parse(blueberryBushCountBox.text);
                if (evalBlueberryBushCount > -1)
                {
                    BlueberryBushCount = evalBlueberryBushCount;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                blueberryBushCountBox.text = BlueberryBushCount.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalDandelionBushCount = int.Parse(dandelionBushCountBox.text);
                if (evalDandelionBushCount > -1)
                {
                    DandelionBushCount = evalDandelionBushCount;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                dandelionBushCountBox.text = DandelionBushCount.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            try
            {
                int evalSlopeCount = int.Parse(slopeCountBox.text);
                if (evalSlopeCount > -1)
                {
                    SlopeCount = evalSlopeCount;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                slopeCountBox.text = SlopeCount.ToString();
                _dialogBoxShower.Create().SetMessage("Unable to validate Input! Input has been restored to previous value. Please check your input is within parameters!").SetConfirmButton(delegate () { }, "OK").Show();
                return false;
            }
            return true;
        }
    }
}
