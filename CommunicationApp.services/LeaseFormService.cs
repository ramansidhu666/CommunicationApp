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
    public interface ILeaseFormService
    {
        //Main Function
        List<LeaseForm> LeaseForm();
        LeaseForm GetLeaseForm(int id);
        void InsertLeaseForm(LeaseForm LeaseForm);
        void UpdateLeaseForm(LeaseForm LeaseForm);
        void DeleteLeaseForm(LeaseForm LeaseForm);
        //Other Functions
    }
    public class LeaseFormService : ILeaseFormService
    {
        private UnitOfWork UnitOfWork;
      public LeaseFormService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<LeaseForm> LeaseForm()
      {
          return UnitOfWork.LeaseFormRepository.GetAll().ToList();
      }
      public LeaseForm GetLeaseForm(int id)
      {
          return UnitOfWork.LeaseFormRepository.GetById(id);
      }

      public void InsertLeaseForm(LeaseForm LeaseForm)
      {
          UnitOfWork.LeaseFormRepository.Insert(LeaseForm);
      }

      public void UpdateLeaseForm(LeaseForm LeaseForm)
      {
          UnitOfWork.LeaseFormRepository.Update(LeaseForm);
      }

      public void DeleteLeaseForm(LeaseForm LeaseForm)
      {
          UnitOfWork.LeaseFormRepository.Delete(LeaseForm);
      }
    }
}
