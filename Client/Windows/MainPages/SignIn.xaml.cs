using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Client.Windows.MainPages
{
    public partial class SignIn : Page
    {
        public SignIn(Windows.Main owner)
        {
            this.Owner = owner;
            InitializeComponent();
        }

        // Properties
        public Windows.Main Owner { get; set; }

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
                        this.Owner.Connection.Connect(host, port);
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
