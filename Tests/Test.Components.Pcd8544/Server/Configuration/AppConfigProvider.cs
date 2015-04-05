using System.Configuration;

namespace Test.Components.Pcd8544.Server.Configuration
{
    public sealed class AppConfigProvider : IConfigProvider
    {
        public string GetKey(string keyName) {
            return ConfigurationManager.AppSettings.Get(keyName);
        }
    }
}