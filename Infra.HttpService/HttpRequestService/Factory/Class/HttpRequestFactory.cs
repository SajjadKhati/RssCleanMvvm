using Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Interfaces;
using Infra.HttpService.HttpRequestService.Class;

namespace Infra.HttpService.HttpRequestService.Factory.Class
{
    public class HttpRequestFactory : IHttpRequestFactory
    {

        public IHttpRequest Create()
        {
            return new HttpRequest(new HttpClient());
        }

    }
}
