using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Dominio.Factory;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Negocio.Repositories.ControlPanel;
using PinheiroSereni.Negocio.Roles.ControlPanel;
using PinheiroSereni.Negocio.Roles.ControlPanel.Report;
using System;
using System.Web.Mvc;
using PinheiroSereni.Negocio.Repositories.ControlPanel.Report;
using PinheiroSereni.Negocio.Roles.Chat;
using PinheiroSereni.Negocio.Repositories.Chat;
using System.Collections.Generic;

namespace ControlPanel.Controllers
{
    public class ReportController : SuperController
    {
        #region Listagem de clientes
        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Rpt01(int? index)
        {
            ViewBag.drpOrdenacao = BindDropDownListFactory.BindEnum<drpOrdenacao>("A");
            using (PinheiroSereniContext db = new PinheiroSereniContext())
                ViewBag.drpEmpreendimentos = BindDropDownListFactory.Bind<drpEmpreedimentos>(db, "1");
            return Rpt01List(index, DateTime.Today.AddDays(-30).ToString("dd/MM/yyyy"), DateTime.Today.ToString("dd/MM/yyyy"), "A", "1", "40");
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Rpt01List(int? index, string dt_inicio, string dt_fim, string ordenacao, string empreendimentoId, string pageSize)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            ControllerFactory<Rpt01Model> factory = new ControllerFactory<Rpt01Model>();
            IPagedList pagedList = factory.getPagedList(index, int.Parse(pageSize), DateTime.Parse(dt_inicio), DateTime.Parse(dt_fim), ordenacao, int.Parse(empreendimentoId));
            return View(pagedList);
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult Rpt01(FormCollection collection)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            return RedirectToAction("Rpt01Print", new { dt_inicio = collection["dt_inicio"].ToString(), dt_fim = collection["dt_fim"].ToString(), ordenacao = collection["ordenacao"].ToString(), empreendimentoId = collection ["empreendimentoId"] });
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Rpt01Print(string dt_inicio, string dt_fim, string ordenacao, string empreendimentoId)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");
            ViewBag.dt_inicio = dt_inicio;
            ViewBag.dt_fim = dt_fim;

            ControllerFactory<Rpt01Model> factory = new ControllerFactory<Rpt01Model>();
            IList<Rpt01Repository> list = (List<Rpt01Repository>)factory.ListRepository(DateTime.Parse(dt_inicio), DateTime.Parse(dt_fim), ordenacao, int.Parse(empreendimentoId));
            return View(list);
        }
        #endregion

        #region Estatística de atendimentos
        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Rpt02(int? index)
        {
            using (PinheiroSereniContext db = new PinheiroSereniContext())
            {
                ViewBag.drpCorretores = BindDropDownListFactory.Bind<drpCorretores>(db, "", "Todos...");
                ViewBag.drpEmpreendimentos = BindDropDownListFactory.Bind<drpEmpreedimentos>(db, "1");
            }

            return Rpt02List(index, DateTime.Today.AddDays(-30).ToString("dd/MM/yyyy"), DateTime.Today.ToString("dd/MM/yyyy"), null, "1", "15");
        }
        
        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Rpt02List(int? index, string dt_inicio, string dt_fim, string corretorId, string empreendimentoId, string pageSize)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToActionPermanent("Index", "Home");

            int? _corretorId = null;
            if (corretorId != null)
                if (corretorId != "")
                    _corretorId = int.Parse(corretorId);
            
            ControllerFactory<Rpt02Model> factory = new ControllerFactory<Rpt02Model>();
            IPagedList pagedList = factory.getPagedList(index, int.Parse(pageSize), DateTime.Parse(dt_inicio), DateTime.Parse(dt_fim), _corretorId, int.Parse(empreendimentoId));
            return View(pagedList);
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult Rpt02(FormCollection collection)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            return RedirectToAction("Rpt02Print", new { dt_inicio = collection["dt_inicio"].ToString(), dt_fim = collection["dt_fim"].ToString(), corretorId = collection["drpCorretor"].ToString(), empreendimentoId = collection["empreendimentoId"]});
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Rpt02Print(string dt_inicio, string dt_fim, string corretorId, string empreendimentoId)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");
            ViewBag.dt_inicio = dt_inicio;
            ViewBag.dt_fim = dt_fim;
            ViewBag.nome_corretor = "Todos os corretores";

            int? _corretorId = null;
            if (corretorId != null)
                if (corretorId != "")
                    _corretorId = int.Parse(corretorId);

