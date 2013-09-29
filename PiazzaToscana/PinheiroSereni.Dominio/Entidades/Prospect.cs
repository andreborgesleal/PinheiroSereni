using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinheiroSereni.Dominio.Entidades
{
    [Table("Prospect")]
    public class Prospect
    {
        [Key, Column(Order = 0)]
        [DisplayName("E-Mail *")]
        [StringLength(200, ErrorMessage = "O e-mail deve ter no máximo 200 caracteres")]
        [Required(ErrorMessage = "Por favor, informe o seu e-mail")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "E-mail inválido.")]
        public string email { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("Empreendimento")]
        public int empreendimentoId { get; set; }

        [DisplayName("Nome *")]
        [Required(ErrorMessage = "Por favor, informe o seu nome")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres")]
        public string nome { get; set; }

        [DisplayName("Telefone")]
        public string telefone { get; set; }

        [DisplayName("Dt.Cadastro")]
        public DateTime? dt_cadastro { get; set; }

        public string isFolderDigital { get; set; }

        public virtual Empreendimento Empreendimento { get; set; }

    }
}
