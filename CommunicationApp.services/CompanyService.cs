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
    public interface ICompanyService
    {
        //Main Function
        List<Company> GetCompanies();
        Company GetCompany(int id);
        void InsertCompany(Company Company);
        void UpdateCompany(Company Company);
        void DeleteCompany(Company Company);
        //Other Functions
    }
    public class CompanyService : ICompanyService
    {
        private UnitOfWork UnitOfWork;
      public CompanyService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<Company> GetCompanies()
      {
          return UnitOfWork.CompanyRepository.GetAll().ToList();
      }
      public Company GetCompany(int id)
      {
          return UnitOfWork.CompanyRepository.GetById(id);
      }

      public void InsertCompany(Company Company)
      {
          UnitOfWork.CompanyRepository.Insert(Company);
      }

      public void UpdateCompany(Company Company)
      {
          UnitOfWork.CompanyRepository.Update(Company);
      }

      public void DeleteCompany(Company Company)
      {
          UnitOfWork.CompanyRepository.Delete(Company);
      }
    }
}
