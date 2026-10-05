using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Whisper.net;
using Whisper.net.Ggml;

namespace ReuniaoIA.Api.Services;

public class WhisperService : IWhisperService
{
    public async Task<string> TranscreverAsync(
        Stream audioStream,
        CancellationToken cancellationToken = default)
    {
        var modelPath = await ObterModeloAsync();

        using var whisperFactory =
            WhisperFactory.FromPath(modelPath);

        using var processor = whisperFactory
            .CreateBuilder()
            .WithLanguage("pt")
            .Build();

        // Copia o áudio recebido para memória
        using var inputStream = new MemoryStream();

        await audioStream.CopyToAsync(
            inputStream,
            cancellationToken);

        inputStream.Position = 0;

        // Converte o áudio para WAV 16 kHz mono
        using var wavStream = ConverterPara16KhzMono(inputStream);

        wavStream.Position = 0;

        var resultado = new List<string>();

        await foreach (var segment in processor.ProcessAsync(
            wavStream,
            cancellationToken))
        {
            resultado.Add(segment.Text);
        }

        return string.Join(" ", resultado);
    }

    private static MemoryStream ConverterPara16KhzMono(
        Stream audioStream)
    {
        using var reader = new WaveFileReader(audioStream);

        var monoProvider = reader
            .ToSampleProvider()
            .ToMono();

        var resampler = new WdlResamplingSampleProvider(
            monoProvider,
            16000);

        var outputStream = new MemoryStream();

        WaveFileWriter.WriteWavFileToStream(
            outputStream,
            resampler.ToWaveProvider16());

        outputStream.Position = 0;

        return outputStream;
    }

    private static async Task<string> ObterModeloAsync()
    {
        var modelsPath = Path.Combine(
            AppContext.BaseDirectory,
            "Models");

        Directory.CreateDirectory(modelsPath);

        var modelPath = Path.Combine(
            modelsPath,
            "ggml-large.bin");

        if (File.Exists(modelPath))
        {
            return modelPath;
        }

        Console.WriteLine(
            "Modelo Whisper não encontrado.");

        Console.WriteLine(
            "Baixando ggml-large.bin...");

        using var modelStream =
            await WhisperGgmlDownloader.Default
                .GetGgmlModelAsync(GgmlType.Base);

        await using var fileStream =
            File.Create(modelPath);

        await modelStream.CopyToAsync(fileStream);

        Console.WriteLine(
            "Modelo Whisper baixado.");

        return modelPath;
    }
}