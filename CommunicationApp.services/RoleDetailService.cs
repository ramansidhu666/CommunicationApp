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
    public interface IRoleDetailService
    {
        //Main Function
        List<RoleDetail> GetRoleDetails();
        RoleDetail GetRoleDetail(int id);
        void InsertRoleDetail(RoleDetail RoleDetail);
        void UpdateRoleDetail(RoleDetail RoleDetail);
        void DeleteRoleDetail(RoleDetail RoleDetail);
        //Other Functions
        List<RoleDetail> GetRoleDetails(int RoleId);
    }
    public class RoleDetailService : IRoleDetailService
    {
        private UnitOfWork UnitOfWork;
      public RoleDetailService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<RoleDetail> GetRoleDetails()
      {
          return UnitOfWork.RoleDetailRepository.GetAll().ToList();
      }
      public RoleDetail GetRoleDetail(int id)
      {
          return UnitOfWork.RoleDetailRepository.GetById(id);
      }

      public void InsertRoleDetail(RoleDetail RoleDetail)
      {
          UnitOfWork.RoleDetailRepository.Insert(RoleDetail);
      }

      public void UpdateRoleDetail(RoleDetail RoleDetail)
      {
          UnitOfWork.RoleDetailRepository.Update(RoleDetail);
      }

      public void DeleteRoleDetail(RoleDetail RoleDetail)
      {
          UnitOfWork.RoleDetailRepository.Delete(RoleDetail);
      }
      public List<RoleDetail> GetRoleDetails(int RoleId)
      {
          List<RoleDetail> tbuserdetails = GetRoleDetails().Where(z => z.RoleId == RoleId).ToList();
          return tbuserdetails;
      }
    }
}
