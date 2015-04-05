using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Test.Components.Pcd8544.WpfGui.Abstract;
using Test.Components.Pcd8544.WpfGui.Commands;

namespace Test.Components.Pcd8544.WpfGui.Controls
{
    public class PainterViewModel : ObservableObject
    {
        public event EventHandler ImageUpdated;

        private const int WIDTH = 84;
        private const int HEIGHT = 48;
        private const int DPI = 96;

        private readonly ICommand mouseDownCommand;
        private readonly ICommand mouseMoveCommand;
        
        private WriteableBitmap bitmap;
        private int stride;
        private byte[] pixels;

        public ImageSource ImageSource {
            get { return bitmap; }
        }
        
        public ICommand MouseDownCommand {
            get { return mouseDownCommand; }
        }

        public ICommand MouseMoveCommand {
            get { return mouseMoveCommand; }
        }

        public PainterViewModel() {
            mouseDownCommand = new DelegateCommand<MouseButtonEventArgs>(MouseDown, null);
            mouseMoveCommand = new DelegateCommand<MouseEventArgs>(MouseMove, null);

            InitializeBitmap();
        }

        private void InitializeBitmap() {
            pixels = GetBitmap(WIDTH, HEIGHT, out stride);

            bitmap = new WriteableBitmap(
                WIDTH, 
                HEIGHT, 
                DPI, 
                DPI, 
                PixelFormats.BlackWhite,
                null);

            WriteToBitmap();
        }

        private void WriteToBitmap() {
            bitmap.WritePixels(new Int32Rect(0, 0, WIDTH, HEIGHT), pixels, stride, 0);

            OnImageUpdated();
        }

        private static byte[] GetBitmap(int width, int height, out int bitmapStride) {
            bitmapStride = (int)Math.Ceiling((double)width / 8);
            
            var pixelBuf = new byte[bitmapStride * height];
            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width; x++) {
                    WritePixel(pixelBuf, bitmapStride, x, y, Colors.Black);
                }
            }

            return pixelBuf;
        }

        private void MouseMove(MouseEventArgs e) {
            var position = GetPixelPosition(e);
            
            Color color;
            if (e.LeftButton == MouseButtonState.Pressed) {
                color = Colors.White;
            } else if (e.RightButton == MouseButtonState.Pressed) {
                color = Colors.Black;
            } else {
                return;
            }

            WritePixel(position, color);
            WriteToBitmap();
        }

        private void MouseDown(MouseButtonEventArgs e) {
            MouseMove(e);
        }

        private Point GetPixelPosition(MouseEventArgs e) {
            var source = e.Source as UIElement;
            if (source == null) {
                throw new InvalidOperationException("EventArgs doesn't have an associated UI element.");
            }

            var size = source.RenderSize;
            var position = e.GetPosition(source);

            var x = (position.X * WIDTH) / size.Width;
            var y = (position.Y * HEIGHT) / size.Height;

            return new Point(x, y);
        }

        private void WritePixel(Point position, Color color) {
            WritePixel(pixels, stride, (int)position.X, (int)position.Y, color);
        }

        private static void WritePixel(IList<byte> pixelBuf, int stride, int x, int y, Color color) {
            var byteOffset = y * stride + x / 8;
            var bitOffset = x % 8;

            var index = Math.Min(byteOffset, pixelBuf.Count -1);
            var b = pixelBuf[index];

            if (color == Colors.White) {
                b |= (byte) (1 << (7 - bitOffset));
            } else {
                b &= (byte)~((1 << (7 - bitOffset)));
            }
            pixelBuf[index] = b;
        }

        protected virtual void OnImageUpdated() {
            var handler = ImageUpdated;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
    }
}