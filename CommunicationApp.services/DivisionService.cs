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
    public interface IDivisionService
    {
        //Main Function
        List<Division> GetDivisions();
        Division GetDivision(int id);
        void InsertDivision(Division Division);
        void UpdateDivision(Division Division);
        void DeleteDivision(Division Division);
        //Other Functions
    }

    public class DivisionService : IDivisionService
    {
        private UnitOfWork UnitOfWork;
      public DivisionService()
      {
          UnitOfWork = new UnitOfWork();
          
      }

      public List<Division> GetDivisions()
      {
          return UnitOfWork.DivisionRepository.GetAll().ToList();
      }
      public Division GetDivision(int id)
      {
          return UnitOfWork.DivisionRepository.GetById(id);
      }

      public void InsertDivision(Division Division)
      {
          UnitOfWork.DivisionRepository.Insert(Division);
      }

      public void UpdateDivision(Division Division)
      {
          UnitOfWork.DivisionRepository.Update(Division);
      }

      public void DeleteDivision(Division Division)
      {
          UnitOfWork.DivisionRepository.Delete(Division);
      }
    }
}
