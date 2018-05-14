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
    public interface ICityService
    {
        //Main Function
        List<City> GetCities();
        City GetCity(int id);
        void InsertCity(City City);
        void UpdateCity(City City);
        void DeleteCity(City City);
        //Other Functions
    }
    public class CityService : ICityService
    {
        private UnitOfWork UnitOfWork;
      public CityService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<City> GetCities()
      {
          return UnitOfWork.CityRepository.GetAll().ToList();
      }
      public City GetCity(int id)
      {
          return UnitOfWork.CityRepository.GetById(id);
      }

      public void InsertCity(City City)
      {
          UnitOfWork.CityRepository.Insert(City);
      }

      public void UpdateCity(City City)
      {
          UnitOfWork.CityRepository.Update(City);
      }

      public void DeleteCity(City City)
      {
          UnitOfWork.CityRepository.Delete(City);
      }
    }
}