            ControllerFactory<Rpt02Model> factory = new ControllerFactory<Rpt02Model>();
            IList<Rpt02Repository> list = (List<Rpt02Repository>)factory.ListRepository(DateTime.Parse(dt_inicio), DateTime.Parse(dt_fim), _corretorId, int.Parse(empreendimentoId));
            if (_corretorId.HasValue)
            {
                ControllerFactory<CorretorCrud> corr = new ControllerFactory<CorretorCrud>();
                ViewBag.nome_corretor = ((CorretorRepository)corr.getObject(_corretorId)).corretor.nome;
            }
            return View(list);
        }
        
        #endregion

        #region Desempenho HotSite
        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Rpt03(int? index)
        {
            using (PinheiroSereniContext db = new PinheiroSereniContext())
            {
                ViewBag.drpCorretores = BindDropDownListFactory.Bind<drpCorretores>(db, "", "Todos...");
                ViewBag.drpEmpreendimentos = BindDropDownListFactory.Bind<drpEmpreedimentos>(db, "1");
            }
            ViewBag.drpTipos = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpTipo>("A");

            return Rpt03List(index, DateTime.Today.AddDays(-30).ToString("dd/MM/yyyy"), DateTime.Today.ToString("dd/MM/yyyy"), null, "0", "1", "15");
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Rpt03List(int? index, string dt_inicio, string dt_fim, string corretorId, string tipo, string empreendimentoId, string pageSize)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToActionPermanent("Index", "Home");

            int? _corretorId = null;
            if (corretorId != null)
                if (corretorId != "")
                    _corretorId = int.Parse(corretorId);

            ControllerFactory<Rpt03Model> factory = new ControllerFactory<Rpt03Model>();
            IPagedList pagedList = factory.getPagedList(index, int.Parse(pageSize), DateTime.Parse(dt_inicio), DateTime.Parse(dt_fim), _corretorId, (tipo == "" ? 0 : int.Parse(tipo)), int.Parse(empreendimentoId));
            return View(pagedList);
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Rpt03DetailChat(int? index, string id, string pageSize)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToActionPermanent("Index", "Home");

            ControllerFactory<ChatModel> factory = new ControllerFactory<ChatModel>();
            ChatRepository chat = (ChatRepository)factory.getRepository(int.Parse(id));

            return View(chat);
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Rpt03DetailMens(int? index, string id, string pageSize)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToActionPermanent("Index", "Home");

            ControllerFactory<Rpt03Model> factory = new ControllerFactory<Rpt03Model>();
            Rpt03Repository r = (Rpt03Repository)factory.getRepository(int.Parse(id));

            return View(r);
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Rpt03DetailSms(int? index, string id, string pageSize)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToActionPermanent("Index", "Home");

            Rpt03Repository r = new Rpt03Repository()
            {
                mensagem = new Validate()
                {
                    Code = 40,
                    Message = "Esta funcionalidade não possuir detalhamento",
                    MessageBase = "Somente os itens Chat e Atendimento por E-Mail têm detalhes para exibir"
                }
            };

            return View(r);
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult Rpt03(FormCollection collection)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            return RedirectToAction("Rpt03Print", new { dt_inicio = collection["dt_inicio"].ToString(), dt_fim = collection["dt_fim"].ToString(), corretorId = collection["drpCorretor"].ToString(), tipo = collection["drpTipo"].ToString(), empreendimentoId = collection["empreendimentoId"] });
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Rpt03Print(string dt_inicio, string dt_fim, string corretorId, string tipo, string empreendimentoId)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");
            ViewBag.dt_inicio = dt_inicio;
            ViewBag.dt_fim = dt_fim;
            ViewBag.nome_corretor = "Todos os corretores";
            ViewBag.tipo = "Todos os tipos";

            int? _corretorId = null;
            if (corretorId != null)
                if (corretorId != "")
                    _corretorId = int.Parse(corretorId);

            ControllerFactory<Rpt03Model> factory = new ControllerFactory<Rpt03Model>();
            IList<Rpt03Repository> list = (List<Rpt03Repository>)factory.ListRepository(DateTime.Parse(dt_inicio), DateTime.Parse(dt_fim), _corretorId, (tipo == "" ? 0 : int.Parse(tipo)), int.Parse(empreendimentoId));
            if (_corretorId.HasValue)
            {
                ControllerFactory<CorretorCrud> corr = new ControllerFactory<CorretorCrud>();
                ViewBag.nome_corretor = ((CorretorRepository)corr.getObject(_corretorId)).corretor.nome;
            }
            // "1", "Atendimento por e-mail" }, { "2", "Chat" }, { "3", "Ligamos para você"
            switch (tipo)
            {
                case "1":
                    ViewBag.tipo = "Atendimento por e-mail";
                    break;
                case "2":
                    ViewBag.tipo = "Chat";
                    break;
                case "3":
                    ViewBag.tipo = "Ligamos para você";
                    break;
            }
            
            return View(list);
        }

        #endregion
    }
}
