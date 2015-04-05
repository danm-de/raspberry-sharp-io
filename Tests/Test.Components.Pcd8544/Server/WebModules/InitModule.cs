using System;
using Common.Logging;
using Nancy;
using Raspberry.IO.GeneralPurpose;
using Test.Components.Pcd8544.Server.Display;

namespace Test.Components.Pcd8544.Server.WebModules
{
    public sealed class InitModule : NancyModule
    {
        private readonly IDisplayServer displayServer;
        private readonly ILog logger = LogManager.GetLogger(typeof(InitModule));

        public InitModule(IDisplayServer displayServer) {
            this.displayServer = displayServer;

            Put["/init"] = ctx => Init(ctx);
        }

        private dynamic Init(dynamic ctx) {
            var resetPin = Enum.Parse(typeof(ProcessorPin), Request.Query["ResetPin"]);
            var dcModePin = Enum.Parse(typeof(ProcessorPin), Request.Query["DcModePin"]);

            logger.DebugFormat("Got initialization from {0} with reset pin at {1} and data/command mode pin at {2}",
                Request.UserHostAddress,
                resetPin,
                dcModePin);

            displayServer.Start(resetPin, dcModePin);

            return HttpStatusCode.OK;
        }
    }
}