using Personal_Notes_Management_System.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Personal_Notes_Management_System.Model
{
    [Table]
    public partial class ToDoCategory : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Define private fields, public properties and database columns

        // Define ID: private field, public property, and database column.
        private int _id;

        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        // Define category name: private field, public property, and database column.
        private string _name;

        [Column]
        public string Name
        {
            get { return _name; }
            set
            {
                NotifyPropertyChanging("Name");
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private bool _isShowonmap;
        [Column]
        public bool IsShowOnMap
        {
            get { return _isShowonmap; }
            set
            {
                if (_isShowonmap != value)
                {
                    NotifyPropertyChanging("IsShowOnMap");
                    _isShowonmap = value;
                    NotifyPropertyChanged("IsShowOnMap");
                }
            }
        }

        private string _iconpath;
        [Column(CanBeNull=false)]
        public string IconPath
        {
            get { return _iconpath; }
            set
            {
                if (_iconpath != value)
                {
                    NotifyPropertyChanging("IconPath");
                    _iconpath = value;
                    NotifyPropertyChanged("IconPath");
                }
            }
        }

        public BitmapImage IconSource
        {
            get 
            { 
                return new BitmapImage(new Uri(IconPath, UriKind.RelativeOrAbsolute));
            } 
        }
        //

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        // Define the entity set for the collection side of the relationship.
        private EntitySet<ToDoNote> _todos;

        [Association(Storage = "_todos", OtherKey = "_categoryId", ThisKey = "Id")]
        public EntitySet<ToDoNote> ToDos
        {
            get { return this._todos; }
            set { this._todos.Assign(value); }
        }

        // Assign handlers for the add and remove operations, respectively.
        public ToDoCategory()
        {
            _todos = new EntitySet<ToDoNote>(
                new Action<ToDoNote>(this.attach_ToDo),
                new Action<ToDoNote>(this.detach_ToDo)
                );
        }

        // Called during an add operation
        private void attach_ToDo(ToDoNote toDo)
        {
            NotifyPropertyChanging("ToDoNote");
            toDo.Category = this;
        }

        // Called during a remove operation
        private void detach_ToDo(ToDoNote toDo)
        {
            NotifyPropertyChanging("ToDoNote");
            toDo.Category = null;
        }

        #endregion

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

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
