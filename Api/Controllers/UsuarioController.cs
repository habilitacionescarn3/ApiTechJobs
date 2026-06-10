using Api.Configuration;
using Api.Helper;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Model.Enum;
using Model.Request;
using Model.Response;
using Services;
using Services.Interfaces;
using Utils;

namespace Api.Controllers
{
    [Route("usuario")]
    [ApiController]
    [ProducesErrorResponseType(typeof(ErroResponse))]
    public class UsuarioController(IUsuarioService usuarioService, IEmpresaService empresaService) : ControllerBase
    {
        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        /// <param name="request">Dados para criar usuário</param>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost]
        public async Task<IActionResult> NovoUsuario([FromBody] NovoUsuarioRequest request)
        {
            try
            {
                await usuarioService.NovoUsuario(request);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.GerarRespostaErro());
            }
        }

        /// <summary>
        /// Realiza o login de um usuário
        /// </summary>
        /// <param name="request">Dados para login</param>
        /// <returns>O token JWT do usuário</returns>
        [HttpPost("token")]
        public IActionResult LogarUsuario([FromBody] LogarUsuarioRequest request)
        {
            try
            {
                var token = usuarioService.LogarUsuario(request);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.GerarRespostaErro());
            }
        }

        [AutorizarPerfis(EnumPerfil.Empresa, EnumPerfil.Candidato)]
        [HttpPut("foto-perfil")]
        public async Task<IActionResult> EditarFotoPerfil(IFormFile file)
        {
            try
            {
                await usuarioService.EditarFotoPerfil(User.ObterId(), file);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.GerarRespostaErro());
            }
        }

        [AutorizarPerfis(EnumPerfil.Empresa, EnumPerfil.Candidato)]
        [HttpDelete("foto-perfil")]
        public async Task<IActionResult> DeletarFotoPerfil()
        {
            try
            {
                await usuarioService.DeletarFotoPerfil(User.ObterId());

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.GerarRespostaErro());
            }
        }

        [AutorizarPerfis(EnumPerfil.Empresa, EnumPerfil.Candidato)]
        [HttpGet("foto-perfil")]
        public async Task<IActionResult> ObterFotoPerfil()
        {
            try
            {
                var url = await usuarioService.GerarUrlAssinadaFotoPerfil(User.ObterId());

                return Ok(url);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.GerarRespostaErro());
            }
        }


        [AutorizarPerfis(EnumPerfil.Candidato)]
        [HttpGet("foto-perfil/empresa/{idEmpresa}")]
        public async Task<IActionResult> ObterFotoPerfilEmpresa(int idEmpresa)
        {
            try
            {
                var url = await empresaService.GerarUrlAssinadaFotoPerfil(idEmpresa);

                return Ok(url);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.GerarRespostaErro());
            }
        }

        /// <summary>
        /// Gera e envia um novo link de validação para o e-mail do usuário autenticado
        /// </summary>
        [AutorizarPerfis(EnumPerfil.Empresa, EnumPerfil.Candidato)]
        [HttpPost("email/validacao")]
        public async Task<IActionResult> GerarValidacaoEmail()
        {
            try
            {
                await usuarioService.GerarValidacaoEmail(User.ObterId());

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.GerarRespostaErro());
            }
        }

        /// <summary>
        /// Valida o e-mail do usuário a partir do código enviado por e-mail
        /// </summary>
        [HttpGet("email/validar")]
        public IActionResult ValidarEmail([FromQuery] string codigo)
        {
            try
            {
                usuarioService.ValidarEmail(codigo);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GerarRespostaErro());
            }
        }

        [AutorizarPerfis(EnumPerfil.Empresa, EnumPerfil.Candidato)]
        [HttpGet("email/validado")]
        public IActionResult ObterValidacaoEmail([FromQuery(Name = "idBusca")] int? idUsuario)
        {
            var emailValidado = usuarioService.ObterValidacaoEmail(idUsuario ?? User.ObterId());

            return Ok(emailValidado);
        }

        [AutorizarPerfis(EnumPerfil.Empresa)]
        [HttpGet("foto-perfil/candidato/{idCandidato}")]
        public async Task<IActionResult> ObterFotoPerfilCandidato(int idCandidato)
        {
            try
            {
                var url = await usuarioService.GerarUrlAssinadaFotoPerfilCandidato(idCandidato);

                return Ok(url);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.GerarRespostaErro());
            }
        }

        [AutorizarPerfis(EnumPerfil.Empresa, EnumPerfil.Candidato)]
        [HttpGet("notificacao")]
        public IActionResult ObterNotificacoes()
        {
            try
            {
                var notificacoes = usuarioService.ObterNotificacoes(User.ObterId());

                return Ok(notificacoes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.GerarRespostaErro());
            }
        }

        [AutorizarPerfis(EnumPerfil.Empresa, EnumPerfil.Candidato)]
        [HttpGet("notificacao/nao-lidas")]
        public IActionResult ObterQuantidadeNotificacoesNaoLidas()
        {
            try
            {
                var quantidade = usuarioService.ObterQuantidadeNotificacoesNaoLidas(User.ObterId());

                return Ok(quantidade);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.GerarRespostaErro());
            }
        }

        [AutorizarPerfis(EnumPerfil.Empresa, EnumPerfil.Candidato)]
        [HttpPut("notificacao/{id}/lida")]
        public IActionResult MarcarNotificacaoComoLida([FromRoute] int id)
        {
            try
            {
                usuarioService.MarcarNotificacaoComoLida(id, User.ObterId());

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.GerarRespostaErro());
            }
        }

        [AutorizarPerfis(EnumPerfil.Empresa, EnumPerfil.Candidato)]
        [HttpPut("notificacao/lidas")]
        public IActionResult MarcarTodasNotificacoesComoLidas()
        {
            try
            {
                usuarioService.MarcarTodasNotificacoesComoLidas(User.ObterId());

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.GerarRespostaErro());
            }
        }
    }
}
