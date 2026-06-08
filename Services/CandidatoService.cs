using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Model;
using Model.DTO;
using Model.Enum;
using Model.Request;
using Model.Response;
using Repositories;
using Services.Interfaces;
using Services.Utils.Interface;
using Utils;

namespace Services;

public class CandidatoService(CandidatoRepository candidatoRepository, InformacaoCandidatoRepository informacaoCandidatoRepository,
    ExperienciaCandidatoRepository experienciaCandidatoRepository, CandidatoVagaRepository candidatoVagaRepository, IAwsService awsService) : ICandidatoService
{
    private readonly string _folder = "cv";

    public void Adicionar(Candidato candidato) => candidatoRepository.Adicionar(candidato);


    public async Task AplicarVaga(AplicarVagaRequest aplicarVaga)
    {
        var candidato = candidatoRepository.ObterCandidatoPorIdUsuario(aplicarVaga.IdUsuario);

        string fileKey = await awsService.UploadFileAsync(aplicarVaga.IFile, _folder);

        candidatoVagaRepository.Adicionar(new CandidatoVaga
        {
            FileKey = fileKey,
            IdCandidato = candidato.Id,
            IdVaga = aplicarVaga.IdVaga,
            Situacao = EnumSituacao.EmAnalise,
            DataAtualizacao = HorarioBrasilia.DataAtual,
           DataCadastro = HorarioBrasilia.DataAtual 
        });
    }

    public IList<AplicacaoCandidatoResponse> ObterAplicacoes(int idUsuario) => candidatoVagaRepository.ObterAplicacoesPorCandidato(idUsuario);
    public AplicacaoCandidatoResponse? ObterVaga(int idVaga, int idUsuario) => candidatoVagaRepository.ObterVaga(idVaga, idUsuario);

    public InformacoesCandidatoResponse ObterInformacoesPorUsuario(int idUsuario)
    {
        var informacoes = informacaoCandidatoRepository.ObterInformacoesPorUsuario(idUsuario);
        var dadosVagas = candidatoVagaRepository.ObterDadosDashboard(idUsuario);
        var experiencias = experienciaCandidatoRepository.ObterExperienciasCandidato(idUsuario);

        return new(informacoes, dadosVagas, experiencias);
    }

    public void AtualizarInformacoesCandidato(int idUsuario, AtualizarInformacoesCandidatoRequest request)
    {
        var candidato = candidatoRepository.ObterCandidatoPorIdUsuario(idUsuario);

        var informacao = informacaoCandidatoRepository.ObterInformacoesPorUsuario(idUsuario);

        if (informacao != null)
        {
            informacao.AtualizarModel(request);

            informacaoCandidatoRepository.Editar(informacao);
        }
        else
        {
            informacaoCandidatoRepository.Adicionar(new InformacaoCandidato
            {
                IdCandidato = candidato.Id,
                Descricao = request.Descricao,
                Habilidades = request.Habilidades,
                EmailPessoal = request.EmailPessoal,
                EmailCorporativo = request.EmailCorporativo,
                Telefone = request.Telefone,
                Linkedin = request.Linkedin,
                Github = request.Github,
                Area = request.Area,
                AnosExperiencia = request.AnosExperiencia,
                Cidade = request.Cidade,
                Estado = request.Estado,
                Preferencias = request.Preferencias,
            });
        }


        if (request.Experiencias?.Count > 0)
        {
            experienciaCandidatoRepository.Excluir(idUsuario);

            foreach (var exp in request.Experiencias)
            {
                experienciaCandidatoRepository.Adicionar(new ExperienciaCandidato
                {
                    IdCandidato = candidato.Id,
                    TipoExperiencia = exp.TipoExperiencia,
                    Instituicao = exp.Instituicao,
                    Descricao = exp.Descricao,
                    DataInicio = exp.DataInicio,
                    DataFim = exp.DataFim
                });
            }
        }
    }
}
