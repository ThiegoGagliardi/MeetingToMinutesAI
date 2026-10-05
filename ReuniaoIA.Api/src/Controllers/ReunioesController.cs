using Microsoft.AspNetCore.Mvc;
using ReuniaoIA.Api.Services;

namespace ReuniaoIA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReunioesController : ControllerBase
{
    private readonly IWhisperService _whisperService;
    private readonly IOllamaService _ollamaService;

    public ReunioesController(
        IWhisperService whisperService,
        IOllamaService ollamaService)
    {
        _whisperService = whisperService;
        _ollamaService = ollamaService;
    }

    [HttpPost("gerar-ata")]
    [RequestSizeLimit(500_000_000)]
    public async Task<IActionResult> GerarAta(
        IFormFile arquivo,
        CancellationToken cancellationToken)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest("Arquivo de áudio não informado.");

        await using var stream = arquivo.OpenReadStream();

        var transcricao = await _whisperService.TranscreverAsync(
            stream,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(transcricao))
            return BadRequest("Não foi possível obter a transcrição.");

        var ata = await _ollamaService.GerarAtaAsync(
            transcricao,
            cancellationToken);

        return Ok(new
        {
            Transcricao = transcricao,
            Ata = ata
        });
    }
}