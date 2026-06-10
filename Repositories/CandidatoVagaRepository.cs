using Azure.Core;
using Dapper;
using Microsoft.Extensions.Configuration;
using Model;
using Model.DTO;
using Model.Response;
using Repositories.Generico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories;

public class CandidatoVagaRepository(IConfiguration configuration) : GenericRepository<CandidatoVaga>(configuration)
{
    public IList<CandidatoVagaDTO> ObterAplicacoes(int idVaga)
    {
        using var conexao = CriarConexao();

        const string sqlCommand = @"SELECT CV.Id, CV.DataCadastro, CV.Situacao, C.Nome, C.Email FROM CandidatoVaga AS CV
                                    LEFT JOIN Candidato AS C
                                    ON C.Id = CV.IdCandidato
                                    WHERE IdVaga = @idVaga";


        return [.. conexao.Query<CandidatoVagaDTO>(sqlCommand, new { idVaga })];
    }

    public override void Editar(CandidatoVaga obj)
    {
        using var conexao = CriarConexao();

        const string sqlCommand = "UPDATE CandidatoVaga SET Situacao = @Situacao, DataAtualizacao = @DataAtualizacao WHERE Id = @Id";

        conexao.Execute(sqlCommand, obj);
    }

    public IList<AplicacaoCandidatoResponse> ObterAplicacoesPorCandidato(int idUsuario)
    {
        using var conexao = CriarConexao();

        const string sqlCommand = @"SELECT V.*, E.Nome AS NomeEmpresa, CV.Situacao, CV.DataAtualizacao AS DataAtualizacaoAplicacao FROM Candidato AS C
                                    LEFT JOIN CandidatoVaga AS CV
                                    ON CV.IdCandidato = C.Id
                                    LEFT JOIN Vaga AS V
                                    ON CV.IdVaga = V.Id
                                    LEFT JOIN Empresa AS E
                                    ON E.Id = V.IdEmpresa
                                    WHERE C.IdUsuario = @idUsuario";

        return [.. conexao.Query<AplicacaoCandidatoResponse>(sqlCommand, new { idUsuario })];
    }

    public AplicacaoCandidatoResponse? ObterVaga(int idVaga, int idUsuario)
    {
        using var conexao = CriarConexao();

        const string sqlCommand = @"SELECT 
                                V.*, 
                                E.Nome AS NomeEmpresa,
                                CV.Situacao, CV.DataAtualizacao AS DataAtualizacaoAplicacao
                            FROM Vaga AS V
                            LEFT JOIN Empresa AS E
                                ON E.Id = V.IdEmpresa
                            LEFT JOIN Candidato AS C
                                ON C.IdUsuario = @idUsuario
                            LEFT JOIN CandidatoVaga AS CV
                                ON CV.IdVaga = V.Id
                                AND CV.IdCandidato = C.Id
                            WHERE V.Id = @idVaga";

        return conexao.QuerySingleOrDefault<AplicacaoCandidatoResponse>(sqlCommand, new { idVaga, idUsuario });
    }

    public DadosVagasCandidatoDTO ObterDadosDashboard(int idUsuario)
    {
        using var conexao = CriarConexao();

        const string sqlCommand = @"SELECT 
                                        COUNT(CV.Id) AS VagasAplicadas,
                                        COUNT(CASE WHEN Situacao = 1 THEN 1 END) AS ProcessosAtivos
                                    FROM Candidato AS C
                                    LEFT JOIN CandidatoVaga AS CV
                                    ON CV.IdCandidato = C.Id
                                    WHERE IdUsuario = @idUsuario";

        return conexao.QuerySingle<DadosVagasCandidatoDTO>(sqlCommand, new { idUsuario });
    }

    public DadosVagasEmpresaDTO ObterDadosDashboardEmpresa(int idUsuario)
    {
        using var conexao = CriarConexao();

        const string sqlCommand = @"SELECT 
                                        COUNT(*) AS Candidatos,
                                        COUNT(CASE WHEN Situacao = 2 THEN 1 END) AS Aprovacoes
                                    FROM CandidatoVaga AS CV
                                    LEFT JOIN Vaga AS V
                                    ON V.Id = CV.IdVaga
                                    LEFT JOIN Empresa AS E
                                    ON E.Id = V.IdEmpresa
                                    WHERE IdUsuario = @idUsuario";

        return conexao.QuerySingle<DadosVagasEmpresaDTO>(sqlCommand, new { idUsuario });
    }
}
