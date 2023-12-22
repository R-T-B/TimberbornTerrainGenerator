using HarmonyLib;
using Timberborn.CoreUI;
using Timberborn.MapSystemUI;
using UnityEngine.UIElements;
using TimberApi.DependencyContainerSystem;
using TimberApi.UiBuilderSystem;

namespace TimberbornTerrainGenerator
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(NewMapBox), "GetPanel")]
    public static class NewMapBoxPatch {
        
        private static void Postfix(NewMapBox __instance, VisualElement __result) 
        {

            var builder = DependencyContainer.GetInstance<UIBuilder>();
            var settings = builder.Presets().Buttons().Button(null, text: "Random map");
            
            var root = __result;
            var createButton = __result.Q<LocalizableButton>("StartButton");
            createButton.text = "Create New Empty Map";
            root.Add(settings);
            root.style.height = 240f;
            settings.clicked += ShowSettingsBox;
            
        }

        private static void ShowSettingsBox() 
        {
            var panelStack = DependencyContainer.GetInstance<PanelStack>();
            var randomMapSettingsBox = DependencyContainer.GetInstance<RandomMapSettingsBox>();
            panelStack.HideAndPush(randomMapSettingsBox);
        }
    }
}