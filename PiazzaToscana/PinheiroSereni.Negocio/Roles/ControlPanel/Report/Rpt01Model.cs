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
    public class Rpt01Model : Context, IListRepository
    {
        #region Métodos da Interface IListRepository
        public IPagedList getPagedList(int? index, int pageSize = 40, params object[] param)
        {
            try
            {
                int pageIndex = index ?? 0;

                IEnumerable<Rpt01Repository> list = (IEnumerable<Rpt01Repository>)ListRepository(param);

                PagedListObject First = new PagedListObject() { index = pageIndex };
                PagedListObject Last = new PagedListObject() { index = pageIndex };
                PagedListObject Prior = new PagedListObject() { index = pageIndex };
                PagedListObject Next = new PagedListObject() { index = pageIndex };

                PagedListObject[] routeValues = { First, Prior, Next, Last };

                return new PagedList<Rpt01Repository>(list.ToList(), pageIndex, pageSize, routeValues);
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
            string ordenacao = param[2].ToString();
            int empreendimentoId = (int)(param[3]);

            using (db = new PinheiroSereniContext())
            {
                IEnumerable<Rpt01Repository> q1 = (from p in db.Prospects
                                                   where p.dt_cadastro >= dt_inicio && p.dt_cadastro <= dt_fim &&
                                                         p.empreendimentoId == empreendimentoId
                                                   orderby p.nome 
                                                   select new Rpt01Repository
                                                   {
                                                        mensagem = new Validate() { Code = 0 },
                                                        sessionId = System.Web.HttpContext.Current.Session.SessionID,
                                                        email = p.email,
                                                        empreendimentoId = p.empreendimentoId,
                                                        nome = p.nome,
                                                        telefone = p.telefone,
                                                        dt_cadastro = p.dt_cadastro.Value,
                                                        isFolderDigital = p.isFolderDigital == "S" ? "SIM" : "NÃO",
                                                        isAtendimentoEmail = (from m in db.Mensagems
                                                                              where m.email == p.email
                                                                              select m.email).Count() > 0 ? "SIM" : "NÃO",
                                                        isChat = (from c in db.Chats
                                                                  where c.email == p.email
                                                                  select c.email).Count() > 0 ? "SIM" : "NÃO",
                                                        isSms = (from s in db.SMSs
                                                                 where s.telefone == p.telefone 
                                                                 select s.nome).Count() > 0 ? "SIM" : "NÃO"
                                                    }).ToList();

                if (ordenacao == "D")
                    return q1.OrderBy(m => m.dt_cadastro).ToList();
                else
                    return q1;
            }            
        }
        public Repository getRepository(Object id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
