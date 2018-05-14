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
    public interface IFormService
    {
        //Main Function
        List<Form> GetForms();
        Form GetForm(int id);
        void InsertForm(Form Form);
        void UpdateForm(Form Form);
        void DeleteForm(Form Form);
        //Other Functions
    }
    public class FormService : IFormService
    {
        private UnitOfWork UnitOfWork;
      public FormService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<Form> GetForms()
      {
          return UnitOfWork.FormRepository.GetAll().ToList();
      }
      public Form GetForm(int id)
      {
          return UnitOfWork.FormRepository.GetById(id);
      }

      public void InsertForm(Form Form)
      {
          UnitOfWork.FormRepository.Insert(Form);
      }

      public void UpdateForm(Form Form)
      {
          UnitOfWork.FormRepository.Update(Form);
      }

      public void DeleteForm(Form Form)
      {
          UnitOfWork.FormRepository.Delete(Form);
      }
    }
}
