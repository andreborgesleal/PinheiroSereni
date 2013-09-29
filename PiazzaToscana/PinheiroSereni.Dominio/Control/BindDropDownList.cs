using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.Linq;
using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Entidades;
using System.Linq;
using System.Data.Entity;
using System.Data.Objects.SqlClient;

namespace PinheiroSereni.Dominio.Control
{

    public class drpCorretoras : IBindDropDownList
    {
        public IEnumerable<SelectListItem> List(PinheiroSereniContext pinheiroSereni_db, params object[] param)
        {
            IEnumerable<SelectListItem> q = (from f in pinheiroSereni_db.Corretoras
                                             select new SelectListItem()
                                             {
                                                 Value = SqlFunctions.StringConvert((decimal)f.corretoraId).Trim(),
                                                 Text = f.nome
                                             }).ToList();
                
            return q; 
        }

    }

    public class drpCorretores : IBindDropDownList
    {
        public IEnumerable<SelectListItem> List(PinheiroSereniContext pinheiroSereni_db, params object[] param)
        {
            IEnumerable<SelectListItem> q = (from f in pinheiroSereni_db.CorretorOnlines
                                             select new SelectListItem()
                                             {
                                                 Value = SqlFunctions.StringConvert((decimal)f.corretorId).Trim(),
                                                 Text = f.nome
                                             }).ToList();

            return q;
        }

    }

    public class drpEmpreedimentos : IBindDropDownList
    {
        public IEnumerable<SelectListItem> List(PinheiroSereniContext pinheiroSereni_db, params object[] param)
        {
            IEnumerable<SelectListItem> q = (from f in pinheiroSereni_db.Empreendimentos
                                             select new SelectListItem()
                                             {
                                                 Value = SqlFunctions.StringConvert((decimal)f.empreendimentoId).Trim(),
                                                 Text = f.nomeEmpreendimento
                                             }).ToList();

            return q;
        }

    }


}
