using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactApi.Data;
using ContactApi.Models;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("throw")]
    public IActionResult Throw() => throw new Exception("Erro proposital para teste");
}
