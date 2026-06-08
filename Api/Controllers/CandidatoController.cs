using Api.Configuration;
using Api.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Enum;
using Model.Request;
using Model.Response;
using Services.Interfaces;

namespace Api.Controllers
{
    [Route("candidato")]
    [ApiController]
    [ProducesErrorResponseType(typeof(ErroResponse))]
    [AutorizarPerfis(EnumPerfil.Candidato)]
    public class CandidatoController(ICandidatoService candidatoService) : ControllerBase
    {
        private readonly ICandidatoService _candidatoService = candidatoService;

        [HttpPost("vaga/{idVaga}")]
        public async Task<IActionResult> AplicarVaga([FromRoute] int idVaga, IFormFile file)
        {
            await _candidatoService.AplicarVaga(new AplicarVagaRequest
            {
                IdVaga = idVaga,
                IFile = file,
                IdUsuario = User.ObterId()
            });

            return NoContent();
        }

        [HttpGet("aplicacoes")]
        public IActionResult ObterAplicacoes()
        {
            var aplicacoes = _candidatoService.ObterAplicacoes(User.ObterId());

            return Ok(aplicacoes);
        }

        [HttpGet("vaga/{id}")]
        public IActionResult ObterAplicacao(int id)
        {
            var aplicacoes = _candidatoService.ObterVaga(id, User.ObterId());

            return Ok(aplicacoes);
        }

        [HttpGet("informacoes")]
        public IActionResult ObterInformacoesPorUsuario()
        {
            var informacoes = _candidatoService.ObterInformacoesPorUsuario(User.ObterId());

            return Ok(informacoes);
        }

        [HttpPut("informacoes")]
        public IActionResult AtualizarInformacoes([FromBody] AtualizarInformacoesCandidatoRequest request)
        {
            _candidatoService.AtualizarInformacoesCandidato(User.ObterId(), request);

            return NoContent();
        }
    }
}
