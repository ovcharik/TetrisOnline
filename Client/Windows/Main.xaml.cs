using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Navigation;

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
#if DEBUG
            this.Title += " - DEBUG VERSION";
#endif
            SignInPage = new MainPages.SignIn(this);
            MainPage = new MainPages.Main(this);

            Connection = ServerSide.Connection.Instance;
            Connection.Data.Dispatcher = Dispatcher;

            ServerSide.NavigationService.RaiseGotoMainPage += GotoMainPage;
            ServerSide.NavigationService.RaiseGotoSignInPage += GotoSignInPage;

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
                this.Dispatcher.Invoke(delegate
                {
                    this.SignInPage.textBoxAlert.Text = e.Message;
                    this.SignInPage.textBoxAlert.Visibility = Visibility.Visible;
                    GotoSignInPage(null, null);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GotoMainPage(object sender, object e)
        {
            try
            {
                this.Dispatcher.Invoke(delegate
                {
                    this.MainFrame.Navigate(MainPage);
                    SignInPage.buttonSignIn.IsEnabled = true;
                });
            }
            catch { }
        }

        public void GotoSignInPage(object sender, object e)
        {
            try
            {
                this.Dispatcher.Invoke(delegate
                {
                    this.MainFrame.Navigate(SignInPage);
                    SignInPage.buttonSignIn.IsEnabled = true;
                });
            }
            catch { }
        }

    }
}
