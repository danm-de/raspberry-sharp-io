using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mime;
using System.Windows.Media.Imaging;
using Nancy;
using Test.Components.Pcd8544.Server.Display;
using Test.Components.Pcd8544.Server.ExtensionMethods;

namespace Test.Components.Pcd8544.Server.WebModules
{
    public sealed class ImageModule : NancyModule
    {
        private IDisplayServer displayServer;

        public ImageModule(IDisplayServer displayServer) {
            this.displayServer = displayServer;

            Post["/image"] = ctx => SetImage(ctx);
        }

        private dynamic SetImage(dynamic ctx) {
            var httpFile = Request.Files.FirstOrDefault();
            if (httpFile == null) {
                return Response
                    .AsText("Image required")
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            var image = BitmapDecoder.Create(
                httpFile.Value, 
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.Default
            );

            // Not allowed yet
            return HttpStatusCode.Forbidden;
        }
    }
}