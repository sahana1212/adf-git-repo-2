using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VisualNovelManager.CustomClasses
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
