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
    public interface IBrokerageServices
    {
        //Main Function
        List<Brokerage> GetCompanies();
        Brokerage GetBrokerage(int id);
        void InsertBrokerage(Brokerage Brokerage);
        void UpdateBrokerage(Brokerage Brokerage);
        void DeleteBrokerage(Brokerage Brokerage);
        //Other Functions
    }
    public class BrokerageServices : IBrokerageServices
    {
        private UnitOfWork UnitOfWork;
      public BrokerageServices()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<Brokerage> GetCompanies()
      {
          return UnitOfWork.BrokerageRepository.GetAll().ToList();
      }
      public Brokerage GetBrokerage(int id)
      {
          return UnitOfWork.BrokerageRepository.GetById(id);
      }

      public void InsertBrokerage(Brokerage Brokerage)
      {
          UnitOfWork.BrokerageRepository.Insert(Brokerage);
      }

      public void UpdateBrokerage(Brokerage Brokerage)
      {
          UnitOfWork.BrokerageRepository.Update(Brokerage);
      }

      public void DeleteBrokerage(Brokerage Brokerage)
      {
          UnitOfWork.BrokerageRepository.Delete(Brokerage);
      }
    }
}
