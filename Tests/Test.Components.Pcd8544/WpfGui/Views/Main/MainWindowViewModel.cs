using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Raspberry.IO.GeneralPurpose;
using RestSharp;
using Test.Components.Pcd8544.WpfGui.Abstract;
using Test.Components.Pcd8544.WpfGui.Commands;

namespace Test.Components.Pcd8544.WpfGui.Views
{
    public sealed class MainWindowViewModel : ObservableObject
    {
        private static readonly List<ProcessorPin> availablePins = Enum
            .GetValues(typeof(ProcessorPin))
            .Cast<ProcessorPin>()
            .ToList();

        private readonly DelegateCommand connectCommand;
        private readonly DelegateCommand disconnectCommand;
        private readonly DelegateCommand resetCommand;
        private readonly DelegateCommand<BitmapSource> imageUpdatedCommand;

        private ProcessorPin resetPin = ProcessorPin.Pin18;
        private ProcessorPin dcModePin = ProcessorPin.Pin17;
        private bool isConnected = false;
        private string connectUrl = "http://localhost:8333";
        private RestClient client;

        public IEnumerable<ProcessorPin> AvailablePins {
            get {
                return new ReadOnlyCollection<ProcessorPin>(availablePins);
            } 
        }

        public ProcessorPin ResetPin {
            get { return resetPin; }
            set { SetProperty(ref resetPin, value); }
        }

        public ProcessorPin DcModePin {
            get { return dcModePin; }
            set { SetProperty(ref dcModePin, value); }
        }

        public bool IsConnected {
            get { return isConnected; }
            set { 
                SetProperty(ref isConnected, value);
                
                connectCommand.RaiseCanExecuteChanged();
                disconnectCommand.RaiseCanExecuteChanged();
                resetCommand.RaiseCanExecuteChanged();
                imageUpdatedCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand ConnectCommand {
            get { return connectCommand; }
        }

        public ICommand DisconnectCommand {
            get { return disconnectCommand; }
        }

        public ICommand ResetCommand {
            get { return resetCommand; }
        }

        public ICommand<BitmapSource> UpdateImageCommand {
            get { return imageUpdatedCommand; }
        }

        public string ConnectUrl {
            get { return connectUrl; }
            set { SetProperty(ref connectUrl, value); }
        }

        public MainWindowViewModel() {
            connectCommand = new DelegateCommand(Connect, CanConnect);
            disconnectCommand = new DelegateCommand(Disconnect, CanDisconnect);
            resetCommand = new DelegateCommand(Reset, CanReset);
            imageUpdatedCommand = new DelegateCommand<BitmapSource>(UpdateImage, CanUpdateImage);
        }

        private void Disconnect() {
            IsConnected = false;
        }

        private void Connect() {
            client = new RestClient(connectUrl);
            SendInitialization();
            IsConnected = true;
        }

        private void Reset() {
            SendReset();
        }

        private void UpdateImage(BitmapSource imageSource) {
            SendImage(imageSource);
        }

        private bool CanConnect() {
            return !isConnected;
        }

        private bool CanDisconnect() {
            return isConnected;
        }

        private bool CanReset() {
            return isConnected;
        }

        private bool CanUpdateImage(ImageSource imageSource) {
            return isConnected;
        }

        private void SendInitialization() {
            var request = new RestRequest("/init?ResetPin={ResetPin}&DcModePin={DcModePin}", Method.PUT);
            request.AddUrlSegment("ResetPin", resetPin.ToString());
            request.AddUrlSegment("DcModePin", dcModePin.ToString());

            var response = client.Execute(request);
            ThrowOnInvalidResponse(response);
        }

        private async void SendImage(BitmapSource imageSource) {
            var request = new RestRequest("/image", Method.POST);

            var encoder = new PngBitmapEncoder();
            using (var stream = new MemoryStream()) {
                encoder.Frames.Add(BitmapFrame.Create(imageSource));
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                request.AddFile("image.png", stream.ToArray(), "image.png", "image/png");
            }

            var response = await client.ExecuteTaskAsync(request);
            ThrowOnInvalidResponse(response);
        }

        private void SendReset() {
            var request = new RestRequest("/reset", Method.POST);

            var response = client.Execute(request);
            ThrowOnInvalidResponse(response);
        }

        private void ThrowOnInvalidResponse(IRestResponse response) {
            if (response.ErrorMessage != null) {
                throw new Exception(response.ErrorMessage, response.ErrorException);
            }

            if (response.StatusCode != HttpStatusCode.OK) {
                throw new Exception(string.Format("Server {0} did not accept our request.\n{1}", 
                    connectUrl, 
                    response.Content));
            }
        }
    }
}