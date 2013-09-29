using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinheiroSereni.Dominio.Entidades
{
    [Table("Parametro")]
    public class Parametro
    {
        [Key]
        [DisplayName("ID")]
        [Required(ErrorMessage = "ID do parâmetro deve ser informado")]
        public int parametroID { get; set; }

        [DisplayName("Nome do Parâmetro *")]
        [Required(ErrorMessage = "Nome do parâmetro deve ser informado")]
        public string nome { get; set; }

        [DisplayName("Valor do Parâmetro *")]
        [Required(ErrorMessage = "Valor do parâmetro deve ser informado")]
        public string valor { get; set; }
    }
}
