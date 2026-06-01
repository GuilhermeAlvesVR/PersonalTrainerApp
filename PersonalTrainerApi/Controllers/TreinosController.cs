using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalTrainerApi.Data;
using PersonalTrainerApi.Models;

namespace PersonalTrainerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TreinosController : ControllerBase
{
    private readonly AppDbContext _context;

    public TreinosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("aluno/{alunoId}")]
    public async Task<ActionResult<List<Treino>>> GetPorAluno(int alunoId)
    {
        var treinos = await _context.Treinos
            .Include(t => t.Exercicios)
            .Where(t => t.AlunoId == alunoId)
            .OrderBy(t => t.LetraTreino)
            .ToListAsync();

        return Ok(treinos);
    }

    [HttpPost]
    public async Task<ActionResult<Treino>> Post(Treino treino)
    {
        if (treino.Id > 0)
        {
            var treinoExistente = await _context.Treinos
                .Include(t => t.Exercicios)
                .FirstOrDefaultAsync(t => t.Id == treino.Id);

            if (treinoExistente == null)
                return NotFound();

            treinoExistente.LetraTreino = treino.LetraTreino;
            treinoExistente.NomeTreino = treino.NomeTreino;
            treinoExistente.AlunoId = treino.AlunoId;

            _context.Exercicios.RemoveRange(treinoExistente.Exercicios);

            treinoExistente.Exercicios = treino.Exercicios
                .Select(e => new Exercicio
                {
                    Nome = e.Nome,
                    Series = e.Series,
                    Repeticoes = e.Repeticoes,
                    Carga = e.Carga
                })
                .ToList();

            await _context.SaveChangesAsync();

            return Ok(treinoExistente);
        }

        _context.Treinos.Add(treino);
        await _context.SaveChangesAsync();

        return Ok(treino);
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var treino = await _context.Treinos
            .Include(t => t.Exercicios)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (treino == null)
            return NotFound();

        _context.Exercicios.RemoveRange(treino.Exercicios);
        _context.Treinos.Remove(treino);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
