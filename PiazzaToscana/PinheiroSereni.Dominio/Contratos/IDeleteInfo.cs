using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace PinheiroSereni.Dominio.Contratos
{
    public interface IDeleteInfo
    {
        String DisplayColumn();
        String DisplayValue();
        IList<IDeleteInfo> ListDeleteFields(DbContext pinheiroSereni_db, params object[] param);
    }

    public interface IDeleteInfoPartial 
    {
        String Action();
        IList<IDeleteInfo> ListDeleteInfo();
    }

}
