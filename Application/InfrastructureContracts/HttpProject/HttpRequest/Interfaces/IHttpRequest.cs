namespace Application.InfrastructureContracts.HttpProject.HttpRequest.Interfaces
{
    public interface IHttpRequest : IDisposable
    {

        Task<string> GetStringAsync(Uri requestUri);


        Task<byte[]> GetByteArrayAsync(Uri requestUri);

    }
}
