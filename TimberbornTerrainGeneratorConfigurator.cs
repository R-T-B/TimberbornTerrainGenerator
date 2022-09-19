using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;

namespace TimberbornTerrainGenerator
{
    [Configurator(SceneEntrypoint.MainMenu | SceneEntrypoint.MapEditor)]
    public class TimberbornTerrainGeneratorConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<RandomMapSettingsBox>().AsSingleton();
        }
    }
}