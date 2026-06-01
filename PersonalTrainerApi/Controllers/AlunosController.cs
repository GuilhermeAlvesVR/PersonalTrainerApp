using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalTrainerApi.Data;
using PersonalTrainerApi.Models;

namespace PersonalTrainerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlunosController : ControllerBase
{
    private readonly AppDbContext _context;

    public AlunosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Aluno>>> Get()
    {
        var alunos = await _context.Alunos
            .OrderBy(a => a.Nome)
            .ToListAsync();

        return Ok(alunos);
    }

    [HttpPost]
    public async Task<ActionResult> Post(Aluno aluno)
    {
        _context.Alunos.Add(aluno);

        await _context.SaveChangesAsync();

        return Ok(aluno);
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var aluno = await _context.Alunos
            .Include(a => a.Treinos)
                .ThenInclude(t => t.Exercicios)
            .Include(a => a.Avaliacoes)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (aluno == null)
            return NotFound();

        _context.Alunos.Remove(aluno);

        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpPut("{id}/ultimo-treino")]
    public async Task<ActionResult> AtualizarUltimoTreino(int id, [FromBody] string ultimoTreino)
    {
        var aluno = await _context.Alunos.FindAsync(id);

        if (aluno == null)
            return NotFound();

        aluno.UltimoTreinoRealizado = ultimoTreino;

        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, Aluno alunoAtualizado)
    {
        var aluno = await _context.Alunos.FindAsync(id);

        if (aluno == null)
            return NotFound();

        aluno.Nome = alunoAtualizado.Nome;
        aluno.Idade = alunoAtualizado.Idade;
        aluno.Peso = alunoAtualizado.Peso;
        aluno.Altura = alunoAtualizado.Altura;
        aluno.Objetivo = alunoAtualizado.Objetivo;
        aluno.Observacoes = alunoAtualizado.Observacoes;
        aluno.Ativo = alunoAtualizado.Ativo;

        await _context.SaveChangesAsync();

        return Ok(aluno);
    }
}