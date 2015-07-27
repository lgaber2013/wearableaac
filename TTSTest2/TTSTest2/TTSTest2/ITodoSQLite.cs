using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

/*
 * Description:
 * 
 * This is the 'SQLite file management for a TodoItemDatabase' interface. It is the first part of the requirements for using a 
 * DependencyService method, which we need to use because somee code needs to be written specific to a platform.
 * Thus, this serves as the general guidelines and requirements for a platform's use of this interface.
 * 
 * This interface's methods are targeted specifically for implementing the creation and management of a TodoItemDatabase.
 * 
 * */

namespace TTSTest2
{
    public interface ITodoSQLite
    {
        SQLiteConnection GetConnection();
    }
}
