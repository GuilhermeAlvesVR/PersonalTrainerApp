using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalTrainerApi.Data;
using PersonalTrainerApi.Models;

namespace PersonalTrainerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvaliacoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public AvaliacoesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("aluno/{alunoId}")]
    public async Task<ActionResult<List<AvaliacaoFisica>>> GetPorAluno(int alunoId)
    {
        var avaliacoes = await _context.AvaliacoesFisicas
            .Where(a => a.AlunoId == alunoId)
            .OrderByDescending(a => a.Data)
            .ToListAsync();

        return Ok(avaliacoes);
    }

    [HttpPost]
    public async Task<ActionResult<AvaliacaoFisica>> Post(AvaliacaoFisica avaliacao)
    {
        avaliacao.Id = 0;
        avaliacao.Data = DateTime.UtcNow;

        _context.AvaliacoesFisicas.Add(avaliacao);
        await _context.SaveChangesAsync();

        return Ok(avaliacao);
    }
}   