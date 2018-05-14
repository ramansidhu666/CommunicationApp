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
    public interface IBrokerageServiceServices
    {
        //Main Function
        List<BrokerageSevice> BrokerageSevicess();
        BrokerageSevice GetBrokerageService(int id);
        void InsertBrokerageService(BrokerageSevice BrokerageService);
        void UpdateBrokerageService(BrokerageSevice BrokerageService);
        void DeleteBrokerageService(BrokerageSevice BrokerageService);
        //Other Functions
    }
    public class BrokerageServiceServices : IBrokerageServiceServices
    {
        private UnitOfWork UnitOfWork;
      public BrokerageServiceServices()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<BrokerageSevice> BrokerageSevicess()
      {
          return UnitOfWork.BrokerageServiceRepository.GetAll().ToList();
      }
      public BrokerageSevice GetBrokerageService(int id)
      {
          return UnitOfWork.BrokerageServiceRepository.GetById(id);
      }

      public void InsertBrokerageService(BrokerageSevice BrokerageService)
      {
          UnitOfWork.BrokerageServiceRepository.Insert(BrokerageService);
      }

      public void UpdateBrokerageService(BrokerageSevice BrokerageService)
      {
          UnitOfWork.BrokerageServiceRepository.Update(BrokerageService);
      }

      public void DeleteBrokerageService(BrokerageSevice BrokerageService)
      {
          UnitOfWork.BrokerageServiceRepository.Delete(BrokerageService);
      }
    }
}
