﻿using BasicExample.Misc;
using System;
using System.Diagnostics;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace BasicExample.DateTimeExamples
{
    public class DateTimeExamples : BaseTests
    {
        public DateTimeExamples(ITestOutputHelper output) : base(output) { }

        [Fact(DisplayName = "基本")]
        public void Example1()
        {
            // システム日時の取得
            Console.WriteLine(DateTime.Now);    // "2021/07/08 21:30:06"
            Console.WriteLine(DateTime.Today);  // "2021/07/08 0:00:00"
            Console.WriteLine(DateTime.UtcNow); // "2021/07/08 12:30:06"

            // 特定日時の生成
            DateTime dt = new DateTime(2001, 2, 3, 4, 5, 6, 987);
            Console.WriteLine(dt); // "2001/02/03 4:05:06"

            // 最小・最大
            Console.WriteLine(DateTime.MinValue.ToString("O")); // "0001-01-01T00:00:00.0000000"
            Console.WriteLine(DateTime.MaxValue.ToString("O")); // "9999-12-31T23:59:59.9999999"

            // 年/月/日/時/分/秒の取得
            Console.WriteLine(dt.Date);        // "2001/02/03 0:00:00"
            Console.WriteLine(dt.Year);        // 2001
            Console.WriteLine(dt.Month);       // 2
            Console.WriteLine(dt.Day);         // 3
            Console.WriteLine(dt.Hour);        // 4
            Console.WriteLine(dt.Minute);      // 5
            Console.WriteLine(dt.Second);      // 6
            Console.WriteLine(dt.Millisecond); // 987

            // 曜日
            Console.WriteLine(dt.DayOfWeek);      // "Saturday"(DayOfWeek列挙体)
            Console.WriteLine((int)dt.DayOfWeek); // 6 (cf. Sunday = 0)
        }

        [Fact(DisplayName = "タイムゾーン変換")]
        public void Example2()
        {
            // ローカルゾーン情報
            TimeZoneInfo zoneInfo = TimeZoneInfo.Local;
            Console.WriteLine(zoneInfo.Id);           // "Tokyo Standard Time"
            Console.WriteLine(zoneInfo.DisplayName);  // "(UTC+09:00) 大阪、札幌、東京"
            Console.WriteLine(zoneInfo.StandardName); // "東京 (標準時)"

            // ローカルゾーン・UTC時刻変換
            DateTime local = new DateTime(2023, 4, 5, 13, 34, 56, 987);
            DateTime utc = local.ToUniversalTime();
            Console.WriteLine(local);     // "2023/04/05 13:34:56" (UTC+9:00)
            Console.WriteLine(utc);       // "2023/04/05 4:34:56"  (UTC+0:00)

            // 任意のタイムゾーンの日時に変換
            TimeZoneInfo zone1 = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            DateTime zone1dt = TimeZoneInfo.ConvertTime(local, zone1);
            Console.WriteLine(zone1dt);   // "2023/04/05 7:34:56"  (UTC+3:00)
        }

        [Fact(DisplayName = "サポートする標準タイムゾーン一覧")]
        public void Example2a()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            foreach (var tz in timeZones)
                Console.WriteLine("[{0,-31}]: {1}", tz.Id, tz.DisplayName);
            // ...
            // [Cape Verde Standard Time       ]: (UTC-01:00) カーボベルデ諸島
            // [UTC                            ]: (UTC) Coordinated Universal Time
            // [Sao Tome Standard Time         ]: (UTC+00:00) サントメ
            // [Russian Standard Time          ]: (UTC+03:00) モスクワ、サンクトペテルブルク
            // [Tokyo Standard Time            ]: (UTC+09:00) 大阪、札幌、東京
            // ...
        }

        [Fact(DisplayName = "文字列変換")]
        public void Example3()
        {
            // 標準として定められた書式で出力する場合は「標準の日時書式指定」
            // それ以外の個別に指定する書式は「カスタム日時形式文字列」
            // 日本の業務システムで使用する8桁表記の日付は標準にないのでカスタムを使用が必要。

            // 標準の日時書式指定
            // https://docs.microsoft.com/ja-jp/dotnet/standard/base-types/standard-date-and-time-format-strings

            // カスタムの日時形式文字列
            // https://docs.microsoft.com/ja-jp/dotnet/standard/base-types/custom-date-and-time-format-strings
            // MMとmm、HHとhhに注意

            DateTime dt = new DateTime(2023, 4, 5, 13, 34, 56, 987);
            Console.WriteLine(dt.ToString());                    // "2023/04/05 13:34:56"
            Console.WriteLine(dt.ToString("O"));                 // "2023-04-05T13:34:56.9870000" (ISO8601)
            Console.WriteLine(dt.ToString("yyyyMMdd"));          // "20230405"
            Console.WriteLine(dt.ToString("yyyyMMddHHmmssfff")); // "20230405133456987"
            Console.WriteLine(dt.ToShortDateString());           // "2023/04/05"
            Console.WriteLine(dt.ToLongDateString());            // "2023年4月5日"

            // カレントカルチャーで良い場合は第3引数はnullで良い
            // https://docs.microsoft.com/en-us/dotnet/api/system.datetime.parseexact?view=net-5.0#System_DateTime_ParseExact_System_String_System_String_System_IFormatProvider_
            // "If provider is null, the CultureInfo object that corresponds to the current culture is used."
            var p1 = DateTime.ParseExact("12340506", "yyyyMMdd", null);
            Console.WriteLine(p1); // "1234/05/06 0:00:00"
        }

        [Fact(DisplayName = "月末月初")]
        public void Example4()
        {
            DateTime dt = new DateTime(2023, 4, 5);
            DateTime firstDateOfMonth = new DateTime(dt.Year, dt.Month, 1);
            DateTime lastDateOfMonth =
                new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1);

            Console.WriteLine(firstDateOfMonth); // "2023/04/01 0:00:00"
            Console.WriteLine(lastDateOfMonth);  // "2023/04/30 0:00:00"
        }

        [Fact(DisplayName = "日付の差分")]
        public void Example5()
        {
            // DateTimeでは演算子が定義されているので+-で操作可
            // https://docs.microsoft.com/ja-jp/dotnet/api/system.datetime?view=netcore-3.1#operators

            // 日時の差分
            DateTime dt1a = new DateTime(2023, 4, 5, 21, 34, 46, 789);
            DateTime dt1b = new DateTime(2023, 4, 6, 23, 37, 50, 900);
            TimeSpan interval = dt1b - dt1a;
            Console.WriteLine(interval.TotalDays);         // 1.0854642476851852
            Console.WriteLine(interval.TotalHours);        // 26.051141944444446
            Console.WriteLine(interval.TotalMinutes);      // 1563.0685166666667
            Console.WriteLine(interval.TotalSeconds);      // 93784.111
            Console.WriteLine(interval.TotalMilliseconds); // 93784111
            Console.WriteLine(interval.Days);              // 1
            Console.WriteLine(interval.Hours);             // 2
            Console.WriteLine(interval.Minutes);           // 3
            Console.WriteLine(interval.Seconds);           // 4
            Console.WriteLine(interval.Milliseconds);      // 111

            // 経過時間の取得
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Thread.Sleep(3000);
            stopWatch.Stop();
            TimeSpan elapsed = stopWatch.Elapsed;
            Console.WriteLine(elapsed);                   // "00:00:03.0091434"
            Console.WriteLine(elapsed.TotalMilliseconds); // 3009.1434
        }

    }

}
