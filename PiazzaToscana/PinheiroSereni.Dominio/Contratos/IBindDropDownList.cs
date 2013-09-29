using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.Linq;
using PinheiroSereni.Dominio.Entidades;

namespace PinheiroSereni.Dominio.Contratos
{
    public interface IBindDropDownList
    {
        IEnumerable<SelectListItem> List(PinheiroSereniContext pinheiroSereni_db, params object[] param);
    }
}
