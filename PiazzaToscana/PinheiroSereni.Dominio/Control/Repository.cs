using PinheiroSereni.Dominio.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinheiroSereni.Dominio.Control
{
    public abstract class Repository 
    {
        public string sessionId { get; set; }
        public Validate mensagem { get; set; }
    }
}
