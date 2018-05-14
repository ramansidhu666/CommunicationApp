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
    public class OpenHouseService : IOpenHouseService
    {
      private IRepository<OpenHouse> OpenHouseRepository;
      public OpenHouseService(IRepository<OpenHouse> OpenHouseRepository)
      {
          this.OpenHouseRepository = OpenHouseRepository;
      }

      public List<OpenHouse> GetOpenHouses()
      {
          return OpenHouseRepository.GetAll().ToList();
      }
      public OpenHouse GetOpenHouse(int id)
      {
          return OpenHouseRepository.GetById(id);
      }

      public void InsertOpenHouse(OpenHouse OpenHouse)
      {
          OpenHouseRepository.Insert(OpenHouse);
      }

      public void UpdateOpenHouse(OpenHouse OpenHouse)
      {
          OpenHouseRepository.Update(OpenHouse);
      }

      public void DeleteOpenHouse(OpenHouse OpenHouse)
      {
          OpenHouseRepository.Delete(OpenHouse);
      }
    }
}
