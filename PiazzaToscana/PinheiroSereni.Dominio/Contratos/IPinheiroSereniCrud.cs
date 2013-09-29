using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinheiroSereni.Library;
using PinheiroSereni.Dominio.Control;

namespace PinheiroSereni.Dominio.Contratos
{
    public interface IPinheiroSereniCrud
    {
        Repository getObject(Object id);
        Validate Validate(Repository value, PinheiroSereni.Dominio.Enumeracoes.Crud operation);
        IEnumerable<Repository> List();
        Repository Insert(Repository value);
        Repository Update(Repository value);
        Repository Delete(Repository value);
    }
}
