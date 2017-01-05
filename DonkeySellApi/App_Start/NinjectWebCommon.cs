using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DonkeySellApi.ChatHelpers;
using DonkeySellApi.Extra;
using DonkeySellApi.Models;
using DonkeySellApi.Workers;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(DonkeySellApi.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(DonkeySellApi.App_Start.NinjectWebCommon), "Stop")]

namespace DonkeySellApi.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

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

                RegisterServices(kernel);
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
            kernel.Bind<ILogger>().To<Logger>();
            kernel.Bind<ICrudOnUsers>().To<CrudOnUsers>();
            kernel.Bind<ICrudOnMessages>().To<CrudOnMessages>();
            kernel.Bind<ICrudOnProducts>().To<CrudOnProducts>();
            kernel.Bind<IAuthorization>().To<Authorization>();
            kernel.Bind<IMyQueryBuilder>().To<MyQueryBuilder>();
            kernel.Bind<IGetCitiesAndCategories>().To<GetCitiesAndCategories>();
            kernel.Bind<ICrudOnFavorites>().To<CrudOnFavorites>();
            kernel.Bind<IMyPasswordGenerator>().To<MyPasswordGenerator>();
            kernel.Bind<IMailSender>().To<MailSender>();
            kernel.Bind<ICrudOnFriends>().To<CrudOnFriends>();
            kernel.Bind<IThrowExceptionToUser>().To<ThrowExceptionToUser>();
            kernel.Bind<IChatHelpers>().To<ChatHelpers.ChatHelpers>();
            kernel.Bind<ICrudOnImprovements>().To<CrudOnImprovements>();
            kernel.Bind<ICrudOnAlerts>().To<CrudOnAlerts>();
            kernel.Bind<DonkeySellContext>().To<DonkeySellContext>().InRequestScope();
        }        
    }
}
