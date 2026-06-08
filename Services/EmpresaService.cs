using Model;
using Model.Enum;
using Model.Request;
using Model.Response;
using Repositories;
using Services.Interfaces;
using Services.Utils.Interface;
using Utils;

namespace Services;

public class EmpresaService(EmpresaRepository empresaRepository, VagaRepository vagaRepository, CandidatoVagaRepository candidatoVagaRepository, InformacaoEmpresaRepository informacaoEmpresaRepository, IAwsService awsService) : IEmpresaService
{
    public int Adicionar(Empresa empresa) => empresaRepository.Adicionar(empresa);

    public Empresa ObterEmpresaPorIdUsuario(int idUsuario) => empresaRepository.ObterEmpresaPorIdUsuario(idUsuario);

    public int AdicionarVaga(int idUsuario, AdicionarVagaRequest novaVaga)
    {
        var empresa = empresaRepository.ObterEmpresaPorIdUsuario(idUsuario);

        var vaga = new Vaga
        {
            Cargo = novaVaga.Cargo,
            Cep = novaVaga.Cep,
            DataCadastro = DateTime.Now,
            DataFimInscricoes = novaVaga.DataFimInscricoes ?? DateTime.Now,
            Descricao = novaVaga.Descricao,
            IdEmpresa = empresa.Id,
            Interna = novaVaga.Interna,
            Modelo = novaVaga.Modelo,
            NivelExperiencia = novaVaga.NivelExperiencia,
            Nome = novaVaga.Nome,
            Numero = novaVaga.Numero,
            SalarioPrevisto = novaVaga.SalarioPrevisto,
            Tecnologias = novaVaga.Tecnologias,
            Requisitos = novaVaga.Requisitos,
            Beneficios = novaVaga.Beneficios
        };

        return vagaRepository.Adicionar(vaga);
    }

    public IList<VagaResponse> ObterVagas(int idUsuario) => vagaRepository.ObterVagasPorIdUsuarioEmpresa(idUsuario);

    public void RetornarResultado(int idAplicacao, EnumSituacao situacao) => candidatoVagaRepository.Editar(new CandidatoVaga
    {
        Id = idAplicacao,
        Situacao = situacao,
        DataAtualizacao = HorarioBrasilia.DataAtual
    });

    public InformacoesEmpresaResponse ObterInformacoesPorUsuario(int idUsuario)
    {
        var informacoes = informacaoEmpresaRepository.ObterInformacoesPorUsuario(idUsuario);
        var vagasDisponiveis = vagaRepository.ObterVagasDisponiveis(idUsuario);
        var dadosVagasEmpresa = candidatoVagaRepository.ObterDadosDashboardEmpresa(idUsuario);

        return new(informacoes, vagasDisponiveis, dadosVagasEmpresa.Candidatos);
    }

    public void AtualizarInformacoesEmpresa(int idUsuario, AtualizarInformacoesEmpresaRequest request)
    {
        var empresa = empresaRepository.ObterEmpresaPorIdUsuario(idUsuario);

        var informacao = informacaoEmpresaRepository.ObterInformacoesPorUsuario(idUsuario);

        if (informacao != null)
        {
            informacao.AtualizarModel(request);

            informacaoEmpresaRepository.Editar(informacao);
        }
        else
        {
            informacaoEmpresaRepository.Adicionar(new InformacaoEmpresa
            {
                IdEmpresa = empresa.Id,
                Descricao = request.Descricao,
                LinkSite = request.LinkSite,
                Setor = request.Setor,
                Tecnologias = request.Tecnologias,
            });
        }
    }

    public async Task<string?> GerarUrlAssinadaFotoPerfil(int idEmpresa)
    {
        var fileKey = empresaRepository.ObterChaveFotoPerfilEmpresa(idEmpresa);

        if (string.IsNullOrWhiteSpace(fileKey))
            return null;

        return await awsService.PreSignedURL(fileKey);
    }
}
