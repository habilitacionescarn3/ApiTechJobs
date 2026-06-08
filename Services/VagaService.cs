using Amazon.S3;
using Amazon.S3.Model;
using Model;
using Model.Request;
using Model.Response;
using Repositories;
using Services.Interfaces;
using Services.Utils.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services;

public class VagaService(VagaRepository vagaRepository, CandidatoVagaRepository candidatoVagaRepository, IAwsService awsService) : IVagaService
{
    public VagaCandidatoResponse? ObterVaga(int id) => vagaRepository.ObterPorId(id);

    public void Excluir(int id) => vagaRepository.Excluir(id);

    public IList<VagaCandidatoResponse> ObterTodas(ObterTodasVagasRequest request) => vagaRepository.ObterTodos(request);

    public bool ValidarVagaEmpresa(int idVaga, int idUsuarioEmpresa) => vagaRepository.ObterVagaPorIdUsuarioEmpresa(idVaga, idUsuarioEmpresa) != null;

    public VagaEmpresaResponse ObterVagaEmpresaPorId(int id)
    {
        var vaga = vagaRepository.ObterPorId(id);
        var aplicacoes = candidatoVagaRepository.ObterAplicacoes(id);

        return new VagaEmpresaResponse
        {
            Aplicacoes = aplicacoes ?? [],
            Vaga = vaga
        };
    }

    public async Task<string> GerarUrlAssinada(int id)
    {
        var aplicacao = candidatoVagaRepository.ObterPorId(id);

        return await awsService.PreSignedURL(aplicacao.FileKey);
    }
}
