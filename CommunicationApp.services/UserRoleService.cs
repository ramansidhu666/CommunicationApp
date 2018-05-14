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
    public interface IUserRoleService
    {
        //Main Function
        List<UserRole> GetUserRoles();
        UserRole GetUserRole(int id);
        void InsertUserRole(UserRole UserRole);
        void UpdateUserRole(UserRole UserRole);
        void DeleteUserRole(UserRole UserRole);
        //Other Functions
        IEnumerable<UserRole> Role();
    }
    public class UserRoleService : IUserRoleService
    {
        private UnitOfWork UnitOfWork;
      public UserRoleService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<UserRole> GetUserRoles()
      {
          return UnitOfWork.UserRoleRepository.GetAll().ToList();
      }
      public UserRole GetUserRole(int id)
      {
          return UnitOfWork.UserRoleRepository.GetById(id);
      }

      public void InsertUserRole(UserRole UserRole)
      {
          UnitOfWork.UserRoleRepository.Insert(UserRole);
      }

      public void UpdateUserRole(UserRole UserRole)
      {
          UnitOfWork.UserRoleRepository.Update(UserRole);
      }

      public void DeleteUserRole(UserRole UserRole)
      {
          UnitOfWork.UserRoleRepository.Delete(UserRole);
      }
      public IEnumerable<UserRole> Role()
      {
          var role = GetUserRoles();
          return role;
      }
    }
}
