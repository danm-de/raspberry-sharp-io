using System;
using System.Threading;
using Raspberry.IO.SerialPeripheralInterface;

namespace Raspberry.IO.Components.Displays.Pcd8544
{
    public class Pcd8544Connection
    {
        private readonly INativeSpiConnection spiConnection;
        private readonly IOutputBinaryPin resetPin;
        private readonly IOutputBinaryPin dcModePin;

        private static readonly TimeSpan RESET_VDD_WAIT_TIME = TimeSpan.FromMilliseconds(30);
        private static readonly TimeSpan RESET_WAIT_TIME = TimeSpan.FromMilliseconds(50);

        public Pcd8544Connection(INativeSpiConnection spiConnection, IOutputBinaryPin reset, IOutputBinaryPin dcMode) {
            spiConnection.ThrowIfArgumentNull("spiConnection");
            
            this.spiConnection = spiConnection;
            resetPin = reset;
            dcModePin = dcMode;            
          
            Initialize();
        }

        private void Initialize() {
            Reset();
            dcModePin.Write(false);
        }

        private void Reset() {
            Thread.Sleep(RESET_VDD_WAIT_TIME);
            resetPin.Write(false);
            Thread.Sleep(RESET_WAIT_TIME);
            resetPin.Write(true);
        }
    }
}