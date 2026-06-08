using Api.Configuration;
using Api.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enum;
using Model.Request;
using Services.Interfaces;

namespace Api.Controllers
{
    [Route("vaga")]
    [ApiController]
    public class VagaController(IVagaService vagaService, IEmpresaService empresaService) : ControllerBase
    {
        private readonly IVagaService _vagaService = vagaService;
        private readonly IEmpresaService _empresaService = empresaService;

        /// <summary>
        /// Adciona uma vaga
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Id da vaga cadastrada</returns>
        [AutorizarPerfis(EnumPerfil.Empresa)]
        [HttpPost]
        public IActionResult AdicionarVaga([FromBody] AdicionarVagaRequest request)
        {
            int id = _empresaService.AdicionarVaga(User.ObterId(), request);

            return Ok(id);
        }

        /// <summary>
        /// Obtém as vagas de uma empresa
        /// </summary>
        /// <returns>Lista de vagas</returns>
        [AutorizarPerfis(EnumPerfil.Empresa)]
        [HttpGet("empresa")]
        public IActionResult ObterVagas()
        {
            var vagas = _empresaService.ObterVagas(User.ObterId());

            return Ok(vagas);
        }
        /// <summary>
        /// Obtém as vagas de uma empresa
        /// </summary>
        /// <returns>Lista de vagas</returns>
        [AutorizarPerfis(EnumPerfil.Empresa)]
        [HttpGet("url-cv-aplicacao/{id}")]
        public async Task<IActionResult> GerarUrlAssinada(int id)
        {
            var url = await _vagaService.GerarUrlAssinada(id);

            return Ok(url);
        }

        /// <summary>
        /// Busca de vagas parametrizadas
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("todas")]
        public IActionResult ObterVagasDisponiveis([FromQuery] ObterTodasVagasRequest request)
        {
            var vagas = _vagaService.ObterTodas(request);

            return Ok(vagas);
        }

        /// <summary>
        /// Obtém uma vaga por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Dados da vaga</returns>
        [HttpGet("{id}")]
        public IActionResult ObterVaga([FromRoute] int id)
        {
            var vaga = _vagaService.ObterVaga(id);

            return Ok(vaga);
        }

        [VagaActionFilter]
        [AutorizarPerfis(EnumPerfil.Empresa)]
        [HttpGet("empresa/{id}")]
        public IActionResult ObterVagaEmpresa([FromRoute] int id)
        {
            var vaga = _vagaService.ObterVagaEmpresaPorId(id);

            return Ok(vaga);
        }

        /// <summary>
        /// Edita uma vaga
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vaga"></param>
        /// <returns></returns>
        [VagaActionFilter]
        [HttpPut("{id}")]
        [AutorizarPerfis(EnumPerfil.Empresa)]
        public IActionResult EditarVaga([FromRoute] int id, [FromBody] Vaga vaga)
        {
            _vagaService.Editar(id, vaga);

            return NoContent();
        }

        /// <summary>
        /// Exclui uma vaga
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [VagaActionFilter]
        [HttpDelete("{id}")]
        [AutorizarPerfis(EnumPerfil.Empresa)]
        public IActionResult ExcluirVaga([FromRoute] int id)
        {
            _vagaService.Excluir(id);
            
            return NoContent();
        }
    }
}
