using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalTrainerApi.Data;
using PersonalTrainerApi.Models;

namespace PersonalTrainerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgendaHorariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public AgendaHorariosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<AgendaHorario>>> Get()
    {
        var horarios = await _context.AgendaHorarios
            .OrderBy(h => h.Id)
            .ToListAsync();

        return Ok(horarios);
    }

    [HttpPut]
    public async Task<ActionResult> Put(List<AgendaHorario> horarios)
    {
        _context.AgendaHorarios.RemoveRange(_context.AgendaHorarios);

        foreach (var horario in horarios)
        {
            horario.Id = 0;
            _context.AgendaHorarios.Add(horario);
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
