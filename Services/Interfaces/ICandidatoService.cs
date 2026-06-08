using Model;
using Model.DTO;
using Model.Request;
using Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces;

public interface ICandidatoService
{
    void Adicionar(Candidato candidato);
    Task AplicarVaga(AplicarVagaRequest aplicarVaga);
    IList<AplicacaoCandidatoResponse> ObterAplicacoes(int idUsuario);
    AplicacaoCandidatoResponse? ObterVaga(int id, int idUsuario);
    InformacoesCandidatoResponse ObterInformacoesPorUsuario(int idUsuario);
    void AtualizarInformacoesCandidato(int idUsuario, AtualizarInformacoesCandidatoRequest request);
}
