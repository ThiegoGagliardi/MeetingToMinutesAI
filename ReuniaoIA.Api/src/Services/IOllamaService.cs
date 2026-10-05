namespace ReuniaoIA.Api.Services;

public interface IOllamaService
{
    Task<string> GerarAtaAsync(
        string transcricao,
        CancellationToken cancellationToken = default);
}