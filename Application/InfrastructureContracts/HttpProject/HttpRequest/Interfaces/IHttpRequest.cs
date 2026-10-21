namespace Application.InfrastructureContracts.HttpProject.HttpRequest.Interfaces
{
    public interface IHttpRequest : IDisposable
    {

        Task<string> GetStringAsync(Uri requestUri, CancellationToken cancellationToken = default);


        Task<byte[]> GetByteArrayAsync(Uri requestUri, CancellationToken cancellationToken = default);

    }
}
