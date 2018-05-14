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
    public interface IClauseTypeServices
    {
        //Main Function
        List<ClauseType> GetClauseTypes();
        ClauseType GetClauseType(int id);
        void InsertClauseType(ClauseType ClauseType);
        void UpdateClauseType(ClauseType ClauseType);
        void DeleteClauseType(ClauseType ClauseType);
        //Other Functions
    }
    public class ClauseTypeServices : IClauseTypeServices
    {
        private UnitOfWork UnitOfWork;
      public ClauseTypeServices()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<ClauseType> GetClauseTypes()
      {
          return UnitOfWork.ClauseTypeRepository.GetAll().ToList();
      }
      public ClauseType GetClauseType(int id)
      {
          return UnitOfWork.ClauseTypeRepository.GetById(id);
      }

      public void InsertClauseType(ClauseType ClauseType)
      {
          UnitOfWork.ClauseTypeRepository.Insert(ClauseType);
      }

      public void UpdateClauseType(ClauseType ClauseType)
      {
          UnitOfWork.ClauseTypeRepository.Update(ClauseType);
      }

      public void DeleteClauseType(ClauseType ClauseType)
      {
          UnitOfWork.ClauseTypeRepository.Delete(ClauseType);
      }
    }
}
