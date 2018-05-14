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
    public interface IViewsService
    {
        //Main Function
        List<Views> GetViewss();
        Views GetViews(int id);
        void InsertViews(Views Views);
        void UpdateViews(Views Views);
        void DeleteViews(Views Views);
        //Other Functions
    }

    public class ViewsService : IViewsService
    {
        private UnitOfWork UnitOfWork;
      public ViewsService()
      {
          UnitOfWork = new UnitOfWork();
          
      }

      public List<Views> GetViewss()
      {
          return UnitOfWork.ViewsRepository.GetAll().ToList();
      }
      public Views GetViews(int id)
      {
          return UnitOfWork.ViewsRepository.GetById(id);
      }

      public void InsertViews(Views Views)
      {
          UnitOfWork.ViewsRepository.Insert(Views);
      }

      public void UpdateViews(Views Views)
      {
          UnitOfWork.ViewsRepository.Update(Views);
      }

      public void DeleteViews(Views Views)
      {
          UnitOfWork.ViewsRepository.Delete(Views);
      }
    }
}
