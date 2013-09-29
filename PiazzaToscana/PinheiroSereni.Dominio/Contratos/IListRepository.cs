using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.Linq;
using System.Data.Entity;
using PinheiroSereni.Dominio.Control;

namespace PinheiroSereni.Dominio.Contratos
{
    public interface IListRepository
    {
        IPagedList getPagedList(int? index, int pageSize = 40, params object[] param);
        IEnumerable<Repository> ListRepository(params object[] param);
        Repository getRepository(Object id);
    }
}
