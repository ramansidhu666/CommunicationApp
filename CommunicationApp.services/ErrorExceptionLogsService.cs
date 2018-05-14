using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationApp.Entity;
using CommunicationApp.Core;
using CommunicationApp.Data;
namespace CommunicationApp.Services
{
    public interface IErrorExceptionLogService
    {
        List<ErrorExceptionLogs> GetAll();
        ErrorExceptionLogs GetById(string id);
        List<ErrorExceptionLogs> GetErrorExceptionLogs();
        void Insert(ErrorExceptionLogs model);
        void Update(ErrorExceptionLogs model);
        void Delete(ErrorExceptionLogs model);
    }
    public class ErrorExceptionLogService : IErrorExceptionLogService
    {
        private UnitOfWork UnitOfWork; 
        
        public ErrorExceptionLogService()
        {
            UnitOfWork = new UnitOfWork();
        }

        public List<ErrorExceptionLogs> GetAll()
        {
            //return _cacheManager.Get(ErrorExceptionLogs_ALL_KEY, () =>
            //{
            //    return UnitOfWork._ErrorExceptionLogRepository.GetAll().ToList();
            //});

            return UnitOfWork.ErrorExceptionLogsRepository.GetAll().ToList();
        }

        public ErrorExceptionLogs GetById(string id)
        {
            return GetAll().Find(l => l.EventId.ToString() == id);
        }

        public List<ErrorExceptionLogs> GetErrorExceptionLogs()
        {
            var ErrorExceptionLogs = UnitOfWork.ErrorExceptionLogsRepository.GetBy(x => new
            {
                x.EventId,
                x.LogDateTime,
                x.Source,
                x.Message,
                x.QueryString,
                x.TargetSite,
                x.StackTrace,
                x.ServerName,
                x.RequestURL,
                x.UserAgent,
                x.UserIP,
                x.UserAuthentication,
                x.UserName
            }, x => x.EventId != -1);

            var ErrorExceptionLogList = new List<ErrorExceptionLogs>();
            foreach (var item in ErrorExceptionLogs)
            {
                ErrorExceptionLogList.Add(new ErrorExceptionLogs {  EventId=item.EventId, LogDateTime = item.LogDateTime,
                                                                    Source = item.Source,
                                                                    Message = item.Message,
                                                                    QueryString = item.QueryString,
                                                                    TargetSite = item.TargetSite,
                                                                    StackTrace = item.StackTrace,
                                                                    ServerName = item.ServerName,
                                                                    RequestURL = item.RequestURL,
                                                                    UserAgent = item.UserAgent,
                                                                    UserIP = item.UserIP,
                                                                    UserAuthentication = item.UserAuthentication,
                                                                    UserName = item.UserName});
            }
            return ErrorExceptionLogList;
        }

        public void Insert(ErrorExceptionLogs model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("ErrorExceptionLog");
            }
            
            List<ErrorExceptionLogs> ErrorExceptionLogs = GetAll();
            ErrorExceptionLogs.Add(model);
            UnitOfWork.ErrorExceptionLogsRepository.Insert(model);
        }

        public void Update(ErrorExceptionLogs model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("ErrorExceptionLog");
            }

            ErrorExceptionLogs ErrorExceptionLog = GetById(model.EventId.ToString());

            if (ErrorExceptionLog != null)
                UnitOfWork.ErrorExceptionLogsRepository.Update(model);
        }

        public void Delete(ErrorExceptionLogs model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("ErrorExceptionLogs");
            }

            List<ErrorExceptionLogs> ErrorExceptionLogs = GetAll();
            ErrorExceptionLogs.Remove(ErrorExceptionLogs.Find(l => l.EventId == model.EventId));
            UnitOfWork.ErrorExceptionLogsRepository.Delete(model);
        }
    }
}
