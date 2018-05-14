using CommunicationApp.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Data
{
   public class DataContext :DbContext,IDbContext
    {
       public DataContext()
           : base()
       {

       }
       public DataContext(string connection)
           : base(connection)
       {

       }
    
       protected override void OnModelCreating(DbModelBuilder modelBuilder)
       {
            
           var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
           .Where(type => !String.IsNullOrEmpty(type.Namespace))
           .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
              type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));

           //Database.SetInitializer<DataContext>(null);

           foreach (var type in typesToRegister)
           {
               dynamic configurationInstance = Activator.CreateInstance(type);
               modelBuilder.Configurations.Add(configurationInstance);
           }

           base.OnModelCreating(modelBuilder);
       }
       public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
       {
           return base.Set<TEntity>();
       }
    }
}
