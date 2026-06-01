using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalTrainerApi.Data;
using PersonalTrainerApi.Models;

namespace PersonalTrainerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExerciciosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExerciciosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("treino/{treinoId}")]
    public async Task<ActionResult<List<Exercicio>>> GetPorTreino(int treinoId)
    {
        var exercicios = await _context.Exercicios
            .Where(e => e.TreinoId == treinoId)
            .ToListAsync();

        return Ok(exercicios);
    }

    [HttpPost]
    public async Task<ActionResult<Exercicio>> Post(Exercicio exercicio)
    {
        _context.Exercicios.Add(exercicio);
        await _context.SaveChangesAsync();

        return Ok(exercicio);
    }
}