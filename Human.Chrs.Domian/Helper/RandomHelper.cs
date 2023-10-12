using System;
using System.Security.Cryptography;
using System.Text;

namespace Human.Chrs.Domain.Helper
{
    public static class RandomHelper
    {
        public const string Numberic = "0123456789";

        public const string UppercaseAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public const string LowercaseAlphabet = "abcdefghijklmnopqrstuvwxyz";

        public static string GenerateLowercaseString(int length)
        {
            return GenerateString(length, LowercaseAlphabet + Numberic, exceptConfuse: true);
        }

        public static string GenerateString(int length, string characters, bool exceptConfuse = false)
        {
            var s = new StringBuilder();
            var r = new Random(Guid.NewGuid().GetHashCode());

            string chars = characters;
            if (exceptConfuse)
            {
                var regex = new System.Text.RegularExpressions.Regex("[01IOl]");
                chars = regex.Replace(chars, string.Empty);
            }

            for (int i = 0; i < length; i++)
            {
                s.Append(chars.Substring(r.Next(0, chars.Length - 1), 1));
            }

            return s.ToString();
        }

        public static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[4];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(validChars[(int)(num % (uint)validChars.Length)]);
                }
            }

            return res.ToString();
        }

        public static string GenerateString(int length)
        {
            return GenerateString(length, "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789");
        }

        public static string GenerateNumber(int digit = 6)
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            return r.Next(0, 99999).ToString($"D{digit}");
        }

        public static string GenerateChinessName()
        {
            var r = new Random();

            string[] crabofirstName = new string[] {
                "白", "畢", "卞", "蔡", "曹", "岑", "常", "車", "陳", "成", "程", "池", "鄧", "丁", "範", "方", "樊", "閆", "倪", "周",
                "費", "馮", "符", "元", "袁", "嶽", "雲", "曾", "詹", "張", "章", "趙", "鄭", "鍾", "周", "鄒", "朱", "褚", "莊", "卓",
                "傅", "甘", "高", "葛", "龔", "古", "關", "郭", "韓", "何", "賀", "洪", "侯", "胡", "華", "黃", "霍", "姬", "簡", "江",
                "姜", "蔣", "金", "康", "柯", "孔", "賴", "郎", "樂", "雷", "黎", "李", "連", "廉", "樑", "廖", "林", "凌", "劉", "柳",
                "龍", "盧", "魯", "陸", "路", "呂", "羅", "駱", "馬", "梅", "孟", "莫", "母", "穆", "倪", "寧", "歐", "區", "潘", "彭",
                "蒲", "皮", "齊", "戚", "錢", "強", "秦", "丘", "邱", "饒", "任", "沈", "盛", "施", "石", "時", "史", "司徒", "蘇", "孫",
                "譚", "湯", "唐", "陶", "田", "童", "塗", "王", "危", "韋", "衛", "魏", "溫", "文", "翁", "巫", "鄔", "吳", "伍", "武",
                "席", "夏", "蕭", "謝", "辛", "邢", "徐", "許", "薛", "嚴", "顏", "楊", "葉", "易", "殷", "尤", "於", "餘", "俞", "虞"
            };

            string lastName = "震南洛栩嘉光琛瀟聞鵬宇斌威漢火科技夢琪憶柳之召騰飛慕青問蘭爾嵐元香初夏沛菡傲珊曼文樂菱癡珊恨玉惜香寒新柔語蓉海安夜蓉涵柏水桃醉藍春語琴從彤" +
                "傲晴語菱碧彤元霜憐夢紫寒妙彤曼易南蓮紫翠雨寒易煙如萱若南尋真曉亦向珊慕靈以蕊尋雁映易雪柳孤嵐笑霜海雲凝天沛珊寒雲冰旋宛兒" +
                "綠真盼曉霜碧凡夏菡曼香若煙半夢雅綠冰藍靈槐平安書翠翠風香巧代雲夢曼幼翠友巧聽寒夢柏醉易訪旋亦玉凌萱訪卉懷亦笑藍春翠靖柏夜蕾" +
                "冰夏夢鬆書雪樂楓念薇靖雁尋春恨山從寒憶香覓波靜曼凡旋以亦念露芷蕾千帥新波代真新蕾雁玉冷卉紫千琴恨天傲芙盼山懷蝶冰山柏翠萱恨鬆問旋" +
                "南白易問筠如霜半芹丹珍冰彤亦寒寒雁憐雲尋文樂丹翠柔谷山之瑤冰露爾珍谷雪樂萱涵菡海蓮傲蕾青槐洛冬易夢惜雪宛海之柔夏青妙菡春竹癡夢紫藍曉巧幻柏" +
                "元風冰楓訪蕊南春芷蕊凡蕾凡柔安蕾天荷含玉書雅琴書瑤春雁從安夏槐念芹懷萍代曼幻珊谷絲秋翠白晴海露代荷含玉書蕾聽訪琴靈雁秋春雪青樂瑤含煙涵雙" +
                "平蝶雅蕊傲之靈薇綠春含蕾夢蓉初丹聽聽蓉語芙夏彤凌瑤憶翠幻靈憐菡紫南依珊妙竹訪煙憐蕾映寒友綠冰萍惜霜凌香芷蕾雁卉迎夢元柏代萱紫真千青凌寒" +
                "紫安寒安懷蕊秋荷涵雁以山凡梅盼曼翠彤谷新巧冷安千萍冰煙雅友綠南鬆詩云飛風寄靈書芹幼蓉以藍笑寒憶寒秋煙芷巧水香映之醉波幻蓮夜山芷卉向彤小玉幼";

            string name = crabofirstName[r.Next(0, crabofirstName.Length - 1)] + lastName[r.Next(0, lastName.Length - 1)] + lastName[r.Next(0, lastName.Length - 1)];

            return name;
        }

        public static DateTime? GenerateRandomDate()
        {
            var r = new Random();

            DateTime start = new DateTime(2022, 1, 1);
            int range = (DateTime.Today - start).Days;
            int days = r.Next(range);
            if (days % 3 == 0)
            {
                return null;
            }
            else
            {
                return start.AddDays(days);
            }
        }
    }
}