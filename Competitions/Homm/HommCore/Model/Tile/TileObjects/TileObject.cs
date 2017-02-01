using System;
using System.ComponentModel;

namespace HoMM
{
    public abstract class TileObject: INotifyPropertyChanged
    {
        public string unityID;
        public readonly Location location;
        
        public abstract bool IsPassable { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        protected TileObject(Location location)
        {
            this.location = location;
        }

        public virtual void InteractWithPlayer(Player p) { }

        public event Action<TileObject> Remove;

        public void OnRemove()
        {
            if(Remove != null)
                Remove(this);
        }
    }
}
