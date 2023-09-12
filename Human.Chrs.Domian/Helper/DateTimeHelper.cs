using System;

namespace Human.Chrs.Domain.Helper
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// 台北時間(UTC+8)
        /// <para>避免伺服器時間與台北時間不同</para>
        /// </summary>
        public static DateTime TaipeiNow
        {
            get
            {
                return DateTime.UtcNow.AddHours(8);
            }
        }

        public static long TaipeiNowTimestamp
        {
            get
            {
                return ((DateTimeOffset)TaipeiNow).ToUnixTimeSeconds();
            }
        }
    }
}