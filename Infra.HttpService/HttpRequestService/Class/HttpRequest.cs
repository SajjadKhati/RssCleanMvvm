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


        public async Task<string> GetStringAsync(Uri requestUri, CancellationToken cancellationToken = default)
        {
            string response;
            try
            {
                response = await this.httpClient.GetStringAsync(requestUri, cancellationToken);
            }
            catch (InvalidOperationException exception)
            {
                throw new InvalidOperationException("مقدار url داده شده ، باید مقدار مطلق باشد .", exception);
            }
            catch (HttpRequestException exception)
            {
                throw new HttpRequestException("درخواست بخاطر یک مشکل اساسی مانند ارتباط شبکه ، شکست خوردن DNS ، اعتبارسنجی گواهینامه ی خدمات دهنده (سرور) ، شکست خورد .", exception);
            }
            /// ⚠️ چرا این catch فیلتر when دارد؟
            ///
            /// TaskCanceledException، هم برای timeout و هم برای لغو کاربر پرتاب می‌شود.
            /// تفاوت این دو، فقط در مقدار IsCancellationRequested است:
            ///   - لغو کاربر → IsCancellationRequested == true
            ///   - timeout → IsCancellationRequested == false
            ///
            /// بدون when، این catch هر دو حالت را می‌گرفت و لغو کاربر به اشتباه
            /// به‌عنوان timeout به لایه‌ی بالا گزارش می‌شد (که پیام اشتباهی به کاربر می‌داد).
            ///
            /// با when، فقط حالت timeout اینجا گرفته می‌شود و لغو کاربر عمداً propagate
            /// می‌شود تا در لایه‌ی Application (UseCase) مدیریت شود.
            ///
            /// مسئولیت پیام‌دهی به کاربر، وظیفه‌ی Application است، نه Infra.
            catch (TaskCanceledException exception) when (cancellationToken.IsCancellationRequested == false)
            {
                throw new TaskCanceledException("زمان اجرای درخواست ، تمام شد (time out) .", exception);
            }

            return response;
        }


        public async Task<byte[]> GetByteArrayAsync(Uri requestUri, CancellationToken cancellationToken = default)
        {
            byte[] response;

            try
            {
                response = await this.httpClient.GetByteArrayAsync(requestUri, cancellationToken);
            }
            catch (InvalidOperationException exception)
            {
                throw new InvalidOperationException("مقدار url داده شده ، باید مقدار مطلق باشد .", exception);
            }
            catch (HttpRequestException exception)
            {
                throw new HttpRequestException("درخواست بخاطر یک مشکل اساسی مانند ارتباط شبکه ، شکست خوردن DNS ، اعتبارسنجی گواهینامه ی خدمات دهنده (سرور) ، شکست خورد .", exception);
            }
            /// ⚠️ چرا این catch فیلتر when دارد؟
            ///
            /// TaskCanceledException، هم برای timeout و هم برای لغو کاربر پرتاب می‌شود.
            /// تفاوت این دو، فقط در مقدار IsCancellationRequested است:
            ///   - لغو کاربر → IsCancellationRequested == true
            ///   - timeout → IsCancellationRequested == false
            ///
            /// بدون when، این catch هر دو حالت را می‌گرفت و لغو کاربر به اشتباه
            /// به‌عنوان timeout به لایه‌ی بالا گزارش می‌شد (که پیام اشتباهی به کاربر می‌داد).
            ///
            /// با when، فقط حالت timeout اینجا گرفته می‌شود و لغو کاربر عمداً propagate
            /// می‌شود تا در لایه‌ی Application (UseCase) مدیریت شود.
            ///
            /// مسئولیت پیام‌دهی به کاربر، وظیفه‌ی Application است، نه Infra.
            catch (TaskCanceledException exception) when (cancellationToken.IsCancellationRequested == false)
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
