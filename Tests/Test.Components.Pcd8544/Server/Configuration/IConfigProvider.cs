namespace Test.Components.Pcd8544.Server.Configuration
{
    public interface IConfigProvider
    {
        string GetKey(string keyName);
    }
}