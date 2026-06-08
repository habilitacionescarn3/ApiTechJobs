using Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Response;

public class AplicacaoCandidatoResponse : VagaCandidatoResponse
{
    public EnumSituacao Situacao { get; set; }
    public DateTime DataAtualizacaoAplicacao { get; set; }
}
