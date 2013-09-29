using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Dominio.Factory;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Negocio.Repositories;
using PinheiroSereni.Negocio.Repositories.Chat;
using PinheiroSereni.Negocio.Roles;
using PinheiroSereni.Negocio.Roles.Chat;
using PinheiroSereni.Negocio.Roles.ControlPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControlPanel.Controllers;

namespace Chat.Controllers
{
    public class HomeController : SuperController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginChatRepository value)
        {
            if (ModelState.IsValid)
                try
                {
                    #region Autenticação do corretor e criação de sessão
                    LoginChat login = new LoginChat();
                    value.mensagem = login.autenticar(value.corretorId.ToString(), value.senha);
                    if (value.mensagem.Code > 0)
                        throw new PinheiroSereniException(value.mensagem);
                    #endregion

                    return RedirectToAction("Principal", new { sessionId = value.mensagem.Field });
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
                value = new LoginChatRepository();
                value.mensagem = new Validate()
                {
                    Code = 999,
                    Message = MensagemPadrao.Message(999).ToString(),
                    MessageBase = MensagemPadrao.Message(999).ToString()
                };
            }

            return View(value);
        }

        public ActionResult Principal(string sessionId)
        {
            AccessDenied(System.Web.HttpContext.Current.Session.SessionID);

            ChatModel chat = new ChatModel();
            SessaoRepository r = (SessaoRepository)chat.getSessao(sessionId);
            ViewBag.drpStatusOperador = BindDropDownListFactory.BindEnum<drpStatusOperador>("O");

            System.Web.HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            System.Web.HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            System.Web.HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            System.Web.HttpContext.Current.Response.Cache.SetNoStore();

            return View(r);
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Exit(string id)
        {
            AccessDenied(System.Web.HttpContext.Current.Session.SessionID);
            ChatModel chat = new ChatModel();
            chat.Exit(id);
            return RedirectToAction("index");
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ChangeStatusOperador(string id, string newStatus)
        {
            AccessDenied(System.Web.HttpContext.Current.Session.SessionID);
            ChatModel chat = new ChatModel();
            SessaoRepository r = (SessaoRepository)chat.changeStatusOperador(id, newStatus);
            return View(r);
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Listening(string id, int? chatId)
        {
            AccessDenied(System.Web.HttpContext.Current.Session.SessionID);

            ChatModel chat = new ChatModel();
            SessaoRepository r = (SessaoRepository)chat.getSessao(id, chatId);
            return View(r.chatRepositories);
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ActivateEdition(string id, int chatId)
        {
            AccessDenied(System.Web.HttpContext.Current.Session.SessionID);

            ChatModel chat = new ChatModel();
            SessaoRepository r = (SessaoRepository)chat.ActivateEdition(id, chatId);
            return View(r.chatRepositories);         
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Finish(string id, int chatId)
        {
            AccessDenied(System.Web.HttpContext.Current.Session.SessionID);

            ChatModel chat = new ChatModel();
            SessaoRepository r = (SessaoRepository)chat.Finish(id, chatId);
            return View(r.chatRepositories);
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [ValidateInput(false)]
        public ActionResult Send(string id, int chatId, string mensagem)
        {
            AccessDenied(System.Web.HttpContext.Current.Session.SessionID);

            ChatModel chat = new ChatModel();
            SessaoRepository r = (SessaoRepository)chat.Send(id, chatId, mensagem);
            return View(r.chatRepositories);
        }
    }
}
