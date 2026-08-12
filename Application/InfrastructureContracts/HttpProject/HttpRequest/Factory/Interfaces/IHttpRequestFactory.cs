using Application.InfrastructureContracts.HttpProject.HttpRequest.Interfaces;

namespace Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces
{
    public interface IHttpRequestFactory
    {
        IHttpRequest Create();
    }
}
