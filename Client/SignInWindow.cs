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
        private Connection _connection;
        private delegate void FormInvokeDelegate();

        public SignInWindow()
        {
            InitializeComponent();
            this._connection = Connection.Instance;
            this._connection.Client.RaiseUpdateUserId += UpdateUserId;
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            this._connection.Dispose();
            this.Dispose();
        }

        private void buttonSignIn_Click(object sender, EventArgs e)
        {
            string host = this.textBoxHostname.Text;
            string port = this.textBoxPort.Text;

            try
            {
                this._connection.Connect(host, port);
            }
            catch (Exception se)
            {
                MessageBox.Show(se.Message);
                return;
            }

            SignIn();
        }

        private void SignIn()
        {
            string user = this.textBoxUsername.Text;
            JsonBaseObject json = new JsonBaseObject
            {
                String = user
            };
            this._connection.Client.Send(Interface.Events.SIGN_IN, JsonConvert.SerializeObject(json));
        }

        private void UpdateUserId(object sender, String s)
        {
            if (this._mainWindow == null || this._mainWindow.IsDisposed)
            {
                this._mainWindow = new MainWindow();
                this._mainWindow.Owner = this;
            }
            FormInvokeDelegate d = new FormInvokeDelegate(this._mainWindow.Show);
            this.Invoke(d);
        }
    }
}
