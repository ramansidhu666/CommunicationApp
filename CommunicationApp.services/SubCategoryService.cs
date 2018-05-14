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
    public interface ISubCategoryService
    {
        //Main Function
        List<SubCategory> GetSubCategories();
        SubCategory GetSubCategory(int id);
        void InsertSubCategory(SubCategory SubCategory);
        void UpdateSubCategory(SubCategory SubCategory);
        void DeleteSubCategory(SubCategory SubCategory);
        //Other Functions
    }
    public class SubCategoryService : ISubCategoryService
    {
        private UnitOfWork UnitOfWork;
      public SubCategoryService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<SubCategory> GetSubCategories()
      {
          return UnitOfWork.SubCategoryRepository.GetAll().ToList();
      }
      public SubCategory GetSubCategory(int id)
      {
          return UnitOfWork.SubCategoryRepository.GetById(id);
      }

      public void InsertSubCategory(SubCategory SubCategory)
      {
          UnitOfWork.SubCategoryRepository.Insert(SubCategory);
      }

      public void UpdateSubCategory(SubCategory SubCategory)
      {
          UnitOfWork.SubCategoryRepository.Update(SubCategory);
      }

      public void DeleteSubCategory(SubCategory SubCategory)
      {
          UnitOfWork.SubCategoryRepository.Delete(SubCategory);
      }
    }
}
