using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace PinheiroSereni.Dominio.Entidades
{
    public sealed class PinheiroSereniContext : DbContext
    {
        static PinheiroSereniContext()
        {
            Database.SetInitializer<PinheiroSereniContext>(null);
        }

        public PinheiroSereniContext()
            : base("Name=PinheiroSereniContext")
		{
		}

        public DbSet<Prospect> Prospects { get; set; }
        public DbSet<Parametro> Parametros { get; set; }
        public DbSet<Corretora> Corretoras { get; set; }
        public DbSet<CorretorOnline> CorretorOnlines { get; set; }
        public DbSet<Mensagem> Mensagems { get; set; }
        public DbSet<SMS> SMSs { get; set; }
        public DbSet<Sessao> Sessaos { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Empreendimento> Empreendimentos { get; set; }
    }
}
