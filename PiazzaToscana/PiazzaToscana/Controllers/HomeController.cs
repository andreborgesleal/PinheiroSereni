using PiazzaToscana.Models;
using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Dominio.Factory;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Negocio.Repositories;
using PinheiroSereni.Negocio.Roles;
using PinheiroSereni.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PinheiroSereni.Negocio.Roles.Chat;
using PinheiroSereni.Negocio.Repositories.Chat;

namespace PiazzaToscana.Controllers
{
    public class HomeController : Controller
    {
        #region Inscricao
        [ValidateInput(false)]
        public ActionResult Index(string sucesso)
        {
            HttpContext.Server.ScriptTimeout = 900;
            if (sucesso != null)
                ViewBag.Sucesso = sucesso;
            return View();
        }

        [HttpPost]
        public ActionResult Index(ProspectRepository value)
        {
            HttpContext.Server.ScriptTimeout = 900;
            if (ModelState.IsValid)
                try
                {
                    ControllerFactory<Inscricao> factory = new ControllerFactory<Inscricao>();
                    value.prospect.empreendimentoId = 1;  // Piazza Toscana
                    value = (ProspectRepository)factory.Insert(value);
                    if (value.mensagem.Code > 0)
                        throw new PinheiroSereniException(value.mensagem);
                    return RedirectToAction("index", new { sucesso = value.mensagem.ToString() });
                }
                catch (PinheiroSereniException ex)
                {
                    ModelState.AddModelError(value.mensagem.Field, value.mensagem.Message);
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", MensagemPadrao.Message(5, "Sobrenome").ToString());
                    value.mensagem = new Validate()
                    {
                        Code = 5,
                        Message = ex.Message
                    };
                }
                catch (Exception ex)
                {
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString());
                }
            else
            {
                value = new ProspectRepository() ;
                value.mensagem = new Validate()
                {
                    Code = 999,
                    Message = MensagemPadrao.Message(999).ToString()
                };
            }

            return View(value);
        }
        #endregion

        #region Atendimento por E-Mail
        public ActionResult AtendimentoEmail(string sucesso)
        {
            if (sucesso != null)
                ViewBag.Sucesso = sucesso;
            return View();
        }

