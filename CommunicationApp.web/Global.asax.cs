using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

//Using CommunicationApp
using CommunicationApp.Controllers;
using Ninject;
using Ninject.WebApi.DependencyResolver;
using System.Configuration;
using CommunicationApp.Data;
using CommunicationApp.Services;
using System.Web.Http;
using CommunicationApp.App_Start;
using System.Web.Optimization;
using CommunicationApp.Infrastructure;
using CommunicationApp;
//using CommunicationApp.core.Cache;
namespace CommunicationApp
{
    //public class MvcApplication : System.Web.HttpApplication
    //{
    //    protected void Application_Start()
    //    {
    //        AreaRegistration.RegisterAllAreas();
    //        RouteConfig.RegisterRoutes(RouteTable.Routes);
    //    }
    //}

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            //AreaRegistration.RegisterAllAreas();

            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            //log4net.Config.XmlConfigurator.Configure();




            //GlobalConfiguration.Configuration.EnsureInitialized();


            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();
            //RegisterDependencyResolver();
            //register cors handler
            GlobalConfiguration.Configuration.MessageHandlers.Add(new CommunicationApp.App_Start.CorsHandler());
        }

        //private void RegisterDependencyResolver()
        //{
        //    var kernel = new StandardKernel();

        //    // you may need to configure your container here?
        //    RegisterServices(kernel);

        //    DependencyResolver.SetResolver(new SwapStff.App_Start.NinjectMvcDependencyResolver(kernel));
        //}

        //private static void RegisterServices(IKernel kernel)
        //{
        //    var cs = ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString;

        //    kernel.Bind<IDbContext>().To<DataContext>().InSingletonScope().WithConstructorArgument("connection", cs);
        //    kernel.Bind(typeof(IRepository<>)).To(typeof(RepositoryService<>));
        //    kernel.Bind<IUserService>().To<UserService>();
        //    kernel.Bind<IProfileService>().To<ProfileService>();
        //    kernel.Bind<IItemService>().To<ItemService>();
        //    kernel.Bind<IItemMatchService>().To<ItemMatchService>();
        //    kernel.Bind<IChatService>().To<ChatService>();
        //    kernel.Bind<ILoggerService>().To<LoggerService>();
        //    kernel.Bind<ICacheManager>().To<MemoryCacheManager>().Named("Profile_contest_cache_static");
        //}

        //protected void Application_Error(object sender, EventArgs e)
        //{

        //    var httpContext = ((WebApiApplication)sender).Context;
        //    var currentController = " ";
        //    var currentAction = " ";
        //    var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

        //    if (currentRouteData != null)
        //    {
        //        if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
        //        {
        //            currentController = currentRouteData.Values["controller"].ToString();
        //        }

        //        if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
        //        {
        //            currentAction = currentRouteData.Values["action"].ToString();
        //        }
        //    }

        //    var ex = Server.GetLastError();

        //    // Code that runs when an unhandled error occurs

        //    // Get the exception object.
        //    Exception exc = Server.GetLastError();
        //    //HttpSessionState session = HttpContext.Current.Session;


        //    // Handle HTTP errors
        //    if (exc != null)
        //    {
        //        string ErrorMessage = exc.Message.ToString();
        //        ErrorLogging.LogError(exc);
        //    }

        //    var controller = new ErrorController();
        //    var routeData = new RouteData();
        //    var action = "Index";

        //    if (ex is HttpException)
        //    {
        //        var httpEx = ex as HttpException;

        //        switch (httpEx.GetHttpCode())
        //        {
        //            case 404:
        //                action = "NotFound";
        //                break;

        //            case 401:
        //                action = "AccessDenied";
        //                break;
        //        }
        //    }

        //    httpContext.ClearError();
        //    httpContext.Response.Clear();
        //    httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
        //    httpContext.Response.TrySkipIisCustomErrors = true;

        //    routeData.Values["controller"] = "Error";
        //    routeData.Values["action"] = action;

        //    controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
        //    ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));

        //}
    }
}
