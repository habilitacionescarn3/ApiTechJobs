using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Response
{
    public class InformacoesEmpresaResponse(Empresa empresa, InformacaoEmpresa? informacaoEmpresa, IList<VagaCandidatoResponse> vagas, int candidatos, bool? emailValidado = false)
    {
        public int Id { get; set; } = empresa.Id;
        public string Nome { get; set; } = empresa.Nome;
        public string? Setor { get; set; } = informacaoEmpresa?.Setor;
        public string? Tecnologias { get; set; } = informacaoEmpresa?.Tecnologias;
        public string? Descricao { get; set; } = informacaoEmpresa?.Descricao;
        public string? LinkSite { get; set; } = informacaoEmpresa?.LinkSite;
        public int VagasDisponiveis { get; set; } = vagas.Count;
        public int Candidatos { get; set; } = candidatos;
        public IList<VagaCandidatoResponse> Vagas { get; set; } = vagas;
        public bool EmailValidado { get; set; } = emailValidado ?? false;
    }
}
