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
    public interface IOfficeLocationService
    {
        //Main Function
        List<OfficeLocation> GetOfficeLocations();
        OfficeLocation GetOfficeLocation(int id);
        void InsertOfficeLocation(OfficeLocation OfficeLocation);
        void UpdateOfficeLocation(OfficeLocation OfficeLocation);
        void DeleteOfficeLocation(OfficeLocation OfficeLocation);
        //Other Functions
    }
    public class OfficeLocationService : IOfficeLocationService
    {
        private UnitOfWork UnitOfWork;
      public OfficeLocationService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<OfficeLocation> GetOfficeLocations()
      {
          return UnitOfWork.OfficeLocationRepository.GetAll().ToList();
      }
      public OfficeLocation GetOfficeLocation(int id)
      {
          return UnitOfWork.OfficeLocationRepository.GetById(id);
      }

      public void InsertOfficeLocation(OfficeLocation OfficeLocation)
      {
          UnitOfWork.OfficeLocationRepository.Insert(OfficeLocation);
      }

      public void UpdateOfficeLocation(OfficeLocation OfficeLocation)
      {
          UnitOfWork.OfficeLocationRepository.Update(OfficeLocation);
      }

      public void DeleteOfficeLocation(OfficeLocation OfficeLocation)
      {
          UnitOfWork.OfficeLocationRepository.Delete(OfficeLocation);
      }
    }
}
