using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Client.Windows.MainPages
{
    public partial class SignIn : Page
    {
        public SignIn()
        {
            InitializeComponent();
        }

        // Form Events
        private void buttonSignIn_Click(object sender, RoutedEventArgs e)
        {
            this.buttonSignIn.IsEnabled = false;
            this.textBoxAlert.Visibility = Visibility.Hidden;

            String host = this.textBoxHost.Text;
            String port = this.textBoxPort.Text;
            String name = this.textBoxName.Text;
            
            new Thread(delegate()
                {
                    try
                    {
                        ServerSide.Connection.Instance.Connect(host, port);
                        ServerSide.Sender.SignIn(name);
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
    }
}
