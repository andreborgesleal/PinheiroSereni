using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinheiroSereni.Dominio.Entidades
{
    [Table("SMS")]
    public class SMS
    {
        [Key]
        public int smsId { get; set; }

        public int corretorId { get; set; }

        [DisplayName("Empreendimento")]
        public int empreendimentoId { get; set; }

        [DisplayName("Nome *")]
        [Required(ErrorMessage = "Por favor, informe o nome para contato")]
        [StringLength(30, ErrorMessage = "O nome deve ter no máximo 30 caracteres")]
        public string nome { get; set; }

        [DisplayName("Telefone *")]
        //[RegularExpression(@"^\(\d{2}\)\d{4}-\d{4}$", ErrorMessage = "Telefone inválido.")]
        [Required(ErrorMessage = "Por favor, informe o telefone para contato")]
        public string telefone { get; set; }

        [DisplayName("Dt.Cadastro")]
        public DateTime? dt_cadastro { get; set; }

        public virtual CorretorOnline CorretorOnline { get; set; }
    }
}
