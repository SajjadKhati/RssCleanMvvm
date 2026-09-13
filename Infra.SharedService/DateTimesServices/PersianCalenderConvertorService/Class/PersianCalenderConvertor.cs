using Application.InfrastructureContracts.SharedProject.DateTimes.PersianCalenderConvertor.Interface;
using System.Globalization;

namespace Infra.SharedService.DateTimesServices.PersianCalenderConvertorService.Class
{
    public class PersianCalenderConvertor : IPersianCalenderConvertor
    {

        public string ToPersianDateString(DateTime dateTime)
        {
            PersianCalendar persianCalendar = new PersianCalendar();

            int year = persianCalendar.GetYear(dateTime);
            int month = persianCalendar.GetMonth(dateTime);
            int day = persianCalendar.GetDayOfMonth(dateTime);
            int hour = persianCalendar.GetHour(dateTime);
            int minute = persianCalendar.GetMinute(dateTime);
            DayOfWeek dayOfWeek = persianCalendar.GetDayOfWeek(dateTime);

            string[] persianDayNames = { "شنبه", "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه" };
            string[] persianMonthNames = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };

            // تبدیل DayOfWeek به اندیس در آرایه فارسی که با شنبه شروع می‌شود
            int dayIndex = ((int)dayOfWeek + 1) % 7;
            string dayName = persianDayNames[dayIndex];
            string monthName = persianMonthNames[month - 1];

            // چهارشنبه، 2 مهر 1405 - ساعت 14:30
            return $"{dayName}، {day} {monthName} {year} - ساعت {hour:00}:{minute:00}";
        }

    }
}
