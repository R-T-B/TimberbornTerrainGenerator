using Bindito.Core;

namespace TimberbornTerrainGenerator
{
    public class TimberbornTerrainGeneratorConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<RandomMapSettingsBox>().AsSingleton();
        }
    }
}