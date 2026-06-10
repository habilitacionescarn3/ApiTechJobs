using Repositories;
using Repositories.Generico;
using Repositories.Generico.Interface;
using Services;
using Services.Interfaces;
using Services.Utils;
using Services.Utils.Interface;

namespace Api.Configuration;

public static class DependencyInjection
{
    /// <summary>
    /// Injeção de dependências dos serviços e repositories
    /// </summary>
    /// <param name="servicos"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigurarServicos(this IServiceCollection servicos)
    {
        // Action filters
        #region Action filters
        servicos.AddScoped<VagaActionFilter>();
        #endregion

        // Repositories
        #region Repositories
        servicos.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        servicos.AddSingleton<UsuarioRepository>();
        servicos.AddSingleton<CandidatoRepository>();
        servicos.AddSingleton<CandidatoVagaRepository>();
        servicos.AddSingleton<EmpresaRepository>();
        servicos.AddSingleton<VagaRepository>();
        servicos.AddSingleton<ExperienciaCandidatoRepository>();
        servicos.AddSingleton<InformacaoCandidatoRepository>();
        servicos.AddSingleton<InformacaoEmpresaRepository>();
        servicos.AddSingleton<NotificacaoUsuarioRepository>();
        servicos.AddSingleton<ValidacaoEmailRepository>();
        servicos.AddSingleton<RecuperacaoSenhaRepository>();
        #endregion

        // Services
        #region Services
        servicos.AddScoped<IUsuarioService, UsuarioService>();
        servicos.AddScoped<ICandidatoService, CandidatoService>();
        servicos.AddScoped<IEmpresaService, EmpresaService>();
        servicos.AddScoped<IVagaService, VagaService>();
        servicos.AddScoped<IAwsService, AwsService>();
        #endregion

        return servicos;
    }
}
