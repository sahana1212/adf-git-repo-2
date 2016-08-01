using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Visual_Novel_Manager.CustomClasses
{
    public class ObservableCollectionWithRange : ObservableCollection<string>
    {
        public void AddRange(IEnumerable<string> list)
        {
            foreach (string item in list)
            {
                this.Items.Add(item);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void ClearAndAddRange(IEnumerable<string> list)
        {
            this.Items.Clear();

            this.AddRange(list);
        }
    }
}
