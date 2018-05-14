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
    public interface IMessageService
    {
        //Main Function
        List<Message> GetMessages();
        Message GetMessage(int id);
        void InsertMessage(Message Message);
        void UpdateMessage(Message Message);
        void DeleteMessage(Message Message);
        //Other Functions
    }
    public class MessageService : IMessageService
    {
        private UnitOfWork UnitOfWork;
      public MessageService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<Message> GetMessages()
      {
          return UnitOfWork.MessageRepository.GetAll().ToList();
      }
      public Message GetMessage(int id)
      {
          return UnitOfWork.MessageRepository.GetById(id);
      }

      public void InsertMessage(Message Message)
      {
          UnitOfWork.MessageRepository.Insert(Message);
      }

      public void UpdateMessage(Message Message)
      {
          UnitOfWork.MessageRepository.Update(Message);
      }

      public void DeleteMessage(Message Message)
      {
          UnitOfWork.MessageRepository.Delete(Message);
      }
    }
}
