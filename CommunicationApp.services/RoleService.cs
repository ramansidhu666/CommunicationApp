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
    public interface IRoleService
    {
        //Main Function
        List<Role> GetRoles();
        Role GetRole(int id);
        void InsertRole(Role Role);
        void UpdateRole(Role Role);
        void DeleteRole(Role Role);
        //Other Functions
        List<Role> GetRole(Boolean AddSelect = false);
    }
    public class RoleService : IRoleService
    {
        private UnitOfWork UnitOfWork;
      public RoleService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<Role> GetRoles()
      {
          return UnitOfWork.RoleRepository.GetAll().ToList();
      }
      public Role GetRole(int id)
      {
          return UnitOfWork.RoleRepository.GetById(id);
      }

      public void InsertRole(Role Role)
      {
          UnitOfWork.RoleRepository.Insert(Role);
      }

      public void UpdateRole(Role Role)
      {
          UnitOfWork.RoleRepository.Update(Role);
      }

      public void DeleteRole(Role Role)
      {
          UnitOfWork.RoleRepository.Delete(Role);
      }
      public List<Role> GetRole(Boolean AddSelect = false)
      {
          string RoleType = RoleTypes.RoleTypeValue.SuperAdmin.ToString().ToLower();
          var role = GetRoles().Where(x => x.RoleType.ToLower() != RoleType).ToList();
          if (AddSelect)
          {
              role.Insert(0, new Role { RoleId = -1, RoleName = "<--Select-->", RoleDescription = "", RoleType = "", IsActive = false });
          }
          return role;
      }
    }
}
