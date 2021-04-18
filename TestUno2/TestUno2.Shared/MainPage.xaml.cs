using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestUno2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Task ListeningTask;
        private CancellationTokenSource cancellationToken;

        public MainPage()
        {
            this.InitializeComponent();

            Ports = SerialPort.GetPortNames();
        }

        public IEnumerable<string> Ports { get; set; }

        public void OnClick(object sender, RoutedEventArgs e)
        {
            if(ListePorts.SelectedItem == null)
            {
                return;
            }

            if(ListeningTask == null)
            {
                cancellationToken = new CancellationTokenSource();
                ListeningTask = Task.Run(
                    () => ListenThread(
                        (string)ListePorts.SelectedItem),
                        cancellationToken.Token);
            }
            else
            {
                cancellationToken.Cancel();

                ListeningTask = null;
            }
        }

        private async void ListenThread(string portCom)
        {
            using (var port = new SerialPort(portCom))
            {
                port.BaudRate = 9600;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.DataBits = 8;
                port.Handshake = Handshake.None;

                port.Open();

                while (!cancellationToken.Token.IsCancellationRequested)
                { 
                    if(port.BytesToRead > 0)
                    {
                        await Task.Delay(100);

                        string content = port.ReadExisting();

                        content = content.Trim('\0');

                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            await Dispatcher.RunAsync(
                                CoreDispatcherPriority.Normal,
                                () =>
                                {
                                    Data.Text += content;
                                });
                        }
                    }



                    await Task.Delay(10);
                }

                port.Close();
            }
        }
    }
}
