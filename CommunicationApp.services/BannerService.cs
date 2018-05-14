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
    public interface IBannerService
    {
        //Main Function
        List<Banner> GetBanners();
        Banner GetBanner(int id);
        void InsertBanner(Banner Banner);
        void UpdateBanner(Banner Banner);
        void DeleteBanner(Banner Banner);
        //Other Functions
    }
    public class BannerService : IBannerService
    {
        private UnitOfWork UnitOfWork;
      public BannerService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<Banner> GetBanners()
      {
          return UnitOfWork.BannerServiceRepository.GetAll().ToList();
      }
      public Banner GetBanner(int id)
      {
          return UnitOfWork.BannerServiceRepository.GetById(id);
      }

      public void InsertBanner(Banner Banner)
      {
          UnitOfWork.BannerServiceRepository.Insert(Banner);
      }

      public void UpdateBanner(Banner Banner)
      {
          UnitOfWork.BannerServiceRepository.Update(Banner);
      }

      public void DeleteBanner(Banner Banner)
      {
          UnitOfWork.BannerServiceRepository.Delete(Banner);
      }
    }
}
