using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model;
using Model.DTO;
using Model.Enum;
using Model.Options;
using Model.Request;
using Model.Response;
using Repositories;
using Services.Interfaces;
using Services.Utils.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Utils;

namespace Services;

public class UsuarioService(IOptions<JwtSettings> jwt, IOptions<FrontEndSettings> frontEndSettings, UsuarioRepository usuarioRespository,
    ICandidatoService candidatoService, IEmpresaService empresaService, CandidatoRepository candidatoRepository,
    NotificacaoUsuarioRepository notificacaoUsuarioRepository, ValidacaoEmailRepository validacaoEmailRepository,
    RecuperacaoSenhaRepository recuperacaoSenhaRepository, IAwsService awsService) : IUsuarioService
{
    private readonly JwtSettings _jwt = jwt.Value;
    private readonly FrontEndSettings _frontEndSettings = frontEndSettings.Value;
    private readonly UsuarioRepository _usuarioRepository = usuarioRespository;
    private readonly ICandidatoService _candidatoService = candidatoService;
    private readonly IEmpresaService _empresaService = empresaService;

    private readonly string _folderProfile = "profile";

    public async Task NovoUsuario(NovoUsuarioRequest novoUsuario)
    {
        var usuarioExiste = _usuarioRepository.ObterPorLoginEDocumento(novoUsuario.Login, novoUsuario.Documento) is not null;

        if (usuarioExiste)
            throw new Exception("Usuário já existe na plataforma. Verifique os dados e tente novamente");

        var usuario = new Usuario
        {
            DataCadastro = DateTime.Now,
            Login = novoUsuario.Login,
            Perfil = novoUsuario.Perfil,
            Senha = novoUsuario.Senha.CriptografarSenha()
        };

        int idUsuario = _usuarioRepository.Adicionar(usuario);

        switch (novoUsuario.Perfil)
        {
            case EnumPerfil.Candidato:
                var candidato = new Candidato
                {
                    Cpf = novoUsuario.Documento,
                    Email = novoUsuario.Login,
                    IdUsuario = idUsuario,
                    Nome = novoUsuario.Nome,
                };

                _candidatoService.Adicionar(candidato);
                break;

            case EnumPerfil.Empresa:
                var empresa = new Empresa
                {
                    Cep = novoUsuario.Cep,
                    Cnpj = novoUsuario.Documento,
                    Email = novoUsuario.Login,
                    IdUsuario = idUsuario,
                    Nome = novoUsuario.Nome,
                    Numero = novoUsuario.Numero
                };

                _empresaService.Adicionar(empresa);
                break;
        }

        await awsService.SendEmailTemplate(novoUsuario.Login, "TechJobs_BoasVindas", new
        {
            nome = novoUsuario.Nome,
            loginLink = _frontEndSettings.BaseUrl
        });
    }

    public LogarUsuarioResponse LogarUsuario(LogarUsuarioRequest logarUsuario)
    {
        var usuario = _usuarioRepository.ObterCredenciaisUsuario(logarUsuario.Login) ?? throw new Exception("Usuario ou senha incorretos!");

        if (!usuario.Senha.VerificarSenha(logarUsuario.Senha)) throw new Exception("Usuario ou senha incorretos!");

        return GerarToken(usuario);
    }

    public async Task GerarValidacaoEmail(int idUsuario)
    {
        var usuario = _usuarioRepository.ObterDadosUsuario(idUsuario)
            ?? throw new Exception("Usuário não encontrado.");

        if (usuario.EmailValidado)
            throw new Exception("O e-mail deste usuário já foi validado.");

        await EnviarValidacaoEmail(usuario.Id, usuario.Email, usuario.Nome);
    }

    public void ValidarEmail(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new Exception("Código de validação inválido.");

        var tokenHash = GerarTokenHash(codigo);
        var validacaoEmail = validacaoEmailRepository.ObterPorTokenHash(tokenHash)
            ?? throw new Exception("Código de validação inválido.");

        if (validacaoEmail.DataValidacao.HasValue)
            throw new Exception("Este código de validação já foi utilizado.");

        var dataValidacao = DateTime.UtcNow;

        if (validacaoEmail.DataExpiracao < dataValidacao)
            throw new Exception("Código de validação expirado.");

        if (!validacaoEmailRepository.Validar(tokenHash, validacaoEmail.IdUsuario, dataValidacao))
            throw new Exception("Código de validação inválido.");
    }

    public async Task SolicitarRecuperacaoSenha(RecuperarSenhaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login))
            return;

        var usuario = _usuarioRepository.ObterCredenciaisUsuario(request.Login);

        if (usuario == null)
            return;

        if (string.IsNullOrWhiteSpace(_frontEndSettings.BaseUrl))
            throw new Exception("URL de recuperação de senha não configurada.");

        var codigo = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var dataCriacao = DateTime.UtcNow;

        recuperacaoSenhaRepository.Adicionar(new RecuperacaoSenha
        {
            IdUsuario = usuario.Id,
            TokenHash = GerarTokenHash(codigo),
            DataCriacao = dataCriacao,
            DataExpiracao = dataCriacao.AddHours(1)
        });

        var resetPasswordLink = $"{_frontEndSettings.BaseUrl}/redefinir-senha?codigo={Uri.EscapeDataString(codigo)}";

        await awsService.SendEmailTemplate(usuario.Email, "TechJobs_RecuperacaoSenha", new
        {
            nome = usuario.Nome,
            resetPasswordLink
        });
    }

    public void RedefinirSenha(RedefinirSenhaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Codigo))
            throw new Exception("Código de recuperação de senha inválido.");

        if (string.IsNullOrWhiteSpace(request.NovaSenha))
            throw new Exception("A nova senha deve ser informada.");

        var tokenHash = GerarTokenHash(request.Codigo);
        var recuperacaoSenha = recuperacaoSenhaRepository.ObterPorTokenHash(tokenHash)
            ?? throw new Exception("Código de recuperação de senha inválido.");

        if (recuperacaoSenha.DataUtilizacao.HasValue)
            throw new Exception("Este código de recuperação de senha já foi utilizado.");

        var dataUtilizacao = DateTime.UtcNow;

        if (recuperacaoSenha.DataExpiracao < dataUtilizacao)
            throw new Exception("Código de recuperação de senha expirado.");

        var senha = request.NovaSenha.CriptografarSenha();

        if (!recuperacaoSenhaRepository.RedefinirSenha(tokenHash, recuperacaoSenha.IdUsuario, senha, dataUtilizacao))
            throw new Exception("Código de recuperação de senha inválido.");
    }

    public async Task EditarFotoPerfil(int idUsuario, IFormFile file)
    {
        var usuario = _usuarioRepository.ObterPorId(idUsuario);

        if (!string.IsNullOrWhiteSpace(usuario?.ChaveFotoPerfil))
            await awsService.RemoveFileAsync(usuario.ChaveFotoPerfil);

        var fileKey = await awsService.UploadFileAsync(file, _folderProfile);

        _usuarioRepository.AtualizarChaveArquivo(idUsuario, fileKey);
    }

    public async Task DeletarFotoPerfil(int idUsuario)
    {
        var usuario = _usuarioRepository.ObterPorId(idUsuario);

        if (!string.IsNullOrWhiteSpace(usuario?.ChaveFotoPerfil))
        {
            await awsService.RemoveFileAsync(usuario.ChaveFotoPerfil);

            _usuarioRepository.AtualizarChaveArquivo(idUsuario, null);
        }
    }

    public async Task<string?> GerarUrlAssinadaFotoPerfil(int idUsuario)
    {
        var usuario = _usuarioRepository.ObterPorId(idUsuario);

        if (string.IsNullOrWhiteSpace(usuario?.ChaveFotoPerfil))
            return null;

        return await awsService.PreSignedURL(usuario.ChaveFotoPerfil);
    }

    public async Task<string?> GerarUrlAssinadaFotoPerfilCandidato(int idCandidato)
    {
        var candidato = candidatoRepository.ObterPorId(idCandidato);

        if (candidato == null)
            return null;

        return await GerarUrlAssinadaFotoPerfil(candidato.IdUsuario);
    }

    public IList<NotificacaoUsuario> ObterNotificacoes(int idUsuario) =>
        notificacaoUsuarioRepository.ObterPorUsuario(idUsuario);

    public int ObterQuantidadeNotificacoesNaoLidas(int idUsuario) =>
        notificacaoUsuarioRepository.ObterQuantidadeNaoLidas(idUsuario);

    public void MarcarNotificacaoComoLida(int id, int idUsuario) =>
        notificacaoUsuarioRepository.MarcarComoLida(id, idUsuario);

    public void MarcarTodasNotificacoesComoLidas(int idUsuario) =>
        notificacaoUsuarioRepository.MarcarTodasComoLidas(idUsuario);

    private LogarUsuarioResponse GerarToken(CredenciaisUsuarioDTO usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_jwt.SigningKey ?? "");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim("Id", usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
            ]),
            Expires = DateTime.UtcNow.AddDays(1),
            Audience = _jwt.Audience,
            Issuer = _jwt.Issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new LogarUsuarioResponse
        {
            Token = tokenHandler.WriteToken(token),
            Email = usuario.Email,
            NomeUsuario = usuario.Nome,
            Perfil = usuario.Perfil,
        };
    }

    private async Task EnviarValidacaoEmail(int idUsuario, string email, string nome)
    {
        if (string.IsNullOrWhiteSpace(_frontEndSettings.BaseUrl))
            throw new Exception("URL de validação de e-mail não configurada.");

        var codigo = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var dataCriacao = DateTime.UtcNow;

        validacaoEmailRepository.Adicionar(new ValidacaoEmail
        {
            IdUsuario = idUsuario,
            TokenHash = GerarTokenHash(codigo),
            DataCriacao = dataCriacao,
            DataExpiracao = dataCriacao.AddHours(24)
        });

        var verificationLink = $"{_frontEndSettings.BaseUrl}/validacao-email?codigo={Uri.EscapeDataString(codigo)}";

        await awsService.SendEmailTemplate(email, "TechJobs_ValidacaoEmail", new
        {
            nome,
            verificationLink
        });
    }

    private static string GerarTokenHash(string codigo) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(codigo)));

    public bool? ObterValidacaoEmail(int idUsuario) => _usuarioRepository.ObterPorId(idUsuario)?.EmailValidado;
}
