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
    public interface ICountryService
    {
        //Main Function
        List<Country> GetCountries();
        Country GetCountry(int id);
        void InsertCountry(Country Country);
        void UpdateCountry(Country Country);
        void DeleteCountry(Country Country);
        //Other Functions
    }
    public class CountryService : ICountryService
    {
        private UnitOfWork UnitOfWork; 
      public CountryService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<Country> GetCountries()
      {
          return UnitOfWork.CountryRepository.GetAll().ToList();
      }
      public Country GetCountry(int id)
      {
          return UnitOfWork.CountryRepository.GetById(id);
      }

      public void InsertCountry(Country Country)
      {
          UnitOfWork.CountryRepository.Insert(Country);
      }

      public void UpdateCountry(Country Country)
      {
          UnitOfWork.CountryRepository.Update(Country);
      }

      public void DeleteCountry(Country Country)
      {
          UnitOfWork.CountryRepository.Delete(Country);
      }
    }
}
