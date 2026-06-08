using Model.Attributes;
using Model.Enum;

namespace Model;

public class CandidatoVaga
{

    [IgnorarInsert]
    public int Id { get; set; }
    public int IdCandidato { get; set; }
    public int IdVaga { get; set; }
    public EnumSituacao Situacao { get; set; }
    public string? FileKey { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
