using CommunicationApp.Core;
using CommunicationApp.Core.Infrastructure;
using CommunicationApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationApp.Entity;
namespace CommunicationApp.Services
{
    public interface IPropertyService
    {
        //Main Function
        List<Property> GetPropertys();
        Property GetProperty(int id);
        void InsertProperty(Property Property);
        void UpdateProperty(Property Property);
        void DeleteProperty(Property Property);
        //Other Functions
    }
    public class PropertyService : IPropertyService
    {
        private UnitOfWork UnitOfWork;
      public PropertyService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<Property> GetPropertys()
      {
        
          return UnitOfWork.PropertyRepository.GetAll().ToList();
      }
      public Property GetProperty(int id)
      {
          return UnitOfWork.PropertyRepository.GetById(id);
      }

      public void InsertProperty(Property Property)
      {
          UnitOfWork.PropertyRepository.Insert(Property);
      }

      public void UpdateProperty(Property Property)
      {
          UnitOfWork.PropertyRepository.Update(Property);
      }

      public void DeleteProperty(Property Property)
      {
          UnitOfWork.PropertyRepository.Delete(Property);
      }
    }
}
