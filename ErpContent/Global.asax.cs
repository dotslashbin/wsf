using EditorsCommon;
using log4net;
using log4net.Config;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ErpContent
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {



        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);






        protected void Application_Start()
        {


            // configure log4.net
            //
            String configPath = Path.Combine(HttpRuntime.AppDomainAppPath, "log4net.config");
            XmlConfigurator.ConfigureAndWatch(new FileInfo(configPath));
            log.Info("init log system");



            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


        }

        protected void Application_EndRequest()
        {
            var context = new HttpContextWrapper(Context);

            // If we're an ajax request, and doing a 302, then we actually need to do a 401                  

            if (Context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
            {
                Context.Response.Clear(); 
                Context.Response.StatusCode = 401;
            }

        }


    }
}