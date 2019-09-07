using Autofac;
using Autofac.Integration.Mvc;
using DressBookingMVC.DataManager;
using DressBookingMVC.DataManager.Atelier;
using DressBookingMVC.Models.DressBookingDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DressBookingMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var builder = new ContainerBuilder();

            builder.RegisterType<DressBookingDB1Entities>().InstancePerLifetimeScope();
            builder.RegisterType<DressesDataManager>().As<IDataManager<Dress>>();
            builder.RegisterType<OrderValidationDataManager>().As<IDataManager<Order>>();
            builder.RegisterType<AtelierFacade>();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));



        }
    }
}
