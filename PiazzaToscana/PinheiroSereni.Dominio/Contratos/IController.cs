using PinheiroSereni.Dominio.Control;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PinheiroSereni.Dominio.Contratos
{
    public interface IController<T>
    {
        IPagedList getPagedList(int? index, int pageSize = 40, params object[] param);
        IEnumerable<Repository> ListRepository(params object[] param);
        Repository getRepository(Object id);
        Repository getObject(Object id);
        Repository Insert(Repository repository);
        Repository Edit(Repository repository);
        Repository Delete(Repository repository);
        //IDeleteInfoPartial DeleteInfo(String action, params object[] param);
    }
}