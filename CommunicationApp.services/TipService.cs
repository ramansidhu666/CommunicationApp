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
    public interface ITipService
    {
        //Main Function
        List<Tip> GetTips();
        Tip GetTip(int id);
        void InsertTip(Tip Tip);
        void UpdateTip(Tip Tip);
        void DeleteTip(Tip Tip);
        //Other Functions
    }
    public class TipService : ITipService
    {
        private UnitOfWork UnitOfWork;
      public TipService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<Tip> GetTips()
      {
          return UnitOfWork.TipRepository.GetAll().ToList();
      }
      public Tip GetTip(int id)
      {
          return UnitOfWork.TipRepository.GetById(id);
      }

      public void InsertTip(Tip Tip)
      {
          UnitOfWork.TipRepository.Insert(Tip);
      }

      public void UpdateTip(Tip Tip)
      {
          UnitOfWork.TipRepository.Update(Tip);
      }

      public void DeleteTip(Tip Tip)
      {
          UnitOfWork.TipRepository.Delete(Tip);
      }
    }
}
