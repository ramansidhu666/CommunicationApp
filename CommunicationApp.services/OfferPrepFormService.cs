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
    public interface IOfferPrepFormService
    {
        //Main Function
        List<OfferPrepForm> OfferPrepForm();
        OfferPrepForm GetOfferPrepForm(int id);
        void InsertOfferPrepForm(OfferPrepForm OfferPrepForm);
        void UpdateOfferPrepForm(OfferPrepForm OfferPrepForm);
        void DeleteOfferPrepForm(OfferPrepForm OfferPrepForm);
        //Other Functions
    }
    public class OfferPrepFormService : IOfferPrepFormService
    {
        private UnitOfWork UnitOfWork;
      public OfferPrepFormService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<OfferPrepForm> OfferPrepForm()
      {
          return UnitOfWork.OfferPrepFormRepository.GetAll().ToList();
      }
      public OfferPrepForm GetOfferPrepForm(int id)
      {
          return UnitOfWork.OfferPrepFormRepository.GetById(id);
      }

      public void InsertOfferPrepForm(OfferPrepForm OfferPrepForm)
      {
          UnitOfWork.OfferPrepFormRepository.Insert(OfferPrepForm);
      }

      public void UpdateOfferPrepForm(OfferPrepForm OfferPrepForm)
      {
          UnitOfWork.OfferPrepFormRepository.Update(OfferPrepForm);
      }

      public void DeleteOfferPrepForm(OfferPrepForm OfferPrepForm)
      {
          UnitOfWork.OfferPrepFormRepository.Delete(OfferPrepForm);
      }
    }
}
