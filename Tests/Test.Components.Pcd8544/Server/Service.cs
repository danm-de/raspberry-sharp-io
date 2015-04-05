using System;
using System.ServiceProcess;
using Common.Logging;
using Nancy.Hosting.Self;
using Test.Components.Pcd8544.Server.Configuration;

namespace Test.Components.Pcd8544.Server
{
    partial class Service : ServiceBase
    {
        private const string ENDPOINT_KEY = "Endpoint";

        private readonly ILog logger = LogManager.GetLogger(typeof(Service));
        private readonly IConfigProvider configProvider = new AppConfigProvider();
        private NancyHost nancySelfHost;

        public Service() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            var endpoint = configProvider.GetKey(ENDPOINT_KEY);

            var bootstrapper = new Bootstrapper();
            var hostConfiguration = new HostConfiguration {
                UrlReservations = {
                    CreateAutomatically = true
                }
            };

            nancySelfHost = new NancyHost(bootstrapper, hostConfiguration, new Uri(endpoint));
            nancySelfHost.Start();

            logger.InfoFormat("Nancy started on endpoint {0}.", endpoint);
        }

        protected override void OnStop() {
            nancySelfHost.Stop();

            logger.InfoFormat("Nancy stopped.");
        }

        public void Start(string[] args) {
            OnStart(args);
        }
    }
}
