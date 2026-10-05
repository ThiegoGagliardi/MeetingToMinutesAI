namespace ReuniaoIA.Api.Services;

public interface IWhisperService
{
    Task<string> TranscreverAsync(
        Stream audioStream,
        CancellationToken cancellationToken = default);
}