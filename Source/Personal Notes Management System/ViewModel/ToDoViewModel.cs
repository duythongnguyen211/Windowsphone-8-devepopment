using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Personal_Notes_Management_System.Model;
using System.Collections.ObjectModel;

namespace Personal_Notes_Management_System.ViewModel
{
    public class ToDoViewModel : INotifyPropertyChanged
    {
        private ToDoDataContext toDoDB;

        // Class constructor, create the data context object.
        public ToDoViewModel(string toDoDBConnectionString)
        {
            toDoDB = new ToDoDataContext(toDoDBConnectionString);
        }

        // Selected note
        public ToDoNote SelectedNote { get; set; }

        // All to-do items.
        private ToDoNoteCollection _allToDoNotes;
        public ToDoNoteCollection AllToDoNotes
        {
            get { return _allToDoNotes; }
            set
            {
                _allToDoNotes = value;
                NotifyPropertyChanged("AllToDoNotes");
            }
        }

        // search result
        private ToDoNoteCollection _searchresult;
        public ToDoNoteCollection SearchResult
        {
            get { return _searchresult; }
            set
            {
                _searchresult = value;
                NotifyPropertyChanged("SearchResult");
            }
        }

        // A list of all categories, used by the add task page.
        private List<ToDoCategory> _categoriesList;
        private ToDoCategory DefaultCategory;

        //private List<CategoryIcon> _cate_iconlist;
        //public List<CategoryIcon> CategoryIconList
        //{
        //    get { return _cate_iconlist; }
        //    set
        //    {
        //        _cate_iconlist = value;
        //        NotifyPropertyChanged("CategoryIconList");
        //    }
        //}

        public List<ToDoCategory> CategoriesList
        {
            get { return _categoriesList; }
            set
            {
                _categoriesList = value;
                DefaultCategory = value[0];
                NotifyPropertyChanged("CategoriesList");
            }
        }

        private object _categoryselected;
        public object CategorySelected
        {
            get { return _categoryselected; }
            set
            {
                if (_categoryselected != value)
                {
                    _categoryselected = value;
                    NotifyPropertyChanged("CategorySelected");
                }
            }
        }

        // A list of all todonotecollections
        private List<ToDoNoteCollection> _notecollectionlist;

        public List<ToDoNoteCollection> NoteCollectionList
        {
            get { return _notecollectionlist; }
            set
            {
                if (_notecollectionlist != value)
                {
                    _notecollectionlist = value;
                    NotifyPropertyChanged("NoteCollectionList");
                }
            }
        }

        // Write changes in the data context to the database.
        public void SaveChangesToDB()
        {
            toDoDB.SubmitChanges();
        }

        // Query database and load the collections and list used by the pivot pages.
        public void LoadCollectionsFromDatabase()
        {
            SearchResult = new ToDoNoteCollection();

            var toDoNoteInDB = from ToDoNote todo in toDoDB.Notes
                               select todo;

            // Query the database and load all to-do items.
            AllToDoNotes = new ToDoNoteCollection(toDoNoteInDB);

            //Load a list of all category icons
            //CategoryIconList = toDoDB.CategoryIcons.ToList();

            // Load a list of all categories.
            CategoriesList = toDoDB.Categories.ToList();

            NoteCollectionList = new List<ToDoNoteCollection>();
            foreach (ToDoCategory category in CategoriesList)
            {
                ToDoNoteCollection collection = new ToDoNoteCollection(category.ToDos);
                collection.CategoryId = category.Id;
                NoteCollectionList.Add(collection);
            }
        }

        // Add a to-do note to the database and collections.
        public void AddToDoNote(ToDoNote newToDoNote)
        {
            // Add a to-do note to the data context.
            toDoDB.Notes.InsertOnSubmit(newToDoNote);

            // Save changes to the database.
            toDoDB.SubmitChanges();

            // Add a to-do note to the "all" observable collection.
            AllToDoNotes.Collections.Add(newToDoNote);

            // Add a to-do note to the appropriate filtered collection.
            foreach (ToDoNoteCollection collection in NoteCollectionList)
                if (collection.CategoryId.Equals(newToDoNote.Category.Id))
                {
                    collection.Collections.Add(newToDoNote);
                    break;
                }
            ReLoadData();
        }

