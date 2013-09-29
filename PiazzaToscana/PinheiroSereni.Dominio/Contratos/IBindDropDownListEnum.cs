using System.Collections.Generic;
using System.Web.Mvc;

namespace PinheiroSereni.Dominio.Contratos
{
    public interface IBindDropDownListEnum
    {
        IEnumerable<SelectListItem> Bind(string selectedValue = "", string header = "");
    }
}
