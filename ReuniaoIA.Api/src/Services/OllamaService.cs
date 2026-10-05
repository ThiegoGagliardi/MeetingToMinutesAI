using System.Net.Http.Json;
using System.Text.Json;

namespace ReuniaoIA.Api.Services;

public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OllamaService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GerarAtaAsync(
        string transcricao,
        CancellationToken cancellationToken = default)
    {
        var model = _configuration["Ollama:Model"];

        var prompt = $"""
        Você é um assistente especializado em elaborar atas de reuniões.

        Analise a transcrição abaixo e gere uma ata profissional em português.

        A ata deve conter:

        - título da reunião
        - resumo
        - participantes mencionados
        - assuntos discutidos
        - decisões tomadas
        - tarefas definidas
        - responsáveis pelas tarefas
        - prazos mencionados
        - pendências

        Não invente informações.
        Quando uma informação não estiver presente na transcrição,
        utilize null ou uma lista vazia.

        Transcrição:

        {transcricao}
        """;

        var request = new
        {
            model,
            prompt,
            stream = false
        };

        using var response = await _httpClient.PostAsJsonAsync(
            "/api/generate",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<OllamaResponse>(
                cancellationToken: cancellationToken);

        return result?.Response ?? string.Empty;
    }

    private sealed class OllamaResponse
    {
        public string Response { get; set; } = string.Empty;
    }
}