using System;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Test.Components.Pcd8544.WpfGui.Commands;

namespace Test.Components.Pcd8544.WpfGui.Controls
{
    public partial class Painter : UserControl, IDisposable
    {
        
        public static readonly DependencyProperty UpdateImageCommandProperty = DependencyProperty.Register(
            "UpdateImageCommand", typeof(ICommand<BitmapSource>), typeof(Painter), new PropertyMetadata(default(ICommand<BitmapSource>)));

        private IDisposable imageListener;

        private PainterViewModel ViewModel {
            get { return (PainterViewModel)DataContext; }
        }

        public ICommand<BitmapSource> UpdateImageCommand {
            get { return (ICommand<BitmapSource>)GetValue(UpdateImageCommandProperty); }
            set { SetValue(UpdateImageCommandProperty, value); }
        }
        
        public Painter() {
            InitializeComponent();

            var viewModel = ViewModel;
            var synchronizationContext = SynchronizationContext.Current;
            
            imageListener = Observable.FromEventPattern(
                handler => viewModel.ImageUpdated += handler,
                handler => viewModel.ImageUpdated -= handler)
                .Throttle(TimeSpan.FromMilliseconds(50))
                .ObserveOn(synchronizationContext)    
                .Subscribe(_ => ViewModelOnImageUpdated(viewModel));
        }
        
        ~Painter() {
            Dispose(false);
        }
        
        private void ViewModelOnImageUpdated(PainterViewModel viewModel) {
            var cmd = UpdateImageCommand;

            var imageSource = viewModel.ImageSource;

            if (cmd != null && cmd.CanExecute(imageSource)) {
                cmd.Execute(imageSource);
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            if (imageListener != null) {
                imageListener.Dispose();
                imageListener = null;
            }
        }
    }
}
