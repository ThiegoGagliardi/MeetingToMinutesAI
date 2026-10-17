using System.Diagnostics;

namespace ReuniaoIA.Api.Services;

public class AudioConverterService
{
    public async Task<MemoryStream> ConverterParaWavAsync(
        Stream audioStream,
        CancellationToken cancellationToken = default)
    {
        var diretorioTemporario = Path.GetTempPath();

        var arquivoEntrada = Path.Combine(
            diretorioTemporario,
            $"{Guid.NewGuid()}.m4a");

        try
        {
            // Salva o áudio recebido em um arquivo temporário.
            await using (var arquivo = File.Create(arquivoEntrada))
            {
                await audioStream.CopyToAsync(
                    arquivo,
                    cancellationToken);
            }

            var inicioProcesso = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            inicioProcesso.ArgumentList.Add("-y");
            inicioProcesso.ArgumentList.Add("-i");
            inicioProcesso.ArgumentList.Add(arquivoEntrada);
            inicioProcesso.ArgumentList.Add("-vn");
            inicioProcesso.ArgumentList.Add("-ac");
            inicioProcesso.ArgumentList.Add("1");
            inicioProcesso.ArgumentList.Add("-ar");
            inicioProcesso.ArgumentList.Add("16000");
            inicioProcesso.ArgumentList.Add("-c:a");
            inicioProcesso.ArgumentList.Add("pcm_s16le");
            inicioProcesso.ArgumentList.Add("-f");
            inicioProcesso.ArgumentList.Add("wav");
            inicioProcesso.ArgumentList.Add("pipe:1");

            using var processo = new Process
            {
                StartInfo = inicioProcesso
            };

            processo.Start();

            // Lê o WAV produzido pelo FFmpeg diretamente para a memória.
            var streamWav = new MemoryStream();

            var tarefaCopia = processo.StandardOutput.BaseStream.CopyToAsync(
                streamWav,
                cancellationToken);

            var erro = await processo.StandardError.ReadToEndAsync(
                cancellationToken);

            await tarefaCopia;

            await processo.WaitForExitAsync(cancellationToken);

            if (processo.ExitCode != 0)
            {
                streamWav.Dispose();

                throw new InvalidOperationException(
                    $"Erro ao converter áudio: {erro}");
            }

            streamWav.Position = 0;

            return streamWav;
        }
        finally
        {
            if (File.Exists(arquivoEntrada))
            {
                File.Delete(arquivoEntrada);
            }
        }
    }
}