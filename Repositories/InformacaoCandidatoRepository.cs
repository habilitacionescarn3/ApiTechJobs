using Dapper;
using Microsoft.Extensions.Configuration;
using Model;
using Model.DTO;
using Repositories.Generico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class InformacaoCandidatoRepository(IConfiguration configuration) : GenericRepository<InformacaoCandidato>(configuration)
    {
        public InformacaoCandidatoNomeDTO? ObterInformacoesPorIdCandidato(int idCandidato)
        {
            using var conexao = CriarConexao();

            const string sqlCommand = @"SELECT C.Nome, IC.*, U.EmailValidado FROM Candidato AS C
                                        LEFT JOIN InformacaoCandidato AS IC
                                        ON IC.IdCandidato = C.Id
                                        LEFT JOIN Usuario AS U
                                        ON U.Id = C.IdUsuario
                                        WHERE C.Id = @idCandidato";

            return conexao.QuerySingleOrDefault<InformacaoCandidatoNomeDTO>(sqlCommand, new { idCandidato });
        }

        public InformacaoCandidato? ObterInformacoesPorUsuario(int idUsuario)
        {
            using var conexao = CriarConexao();

            const string sqlCommand = @"SELECT IC.*
                                        FROM InformacaoCandidato IC
                                        LEFT JOIN Candidato C
                                            ON C.Id = IC.IdCandidato
                                        WHERE C.IdUsuario = @idUsuario;";

            return conexao.QuerySingleOrDefault<InformacaoCandidato>(sqlCommand, new { idUsuario });
        }

        public override void Editar(InformacaoCandidato obj)
        {
            using var conexao = CriarConexao();

            const string sqlCommand = @"UPDATE InformacaoCandidato
                            SET Linkedin = @Linkedin,
                                Github = @Github,
                                Habilidades = @Habilidades,
                                Descricao = @Descricao,
                                EmailPessoal = @EmailPessoal,
                                EmailCorporativo = @EmailCorporativo,
                                Telefone = @Telefone,
                                Preferencias = @Preferencias,
                                Cidade = @Cidade,
                                Estado = @Estado,
                                AnosExperiencia = @AnosExperiencia,
                                Area = @Area
                            WHERE IdCandidato = @IdCandidato;";

            conexao.Execute(sqlCommand, obj);
        }
    }
}
