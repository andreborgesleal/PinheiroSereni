using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinheiroSereni.Dominio.Entidades
{
    [Table("Mensagem")]
    public class Mensagem
    {
        [Key]
        public int mensagemId { get; set; }

        [DisplayName("E-Mail *")]
        [StringLength(200, ErrorMessage = "O e-mail deve ter no máximo 200 caracteres")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "E-mail inválido.")]
        public string email { get; set; }

        [DisplayName("Empreendimento")]
        public int empreendimentoId { get; set; }

        public int corretorId { get; set; }

        [DisplayName("Assunto *")]
        [StringLength(100, ErrorMessage = "O assunto deve ter no máximo 100 caracteres")]
        public string assunto { get; set; }

        [DisplayName("Tire suas dúvidas aqui *")]
        [Required(ErrorMessage = "Por favor, informe a mensagem desejada")]
        [StringLength(500, ErrorMessage = "A mensagem deve ter no máximo 500 caracteres")]
        public string mensagem { get; set; }

        [DisplayName("Dt.Cadastro")]
        public DateTime? dt_cadastro { get; set; }

        public string emailDirecao1 { get; set; }

        public string emailDirecao2 { get; set; }

        public virtual Prospect Prospect { get; set; }

        public virtual CorretorOnline CorretorOnline { get; set; }

        public virtual Empreendimento Empreendimento { get; set; }
    }
}
