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

        [HttpGet("dashboard")]
        public IActionResult ObterDadosDashboard()
        {
            var dados = _candidatoService.ObterDadosDashboard(User.ObterId());

            return Ok(dados);
        }
    }
}