        [HttpPost]
        public ActionResult AtendimentoEmail(AtendimentoEmailRepository value)
        {
            if (ModelState.IsValid)
                try
                {
                    ControllerFactory<AtendimentoEmail> factory = new ControllerFactory<AtendimentoEmail>();
                    value.corretor = new CorretorDaVezMensagem<Mensagem>();
                    value.msg.mensagem = value.msg.mensagem.Replace("'", "''");
                    value.prospect.nome = value.prospect.nome.ToUpper();
                    value.prospect.email = value.prospect.email.ToLower();
                    value.prospect.empreendimentoId = 1;
                    if (value.prospect.telefone != null)
                        value.prospect.telefone = value.prospect.telefone.Replace("(", "").Replace(")", "").Replace("-", "");
                    value = (AtendimentoEmailRepository)factory.Insert(value);
                    if (value.mensagem.Code > 0)
                        throw new PinheiroSereniException(value.mensagem);
                    return RedirectToAction("Index", new { sucesso = value.mensagem.ToString() });
                }
                catch (PinheiroSereniException ex)
                {
                    ModelState.AddModelError(value.mensagem.Field, value.mensagem.Message);
                }
                catch (Exception ex)
                {
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString() };
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString());
                }
            else
            {
                value = new AtendimentoEmailRepository();
                value.mensagem = new Validate()
                {
                    Code = 999,
                    Message = MensagemPadrao.Message(999).ToString()
                };
            }

            return View(value);
        }
        #endregion

        #region Ligamos para você
        public ActionResult LigamosParaVoce(string sucesso)
        {
            if (sucesso != null)
                ViewBag.Sucesso = sucesso;
            return View();
        }

        [HttpPost]
        public ActionResult LigamosParaVoce(SMSRepository value)    
        {
            if (ModelState.IsValid)
                try
                {
                    ControllerFactory<LigamosParaVoce> factory = new ControllerFactory<LigamosParaVoce>();
                    value.corretor = new CorretorDaVezSMS<SMS>();
                    value.sms.nome = value.sms.nome.ToUpper();
                    value.sms.telefone = value.sms.telefone.Replace("(", "").Replace(")", "").Replace("-", "");
                    value.sms.empreendimentoId = 1;
                    value = (SMSRepository)factory.Insert(value);
                    if (value.mensagem.Code > 0)
                        throw new PinheiroSereniException(value.mensagem.MessageBase, GetType().FullName);
                    return RedirectToAction("Index", new { sucesso = value.mensagem.ToString() });
                }
                catch (PinheiroSereniException ex)
                {
                    ModelState.AddModelError(value.mensagem.Field, value.mensagem.Message);
                }
                catch (Exception ex)
                {
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString() };
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString());
                }
            else
            {
                value = new SMSRepository();
                value.mensagem = new Validate()
                {
                    Code = 999,
                    Message = MensagemPadrao.Message(999).ToString()
                };
            }
            return View(value);
        }
        #endregion

        #region Corretor Online
        public ActionResult Chat()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Chat(ProspectRepository value)
        {
            System.Web.HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            System.Web.HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            System.Web.HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            System.Web.HttpContext.Current.Response.Cache.SetNoStore();

            if (ModelState.IsValid)
                try
                {
                    ChatModel chatModel = new ChatModel();
                    ChatRepository r = new ChatRepository();
                    r.mensagem = new Validate();
                    r.prospect = new Prospect()
                    {
                        email = value.prospect.email.ToLower(),
                        empreendimentoId = 1,
                        nome = value.prospect.nome.ToUpper()
                    };
                    r.chat = new Chat();
                    if (value.prospect.telefone != null)
                        r.prospect.telefone = value.prospect.telefone.Replace("(", "").Replace(")", "").Replace("-", "");
                    r.corretorDaVez = new CorretorDaVezChat<Chat>();
                    r.chat.empreendimentoId = 1;

                    r = (ChatRepository)chatModel.Start(r);

                    if (r.mensagem.Code > 0)
                        throw new PinheiroSereniException(value.mensagem);

                    TempData.Remove("typing");
                    TempData.Add("typing", "");
                    
                    return RedirectToAction("Talk", new { chatId = r.chat.chatId });
                }
                catch (PinheiroSereniException ex)
                {
                    value.mensagem = new Validate() { Code = 35, Message = MensagemPadrao.Message(35).ToString() };
                    ModelState.AddModelError(value.mensagem.Field, value.mensagem.Message);
                }
                catch (Exception ex)
                {
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    value.mensagem = new Validate() { Code = 35, Message = MensagemPadrao.Message(35).ToString() };
                    ModelState.AddModelError("", MensagemPadrao.Message(35).ToString());
                }
            else
            {
                value = new ProspectRepository();
                value.mensagem = new Validate()
                {
                    Code = 999,
                    Message = MensagemPadrao.Message(999).ToString()
                };
            }

            return View(value);
        }

        public ActionResult Talk(string chatId, string sucesso)
        {
            ChatModel chatModel = new ChatModel();
            ChatRepository r = (ChatRepository)chatModel.getRepository(chatId);
            return View(r);
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Listening(string id, string mensagem)
        {
            ChatModel chatModel = new ChatModel();
            string mudou = TempData.Peek("typing").ToString() != mensagem ? "S" : "N";

            ChatRepository r = (ChatRepository)chatModel.getRepository(id);

            chatModel.TypingClient(int.Parse(id), mudou);

            TempData.Remove("typing");
            TempData.Add("typing", mensagem);

            return View(r);
        }

        [ValidateInput(false)]
        public ActionResult Send(string id, string mensagem)
        {
            ChatModel chatModel = new ChatModel();
            ChatRepository r = (ChatRepository)chatModel.Send(int.Parse(id), mensagem);

            TempData.Remove("typing");
            TempData.Add("typing", "");

            return View(r);
        }

        public ActionResult End(string id)
        {
            ChatModel chatModel = new ChatModel();
            ChatRepository r = (ChatRepository)chatModel.ChatOver(int.Parse(id));
            return View(r);
        }

        #endregion

        public ActionResult ShowPic(string img)
        {
            ViewBag.Imagem = img;
            return View();
        }

        public ActionResult EmConstrucao()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
