using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Negocio.Repositories;
using PinheiroSereni.Negocio.Roles;
using PinheiroSereni.Negocio.Roles.ControlPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlPanel.Controllers
{
    public class SuperController : Controller
    {
        protected System.Data.Common.DbTransaction trans = null;
        protected int PageSize = 40;
        protected Validate result { get; set; }

        #region Segurança

        protected bool AccessDenied(string sessionId)
        {
            LoginAdmin l = new LoginAdmin();
            return !l.validarSessao(sessionId);
        }
        #endregion


    }
}
