using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.Models
{
    public class Room : INotifyPropertyChanged
    {
        public Room(Int32 id, User creator, String name, Int32 capacity, Boolean isGameStarted = false)
        {
            this._Members = new ObservableCollection<User>();
            this._Watchers = new ObservableCollection<User>();
            this._Messages = new ObservableCollection<Message>();
            this._Id = id;
            this._Creator = creator;
            this._Name = name;
            this._Capacity = capacity;
            this._isGameStarted = isGameStarted;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private User _Creator;
        public User Creator { get { return _Creator; } }

        private ObservableCollection<User> _Watchers;
        public ObservableCollection<User> Watchers { get { return _Watchers; } }

        private ObservableCollection<User> _Members;
        public ObservableCollection<User> Members { get { return _Members; } }

        private String _Name;
        public String Name { get { return _Name; } }

        private Int32 _Id;
        public Int32 Id { get { return _Id; } }


        private Int32 _Capacity;
        public Int32 Capacity { get { return _Capacity; } }

        private ObservableCollection<Message> _Messages;
        public ObservableCollection<Message> Messages { get { return _Messages; } }

        private Boolean _isGameStarted;
        public Boolean isGameStarted { get { return _isGameStarted; } }

        public String MembersColumn
        {
            get {return (this.Members.Count + " / " + this.Capacity);}
        }

        public String WatchersColumn
        {
            get { return this.Watchers.Count.ToString(); }
        }

        private String _CurrentMessage;
        public String CurrentMessage
        {
            get { return _CurrentMessage; }
            set
            {
                _CurrentMessage = value;
                NotifyPropertyChanged("CurrentMessage");
            }
        }

        // Public Methods
        public bool Entered(User user)
        {
            if (user == null || this._Members.Count == this.Capacity || this._Members.Contains(user)) return false;
            this._Members.Add(user);
            NotifyPropertyChanged("Members");
            NotifyPropertyChanged("MembersColumn");
            return true;
        }

        public bool Watched(User user)
        {
            if (user == null || this._Watchers.Contains(user)) return false;
            this._Watchers.Add(user);
            NotifyPropertyChanged("Watchers");
            NotifyPropertyChanged("WatchersColumn");
            return true;
        }

        public bool Leaved(User user)
        {
            if (user == null || !this._Members.Contains(user)) return false;
            this._Members.Remove(user);
            NotifyPropertyChanged("Members");
            NotifyPropertyChanged("MembersColumn");
            return true;
        }

        public bool NotWatched(User user)
        {
            if (user == null || !this._Watchers.Contains(user)) return false;
            this._Watchers.Remove(user);
            NotifyPropertyChanged("Watchers");
            NotifyPropertyChanged("WatchersColumn");
            return true;
        }

        public void GameStarted()
        {
            this._isGameStarted = true;
            NotifyPropertyChanged("isGameStarted");
        }

        public void AddMessage(Message msg)
        {
            _Messages.Add(msg);
        }

        // Private Methods
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
