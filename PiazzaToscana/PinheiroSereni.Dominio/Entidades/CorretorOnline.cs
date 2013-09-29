using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinheiroSereni.Dominio.Entidades
{
    [Table("CorretorOnline")]
    public class CorretorOnline
    {
        [Key]
        [DisplayName("ID Corretor")]
        public int corretorId { get; set; }

        [DisplayName("Corretora *")]
        [Required(ErrorMessage = "Por favor, informe a corretora")]
        public int corretoraId { get; set; }

        [DisplayName("Nome *")]
        [Required(ErrorMessage = "Por favor, informe o nome do corretor")]
        [StringLength(20, ErrorMessage = "O nome deve ter no máximo 20 caracteres")]
        public string nome { get; set; }

        [DisplayName("Setor")]
        [StringLength(30, ErrorMessage = "O setor deve ter no máximo 30 caracteres")]
        public string setor { get; set; }

        [DisplayName("Telefone *")]
        [Required(ErrorMessage="O telefone do corretor deve ser informado")]
        public string telefone { get; set; }

        [DisplayName("E-Mail *")]
        [StringLength(200, ErrorMessage = "O e-mail deve ter no máximo 200 caracteres")]
        [Required(ErrorMessage = "Por favor, informe o e-mail do corretor")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "E-mail inválido.")]
        public string email { get; set; }

        [DisplayName("Foto (Formato PNG de 15kb à 70kb)")]
        [StringLength(200, ErrorMessage = "O caminho da foto deve ter no máximo 200 caracteres")]
        public string foto { get; set; }

        [DisplayName("Escala de atendimento * (Ex: 1, 2, 3...)")]
        [Required(ErrorMessage = "Por favor, informe a escala do corretor")]
        public int indexEscala { get; set; }

        [DisplayName("Situação *")]
        [Required(ErrorMessage = "Por favor, informe a situação do corretor")]
        public string situacao { get; set; }

        [DisplayName("Senha Chat *")]
        [Required(ErrorMessage = "Por favor, informe a senha de acesso ao Chat")]
        public string senha { get; set; }

        public virtual Corretora Corretora { get; set; }
    }
}
