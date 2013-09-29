using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PinheiroSereni.Negocio.Repositories.ControlPanel
{
    public class LoginAdminRepository : Repository
    {
        [DisplayName("E-Mail *")]
        [Required(ErrorMessage = "Por favor, informe o seu e-mail de acesso")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "E-mail inválido.")]
        [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres")]
        public string emailPinheiroSereni { get; set; }

        [DisplayName("Senha *")]
        [Required(ErrorMessage = "Por favor, informe a senha de acesso")]
        [StringLength(10, ErrorMessage = "A senha deve ter no máximo 10 caracteres")]
        [DataType(DataType.Password)]
        public string autKey { get; set; }
    }
}
