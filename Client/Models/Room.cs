using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.Models
{
    public class Room : INotifyPropertyChanged
    {
        public Room(Int32 id, User creator, String name, Int32 capacity, Boolean isGameStarted = false)
        {
            this._Members = new List<User>();
            this._Watchers = new List<User>();
            this._Id = id;
            this._Creator = creator;
            this._Name = name;
            this._Capacity = capacity;
            this._isGameStarted = isGameStarted;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private User _Creator;
        public User Creator { get { return _Creator; } }

        private List<User> _Watchers;
        public List<User> Watchers { get { return _Watchers; } }

        private List<User> _Members;
        public List<User> Members { get { return _Members; } }

        private String _Name;
        public String Name { get { return _Name; } }

        private Int32 _Id;
        public Int32 Id { get { return _Id; } }


        private Int32 _Capacity;
        public Int32 Capacity { get { return _Capacity; } }

        private List<Message> _Messages;
        public List<Message> Messages { get { return _Messages; } }

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
