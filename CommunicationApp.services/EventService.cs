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
    public interface IEventService
    {
        //Main Function
        List<Event> GetEvents();
        Event GetEvent(int id);
        void InsertEvent(Event Event);
        void UpdateEvent(Event Event);
        void DeleteEvent(Event Event);
        //Other Functions
    }
    public class EventService : IEventService
    {
        private UnitOfWork UnitOfWork;
      public EventService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<Event> GetEvents()
      {
          return UnitOfWork.EventRepository.GetAll().ToList();
      }
      public Event GetEvent(int id)
      {
          return UnitOfWork.EventRepository.GetById(id);
      }

      public void InsertEvent(Event Event)
      {
          UnitOfWork.EventRepository.Insert(Event);
      }

      public void UpdateEvent(Event Event)
      {
          UnitOfWork.EventRepository.Update(Event);
      }

      public void DeleteEvent(Event Event)
      {
          UnitOfWork.EventRepository.Delete(Event);
      }
    }
}
