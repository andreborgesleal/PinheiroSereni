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
    public class Rpt02Model : Context, IListRepository
    {
        #region Métodos da Interface IListRepository
        public IPagedList getPagedList(int? index, int pageSize = 40, params object[] param)
        {
            try
            {
                int pageIndex = index ?? 0;

                IEnumerable<Rpt02Repository> list = (IEnumerable<Rpt02Repository>)ListRepository(param);

                PagedListObject First = new PagedListObject() { index = pageIndex };
                PagedListObject Last = new PagedListObject() { index = pageIndex };
                PagedListObject Prior = new PagedListObject() { index = pageIndex };
                PagedListObject Next = new PagedListObject() { index = pageIndex };

                PagedListObject[] routeValues = { First, Prior, Next, Last };

                return new PagedList<Rpt02Repository>(list.ToList(), pageIndex, pageSize, routeValues);
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
            int empreendimentoId = (int)(param[3]);

            using (db = new PinheiroSereniContext())
            {
                DateTime d = dt_inicio;
                IList<Rpt02Repository> list = new List<Rpt02Repository>();
                while (d <= dt_fim)
                {
                    DateTime d1 = d;
                    DateTime d2 = d1.AddDays(1).AddMinutes(-1);
                    Rpt02Repository r = new Rpt02Repository()
                    {
                        dt_cadastro = d,
                        empreendimentoId = empreendimentoId,
                        qteAtendimentoEmail = (from a in db.Mensagems where a.dt_cadastro >= d1 && a.dt_cadastro <= d2 && a.empreendimentoId == empreendimentoId && (corretorId == null || a.corretorId == corretorId) select a).Count(),
                        qteChat = (from b in db.Chats where b.dt_inicio >= d1 && b.dt_inicio <= d2 && b.dt_fim != null && b.empreendimentoId == empreendimentoId && (corretorId == null || b.corretorId == corretorId) select b).Count(),
                        qteFolderDigital = (from c in db.Prospects where c.dt_cadastro >= d1 && c.dt_cadastro <= d2 && c.empreendimentoId == empreendimentoId && c.isFolderDigital == "S" select c).Count(),
                        qteSms = (from e in db.SMSs where e.dt_cadastro >= d1 && e.dt_cadastro <= d2 && e.empreendimentoId == empreendimentoId && (corretorId == null || e.corretorId == corretorId) select e).Count()
                    };

                    list.Add(r);

                    d = d.AddDays(1);
                }

                Rpt02Repository rT = new Rpt02Repository()
                {
                    qteAtendimentoEmail = list.Sum(m => m.qteAtendimentoEmail),
                    qteChat = list.Sum(m => m.qteChat),
                    qteFolderDigital = list.Sum(m => m.qteFolderDigital),
                    qteSms = list.Sum(m => m.qteSms)
                };

                list.Add(rT);

                return list;
            }
        }
        public Repository getRepository(Object id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
