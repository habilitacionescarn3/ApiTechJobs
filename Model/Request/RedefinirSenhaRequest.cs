namespace Model.Request;

public class RedefinirSenhaRequest
{
    public string Codigo { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
}
