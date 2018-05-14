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
    public interface IAdminStaffService
    {
        //Main Function
        List<AdminStaff> GetAdminStaffs();
        AdminStaff GetAdminStaff(int id);
        void InsertAdminStaff(AdminStaff AdminStaff);
        void UpdateAdminStaff(AdminStaff AdminStaff);
        void DeleteAdminStaff(AdminStaff AdminStaff);
        //Other Functions
    }
    public class AdminStaffService : IAdminStaffService
    {
        private UnitOfWork UnitOfWork;
      public AdminStaffService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<AdminStaff> GetAdminStaffs()
      {
          return UnitOfWork.AdminStaffRepository.GetAll().ToList();
      }
      public AdminStaff GetAdminStaff(int id)
      {
          return UnitOfWork.AdminStaffRepository.GetById(id);
      }

      public void InsertAdminStaff(AdminStaff AdminStaff)
      {
          UnitOfWork.AdminStaffRepository.Insert(AdminStaff);
      }

      public void UpdateAdminStaff(AdminStaff AdminStaff)
      {
          UnitOfWork.AdminStaffRepository.Update(AdminStaff);
      }

      public void DeleteAdminStaff(AdminStaff AdminStaff)
      {
          UnitOfWork.AdminStaffRepository.Delete(AdminStaff);
      }
    }
}
