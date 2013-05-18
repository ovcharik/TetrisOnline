using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для SignIn.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        public MainWindow Owner { get; set; }
        public SignInPage(MainWindow owner)
        {
            this.Owner = owner;
            InitializeComponent();
        }

        private void buttonSignIn_Click(object sender, RoutedEventArgs e)
        {
            this.buttonSignIn.IsEnabled = false;

            String host = this.textBoxHost.Text;
            String port = this.textBoxPort.Text;
            String name = this.textBoxName.Text;
            
            // Говно какое то
            new Thread(delegate()
                {
                    try
                    {
                        this.Owner.Connection.Connect(host, port);
                        Sender.SignIn(name);
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.Invoke(delegate
                        {
                            this.textBoxAlert.Text = ex.Message;
                            this.textBoxAlert.Visibility = Visibility.Visible;
                            this.buttonSignIn.IsEnabled = true;
                        });
                    }
                }).Start();
        }

        public void OnRaiseUpdateId(object sender, String json)
        {
            this.Dispatcher.Invoke(delegate {
                NavigationService.Navigate(this.Owner.MainPage);
                this.buttonSignIn.IsEnabled = true;
            });
        }
    }
}
