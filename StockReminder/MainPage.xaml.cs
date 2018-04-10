using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StockReminder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();
        private HubConnection _hubConnection;
        private const string ServerURI = "https://stockmonitor-core.azurewebsites.net/chat";
        private const string ServerURI_local = "http://localhost:62601/server-push";

        public MainPage()
        {
            this.InitializeComponent();

            var url = ServerURI_local;
            Messages.Add($"Connecting {url}...");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(ServerURI_local)
                .Build();

            _hubConnection.Closed += HubConnectionClosed;

            _hubConnection.On<string, string>("BroadcastMessage", OnMessageReceived);

            try
            {
                _hubConnection.StartAsync();
            }
            catch (HttpRequestException ex)
            {
                Messages.Add(ex.Message);
            }
            Messages.Add("client connected");
        }

        public async void OnMessageReceived(string name, string message)
        {
            try
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Messages.Add($"{name}: {message}");
                });
            }
            catch (Exception ex)
            {
                Messages.Add(ex.Message);
            }
        }

        private void HubConnectionClosed(Exception arg)
        {
            Messages.Add("Hub connection closed");
        }
    }
}
