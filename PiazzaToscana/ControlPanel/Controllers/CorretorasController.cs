using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Dominio.Factory;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Negocio.Repositories.ControlPanel;
using PinheiroSereni.Negocio.Roles.ControlPanel;
using System;
using System.Web.Mvc;

namespace ControlPanel.Controllers
{
    public class CorretorasController : SuperController
    {
        #region Browse
        public ActionResult Browse(int? index, string sucesso = "")
        {
            if (sucesso != null)
                if (sucesso.Trim() != "")
                    ViewBag.Sucesso = sucesso;

            return List(index, "40");
        }

        public ActionResult List(int? index, string pageSize)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            ControllerFactory<CorretoraCrud> factory = new ControllerFactory<CorretoraCrud>();
            IPagedList pagedList = factory.getPagedList(index, int.Parse(pageSize));
            return View(pagedList);
        }
        #endregion

        #region Create
        public ActionResult Create(string sucesso = "")
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            if (sucesso != null)
                if (sucesso.Trim() != "")
                    ViewBag.Sucesso = sucesso;

            return View();
        }

        [HttpPost]
        public ActionResult Create(CorretoraRepository value)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            value.mensagem = new Validate();

            if (ModelState.IsValid)
                try
                {
                    #region gravar os dados do corretor
                    value.mensagem = new Validate();
                    value.corretora.nome = value.corretora.nome.ToUpper();

                    ControllerFactory<CorretoraCrud> factory = new ControllerFactory<CorretoraCrud>();
                    value = (CorretoraRepository)factory.Insert(value);
                    if (value.mensagem.Code > 0)
                        throw new PinheiroSereniException(value.mensagem);
                    #endregion                

                    return RedirectToAction("Create", new { sucesso = MensagemPadrao.Message(0).ToString() + ". ID da corretora = " + value.corretora.corretoraId.ToString() });
                }
                catch (PinheiroSereniException ex)
                {
                    ModelState.AddModelError(value.mensagem.Field, ex.Result.Message);
                    return View(value);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();
                    return View(value);
                }
            else
            {
                value.mensagem.Code = 999;
                value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();
                return View(value);
            }
        }
        #endregion

        #region Edit
        public ActionResult Edit(string id, string sucesso = "")
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            ControllerFactory<CorretoraCrud> factory = new ControllerFactory<CorretoraCrud>();
            CorretoraRepository value = (CorretoraRepository)factory.getObject(int.Parse(id));

            if (sucesso != null)
                if (sucesso.Trim() != "")
                {
                    value.mensagem = new Validate() { Code = 0, Message = sucesso };
                    ViewBag.Sucesso = sucesso;
                }

            return View(value);
        }

        [HttpPost]
        public ActionResult Edit(string id, CorretoraRepository value)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            value.mensagem = new Validate();
            
            if (ModelState.IsValid)
                try
                {
                    ControllerFactory<CorretoraCrud> factory = new ControllerFactory<CorretoraCrud>();

                    #region gravar os dados da corretora
                    value.corretora.corretoraId = int.Parse(id);
                    value.corretora.nome = value.corretora.nome.ToUpper();
                    value = (CorretoraRepository)factory.Edit(value);
                    if (value.mensagem.Code > 0)
                        throw new PinheiroSereniException(value.mensagem);
                    #endregion

                    return RedirectToAction("Browse", new { sucesso = MensagemPadrao.Message(0).ToString() });
                }
                catch (PinheiroSereniException ex)
                {
                    ModelState.AddModelError(value.mensagem.Field, ex.Result.Message);
                    return View(value);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();
                    return View(value);
                }
            else
            {
                value.mensagem.Code = 999;
                value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();
                return View(value);
            }
        }

        #endregion

        #region Delete
        public ActionResult Delete(string id)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            ControllerFactory<CorretoraCrud> factory = new ControllerFactory<CorretoraCrud>();
            CorretoraRepository value = (CorretoraRepository)factory.getObject(int.Parse(id));

            return View(value);
        }

        [HttpPost]
        public ActionResult Delete(string id, CorretoraRepository value)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            value = value ?? new CorretoraRepository();
            value.mensagem = new Validate();

            if (ModelState.IsValid)
                try
                {
                    ControllerFactory<CorretoraCrud> factory = new ControllerFactory<CorretoraCrud>();
                    value = (CorretoraRepository)factory.getObject(int.Parse(id));

                    #region Exclui os dados da corretora
                    value = (CorretoraRepository)factory.Delete(value);
                    if (value.mensagem.Code > 0)
                        throw new PinheiroSereniException(value.mensagem);
                    #endregion

                    return RedirectToAction("Browse", new { sucesso = MensagemPadrao.Message(0).ToString() });
                }
                catch (PinheiroSereniException ex)
                {
                    ModelState.AddModelError(value.mensagem.Field, ex.Result.Message);
                    return View(value);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    value.mensagem.MessageBase = MensagemPadrao.Message(17).ToString();
                    return View(value);
                }
            else
            {
                ControllerFactory<CorretoraCrud> factory = new ControllerFactory<CorretoraCrud>();
                value = (CorretoraRepository)factory.getObject(int.Parse(id));

                value.mensagem.Code = 999;
                value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();
                return View(value);
            }
        }

        #endregion
    }
}
