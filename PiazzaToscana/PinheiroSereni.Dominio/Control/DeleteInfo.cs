using PinheiroSereni.Dominio.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinheiroSereni.Dominio.Control
{
    public class DeleteInfo : IDeleteInfoPartial
    {
        private String _Action { get; set; }
        private IList<IDeleteInfo> _ListDeleteInfo { get; set; }

        public DeleteInfo(IList<IDeleteInfo> list, String action = "")
        {
            _Action = action;
            _ListDeleteInfo = list;
        }

        public String Action() { return _Action; }
        public IList<IDeleteInfo> ListDeleteInfo() { return _ListDeleteInfo; }
    }
}
