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
    public interface IUserService
    {
        //Main Function
        List<User> GetUsers();
        User GetUser(int id);
        void InsertUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        //Other Functions
        string GetUserName(int id);
        int GetUserId(string name);
        User GetUserByName(string userName);
        User GetUserById(int UserId);
        User GetUserByEmailId(string email);
        User GetUserByTrebId(string TrebId);
        User GetUser(string userName, string password);
        User ValidateUser(string userName, string password);
        User ValidateUserByEmail(string EmailId, string password);
        User ValidateUserByTrebId(string TrebId, string password);
    }
    public class UserService : IUserService
    {
        private UnitOfWork UnitOfWork;
      public UserService()
      {
          UnitOfWork = new UnitOfWork();
      }

      public List<User> GetUsers()
      {
          return UnitOfWork.UserRepository.GetAll().ToList();
      }
      public User GetUser(int id)
      {
          return UnitOfWork.UserRepository.GetById(id);
      }

      public void InsertUser(User user)
      {
          UnitOfWork.UserRepository.Insert(user);
      }

      public void UpdateUser(User user)
      {
          UnitOfWork.UserRepository.Update(user);
      }

      public void DeleteUser(User user)
      {
          UnitOfWork.UserRepository.Delete(user);
      }

      public string GetUserName(int id)
      {
          string UserName = GetUsers().Where(z => z.UserId == id).Select(z => z.UserName).FirstOrDefault();
          return UserName;
      }
      public int GetUserId(string name)
      {
          int id = GetUsers().Where(z => z.UserName == name).Select(z => z.UserId).FirstOrDefault();
          return id;
      }
      public User GetUserByName(string userName)
      {
          var user = GetUsers().SingleOrDefault(u => u.UserName == userName);
          return user;
      }
      public User GetUserById(int UserId)
      {
          var user = GetUsers().SingleOrDefault(u => u.UserId == UserId);
          return user;
      }

      public User GetUserByEmailId(string email)
      {
          var user = GetUsers().SingleOrDefault(u => u.UserEmailAddress.ToLower() == email.ToLower());
          return user;
      }//GetUserByTrebId
      public User GetUserByTrebId(string TrebId)
      {
          var user = GetUsers().SingleOrDefault(u => u.TrebId == TrebId);
          return user;
      }
      public User GetUser(string userName, string password)
      {
          var user = GetUsers().SingleOrDefault(u => u.UserName == userName && u.Password == password);
          return user;
      }

      public User ValidateUser(string userName, string password)
      {
          password = SecurityFunction.EncryptString(password);
          var user = GetUsers().SingleOrDefault(u => u.UserName == userName && u.Password == password);
          return user;
      }
      public User ValidateUserByEmail(string EmailId, string password)
      {
          password = SecurityFunction.EncryptString(password);
          var user = GetUsers().SingleOrDefault(u => u.UserEmailAddress == EmailId && u.Password == password);
          return user;
      }
      public User ValidateUserByTrebId(string TrebId, string password)
      {
          password = SecurityFunction.EncryptString(password);
          var user = GetUsers().FirstOrDefault(u => u.TrebId == TrebId && u.Password == password);
          return user;
      }
    }
   
}
