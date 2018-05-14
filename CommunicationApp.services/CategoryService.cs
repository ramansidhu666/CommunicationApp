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
    public interface ICategoryService
    {
        //Main Function
        List<Category> Categories();
        Category GetCategory(int id);
        void InsertCategory(Category Category);
        void UpdateCategory(Category Category);
        void DeleteCategory(Category Category);
        //Other Functions
    }
    public class CategoryService : ICategoryService
    {
        private UnitOfWork UnitOfWork;
      public CategoryService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<Category> Categories()
      {
          return UnitOfWork.CategoryRepository.GetAll().ToList();
      }
      public Category GetCategory(int id)
      {
          return UnitOfWork.CategoryRepository.GetById(id);
      }

      public void InsertCategory(Category Category)
      {
          UnitOfWork.CategoryRepository.Insert(Category);
      }

      public void UpdateCategory(Category Category)
      {
          UnitOfWork.CategoryRepository.Update(Category);
      }

      public void DeleteCategory(Category Category)
      {
          UnitOfWork.CategoryRepository.Delete(Category);
      }
    }
}
