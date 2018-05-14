using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommunicationApp.App_Start
{
    public class NinjectMvcDependencyResolver : NinjectDependencyScope,
                                             System.Web.Mvc.IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectMvcDependencyResolver(IKernel kernel)
            : base(kernel)
        {
            _kernel = kernel;
        }
    }
}