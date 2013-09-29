using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Negocio.Roles.Chat;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using System.Web.UI;
using System.Globalization;
using System.Threading;

namespace Chat
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Use LocalDB for Entity Framework by default
            Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        void Session_Start(object sender, EventArgs e)
        {
            ChatModel chat = new ChatModel();
            chat.CleanInactiveSessions();
        }

        public void Session_End(object sender, EventArgs e)
        {
            ChatModel chat = new ChatModel();
            chat.CleanInactiveSessions();
        }

        public void Application_End(object sender, EventArgs e)
        {
            Session_End(sender, e);
        }

        //public void Application_Error(object sender, EventArgs e)
        //{
        //    //System.Web.HttpContext web = System.Web.HttpContext.Current;
        //    //string msgErro = PinheiroSereniException.Exception(((Chat.MvcApplication)sender).Context.Server.GetLastError(), GetType().FullName, PinheiroSereniException.ErrorType.GenericError);

        //    //// Redirecionar para a página de erro
        //    //((Chat.MvcApplication)sender).Response.Redirect("Error/" + msgErro);



        //    //System.Web.HttpContext web = System.Web.HttpContext.Current;

        //    //Exception error = Server.GetLastError();
        //    //int code = (error is HttpException) ? (error as HttpException).GetHttpCode() : 500;

        //    //PinheiroSereniException.ErrorType error_type = PinheiroSereniException.ErrorType.GenericError;
        //    //if (code == 404) error_type = PinheiroSereniException.ErrorType.UrlNotFondMessage;
        //    //string msgErro = PinheiroSereniException.Exception(((MvcApplication)sender).Context.Server.GetLastError(), GetType().FullName, error_type);

        //    // Redirecionar para a página de erro
        //    //((PontoEletronico.Web.MvcApplication)sender).Response.Redirect("Error/" + msgErro);

        //    //((MvcApplication)sender).Response.Redirect("Error"); // + msgErro);

            

        //}
    }
}