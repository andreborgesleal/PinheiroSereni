using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Library;
using PinheiroSereni.Negocio.Repositories.ControlPanel.Report;

namespace PinheiroSereni.Negocio.Roles.ControlPanel.Report
{
    public class Rpt03ChatModel : Context, IListRepository
    {
        #region Métodos da Interface IListRepository
        public IPagedList getPagedList(int? index, int pageSize = 40, params object[] param)
        {
            try
            {
                int pageIndex = index ?? 0;

                IEnumerable<Rpt03ChatRepository> list = (IEnumerable<Rpt03ChatRepository>)ListRepository(param);

                PagedListObject First = new PagedListObject() { index = pageIndex };
                PagedListObject Last = new PagedListObject() { index = pageIndex };
                PagedListObject Prior = new PagedListObject() { index = pageIndex };
                PagedListObject Next = new PagedListObject() { index = pageIndex };

                PagedListObject[] routeValues = { First, Prior, Next, Last };

                return new PagedList<Rpt03ChatRepository>(list.ToList(), pageIndex, pageSize, routeValues);
            }
            catch (Exception ex)
            {
                throw new Exception(PinheiroSereniException.Exception(ex, GetType().FullName, PinheiroSereniException.ErrorType.PaginationError));
            }
        }
        public IEnumerable<Repository> ListRepository(params object[] param)
        {
            DateTime dt_inicio = (DateTime)param[0];
            DateTime dt_fim = ((DateTime)param[1]).AddDays(1).AddMinutes(-1);
            int? corretorId = (int?)param[2];
            int tipo = (int)param[3];

            using (db = new PinheiroSereniContext())
            {
                IList<Rpt03Repository> q1 = null;
                IList<Rpt03Repository> q2 = null;
                IList<Rpt03Repository> q3 = null;

                IList<Rpt03Repository> q = new List<Rpt03Repository>();

                // Todos ou Atendimento por E-Mail
                if (tipo == 0 || tipo == 1)
                {
                    q1 = (from p in db.Prospects
                          join m in db.Mensagems on p.email equals m.email
                          join c in db.CorretorOnlines on m.CorretorOnline equals c
                          where m.dt_cadastro >= dt_inicio && m.dt_cadastro <= dt_fim && (corretorId == null || m.corretorId == corretorId)
                          orderby m.dt_cadastro
                          select new Rpt03Repository
                          {
                              mensagem = new Validate() { Code = 0 },
                              sessionId = System.Web.HttpContext.Current.Session.SessionID,
                              id = m.mensagemId,
                              email = p.email,
                              nome = p.nome,
                              telefone = p.telefone,
                              dt_cadastro = m.dt_cadastro.Value,
                              nome_corretor = c.nome,
                              corretorId = m.corretorId,
                              tipo = "Atendimento por e-mail"
                          }).ToList();
                    q = q.Union(q1).ToList();
                }

                // Todos ou Chat
                if (tipo == 0 || tipo == 2)
                {
                    q2 = (from p in db.Prospects
                          join ch in db.Chats on p.email equals ch.email
                          join cor in db.CorretorOnlines on ch.CorretorOnline equals cor
                          where ch.dt_inicio >= dt_inicio && ch.dt_inicio <= dt_fim && (corretorId == null || ch.corretorId == corretorId)
                                && ch.dt_fim != null
                          orderby ch.dt_inicio
                          select new Rpt03Repository
                          {
                              mensagem = new Validate() { Code = 0 },
                              sessionId = System.Web.HttpContext.Current.Session.SessionID,
                              id = ch.chatId,
                              email = p.email,
                              nome = p.nome,
                              telefone = p.telefone,
                              dt_cadastro = ch.dt_inicio,
                              nome_corretor = cor.nome,
                              corretorId = ch.corretorId,
                              tipo = "Chat"
                          }).ToList();
                    q = q.Union(q2).ToList();
                }

                // Todos ou SMS
                if (tipo == 0 || tipo == 3)
                {
                    q3 = (from sms in db.SMSs
                          join c in db.CorretorOnlines on sms.CorretorOnline equals c
                          where sms.dt_cadastro >= dt_inicio && sms.dt_cadastro <= dt_fim && (corretorId == null || sms.corretorId == corretorId)
                          orderby sms.dt_cadastro
                          select new Rpt03Repository
                          {
                              mensagem = new Validate() { Code = 0 },
                              sessionId = System.Web.HttpContext.Current.Session.SessionID,
                              id = sms.smsId,
                              nome = sms.nome,
                              telefone = sms.telefone,
                              dt_cadastro = sms.dt_cadastro.Value,
                              nome_corretor = c.nome,
                              corretorId = sms.corretorId,
                              tipo = "Ligamos para você"
                          }).ToList();
                    q = q.Union(q3).ToList();
                }

                return q.OrderBy(m => m.dt_cadastro);
            }
        }
        public Repository getRepository(Object id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
