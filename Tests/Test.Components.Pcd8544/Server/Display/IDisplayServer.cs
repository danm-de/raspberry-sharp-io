using Raspberry.IO.GeneralPurpose;

namespace Test.Components.Pcd8544.Server.Display
{
    public interface IDisplayServer
    {
        void Start(ProcessorPin resetPin, ProcessorPin dcModePin);
    }
}