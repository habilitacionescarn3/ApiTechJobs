using Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model;

public class Vaga
{

    [IgnorarInsert]
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string NivelExperiencia { get; set; } = string.Empty;
    public string? Cep { get; set; }
    public string? Numero { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal? SalarioPrevisto { get; set; }
    public bool Interna { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataFimInscricoes { get; set; }
    public string? Tecnologias { get; set; }
    public string? Requisitos { get; set; }
    public string? Beneficios { get; set; }

    public void AtualizarModel(Vaga vaga)
    {
        Nome = string.IsNullOrWhiteSpace(vaga.Nome) ? Nome : vaga.Nome;
        Cargo = string.IsNullOrWhiteSpace(vaga.Cargo) ? Cargo : vaga.Cargo;
        Modelo = string.IsNullOrWhiteSpace(vaga.Modelo) ? Modelo : vaga.Modelo;
        NivelExperiencia = string.IsNullOrWhiteSpace(vaga.NivelExperiencia) ? NivelExperiencia : vaga.NivelExperiencia;
        Cep = vaga.Cep ?? Cep;
        Numero = vaga.Numero ?? Numero;
        Descricao = string.IsNullOrWhiteSpace(vaga.Descricao) ? Descricao : vaga.Descricao;
        SalarioPrevisto = vaga.SalarioPrevisto ?? SalarioPrevisto;
        Interna = vaga.Interna;
        DataFimInscricoes = vaga.DataFimInscricoes == default ? DataFimInscricoes : vaga.DataFimInscricoes;
        Tecnologias = vaga.Tecnologias ?? Tecnologias;
        Requisitos = vaga.Requisitos ?? Requisitos;
        Beneficios = vaga.Beneficios ?? Beneficios;
    }
}
