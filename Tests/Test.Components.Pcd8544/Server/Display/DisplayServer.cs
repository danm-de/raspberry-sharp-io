using System;
using Raspberry.IO.GeneralPurpose;

namespace Test.Components.Pcd8544.Server.Display
{
    public class DisplayServer : IDisplayServer
    {
        private readonly object sync = new object();

        public void Start(ProcessorPin resetPin, ProcessorPin dcModePin) {
            lock (sync) {
                throw new NotImplementedException();
            }
        }
    }
}