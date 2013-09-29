using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PinheiroSereni.Negocio.Repositories
{
    public class ProspectRepository : Repository
    {
        public Prospect prospect { get; set; }

        [DisplayName("Sobrenome *")]
        [StringLength(50, ErrorMessage = "O Sobrenome deve ter no máximo 50 caracteres")]
        public string sobreNome { get; set; }
       
        public string telefoneSemFormatacao
        {
            get
            {
                return PinheiroSereni.Library.Funcoes.RemoveCaracterEspecial(prospect.telefone);
            }
        }

        public string telefoneComFormatacao
        {
            get
            {
                return PinheiroSereni.Library.Funcoes.FormataTelefone(prospect.telefone);
            }
        }

        public string nomeCompleto
        {
            get
            {
                string n = prospect.nome.Trim().ToUpper();
                if (sobreNome != null)
                    n = (prospect.nome.Trim().ToUpper() + " " + sobreNome.Trim().ToUpper());
                if (n.Length > 50)
                    return n.Substring(0, 50);
                else
                    return n;
            }
        }
    }
}

