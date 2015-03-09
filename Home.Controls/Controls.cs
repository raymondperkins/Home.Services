using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home.Services
{
    public class Controls : List<IControl>
    {
        private object _writeLock = new object();

        new public void Add(IControl item)
        {
            AddControl(item);
        }

        new public void Remove(IControl item)
        {
            RemoveControl(item);
        }

        public void AddControl(IControl item)
        {
            if (string.IsNullOrEmpty(item.Id))
                item.Id = this.Count.ToString();
            // check item doesnt exist already
            IControl existing = this.FirstOrDefault(q => q.Id == item.Id);
            if (existing == null)
            {
                
                lock (_writeLock)
                {
                    base.Add(item);
                }
                OnControlAdded(item);
            }
        }

        public void RemoveControl(IControl item)
        {
            // check item doesnt exist already
            IControl existing = this.FirstOrDefault(q => q.Id == item.Id);
            if (existing != null)
            {
                lock (_writeLock)
                {
                    base.Remove(existing);
                }
                OnControlRemoved(existing);
            }
        }

        private void OnControlAdded(IControl item)
        {

        }

        private void OnControlRemoved(IControl item)
        {
            
        }
    }
}
