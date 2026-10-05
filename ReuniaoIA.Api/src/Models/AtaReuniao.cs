namespace ReuniaoIA.Api.Models;

public class AtaReuniao
{
    public string Titulo { get; set; } = string.Empty;

    public string Resumo { get; set; } = string.Empty;

    public List<string> Participantes { get; set; } = [];

    public List<string> AssuntosDiscutidos { get; set; } = [];

    public List<string> Decisoes { get; set; } = [];

    public List<Tarefa> Tarefas { get; set; } = [];

    public List<string> Pendencias { get; set; } = [];
}

public class Tarefa
{
    public string Descricao { get; set; } = string.Empty;

    public string? Responsavel { get; set; }

    public string? Prazo { get; set; }
}