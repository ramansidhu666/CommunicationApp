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
    public interface IEventCustomerService
    {
        //Main Function
        List<EventCustomer> GetEventCustomers();
        EventCustomer GetEventCustomer(int id);
        void InsertEventCustomer(EventCustomer EventCustomer);
        void UpdateEventCustomer(EventCustomer EventCustomer);
        void DeleteEventCustomer(EventCustomer EventCustomer);
        //Other Functions
    }
    public class EventCustomerService : IEventCustomerService
    {
        private UnitOfWork UnitOfWork; 
      public EventCustomerService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<EventCustomer> GetEventCustomers()
      {
          return UnitOfWork.EventCustomerRepository.GetAll().ToList();
      }
      public EventCustomer GetEventCustomer(int id)
      {
          return UnitOfWork.EventCustomerRepository.GetById(id);
      }

      public void InsertEventCustomer(EventCustomer EventCustomer)
      {
          UnitOfWork.EventCustomerRepository.Insert(EventCustomer);
      }

      public void UpdateEventCustomer(EventCustomer EventCustomer)
      {
          UnitOfWork.EventCustomerRepository.Update(EventCustomer);
      }

      public void DeleteEventCustomer(EventCustomer EventCustomer)
      {
          UnitOfWork.EventCustomerRepository.Delete(EventCustomer);
      }
    }
}
