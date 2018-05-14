using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Data.DbIntializer
{
    public class DataBaseInitializer : IDatabaseInitializer<DataContext>
    {
        public void InitializeDatabase(DataContext context)
        {
            context.Database.CreateIfNotExists();
        }
    }
}
