using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Request;

public class AdicionarVagaRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string NivelExperiencia { get; set; } = string.Empty;
    public string? Cep { get; set; }
    public string? Numero { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal? SalarioPrevisto { get; set; }
    public bool Interna { get; set; }
    public DateTime? DataFimInscricoes { get; set; }
    public string? Tecnologias { get; set; }
    public string? Requisitos { get; set; }
    public string? Beneficios { get; set; }
}
