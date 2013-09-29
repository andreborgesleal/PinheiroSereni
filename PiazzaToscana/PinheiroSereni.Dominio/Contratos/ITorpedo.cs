using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinheiroSereni.Dominio.Contratos
{
    public interface ITorpedo
    {
        Validate send(string mensagem, string nome, string telefoneCliente, string telefoneCorretor);
    }
}
