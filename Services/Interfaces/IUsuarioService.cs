using Microsoft.AspNetCore.Http;
using Model;
using Model.Request;
using Model.Response;

namespace Services.Interfaces;

public interface IUsuarioService
{
    Task NovoUsuario(NovoUsuarioRequest novoUsuario);
    LogarUsuarioResponse LogarUsuario(LogarUsuarioRequest logarUsuario);
    Task GerarValidacaoEmail(int idUsuario);
    void ValidarEmail(string codigo);
    Task SolicitarRecuperacaoSenha(RecuperarSenhaRequest request);
    void RedefinirSenha(RedefinirSenhaRequest request);
    Task EditarFotoPerfil(int idUsuario, IFormFile file);
    Task DeletarFotoPerfil(int idUsuario);
    Task<string?> GerarUrlAssinadaFotoPerfil(int idUsuario);
    Task<string?> GerarUrlAssinadaFotoPerfilCandidato(int idCandidato);
    IList<NotificacaoUsuario> ObterNotificacoes(int idUsuario);
    int ObterQuantidadeNotificacoesNaoLidas(int idUsuario);
    void MarcarNotificacaoComoLida(int id, int idUsuario);
    void MarcarTodasNotificacoesComoLidas(int idUsuario);
    bool? ObterValidacaoEmail(int idUsuario);
}
