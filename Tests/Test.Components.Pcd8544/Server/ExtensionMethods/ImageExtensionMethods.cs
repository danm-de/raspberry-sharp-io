using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Test.Components.Pcd8544.Server.ExtensionMethods
{
    internal static class ImageExtensionMethods
    {
        public static string GetMimeType(this Image image) {
            return image
                .RawFormat
                .GetMimeType();
        }

        public static string GetMimeType(this ImageFormat imageFormat) {
            return ImageCodecInfo
                .GetImageEncoders()
                .First(codec => codec.FormatID == imageFormat.Guid)
                .MimeType;
        } 
    }
}