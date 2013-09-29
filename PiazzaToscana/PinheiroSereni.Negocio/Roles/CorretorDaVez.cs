using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace PinheiroSereni.Negocio.Roles
{
    public class CorretorDaVezMensagem<TEntity> : ICorretorDaVez<TEntity> where TEntity : class
    {
        public CorretorOnline obterCorretor(System.Data.Entity.DbSet<TEntity> value, DbSet<CorretorOnline> listCorretores)
        {
            if (value.Cast<Mensagem>().Count() > 0)
            {
                CorretorOnline ultimoCorretor = listCorretores.Find(value.Cast<Mensagem>().OrderByDescending(m => m.mensagemId).First().corretorId);
                CorretorOnline proxCorretor = listCorretores.Where(m => m.indexEscala > ultimoCorretor.indexEscala && m.situacao == "A").OrderBy(j => j.indexEscala).FirstOrDefault() ?? listCorretores.Where(m => m.situacao == "A").OrderBy(m => m.indexEscala).FirstOrDefault();   
                return proxCorretor;
            }
            else
                return listCorretores.Where(m => m.situacao == "A").OrderBy(m => m.indexEscala).FirstOrDefault();            
        }
    }

    public class CorretorDaVezSMS<TEntity> : ICorretorDaVez<TEntity> where TEntity : class
    {
        public CorretorOnline obterCorretor(System.Data.Entity.DbSet<TEntity> value, DbSet<CorretorOnline> listCorretores)
        {
            if (value.Cast<SMS>().Count() > 0)
            {
                CorretorOnline ultimoCorretor = listCorretores.Find(value.Cast<SMS>().OrderByDescending(m => m.smsId).First().corretorId);
                CorretorOnline proxCorretor = listCorretores.Where(m => m.indexEscala > ultimoCorretor.indexEscala && m.situacao == "A").OrderBy(j => j.indexEscala).FirstOrDefault() ?? listCorretores.Where(m => m.situacao == "A").OrderBy(m => m.indexEscala).FirstOrDefault();
                return proxCorretor;
            }
            else
                return listCorretores.Where(m => m.situacao == "A").OrderBy(m => m.indexEscala).FirstOrDefault();
        }
    }

    public class CorretorDaVezChat<TEntity> : ICorretorDaVez<TEntity> where TEntity : class
    {
        public CorretorOnline obterCorretor(System.Data.Entity.DbSet<TEntity> value, DbSet<CorretorOnline> listCorretores)
        {
            if (value.Cast<PinheiroSereni.Dominio.Entidades.Chat>().Count() > 0)
            {
                CorretorOnline ultimoCorretor = listCorretores.Find(value.Cast<PinheiroSereni.Dominio.Entidades.Chat>().OrderByDescending(m => m.chatId).First().corretorId);
                CorretorOnline proxCorretor = listCorretores.Where(m => m.indexEscala > ultimoCorretor.indexEscala && m.situacao == "A").OrderBy(j => j.indexEscala).FirstOrDefault() ?? listCorretores.Where(m => m.situacao == "A").OrderBy(m => m.indexEscala).FirstOrDefault();
                return proxCorretor;
            }
            else
                return listCorretores.Where(m => m.situacao == "A").OrderBy(m => m.indexEscala).FirstOrDefault();
        }
    }
}
