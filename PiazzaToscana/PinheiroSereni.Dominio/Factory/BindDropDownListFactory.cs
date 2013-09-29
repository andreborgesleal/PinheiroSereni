using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.Linq;
using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Library;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Dominio.Entidades;

namespace PinheiroSereni.Dominio.Factory
{
    public class BindDropDownListFactory
    {
        public static IEnumerable<SelectListItem> Bind<T>(PinheiroSereniContext pinheiroSereni_db, string selectedValue = "", string header = "", params object[] param)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();
            IEnumerable<SelectListItem> entity = new List<SelectListItem>();

            IBindDropDownList Instance = (IBindDropDownList)Activator.CreateInstance(typeof(T));

            try
            {
                entity = (IEnumerable<SelectListItem>)Instance.List(pinheiroSereni_db, param);

                foreach (SelectListItem e in entity)
                    dic.Add(e.Value, e.Text);

                return Funcoes.getDropDownList(dic, selectedValue, header);
            }
            catch (Exception ex)
            {
                Validate result = new Validate() { Field = "", Message = PinheiroSereniException.Exception(ex, "PinheiroSereni.Dominio.Factory.BindDropDownList", PinheiroSereniException.ErrorType.DropDownListError) };
                return null;
            }
        }

        public static IEnumerable<SelectListItem> BindEnum<T>(string selectedValue = "", string header = "")
        {
            IBindDropDownListEnum Instance = (IBindDropDownListEnum)Activator.CreateInstance(typeof(T));
            return Instance.Bind(selectedValue, header);
        }


    }
}
