using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;
using System.Data.Entity;

namespace PinheiroSereni.Negocio.Repositories
{
    public class AtendimentoEmailRepository : Repository 
    {
        public Prospect prospect { get; set; }
        public Mensagem msg { get; set; }
        public ICorretorDaVez<Mensagem> corretor { get; set; }
    }
}
