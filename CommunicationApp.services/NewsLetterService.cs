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
    public interface INewsLetterService
    {
        //Main Function
        List<NewsLetter_Entity> GetNewsLetters();
        NewsLetter_Entity GetNewsLetter(int id);
        void InsertNewsLetter(NewsLetter_Entity NewsLetter);
        void UpdateNewsLetter(NewsLetter_Entity NewsLetter);
        void DeleteNewsLetter(NewsLetter_Entity NewsLetter);
        //Other Functions
    }
    public class NewsLetterService : INewsLetterService
    {
        private UnitOfWork UnitOfWork;
      public NewsLetterService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<NewsLetter_Entity> GetNewsLetters()
      {
          return UnitOfWork.NewsletterRepository.GetAll().ToList();
      }
      public NewsLetter_Entity GetNewsLetter(int id)
      {
          return UnitOfWork.NewsletterRepository.GetById(id);
      }

      public void InsertNewsLetter(NewsLetter_Entity NewsLetter)
      {
          UnitOfWork.NewsletterRepository.Insert(NewsLetter);
      }

      public void UpdateNewsLetter(NewsLetter_Entity NewsLetter)
      {
          UnitOfWork.NewsletterRepository.Update(NewsLetter);
      }

      public void DeleteNewsLetter(NewsLetter_Entity NewsLetter)
      {
          UnitOfWork.NewsletterRepository.Delete(NewsLetter);
      }
    }
}
