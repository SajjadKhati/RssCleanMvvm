using Application.InfrastructureContracts.HttpProject.HttpRequest.Interfaces;

namespace Infra.HttpService.HttpRequestService.Class
{
    public class HttpRequest : IHttpRequest
    {

        private HttpClient httpClient;



        public HttpRequest(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }


        /////////////////////////////


        public async Task<string> GetStringAsync(Uri requestUri)
        {
            string response;
            try
            {
                response = await this.httpClient.GetStringAsync(requestUri);
            }
            catch (InvalidOperationException exception)
            {
                throw new InvalidOperationException("مقدار url داده شده ، باید مقدار مطلق باشد .", exception);
            }
            catch (HttpRequestException exception)
            {
                throw new HttpRequestException("درخواست بخاطر یک مشکل اساسی مانند ارتباط شبکه ، شکست خوردن DNS ، اعتبارسنجی گواهینامه ی خدمات دهنده (سرور) ، شکست خورد .", exception);
            }
            catch (TaskCanceledException exception)
            {
                throw new TaskCanceledException("زمان اجرای درخواست ، تمام شد (time out) .", exception);
            }

            return response;
        }


        public async Task<byte[]> GetByteArrayAsync(Uri requestUri)
        {
            byte[] response;

            try
            {
                response = await this.httpClient.GetByteArrayAsync(requestUri);
            }
            catch (InvalidOperationException exception)
            {
                throw new InvalidOperationException("مقدار url داده شده ، باید مقدار مطلق باشد .", exception);
            }
            catch (HttpRequestException exception)
            {
                throw new HttpRequestException("درخواست بخاطر یک مشکل اساسی مانند ارتباط شبکه ، شکست خوردن DNS ، اعتبارسنجی گواهینامه ی خدمات دهنده (سرور) ، شکست خورد .", exception);
            }
            catch (TaskCanceledException exception)
            {
                throw new TaskCanceledException("زمان اجرای درخواست ، تمام شد (time out) .", exception);
            }

            return response;
        }

        /////////////////////////////


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                httpClient.Dispose();
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
