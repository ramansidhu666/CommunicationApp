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
    public interface IAgentService
    {
        //Main Function
        List<Agent> GetAgents();
        Agent GetAgent(int id);
        void InsertAgent(Agent Agent);
        void UpdateAgent(Agent Agent);
        void DeleteAgent(Agent Agent);
        //Other Functions
    }

    public class AgentService : IAgentService
    {
        private UnitOfWork UnitOfWork;
      public AgentService()
      {
          UnitOfWork = new UnitOfWork();
          
      }

      public List<Agent> GetAgents()
      {
          return UnitOfWork.AgentRepository.GetAll().ToList();
      }
      public Agent GetAgent(int id)
      {
          return UnitOfWork.AgentRepository.GetById(id);
      }

      public void InsertAgent(Agent Agent)
      {
          UnitOfWork.AgentRepository.Insert(Agent);
      }

      public void UpdateAgent(Agent Agent)
      {
          UnitOfWork.AgentRepository.Update(Agent);
      }

      public void DeleteAgent(Agent Agent)
      {
          UnitOfWork.AgentRepository.Delete(Agent);
      }
    }
}
