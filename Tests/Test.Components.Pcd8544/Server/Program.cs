using System;
using System.ServiceProcess;

namespace Test.Components.Pcd8544.Server
{
    public class Program
    {
        static void Main(string[] args) {
            var service = new Service();

            if (Environment.UserInteractive) {
                service.Start(args);
                Console.WriteLine("Press any key to stop the program.");
                Console.ReadKey();
                service.Stop();
            } else {
                ServiceBase.Run(new ServiceBase[] { service });
            }
        }
    }
}
