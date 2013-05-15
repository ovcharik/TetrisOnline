using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

using Interface;
using Interface.Json;
using Newtonsoft;
using Newtonsoft.Json;

namespace Client
{
    public partial class SignInWindow : Form
    {
        private MainWindow _mainWindow;

        public SignInWindow()
        {
            InitializeComponent();
            _mainWindow = new MainWindow();
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void buttonSignIn_Click(object sender, EventArgs e)
        {
            string host = this.textBoxHostname.Text;
            string port = this.textBoxPort.Text;

            try
            {
                Connection.Instance.Connect(host, port);
            }
            catch (Exception se)
            {
                MessageBox.Show(se.Message);
                return;
            }

            Connection.Instance.Client.RaiseSignedUpdateUserId += UpdateUserId;
            Connection.Instance.Client.RaiseSignedUpdateUserId += _mainWindow.UpdateUserId;

            SignIn();
        }

        private void SignIn()
        {
            string user = this.textBoxUsername.Text;
            JsonBaseObject json = new JsonBaseObject
            {
                String = user
            };
            Connection.Instance.Client.Send(Interface.Events.SIGN_IN, JsonConvert.SerializeObject(json));
        }

        private void UpdateUserId(object sender, String s)
        {
            this._mainWindow.Show();
        }
    }
}
