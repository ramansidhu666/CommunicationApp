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
    public interface IStateService
    {
        //Main Function
        List<State> GetStates();
        State GetState(int id);
        void InsertState(State State);
        void UpdateState(State State);
        void DeleteState(State State);
        //Other Functions
    }
    public class StateService : IStateService
    {
        private UnitOfWork UnitOfWork;
      public StateService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<State> GetStates()
      {
          return UnitOfWork.StateRepository.GetAll().ToList();
      }
      public State GetState(int id)
      {
          return UnitOfWork.StateRepository.GetById(id);
      }

      public void InsertState(State State)
      {
          UnitOfWork.StateRepository.Insert(State);
      }

      public void UpdateState(State State)
      {
          UnitOfWork.StateRepository.Update(State);
      }

      public void DeleteState(State State)
      {
          UnitOfWork.StateRepository.Delete(State);
      }
    }
}
