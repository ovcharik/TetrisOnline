using System;
using System.Windows;
using System.Windows.Threading;

namespace Client.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();

            SignInPage = new MainPages.SignIn(this);
            MainPage = new MainPages.Main(this);

            Connection = ServerSide.Connection.Instance;
            Connection.Data.Dispatcher = Dispatcher;

            Connection.ClientSide.RaiseUpdateUserId += SignInPage.OnRaiseUpdateId;
            Connection.ClientSide.RaiseReceiveStoped += OnRaiseReceiveStoped;
        }

        // Properties
        public MainPages.SignIn SignInPage;
        public MainPages.Main MainPage;
        public ServerSide.Connection Connection { get; set; }

        // Form Events Handlers
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.MainFrame.Content = this.SignInPage;
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            this.Connection.Dispose();
        }

        // Server Events Handlers
        public void OnRaiseReceiveStoped(object sender, Exception e)
        {
            try
            {
                Dispatcher.Invoke(delegate
                {
                    MessageBox.Show(e.Message);
                    this.Connection.Disconnect();
                    this.MainFrame.Content = this.SignInPage;
                });
            }
            catch { }
        }

    }
}
