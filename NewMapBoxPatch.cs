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

            var wrapper = new VisualElement 
            {
                style = 
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1,
                    justifyContent = Justify.Center,
                    paddingTop = new StyleLength(8f)
                }
            };
            var settings = builder.Presets().Buttons().Button(null, text: "Random map");
            wrapper.Add(settings);
            
            var root = __result.Q<VisualElement>("NewMapPanel");
            var createButton = __result.Q<LocalizableButton>("StartButton");
            createButton.text = "Create New Empty Map";
            root.Add(wrapper);
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