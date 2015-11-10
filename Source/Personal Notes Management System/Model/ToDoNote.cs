using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace Personal_Notes_Management_System.Model
{
    [Table]
    public partial class ToDoNote : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Define private fields, public properties and database columns

        // Define ID: private field, public property, and database column.
        private int _toDoNoteId;
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ToDoNoteId
        {
            get { return _toDoNoteId; }
            set
            {
                if (_toDoNoteId != value)
                {
                    NotifyPropertyChanging("ToDoNoteId");
                    _toDoNoteId = value;
                    NotifyPropertyChanged("ToDoNoteId");
                }
            }
        }

        // Define note title: private field, public property, and database column.
        private string _noteTitle;
        [Column(CanBeNull=false)]
        public string NoteTitle
        {
            get { return _noteTitle; }
            set
            {
                if (_noteTitle != value)
                {
                    NotifyPropertyChanging("NoteTitle");
                    _noteTitle = value;
                    NotifyPropertyChanged("NoteTitle");
                }
            }
        }

        //Define note description: private field, public property, and database column.
        private string _noteDescription;
        [Column]
        public string NoteDescription
        {
            get { return _noteDescription; }
            set
            {
                if (_noteDescription != value)
                {
                    NotifyPropertyChanging("NoteDesription");
                    _noteDescription = value;
                    NotifyPropertyChanged("NoteDesription");
                }
            }
        }

        //Define note tags: private field, public property, and database column.
        private string _noteTags;
        [Column]
        public string NoteTags
        {
            get { return _noteTags; }
            set
            {
                if (_noteTags != value)
                {
                    NotifyPropertyChanging("NoteTags");
                    _noteTags = value;
                    NotifyPropertyChanged("NoteTags");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        // Internal column for the associated ToDoCategory ID value
        [Column]
        internal int _categoryId;

        // Entity reference, to identify the ToDoCategory "storage" table
        private EntityRef<ToDoCategory> _category;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_category", ThisKey = "_categoryId", OtherKey = "Id", IsForeignKey = true)]
        public ToDoCategory Category
        {
            get { return _category.Entity; }
            set
            {
                NotifyPropertyChanging("Category");
                _category.Entity = value;

                if (value != null)
                {
                    _categoryId = value.Id;
                }
                else _categoryId = -1;

                NotifyPropertyChanging("Category");
            }
        }

        //Define location: private field, public property and database column.
        private string _noteLocation;

        [Column]
        public string NoteLocation
        {
            get { return _noteLocation; }
            set
            {
                if (_noteLocation != value)
                {
                    NotifyPropertyChanging("NoteLocation");
                    _noteLocation = value;
                    NotifyPropertyChanged("NoteLocation");
                }
            }
        }

        //Define photos of note: private field, public property and database column.
        private string _notePhotos;

        [Column]
        public string NotePhotos
        {
            get { return _notePhotos; }
            set
            {
                if (_notePhotos != value)
                {
                    NotifyPropertyChanging("NotePhotos");
                    _notePhotos = value;
                    NotifyPropertyChanged("NotePhotos");
                }
            }
        }

        //Define voice note: private field, public property and database column.
        private string _noteVoices;

        [Column]
        public string NoteVoices
        {
            get { return _noteVoices; }
            set
            {
                if (_noteVoices != value)
                {
                    NotifyPropertyChanging("NoteVoices");
                    _noteVoices = value;
                    NotifyPropertyChanged("NoteVoices");
                }
            }
        }

        //Define time reminder: private field, public property, and database column.
        private DateTime? _noteTimeRem;

        [Column]
        public string NoteTimeReminder
        {
            get
            {
                if (_noteTimeRem != null)
                    return _noteTimeRem.ToString();
                else return "";
            }
            set
            {
                try
                {
                    if (_noteTimeRem != DateTime.Parse(value))
                    {
                        NotifyPropertyChanging("NoteTimeReminder");
                        _noteTimeRem = DateTime.Parse(value);
                        NotifyPropertyChanged("NoteTimeReminder");
                    }
                }
                catch (Exception)
                {
                    NotifyPropertyChanging("NoteTimeReminder");
                    _noteTimeRem = DateTime.Now;
                    NotifyPropertyChanged("NoteTimeReminder");
                }
            }
        }

        public DateTime? TimeReminder 
        {
            get { return _noteTimeRem; }
            set
            {
                if (_noteTimeRem != value)
                {
                    NoteTimeReminder = value.ToString();
                }
            }
        }

        //Define time reminder enable value: private field, public property, and database column.
        private bool _isTimereminder;

        [Column]
        public bool IsTimeReminder
        {
            get { return _isTimereminder; }
            set
            {
                if (_isTimereminder != value)
                {
                    NotifyPropertyChanging("IsTimeReminder");
                    _isTimereminder = value;
                    NotifyPropertyChanged("IsTimeReminder");
                }
            }
        }

        //Define distance reminder: private field, public property, and database column.
        private double _noteDistanceRem;

        [Column]
        public double NoteDistanceReminder
        {
            get { return _noteDistanceRem; }
            set
            {
                if (_noteDistanceRem != value)
                {
                    NotifyPropertyChanging("NoteDistanceReminder");
                    _noteDistanceRem = value;
                    NotifyPropertyChanged("NoteDistanceReminder");
                }
            }
        }

        //Define distance reminder enable value: private field, public property, and database column.
        private bool _isDistanceReminder;

        [Column]
        public bool IsDistanceReminder
        {
            get { return _isDistanceReminder; }
            set
            {
                if (_isDistanceReminder != value)
                {
                    NotifyPropertyChanging("IsDistanceReminder");
                    _isDistanceReminder = value;
                    NotifyPropertyChanged("IsDistanceReminder");
                }
            }
        }

        // Define completion value: private field, public property, and database column.
        private bool _isComplete;

        [Column]
        public bool IsComplete
        {
            get { return _isComplete; }
            set
            {
                if (_isComplete != value)
                {
                    NotifyPropertyChanging("IsComplete");
                    _isComplete = value;
                    NotifyPropertyChanged("IsComplete");
                }
            }
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
