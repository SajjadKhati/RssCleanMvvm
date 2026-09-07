using Application.InfrastructureContracts.SharedProject.DateTimes.PersianCalenderConvertor.Interface;
using System.Globalization;

namespace Infra.SharedService.DateTimesServices.PersianCalenderConvertorService.Class
{
    public class PersianCalenderConvertor : IPersianCalenderConvertor
    {

        public string ToPersianDateString(DateTime dateTime)
        {
            PersianCalendar pc = new PersianCalendar();

            int year = pc.GetYear(dateTime);
            int month = pc.GetMonth(dateTime);
            int day = pc.GetDayOfMonth(dateTime);
            DayOfWeek dayOfWeek = pc.GetDayOfWeek(dateTime);

            string[] persianDayNames = { "شنبه", "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه" };
            string[] persianMonthNames = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };

            // تبدیل DayOfWeek به اندیس در آرایه فارسی که با شنبه شروع می‌شود
            int dayIndex = ((int)dayOfWeek + 1) % 7;
            string dayName = persianDayNames[dayIndex];
            string monthName = persianMonthNames[month - 1];

            return $"{dayName}، {day} {monthName} {year} ({year}/{month:00}/{day:00})";
        }

    }
}
