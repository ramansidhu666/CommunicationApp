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
    public interface ICustomerService
    {
        //Main Function
        List<Customer> GetCustomers();
        Customer GetCustomer(int id);
        void InsertCustomer(Customer Customer);
        void UpdateCustomer(Customer Customer);
        void DeleteCustomer(Customer Customer);
        //Other Functions
    }
    public class CustomerService : ICustomerService
    {
        private UnitOfWork UnitOfWork;
      public CustomerService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<Customer> GetCustomers()
      {
          return UnitOfWork.CustomerRepository.GetAll().ToList();
      }
      public Customer GetCustomer(int id)
      {
          return UnitOfWork.CustomerRepository.GetById(id);
      }

      public void InsertCustomer(Customer Customer)
      {
          UnitOfWork.CustomerRepository.Insert(Customer);
      }

      public void UpdateCustomer(Customer Customer)
      {
          UnitOfWork.CustomerRepository.Update(Customer);
      }

      public void DeleteCustomer(Customer Customer)
      {
          UnitOfWork.CustomerRepository.Delete(Customer);
      }
    }
}
