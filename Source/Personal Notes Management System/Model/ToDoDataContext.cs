using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal_Notes_Management_System.Model
{
    public class ToDoDataContext: DataContext
    {
        // Pass the connection string to the base class.
        public ToDoDataContext(string connectionString)
            : base(connectionString)
        { }

        // Specify a table for the to-do items.
        public Table<ToDoNote> Notes;

        // Specify a table for the categories.
        public Table<ToDoCategory> Categories;

        //Specify a table for the category icon
        //public Table<CategoryIcon> CategoryIcons;
    }
}
