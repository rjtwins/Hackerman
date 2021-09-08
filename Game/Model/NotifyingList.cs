using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model
{
    public class NotifyingList<T> : List<T>
    {
        public delegate void AddOrRemoveEventHandler(object sender, NotifyingListAddOrRemoveEventArgs<T> e);

        public event AddOrRemoveEventHandler OnAddOrRemove;

        public void AddAndNotify(T item)
        {
            this.Add(item);
            if(OnAddOrRemove != null)
            {
                OnAddOrRemove(this, new NotifyingListAddOrRemoveEventArgs<T>(item, NotifyingListAddOrRemove.ADD));
            }
        }

        public void RemoveAndNotify(T item)
        {
            this.Remove(item);
            if (OnAddOrRemove != null)
            {
                OnAddOrRemove(this, new NotifyingListAddOrRemoveEventArgs<T>(item, NotifyingListAddOrRemove.REMOVE));
            }
        }
    }

    public enum NotifyingListAddOrRemove { ADD, REMOVE};

    public class NotifyingListAddOrRemoveEventArgs<T> : EventArgs
    {
        public T Item { get; private set; }
        public NotifyingListAddOrRemove Action { get; private set; }
        public NotifyingListAddOrRemoveEventArgs(T item, NotifyingListAddOrRemove action)
        {
            this.Item = item;
            this.Action = action;
        }
    }
}
