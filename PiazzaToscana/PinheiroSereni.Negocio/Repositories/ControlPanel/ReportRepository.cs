using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;

namespace PinheiroSereni.Negocio.Repositories.ControlPanel.Report
{
    public class Rpt01Repository : Repository
    {
        public string email { get; set; }
        public string nome { get; set; }
        public string telefone { get; set; }
        public DateTime dt_cadastro { get; set; }
        public string isFolderDigital { get; set; }
        public string isAtendimentoEmail { get; set; }
        public string isChat { get; set; }
        public string isSms { get; set; }
        public int empreendimentoId { get; set; }
    }

    public class Rpt02Repository : Repository
    {
        public DateTime? dt_cadastro { get; set; }
        public int? qteFolderDigital { get; set; }
        public int? qteAtendimentoEmail { get; set; }
        public int? qteChat { get; set; }
        public int? qteSms { get; set; }
        public int empreendimentoId { get; set; }
    }

    public class Rpt03Repository : Repository
    {
        public int? id { get; set; }
        public string email { get; set; }
        public string nome { get; set; }
        public string telefone { get; set; }
        public DateTime dt_cadastro { get; set; }
        public int? corretorId { get; set; }
        public string nome_corretor { get; set; }
        public string tipo { get; set; } // Atendimento por E-mail, Chat ou Ligamos para você
        public string mensagemEmail { get; set; }
        public int empreendimentoId { get; set; }
    }


}
