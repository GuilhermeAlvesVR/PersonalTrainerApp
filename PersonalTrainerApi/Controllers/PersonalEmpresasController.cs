using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalTrainerApi.Data;
using PersonalTrainerApi.Models;

namespace PersonalTrainerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonalEmpresasController : ControllerBase
{
    private readonly AppDbContext _context;

    public PersonalEmpresasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<PersonalEmpresa>>> Get()
    {
        var empresas = await _context.PersonalEmpresas.ToListAsync();

        return Ok(empresas);
    }

    [HttpPost]
    public async Task<ActionResult<PersonalEmpresa>> Post(PersonalEmpresa empresa)
    {
        _context.PersonalEmpresas.Add(empresa);

        await _context.SaveChangesAsync();

        return Ok(empresa);
    }
}