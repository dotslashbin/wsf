using EditorsCommon;
using EditorsCommon.Publication;
using EditorsDbLayer;
using EditorsDbLayer.Data.Publication;
using log4net.Config;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpContent.Tests
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class UnitTestInit
    {
        public static UnityContainer container;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        [Microsoft.VisualStudio.TestTools.UnitTesting.AssemblyInitialize]
        public static void InitUnityContainer(TestContext context)
        {

            //       '
            // configure log4.net
            //
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));


            container = new Microsoft.Practices.Unity.UnityContainer();

            container.AddNewExtension<Interception>();

            container.RegisterType<ISqlConnectionFactory, SqlConnectionFactory>(
                new ContainerControlledLifetimeManager(),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>());

            container.RegisterType<IIssueStorage, IssueStorage>(
                new ContainerControlledLifetimeManager(),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>());

            container.RegisterType<IVinStorage, VinStorage>(
                new ContainerControlledLifetimeManager(),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>());

            container.RegisterType<ITastingEventStorage, TastingEventStorage>(
                new ContainerControlledLifetimeManager(),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>());

            container.RegisterType<ITastingNoteStorage, TastingNoteStorage>(
                new ContainerControlledLifetimeManager(),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>());

            container.RegisterType<ISqlConnectionFactory, SqlConnectionFactory>(
                new ContainerControlledLifetimeManager(),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LoggingInterceptionBehavior>());
        }
    }
}
