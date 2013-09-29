using PinheiroSereni.Dominio.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PinheiroSereni.Negocio.Roles.Chat;
using PinheiroSereni.Negocio.Repositories;

namespace ControlPanel.Controllers
{
    public class SuperController : Controller
    {
        protected System.Data.Common.DbTransaction trans = null;
        protected int PageSize = 40;
        protected Validate result { get; set; }

        #region Segurança

        protected ActionResult AccessDenied(string sessionId)
        {
            LoginChat l = new LoginChat();
            if (!l.validarSessao(sessionId))
                return RedirectToAction("Index");
            return View();
        }
        #endregion

    }
}
