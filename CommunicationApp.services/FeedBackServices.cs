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
    public interface IFeedBackService
    {
        //Main Function
        List<FeedBack> GetFeedBacks();
        FeedBack GetFeedBack(int id);
        void InsertFeedBack(FeedBack FeedBack);
        void UpdateFeedBack(FeedBack FeedBack);
        void DeleteFeedBack(FeedBack FeedBack);
        //Other Functions
    }
    public class FeedBackService : IFeedBackService
    {
        private UnitOfWork UnitOfWork;
      public FeedBackService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<FeedBack> GetFeedBacks()
      {
          return UnitOfWork.FeedBackRepository.GetAll().ToList();
      }
      public FeedBack GetFeedBack(int id)
      {
          return UnitOfWork.FeedBackRepository.GetById(id);
      }

      public void InsertFeedBack(FeedBack FeedBack)
      {
          UnitOfWork.FeedBackRepository.Insert(FeedBack);
      }

      public void UpdateFeedBack(FeedBack FeedBack)
      {
          UnitOfWork.FeedBackRepository.Update(FeedBack);
      }

      public void DeleteFeedBack(FeedBack FeedBack)
      {
          UnitOfWork.FeedBackRepository.Delete(FeedBack);
      }
    }
}
