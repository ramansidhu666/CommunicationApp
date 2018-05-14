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
    public interface IChattelsTypesServices
    {
        //Main Function
        List<ChattelsTypes> GetChattelsTypes();
        ChattelsTypes GetChattelsTypes(int id);
        void InsertChattelsTypes(ChattelsTypes ChattelsTypes);
        void UpdateChattelsTypes(ChattelsTypes ChattelsTypes);
        void DeleteChattelsTypes(ChattelsTypes ChattelsTypes);
        //Other Functions
    }
    public class ChattelsTypesServices : IChattelsTypesServices
    {
        private UnitOfWork UnitOfWork;
      public ChattelsTypesServices()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<ChattelsTypes> GetChattelsTypes()
      {
          return UnitOfWork.ChattelsTypesRepository.GetAll().ToList();
      }
      public ChattelsTypes GetChattelsTypes(int id)
      {
          return UnitOfWork.ChattelsTypesRepository.GetById(id);
      }

      public void InsertChattelsTypes(ChattelsTypes ChattelsTypes)
      {
          UnitOfWork.ChattelsTypesRepository.Insert(ChattelsTypes);
      }

      public void UpdateChattelsTypes(ChattelsTypes ChattelsTypes)
      {
          UnitOfWork.ChattelsTypesRepository.Update(ChattelsTypes);
      }

      public void DeleteChattelsTypes(ChattelsTypes ChattelsTypes)
      {
          UnitOfWork.ChattelsTypesRepository.Delete(ChattelsTypes);
      }
    }
}
