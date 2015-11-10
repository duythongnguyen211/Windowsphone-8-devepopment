using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Personal_Notes_Management_System.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Personal_Notes_Management_System.ViewModel
{
    public class ToDoNoteCollection
    {
        // Class constructors
        public ToDoNoteCollection()
        {
            Collections = new ObservableCollection<ToDoNote>();
        }
        public ToDoNoteCollection(IEnumerable<ToDoNote> collection)
        {
            Collections = new ObservableCollection<ToDoNote>(collection);
        }

        // Category
        private int _categoryId;

        public int CategoryId
        {
            get { return _categoryId; }
            set
            {
                if (_categoryId != value)
                {
                    _categoryId = value;
                }
            }
        }

        private ObservableCollection<ToDoNote> _data;

        public ObservableCollection<ToDoNote> Collections
        {
            get { return _data; }
            set
            {
                if (_data != value)
                {
                    _data = value;
                    NotifyPropertyChanged("Collections");
                }
            }
        }

        public int Update(ToDoNote toDoNoteForUpdate)
        {
            int index;
            for (index = Collections.Count - 1; index >= 0; --index)
                if (Collections[index].ToDoNoteId == toDoNoteForUpdate.ToDoNoteId)
                {
                    Collections[index] = toDoNoteForUpdate;
                    break;
                }
            return index;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
