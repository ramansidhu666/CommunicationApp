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
    public interface ISupplierService
    {
        //Main Function
        List<Supplier> GetSuppliers();
        Supplier GetSupplier(int id);
        void InsertSupplier(Supplier Supplier);
        void UpdateSupplier(Supplier Supplier);
        void DeleteSupplier(Supplier Supplier);
        //Other Functions
    }

    public class SupplierService : ISupplierService
    {
        private UnitOfWork UnitOfWork;
      public SupplierService()
      {
          UnitOfWork = new UnitOfWork();
          
      }

      public List<Supplier> GetSuppliers()
      {
          return UnitOfWork.SupplierRepository.GetAll().ToList();
      }
      public Supplier GetSupplier(int id)
      {
          return UnitOfWork.SupplierRepository.GetById(id);
      }

      public void InsertSupplier(Supplier Supplier)
      {
          UnitOfWork.SupplierRepository.Insert(Supplier);
      }

      public void UpdateSupplier(Supplier Supplier)
      {
          UnitOfWork.SupplierRepository.Update(Supplier);
      }

      public void DeleteSupplier(Supplier Supplier)
      {
          UnitOfWork.SupplierRepository.Delete(Supplier);
      }
    }
}
