using PinheiroSereni.Dominio.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PinheiroSereni.Negocio.Repositories.Chat
{
    public class LoginChatRepository : Repository
    {
        [DisplayName("ID do Operador *")]
        [Required(ErrorMessage = "Por favor, informe o seu identificador de acesso")]
        public int corretorId { get; set; }

        [DisplayName("Senha *")]
        [Required(ErrorMessage = "Por favor, informe a senha de acesso")]
        [StringLength(10, ErrorMessage = "A senha deve ter no máximo 10 caracteres")]
        [DataType(DataType.Password)]
        public string senha { get; set; }
    }
}
