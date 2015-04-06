using System;

namespace Raspberry.IO.Components.Displays.Pcd8544
{
    internal static class ExtensionMethods
    {
        public static void ThrowIfArgumentNull<T>(this T value, string parameterName, string message = null) where T : class {
            if (value != null) {
                return;
            }
            
            if (message == null) {
                throw new ArgumentNullException(parameterName);
            }

            throw new ArgumentNullException(parameterName, message);
        }
    }
}