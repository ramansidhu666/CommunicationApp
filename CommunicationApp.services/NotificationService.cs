using CommunicationApp.Data;
using CommunicationApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Services
{
    public interface INotification
    {
        List<Notification> GetNotifications();
        IEnumerable<U> GetBy<U>(Expression<Func<Notification, U>> columns, Expression<Func<Notification, bool>> where);
        Notification GetNotification(int id);
        void InsertNotification(Notification Notification);
        void UpdateNotification(Notification Notification);
        void DeleteNotification(Notification Notification);
    }
   public class NotificationService:INotification
    {

       private UnitOfWork UnitOfWork;
      public NotificationService()
      {
          UnitOfWork = new UnitOfWork();
      }
      public IEnumerable<U> GetBy<U>(Expression<Func<Notification, U>> columns, Expression<Func<Notification, bool>> where)
      {
          return UnitOfWork.NotificationRepository.GetBy(columns, where);
      }
      public List<Notification> GetNotifications()
      {
          return UnitOfWork.NotificationRepository.GetAll().ToList();
      }
      public Notification GetNotification(int id)
      {
          return UnitOfWork.NotificationRepository.GetById(id);
      }

      public void InsertNotification(Notification Notification)
      {
          UnitOfWork.NotificationRepository.Insert(Notification);
      }

      public void UpdateNotification(Notification Notification)
      {
          UnitOfWork.NotificationRepository.Update(Notification);
      }

      public void DeleteNotification(Notification Notification)
      {
          UnitOfWork.NotificationRepository.Delete(Notification);
      }
    }
}
