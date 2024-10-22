using Bindito.Core;

namespace TimberbornTerrainGenerator
{
    [Context("MainMenu")]
    [Context("MapEditor")]
    public class TimberbornTerrainGeneratorConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<RandomMapSettingsBox>().AsSingleton();
        }
    }
}