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
    public interface IBrokerageDetailServices
    {
        //Main Function
        List<BrokerageDetail> BrokerageDetail();
        BrokerageDetail GetBrokerageDetail(int id);
        void InsertBrokerageDetail(BrokerageDetail BrokerageDetail);
        void UpdateBrokerageDetail(BrokerageDetail BrokerageDetail);
        void DeleteBrokerageDetail(BrokerageDetail BrokerageDetail);
        //Other Functions
    }
    public class BrokerageDetailServices : IBrokerageDetailServices
    {
        private UnitOfWork UnitOfWork;
      public BrokerageDetailServices()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<BrokerageDetail> BrokerageDetail()
      {
          return UnitOfWork.BrokerageDetailServiceRepository.GetAll().ToList();
      }
      public BrokerageDetail GetBrokerageDetail(int id)
      {
          return UnitOfWork.BrokerageDetailServiceRepository.GetById(id);
      }

      public void InsertBrokerageDetail(BrokerageDetail BrokerageDetail)
      {
          UnitOfWork.BrokerageDetailServiceRepository.Insert(BrokerageDetail);
      }

      public void UpdateBrokerageDetail(BrokerageDetail BrokerageDetail)
      {
          UnitOfWork.BrokerageDetailServiceRepository.Update(BrokerageDetail);
      }

      public void DeleteBrokerageDetail(BrokerageDetail BrokerageDetail)
      {
          UnitOfWork.BrokerageDetailServiceRepository.Delete(BrokerageDetail);
      }
    }
}
