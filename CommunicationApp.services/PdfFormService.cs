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
    public interface IPdfFormService
    {
        //Main Function
        List<PdfForm> GetPdfForms();
        PdfForm GetPdfForm(int id);
        void InsertPdfForm(PdfForm PdfForm);
        void UpdatePdfForm(PdfForm PdfForm);
        void DeletePdfForm(PdfForm PdfForm);
        //Other Functions
    }
    public class PdfFormService : IPdfFormService
    {
        private UnitOfWork UnitOfWork;
      public PdfFormService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<PdfForm> GetPdfForms()
      {
          return UnitOfWork.PdfFormRepository.GetAll().ToList();
      }
      public PdfForm GetPdfForm(int id)
      {
          return UnitOfWork.PdfFormRepository.GetById(id);
      }

      public void InsertPdfForm(PdfForm PdfForm)
      {
          UnitOfWork.PdfFormRepository.Insert(PdfForm);
      }

      public void UpdatePdfForm(PdfForm PdfForm)
      {
          UnitOfWork.PdfFormRepository.Update(PdfForm);
      }

      public void DeletePdfForm(PdfForm PdfForm)
      {
          UnitOfWork.PdfFormRepository.Delete(PdfForm);
      }
    }
}
