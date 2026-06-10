using Model.Attributes;

namespace Model;

public class RecuperacaoSenha
{
    [IgnorarInsert]
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime DataExpiracao { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUtilizacao { get; set; }
}
