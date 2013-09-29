using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;
using System.Data.Entity;

namespace PinheiroSereni.Negocio.Repositories
{
    public class SMSRepository : Repository 
    {
        public SMS sms { get; set; }
        public ICorretorDaVez<SMS> corretor { get; set; }
    }
}
