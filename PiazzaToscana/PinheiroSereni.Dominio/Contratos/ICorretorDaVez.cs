using PinheiroSereni.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace PinheiroSereni.Dominio.Contratos
{
    public interface ICorretorDaVez<TEntity> where TEntity : class
    {
        CorretorOnline obterCorretor(DbSet<TEntity> value, DbSet<CorretorOnline> listCorretores);
    }
}
