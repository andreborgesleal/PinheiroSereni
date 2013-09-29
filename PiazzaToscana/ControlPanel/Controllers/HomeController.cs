using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PinheiroSereni.Negocio.Repositories.ControlPanel;
using PinheiroSereni.Dominio.Factory;
using PinheiroSereni.Negocio.Roles.ControlPanel;
using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Negocio.Roles.Chat;

namespace ControlPanel.Controllers
{
    public class HomeController : SuperController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginAdminRepository value)
        {
            if (ModelState.IsValid)
                try
                {
                    LoginAdmin login = new LoginAdmin();
                    value.mensagem = login.autenticar(value.emailPinheiroSereni, value.autKey);
                    if (value.mensagem.Code > 0)
                        throw new PinheiroSereniException(value.mensagem);
                    Session.Add("sessionID", System.Web.HttpContext.Current.Session.SessionID);
                    return RedirectToAction("Principal");
                }
                catch (PinheiroSereniException ex)
                {
                    ModelState.AddModelError("", value.mensagem.Message);
                }
                catch (Exception ex)
                {
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString());
                }
            else
            {
                value = new LoginAdminRepository();
                value.mensagem = new Validate()
                {
                    Code = 999,
                    Message = MensagemPadrao.Message(999).ToString(),
                    MessageBase = MensagemPadrao.Message(999).ToString()
                };
            }

            return View(value);
        }

        public ActionResult Principal()
        {
            if (Session["sessionId"] == null)
                return RedirectToAction("Index", "Home");

            if (AccessDenied(Session["sessionId"].ToString()))
                return RedirectToAction("Index", "Home");

            System.Web.HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            System.Web.HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            System.Web.HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            System.Web.HttpContext.Current.Response.Cache.SetNoStore();

            return View();
        }

        public ActionResult Encerrar()
        {
            LoginAdmin l = new LoginAdmin();
            l.EncerrarSessao(System.Web.HttpContext.Current.Session.SessionID);

            return RedirectToAction("Index", "Home");
        }

    }
}
