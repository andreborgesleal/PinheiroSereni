using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
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
    public class ParametrosController : SuperController
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

            ControllerFactory<ParamModel> factory = new ControllerFactory<ParamModel>();
            IPagedList pagedList = factory.getPagedList(index, int.Parse(pageSize));
            return View(pagedList);
        }
        #endregion

        #region Edit
        public ActionResult Edit(string id, string sucesso = "")
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            ControllerFactory<ParamModel> factory = new ControllerFactory<ParamModel>();
            ParamRepository value = (ParamRepository)factory.getObject(int.Parse(id));

            if (sucesso != null)
                if (sucesso.Trim() != "")
                {
                    value.mensagem = new Validate() { Code = 0, Message = sucesso };
                    ViewBag.Sucesso = sucesso;
                }

            return View(value);
        }

        [HttpPost]
        public ActionResult Edit(string id, ParamRepository value)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            value.mensagem = new Validate();

            if (ModelState.IsValid)
                try
                {
                    ControllerFactory<ParamModel> factory = new ControllerFactory<ParamModel>();

                    #region gravar os dados do parâmetro
                    value.parametro.parametroID = int.Parse(id);
                    value = (ParamRepository)factory.Edit(value);
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
    }
}
