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
    public interface IPropertyImageService
    {
        //Main Function
        List<PropertyImage> GetPropertyImages();
        PropertyImage GetPropertyImage(int id);
        void InsertPropertyImage(PropertyImage PropertyImage);
        void UpdatePropertyImage(PropertyImage PropertyImage);
        void DeletePropertyImage(PropertyImage PropertyImage);
        //Other Functions
    }
    public class PropertyImageService : IPropertyImageService
    {
        private UnitOfWork UnitOfWork;
        public PropertyImageService()
      {
          UnitOfWork = new UnitOfWork(); 
      }

      public List<PropertyImage> GetPropertyImages()
      {
          return UnitOfWork.PropertyImageRepository.GetAll().ToList();
      }
      public PropertyImage GetPropertyImage(int id)
      {
          return UnitOfWork.PropertyImageRepository.GetById(id);
      }

      public void InsertPropertyImage(PropertyImage PropertyImage)
      {
          UnitOfWork.PropertyImageRepository.Insert(PropertyImage);
      }

      public void UpdatePropertyImage(PropertyImage PropertyImage)
      {
          UnitOfWork.PropertyImageRepository.Update(PropertyImage);
      }

      public void DeletePropertyImage(PropertyImage PropertyImage)
      {
          UnitOfWork.PropertyImageRepository.Delete(PropertyImage);
      }
     

    }
}