        // Remove a to-do note from the database and collections.
        public void DeleteToDoNote(ToDoNote toDoForDelete)
        {
            // Remove the to-do note from the "all" observable collection.
            AllToDoNotes.Collections.Remove(toDoForDelete);

            // Remove the to-do note from the data context.
            toDoDB.Notes.DeleteOnSubmit(toDoForDelete);

            // Remove the to-do note from the appropriate category.   
            foreach (ToDoNoteCollection collection in NoteCollectionList)
                if (collection.CategoryId.Equals(toDoForDelete.Category.Id))
                {
                    collection.Collections.Remove(toDoForDelete);
                    break;
                }

            // Save changes to the database.
            toDoDB.SubmitChanges();
            ReLoadData();
        }

        //Update a to-do note from the database and collections.
        public void UpdateToDoNote(ToDoNote toDoForUpdate)
        {
            // Update the to-do note from the "all" observable collection.
            AllToDoNotes.Update(toDoForUpdate);

            // Update the to-do note from the appropriate category.   
            foreach (ToDoNoteCollection collection in NoteCollectionList)
                if (collection.CategoryId.Equals(toDoForUpdate.Category.Id))
                {
                    collection.Update(toDoForUpdate);
                    break;
                }

            // Save changes to the database
            toDoDB.SubmitChanges();
            ReLoadData();
        }

        // Add a category to database
        public void AddToDoCategory(ToDoCategory newToDoCategory)
        {
            toDoDB.Categories.InsertOnSubmit(newToDoCategory);
            toDoDB.SubmitChanges();
            ReLoadData();
        }

        // Remove a category from database
        public void RemoveToDoCategory(ToDoCategory toDoForRemove)
        {
            if (toDoForRemove.Id == DefaultCategory.Id)
                return;
            foreach (ToDoNote note in toDoDB.Notes)
                if (note._categoryId == toDoForRemove.Id)
                    note.Category = DefaultCategory;
            toDoDB.Categories.DeleteOnSubmit(toDoForRemove);
            toDoDB.SubmitChanges();
            ReLoadData();
        }

        // Rename a category from database
        public void RenameCategory(ToDoCategory toDoForRename, string newName)
        {
            if (toDoForRename.Id == DefaultCategory.Id)
                return;
            toDoForRename.Name = newName;
            toDoDB.SubmitChanges();
            ReLoadData();
        }

        public void SearchNotes(string patern)
        {
            if (AllToDoNotes.Collections.Count <= 0)
                return;
            SearchResult.Collections.Clear();
            string[] ipatern = patern.Trim().Split(new char[] { ' ' });
            foreach (ToDoNote note in AllToDoNotes.Collections)
            {
                foreach (string hehe in ipatern)
                {
                    if (note.NoteTitle.Contains(hehe) ||
                        (note.NoteTags != null && note.NoteTags.Contains(hehe)) ||
                        (note.NoteDescription != null && note.NoteDescription.Contains(hehe)))
                    {
                        SearchResult.Collections.Add(note);
                        break;
                    }
                }
            }
        }

        public void CheckAll()
        {
            foreach (ToDoNote note in AllToDoNotes.Collections)
                note.IsComplete = true;
            toDoDB.SubmitChanges();
        }

        public void UnCheckAll()
        {
            foreach (ToDoNote note in AllToDoNotes.Collections)
                note.IsComplete = false;
            toDoDB.SubmitChanges();
        }

        public void DeleteAllNote()
        {
            foreach (ToDoNote note in AllToDoNotes.Collections)
                toDoDB.Notes.DeleteOnSubmit(note);
            AllToDoNotes.Collections.Clear();
            foreach (ToDoNoteCollection collection in NoteCollectionList)
                collection.Collections.Clear();
            toDoDB.SubmitChanges();
        }

        public event EventHandler OnDataBaseChange;

        public void ReLoadData()
        {
            LoadCollectionsFromDatabase();
            if (OnDataBaseChange != null)
                OnDataBaseChange(null, null);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
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
