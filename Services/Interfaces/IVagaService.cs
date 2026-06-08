using Model;
using Model.Request;
using Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces;

public interface IVagaService
{
    VagaCandidatoResponse? ObterVaga(int id);
    void Excluir(int id);
    void Editar(int id, Vaga vaga);
    IList<VagaCandidatoResponse> ObterTodas(ObterTodasVagasRequest request);
    bool ValidarVagaEmpresa(int idVaga, int idUsuarioEmpresa);
    VagaEmpresaResponse ObterVagaEmpresaPorId(int id);
    Task<string> GerarUrlAssinada(int id);
}
