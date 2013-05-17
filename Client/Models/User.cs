using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.Models
{
    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _MsgData;
        public String MsgData
        {
            get { return _MsgData; }
            set
            {
                if (value != _MsgData)
                {
                    _MsgData = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Int32 Id { get; set; }
        public String Name { get; set; }
        private String _Color;
        public String Color
        {
            get { return _Color; }
        }

        public int _NewMsgs;
        public String NewMsgs
        {
            get
            {
                if (_NewMsgs > 0) return _NewMsgs.ToString();
                else return "";
            }
        }

        private ObservableCollection<Message> _Messages;
        public ObservableCollection<Message> Messages
        {
            get { return _Messages; }
        }

        public User()
        {
            _Color = Colors.GetColor();
            _Messages = new ObservableCollection<Message>();
            _MsgData = "";
        }

        public void AddMessage(Message msg)
        {
            _Messages.Add(msg);
            _NewMsgs++;
            NotifyPropertyChanged("NewMsgs");
        }

        public void ResetNewMsgs()
        {
            _NewMsgs = 0;
            NotifyPropertyChanged("NewMsgs");
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
