using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Negocio.Repositories.Chat;
using System.Collections.Generic;

namespace PinheiroSereni.Negocio.Repositories
{
    public class SessaoRepository : Repository
    {
        public Sessao sessao { get; set; }
        public CorretorOnline corretorOnline { get; set; }
        public IEnumerable<ChatRepository> chatRepositories { get; set; }
    }
}
