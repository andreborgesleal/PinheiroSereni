using System.Collections.Generic;
using PinheiroSereni.Library;
using System.Web.Mvc;
using PinheiroSereni.Dominio.Contratos;


namespace PinheiroSereni.Dominio.Enumeracoes
{
    public class drpStatusOperador : IBindDropDownListEnum
    {
        public IEnumerable<SelectListItem> Bind(string selectedValue = "", string header = "")
        {
            IDictionary<string, string> dic = new Dictionary<string, string>() { { "O", "ONLINE" }, { "I", "INVISÍVEL"} };
            return Funcoes.getDropDownList(dic, selectedValue, header);
        }
    }

    public class drpSituacao : IBindDropDownListEnum
    {
        public IEnumerable<SelectListItem> Bind(string selectedValue = "", string header = "")
        {
            IDictionary<string, string> dic = new Dictionary<string, string>() { { "A", "Ativado" }, { "D", "Desativado" } };
            return Funcoes.getDropDownList(dic, selectedValue, header);
        }
    }

    public class drpTipo : IBindDropDownListEnum
    {
        public IEnumerable<SelectListItem> Bind(string selectedValue = "", string header = "")
        {
            IDictionary<string, string> dic = new Dictionary<string, string>() { { "0", "Todos..." }, { "1", "Atendimento por e-mail" }, { "2", "Chat" }, { "3", "Ligamos para você" } };
            return Funcoes.getDropDownList(dic, selectedValue, header);
        }
    }

    public class drpOrdenacao : IBindDropDownListEnum
    {
        public IEnumerable<SelectListItem> Bind(string selectedValue = "", string header = "")
        {
            IDictionary<string, string> dic = new Dictionary<string, string>() { { "A", "Alfabética" }, { "D", "Data do Cadastro" } };
            return Funcoes.getDropDownList(dic, selectedValue, header);
        }
    }
}
