
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CommunicationApp.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(CommunicationApp.App_Start.NinjectWebCommon), "Stop")]


namespace CommunicationApp.App_Start
{

    using CommunicationApp.Data;
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using CommunicationApp.Services;
    using CommunicationApp.Core;
    using Ninject.Web.Common;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using System.Configuration;
    using System.Web.Mvc;
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new Ninject.WebApi.DependencyResolver.NinjectDependencyResolver(kernel);
                RegisterServices(kernel);
                DependencyResolver.SetResolver(new CommunicationApp.App_Start.NinjectMvcDependencyResolver(kernel));

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var cs = ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString;

            kernel.Bind<IDbContext>().To<DataContext>().InSingletonScope().WithConstructorArgument("connection", cs);
            kernel.Bind(typeof(IRepository<,>)).To(typeof(RepositoryService<,>)).InRequestScope();

            kernel.Bind<ICityService>().To<CityService>();
            kernel.Bind<ICompanyService>().To<CompanyService>();
            kernel.Bind<ICountryService>().To<CountryService>();
            kernel.Bind<IErrorExceptionLogService>().To<ErrorExceptionLogService>();
            kernel.Bind<IFormService>().To<FormService>();
            kernel.Bind<ICustomerService>().To<CustomerService>();
            kernel.Bind<IRoleDetailService>().To<RoleDetailService>();
            kernel.Bind<IRoleService>().To<RoleService>();
            kernel.Bind<IStateService>().To<StateService>();
            kernel.Bind<IUserRoleService>().To<UserRoleService>();
            kernel.Bind<IUserService>().To<UserService>();
            kernel.Bind<IPropertyService>().To<PropertyService>();
            kernel.Bind<IPropertyImageService>().To<PropertyImageService>();
            kernel.Bind<IOfficeLocationService>().To<OfficeLocationService>();
            kernel.Bind<IAgentService>().To<AgentService>();
            kernel.Bind<IFeedBackService>().To<FeedBackService>();
            kernel.Bind<IEventService>().To<EventService>();
            kernel.Bind<ITipService>().To<TipService>();
            kernel.Bind<IEventCustomerService>().To<EventCustomerService>();
            kernel.Bind<INotification>().To<NotificationService>();
            kernel.Bind<IAdminStaffService>().To<AdminStaffService>();
            kernel.Bind<IViewsService>().To<ViewsService>();
            kernel.Bind<ICategoryService>().To<CategoryService>();
            kernel.Bind<ISubCategoryService>().To<SubCategoryService>();
            kernel.Bind<ISupplierService>().To<SupplierService>();
            kernel.Bind<IMessageService>().To<MessageService>();
            kernel.Bind<IMessageImageService>().To<MessageImageService>();
            kernel.Bind<IDivisionService>().To<DivisionService>();
            kernel.Bind<IPdfFormService>().To<PdfFormService>();
            kernel.Bind<IBannerService>().To<BannerService>();
            kernel.Bind<INewsLetterService>().To<NewsLetterService>();
            kernel.Bind<IBrokerageDetailServices>().To<BrokerageDetailServices>();
            kernel.Bind<IBrokerageServices>().To<BrokerageServices>();
            kernel.Bind<IBrokerageServiceServices>().To<BrokerageServiceServices>();
            kernel.Bind<IChattelsTypesServices>().To<ChattelsTypesServices>();
            kernel.Bind<IClauseTypeServices>().To<ClauseTypeServices>();
            kernel.Bind<ILeaseFormService>().To<LeaseFormService>();
            kernel.Bind<IOfferPrepFormService>().To<OfferPrepFormService>();

            //kernel.Bind<IViewsService>().To<ViewsService>();

        }
    }
}
