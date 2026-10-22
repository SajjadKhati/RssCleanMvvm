using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Shared.DraftRssItemDtoFactoryOrch.Interface;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.DataModels;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.Interface;
using Application.UseCases.Shared.RssItemsDownLoader.Interface;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.Class
{
    public class RssItemDownloaderIntoDraftCacheOrchestrator : IRssItemDownloaderIntoDraftCacheOrchestrator
    {

        private IRssItemsDownLoaderOrchestrator rssItemsDownLoaderOrchestrator;

        private IDraftRssItemDtoFactoryOrchestrator draftRssItemDtoFactoryOrchestrator;

        private IRssItemDraftCacheWriter rssItemDraftCacheWriter;

        private IRssItemDraftCacheReader rssItemDraftCacheReader;


        //////////////////////////////////////////////////////////////////////////////


        public RssItemDownloaderIntoDraftCacheOrchestrator(IRssItemsDownLoaderOrchestrator rssItemsDownLoaderOrchestrator,
            IDraftRssItemDtoFactoryOrchestrator draftRssItemDtoFactoryOrchestrator, IRssItemDraftCacheWriter rssItemDraftCacheWriter,
            IRssItemDraftCacheReader rssItemDraftCacheReader)
        {
            this.rssItemsDownLoaderOrchestrator = rssItemsDownLoaderOrchestrator;
            this.draftRssItemDtoFactoryOrchestrator = draftRssItemDtoFactoryOrchestrator;
            this.rssItemDraftCacheWriter = rssItemDraftCacheWriter;
            this.rssItemDraftCacheReader = rssItemDraftCacheReader;
        }


        //////////////////////////////////////////////////////////////////////////////


        public async Task<RssItemWithDtosResult> DownloadAndStoreInDraftCacheAsync(RssFeedConfig rssFeedConfig, 
            CancellationToken cancellationToken = default)
        {
            List<RssItemWithDto> createdRssItemWithDtos = new List<RssItemWithDto>();
            List<Exception> creationErrors = new List<Exception>();
            bool wasUserCancelled = false;

            /// ⚠️ چرا دو try-catch تودرتو؟
            ///
            /// چون دو نوع خطای متفاوت داریم و هر کدام معنی متفاوتی دارند:
            ///
            ///   • try/catch داخلی (داخل حلقه):
            ///     خطاهای «per-item» — یعنی خطایی که فقط برای یک آیتم رخ داده.
            ///     اگه این catch نبود، خطای یک آیتم، کل حلقه رو متوقف می‌کرد
            ///     و بقیه‌ی آیتم‌ها هرگز پردازش نمی‌شدن.
            ///     هدف: resilience — یک آیتم خراب، بقیه رو خراب نکنه.
            ///
            ///   • try/catch بیرونی (دور کل حلقه):
            ///     خطاهای «batch-wide» — یعنی خطاهایی که کل عملیات رو تحت تأثیر قرار می‌دهند:
            ///       - AggregateException: خطاهای ساخت RssItem (از BuildRssItems)
            ///         که بعد از yield همه‌ی آیتم‌های موفق پرتاب می‌شه.
            ///       - OperationCanceledException: لغو کاربر.
            ///     این‌ها با catch داخلی گرفته نمی‌شن، چون از iterator میان، نه از بدنه‌ی حلقه.
            try
            {
                await foreach (RssItem rssItem in this.rssItemsDownLoaderOrchestrator.DownLoadRssItemsAsync(rssFeedConfig, cancellationToken))
                {
                    if (rssItem == null)
                        continue;
                    try
                    {
                        RssItemWithDto rssItemWithDto = this.CreateRssItemWithDtoAndStoreInDraftCache(rssItem);
                        if (rssItemWithDto == null)
                            continue;
                        createdRssItemWithDtos.Add(rssItemWithDto);
                    }
                    /// ⚠️ چرا catch (Exception) و نه چیز دیگه؟
                    ///
                    /// ساخت DTO ممکنه به هر دلیلی شکست بخوره:
                    ///   - خطای ذخیره‌ی فایل تصویر (دیسک پر، دسترسی نداریم)
                    ///   - خطای تبدیل تاریخ به شمسی
                    ///   - خطای ساخت مسیر فایل
                    /// اینا همه از نوع Exception هستن. برای resilience، همه رو می‌گیریم
                    /// و به creationErrors اضافه می‌کنیم، تا بقیه‌ی آیتم‌ها ادامه پیدا کنن.
                    ///
                    /// why when filter:
                    /// CreateDtoAndStoreInDraftCache الان sync هست و OCE پرتاب نمی‌کنه،
                    /// ولی اگه یه روز async بشه و CancellationToken بگیره، OCE ممکنه
                    /// پرتاب بشه. اون‌وقت باید propagate بشه (نه به‌عنوان خطای آیتم ثبت بشه)،
                    /// تا لایه‌ی بالاتر بفهمه «کاربر لغو کرد»، نه «خطای ساخت DTO».
                    catch (Exception exception) when (exception is not OperationCanceledException)
                    {
                        creationErrors.Add(exception);
                    }
                }
            }
            /// ⚠️ چرا AggregateException رو جدا گرفتیم؟
            ///
            /// BuildRssItems (در RssItemsDownLoaderOrchestrator) خطاهای ساخت
            /// RssItemها رو جمع می‌کنه و در پایان، بعد از yield همه‌ی موفق‌ها،
            /// یه AggregateException پرتاب می‌کنه.
            ///
            /// چرا اینجا گرفته می‌شه و نه در catch داخلی؟
            /// چون AggregateException از iterator (بیرون از حلقه) پرتاب می‌شه،
            /// نه از بدنه‌ی حلقه. پس catch داخلی نمی‌تونه بگیره.
            ///
            /// چرا InnerExceptions گرفته می‌شه (نه خود AggregateException)؟
            /// چون InnerExceptions شامل خطاهای تک‌تکِ هر آیتمه، نه یه خطای تجمیعی.
            /// کاربر می‌خواد «متن خطای اول» رو ببینه، نه «3 از 10 آیتم خراب شد».
            catch (AggregateException aggregateException)
            {
                creationErrors.AddRange(aggregateException.InnerExceptions);
            }
            /// ⚠️ چرا OperationCanceledException رو گرفتیم و به errors اضافه نکردیم؟
            ///
            /// چون لغو، «خطا» نیست — «دستور کنترلی»ـه. کاربر عمداً لغو کرده،
            /// این یه حالت مورد انتظاره، نه یه شکست.
            ///
            /// اگه به creationErrors اضافه می‌کردیم، VM فکر می‌کرد
            /// «خطا رخ داد» و پیام خطا نشون می‌داد. ولی کاربر باید پیام
            /// «عملیات لغو شد» ببینه.
            ///
            /// پس با WasUserCancelled = true، به VM می‌گیم این لغو بوده، نه خطا.
            catch (OperationCanceledException)
            {
                wasUserCancelled = true;
            }
            /// ⚠️ چرا catch (Exception) بیرونی نداریم؟
            ///
            /// چون خطاهای قبل از حلقه (DownloadRssFeedXmlData، GetXmlData) نباید
            /// به‌عنوان «نتیجه‌ی ناقص» برگردونده بشن.
            ///
            /// اگه این خطاها رو catch می‌کردیم و RssItemsWithDtoResult با لیست خالی
            /// برمی‌گردوندیم، VM فکر می‌کرد «هیچ خبر جدیدی نیست» — در حالی که
            /// واقعیت «خطای شبکه» بود. این پیام، گمراه‌کننده‌ست.
            ///
            /// پس این خطاها عمداً propagate می‌شن تا VM (با catch (Exception) خودش)
            /// پیام مناسب («خطای شبکه») رو نشون بده.
            return new RssItemWithDtosResult(createdRssItemWithDtos, creationErrors, wasUserCancelled);
        }


        //////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// ساخت DTO برای یک آیتم دانلودشده و ذخیره‌ی آن در کش موقت.
        ///
        /// چرا این متد قفل ندارد؟
        ///   دلیل اول: عملیات کند (CreateRssItemDtoWithSaveImage) قبل از دسترسی
        ///   به داده‌ی مشترک انجام می‌شود، نه بین دو دسترسی. قفل فقط وقتی لازم است
        ///   که یک عملیات کند، وسط دو دسترسی به همان داده باشد.
        ///   دلیل دوم: هر فراخوانی TryMarkAsAddedIfDraftEntryUnchanged خودش اتمی است
        ///   (write lock داخلی‌اش). پس نیازی به قفل بیرونی ندارد.
        ///   دلیل سوم: هیچ قانونی بین آیتم‌های حلقه نیست — هر RssItem با Id خودش
        ///   مستقل است. پس حتی اگر بین iterateها، نخ دیگری چیزی را تغییر دهد،
        ///   هیچ چیزی در این حلقه خراب نمی‌شود.
        /// </summary>
        private RssItemWithDto CreateRssItemWithDtoAndStoreInDraftCache(RssItem rssItem)
        {
            /// isUnsaved: true — چون این آیتم تازه دانلود شده و هنوز در دیتابیس ذخیره نشده.
            /// این مقدار قبل از TryMarkAsAddedIfDraftEntryUnchanged هم درست است:
            ///   - اگر خودمان در cache بنویسیم → یعنی ذخیره‌نشده (در DB نیست).
            ///   - اگر نخ دیگری نوشته باشد → باز هم یعنی ذخیره‌نشده (در DB نیست).
            /// پس در هر دو حالت، true درست است.
            RssItemDto rssItemDto = this.draftRssItemDtoFactoryOrchestrator
                .CreateRssItemDtoWithSaveImage(rssItem, isUnsaved: true);

            RssItemWithDto rssItemWithDto = new(rssItem, rssItemDto);

            /// originalDraftEntry: null یعنی «انتظار داریم این کلید در cache نباشد».
            /// در RssItemBuilder.CreateRssItemAsync قبلاً IsRssItemValid(id) چک شده بود،
            /// پس انتظار داریم آیتم جدید باشد. ولی «کار از محکم‌کاری عیب نمی‌کند»:
            /// ممکن است نخ دیگری در این فاصله همان آیتم را دانلود و در cache ذخیره کرده باشد.
            ///
            /// TryMarkAsAddedIfDraftEntryUnchanged این انتظار را با واقعیت مقایسه می‌کند:
            ///   - اگر کلید در cache نبود (انتظار ما درست بود) → اضافه می‌کند و true برمی‌گرداند.
            ///   - اگر کلید در cache بود (یکی دیگر اضافه کرده) → اضافه نمی‌کند و false برمی‌گرداند.
            ///
            /// ⚠️ نکته‌ی مهم: false به معنی «در cache نیست» نیست، بلکه به معنی
            /// «یکی دیگر زودتر اضافه کرده» است. یعنی در هر دو حالت (true و false)،
            /// آیتم در cache موجود است. تفاوت فقط در این است که چه کسی آن را نوشته.
            bool isAddedToDraftCache = this.rssItemDraftCacheWriter
                .TryMarkAsAddedIfDraftEntryUnchanged(rssItem.Id, rssItemWithDto, originalDraftEntry: null);

            /// حالت true: خودمان اضافه کردیم. نسخه‌ی خودمان همان نسخه‌ی موجود در cache است.
            if (isAddedToDraftCache == true)
                return rssItemWithDto;

            /// حالت false: نخ دیگری زودتر اضافه کرده. نسخه‌ی ما ممکن است دیگر
            /// «منبع حقیقت» نباشد (مثلاً آن نخ، نسخه‌ی متفاوتی نوشته باشد یا بعداً
            /// ویرایش شده باشد). پس نسخه‌ی موجود در cache را می‌خوانیم و برمی‌گردانیم،
            /// تا همیشه آنچه به VM می‌رسد، با آنچه در cache است یکی باشد.
            ///
            /// ⚠️ پنجره‌ی بین TryMarkAsAddedIfDraftEntryUnchanged و TryGetRssItemWithDto:
            ///   در این فاصله قفل نداریم. اگر نخ دیگری بین این دو خط، آیتم را حذف یا تغییر
            ///   دهد، ممکن است نسخه‌ی ناسازگاری برگردانیم.
            ///   ولی این پنجره بسیار کوتاه است (چند دستورالعمل) و احتمال این‌که نخ دیگری
            ///   دقیقاً در همین فاصله دخالت کند، عملاً صفر است (مخصوصاً که AsyncRelayCommand
            ///   در VM از اجرای هم‌زمان همان کامند جلوگیری می‌کند).
            ///   اگر روزی این ریسک واقعاً مهم شد، می‌توان این دو خط را در یک write lock
            ///   یا یک متد ترکیبی در RssItemDraftCacheManager (مثلاً TryGetOrAdd) قرار داد.
            this.rssItemDraftCacheReader.TryGetRssItemWithDto(rssItem.Id, out rssItemWithDto);
            return rssItemWithDto;
        }

    }
}
