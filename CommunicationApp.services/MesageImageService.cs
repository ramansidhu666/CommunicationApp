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
    public interface IMessageImageService
    {
        //Main Function
        List<MessageImage> GetMessageImages();
        MessageImage GetMessageImage(int id);
        void InsertMessageImage(MessageImage MessageImage);
        void UpdateMessageImage(MessageImage MessageImage);
        void DeleteMessageImage(MessageImage MessageImage);
        //Other Functions
    }
    public class MessageImageService : IMessageImageService
    {
        private UnitOfWork UnitOfWork;
      public MessageImageService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<MessageImage> GetMessageImages()
      {
          return UnitOfWork.MessageImageRepository.GetAll().ToList();
      }
      public MessageImage GetMessageImage(int id)
      {
          return UnitOfWork.MessageImageRepository.GetById(id);
      }

      public void InsertMessageImage(MessageImage MessageImage)
      {
          UnitOfWork.MessageImageRepository.Insert(MessageImage);
      }

      public void UpdateMessageImage(MessageImage MessageImage)
      {
          UnitOfWork.MessageImageRepository.Update(MessageImage);
      }

      public void DeleteMessageImage(MessageImage MessageImage)
      {
          UnitOfWork.MessageImageRepository.Delete(MessageImage);
      }
    }
}
