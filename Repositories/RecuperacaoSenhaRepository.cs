using Dapper;
using Microsoft.Extensions.Configuration;
using Model;
using Repositories.Generico;

namespace Repositories;

public class RecuperacaoSenhaRepository(IConfiguration configuration) : GenericRepository<RecuperacaoSenha>(configuration)
{
    public override int Adicionar(RecuperacaoSenha recuperacaoSenha)
    {
        using var conexao = CriarConexao();
        conexao.Open();

        using var transacao = conexao.BeginTransaction();

        const string invalidarTokensAnteriores = @"UPDATE RecuperacaoSenha
                                                   SET DataUtilizacao = @dataUtilizacao
                                                   WHERE IdUsuario = @idUsuario
                                                       AND DataUtilizacao IS NULL";

        conexao.Execute(invalidarTokensAnteriores, new
        {
            recuperacaoSenha.IdUsuario,
            dataUtilizacao = recuperacaoSenha.DataCriacao
        }, transacao);

        const string adicionarToken = @"INSERT INTO RecuperacaoSenha
                                        (IdUsuario, TokenHash, DataExpiracao, DataCriacao, DataUtilizacao)
                                        OUTPUT INSERTED.Id
                                        VALUES
                                        (@IdUsuario, @TokenHash, @DataExpiracao, @DataCriacao, @DataUtilizacao)";

        var id = conexao.ExecuteScalar<int>(adicionarToken, recuperacaoSenha, transacao);

        transacao.Commit();
        return id;
    }

    public RecuperacaoSenha? ObterPorTokenHash(string tokenHash)
    {
        using var conexao = CriarConexao();

        const string sqlCommand = @"SELECT TOP 1 *
                                    FROM RecuperacaoSenha
                                    WHERE TokenHash = @tokenHash
                                    ORDER BY DataCriacao DESC";

        return conexao.QuerySingleOrDefault<RecuperacaoSenha>(sqlCommand, new { tokenHash });
    }

    public bool RedefinirSenha(string tokenHash, int idUsuario, string senha, DateTime dataUtilizacao)
    {
        using var conexao = CriarConexao();
        conexao.Open();

        using var transacao = conexao.BeginTransaction();

        const string consumirToken = @"UPDATE RecuperacaoSenha
                                       SET DataUtilizacao = @dataUtilizacao
                                       WHERE TokenHash = @tokenHash
                                           AND IdUsuario = @idUsuario
                                           AND DataUtilizacao IS NULL
                                           AND DataExpiracao >= @dataUtilizacao";

        var tokenConsumido = conexao.Execute(consumirToken, new
        {
            tokenHash,
            idUsuario,
            dataUtilizacao
        }, transacao) > 0;

        if (!tokenConsumido)
        {
            transacao.Rollback();
            return false;
        }

        const string atualizarSenha = @"UPDATE Usuario
                                        SET Senha = @senha
                                        WHERE Id = @idUsuario";

        var senhaAtualizada = conexao.Execute(atualizarSenha, new
        {
            senha,
            idUsuario
        }, transacao) > 0;

        if (!senhaAtualizada)
        {
            transacao.Rollback();
            return false;
        }

        transacao.Commit();
        return true;
    }
}
