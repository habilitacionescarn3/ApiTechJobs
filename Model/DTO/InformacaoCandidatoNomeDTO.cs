using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class InformacaoCandidatoNomeDTO : InformacaoCandidato
    {
        public string Nome { get; set; }
        public bool EmailValidado { get; set; }
    }
}
