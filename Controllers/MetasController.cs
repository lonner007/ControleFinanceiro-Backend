using ControleFinanceiro_Backend.Extensions;
using ControleFinanceiro_Backend.Models;
using ControleFinanceiro_Backend.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ControleFinanceiro_Backend.Controllers;
[Authorize][ApiController][Route("Api/Metas")]
public class MetasController : ControllerBase
{
    private readonly MetaRepositorio _repo;
    public MetasController(MetaRepositorio repo) { _repo = repo; }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lista = await _repo.ObterListaMetas(User.GetUserId());
        return Ok(new RespostaHttp<List<Meta>> { StatusCode = 200, Dados = lista, Mensagem = new List<Mensagem> { new() { titulo = "Sucesso", descricao = "Metas carregadas", severity = TipoMensagem.Success } } });
    }

    [HttpPost("CriarMeta")]
    public async Task<IActionResult> Criar([FromBody, Bind("nmMeta,dsMeta,vlAlvo,dtPrazo")] Meta meta)
    { var res = await _repo.CriarMeta(meta, User.GetUserId()); return StatusCode(res.StatusCode, res); }

    [HttpPut("{cdMeta}")]
    public async Task<IActionResult> Atualizar(int cdMeta, [FromBody, Bind("nmMeta,dsMeta,vlAlvo,vlAtual,dtPrazo,ativo")] Meta meta)
    { var res = await _repo.AtualizarMeta(cdMeta, meta, User.GetUserId()); return StatusCode(res.StatusCode, res); }

    [HttpDelete("deletar/{cdMeta}")]
    public async Task<IActionResult> Deletar(int cdMeta)
    { var res = await _repo.DeletarMeta(cdMeta, User.GetUserId()); return StatusCode(res.StatusCode, res); }
}
