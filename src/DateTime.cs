/*
License

Some of this may have been acquired from other sources, whose copyright has 
been lost. So no copyright is claimed and it is unreasonable to grant 
permission to use, copy, modify, etc (as in the normal MIT License). 

If any copyright holders identify their material herein, then the
appropriate copyright notice will be added. 

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;

namespace Mistware.Utils
{

    /// Date and Time extension methods 
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Adjust the time component of a DateTime, so the time is Midnight (00:00)
        /// </summary>
        /// <param name="current">DateTime: dd-mm-yyyy hh:mm:ss.</param>
        /// <returns>DateTime: dd-mm-yyyy 00:00:00.</returns>   
        public static DateTime Midnight(this DateTime current)
        {
            return current.SetTime(0, 0);
        }

        /// <summary>
        /// Adjust the time component of a DateTime, so the time is Noon (12:00)
        /// </summary>
        /// <param name="current">DateTime: dd-mm-yyyy hh:mm:ss.</param>
        /// <returns>DateTime: dd-mm-yyyy 12:00:00.</returns>   
        public static DateTime Noon(this DateTime current)
        {
            return current.SetTime(12, 0);
        }

        /// <summary>
        /// Adjust the time component of a DateTime, so the time is hh:mm
        /// </summary>
        /// <param name="current">DateTime from which the date portion only is taken (dd-mm-yyyy)</param>
        /// <param name="hour">The hour portion - hh</param>
        /// <param name="minute">The minute portion - mm</param>
        /// <returns>DateTime: dd-mm-yyyy hh:mm</returns>   
        public static DateTime SetTime(this DateTime current, int hour, int minute)
        {
            return current.SetTime(hour, minute, 0, 0);
        }

        /// <summary>
        /// Adjust the time component of a DateTime, so the time is hh:mm:ss
        /// </summary>
        /// <param name="current">DateTime from which the date portion only is taken (dd-mm-yyyy)</param>
        /// <param name="hour">The hour portion - hh</param>
        /// <param name="minute">The minute portion - mm</param>
        /// <param name="second">The second portion - ss</param>
        /// <returns>DateTime: dd-mm-yyyy hh:mm:ss</returns>   
        public static DateTime SetTime(this DateTime current, int hour, int minute, int second)
        {
            return current.SetTime(hour, minute, second, 0);
        }

        /// <summary>
        /// Adjust the time component of a DateTime, so the time is hh:mm:ss.ms
        /// </summary>
        /// <param name="current">DateTime from which the date portion only is taken (dd-mm-yyyy)</param>
        /// <param name="hour">The hour portion - hh</param>
        /// <param name="minute">The minute portion - mm</param>
        /// <param name="second">The second portion - ss</param>
        /// <param name="millisecond">The second portion - ms</param>        
        /// <returns>DateTime: dd-mm-yyyy hh:mm:ss.ms</returns>   
        public static DateTime SetTime(this DateTime current, int hour, int minute, int second, int millisecond)
        {
            return new DateTime(current.Year, current.Month, current.Day, hour, minute, second, millisecond);
        }
    
        /// <summary>
        /// Day of Week (chained e.g. int dow = Now().DOW(); ) 
        /// </summary> 
        /// <param name="current">DateTime</param>
        /// <returns>Integer 1 to 7, where 1 is Monday and 7 is Sunday</returns>
        public static int DOW(this DateTime current)
        {
            return Time.DOW(Time.ToDate(current.Year, current.Month, current.Day));
        }

        /// <summary>
        /// Returns the day number of Jan 1st for the year in current.
        /// </summary>
        /// <returns>Integer 1 to 7, where 1 is Monday and 7 is Sunday</returns>
        public static int Jan1WeekDay(this DateTime current)
        {
            return Time.Jan1WeekDay(current.Year);
        }

        /// <summary>
        /// Day of Year (chained: e.g. int doy = Now().DOY(); ) 
        /// </summary>
        /// <param name="current">DateTime</param>
        /// <returns>Integer 1 to 366, where 1 is 1st Jan</returns>
        public static int DOY(this DateTime current)
        {
            return Time.DOY(Time.ToDate(current.Year, current.Month, current.Day));
        }

        /// <summary>
        /// Is a Leap Year (chained: e.g. bool test = Now().IsLeapYear(); )
        /// </summary> 
        /// <param name="current">DateTime</param>
        /// <returns>True if the date is a leap year.</returns>
        public static bool IsLeapYear(this DateTime current)
        {
            return Time.IsLeap(current.Year);
        }

        /// <summary>
        /// ISO Week number, as defined by ISO 8601 (chained: e.g. int weeknum = Now().ISOWeekNum(); )
        /// </summary> 
        /// <param name="current">DateTime</param>
        /// <returns>Integer 1 to 53, where 1 is week one.</returns>
        public static int ISOWeekNum(this DateTime current)
        {
            return Time.ISOWeekNum(Time.ToDate(current.Year, current.Month, current.Day));
        }

        /// <summary>
        /// The date of the start of ISO week one (chained: e.g. DateTime weekone = Now().ISOWeekOne(); )
        /// Weeks start with Monday. Each week's year is the Gregorian year in which the Thursday falls. 
        /// The first week of the year, hence, always contains 4 January. ISO week year numbering therefore 
        /// slightly deviates from the Gregorian for some days close to 1 January.
        /// </summary> 
        /// <param name="current">DateTime</param>
        /// <returns>The date of the start of week one of the current year.</returns>
        public static DateTime ISOWeekOne(this DateTime current)
        {
            long dt = Time.ISOWeekOne(current.Year);
            return new DateTime(Time.Year(dt), Time.Month(dt), Time.Day(dt));
        }

        /// <summary>
        /// The ISO year, which may differ from the Gregorian year for some days close to 1 January. 
        /// Chained: e.g. int isoyear = Now().ISOYear(); 
        /// Weeks start with Monday. Each week's year is the Gregorian year in which the Thursday falls. 
        /// The first week of the year, hence, always contains 4 January. 
        /// ISO week year numbering therefore slightly deviates from the Gregorian for some days close to 1 January. 
        /// </summary>
        /// <param name="current">DateTime</param>
        /// <returns>The ISO year corresponding to the <paramref name="current"/>year.</returns>
        public static int ISOYear(this DateTime current)
        {
            return Time.ISOYear(Time.ToDate(current.Year, current.Month, current.Day));
        }

        /// <summary>
        /// Convert DateTime to a Date Stamp (yyyyMMddhhmm)
        /// </summary>
        /// <param name="current">DateTime</param>
        /// <returns>String with Date Stamp corresponding to the <paramref name="current"/>year.</returns>
        public static string ToDateStamp(this DateTime current)
        {
            long date = Time.ToDate(current.Year, current.Month,  current.Day);
            long time = Time.ToTime(current.Hour, current.Minute, current.Second);
            return Time.ToDateStamp(date+time);
        }

        /// <summary>
        /// Convert DateTime to a Log Stamp (dd/MM hh:mm:ss)
        /// </summary>
        /// <param name="current">DateTime</param>
        /// <returns>String with Log Stamp corresponding to the <paramref name="current"/>year.</returns>
        public static string ToLogStamp(this DateTime current)
        {
            long date = Time.ToDate(current.Year, current.Month,  current.Day);
            long time = Time.ToTime(current.Hour, current.Minute, current.Second);
            return Time.ToLogStamp(date+time);
        }

        /// <summary>
        /// Convert DateTime to a Date String (dd/MM/yyyy)
        /// </summary>
        /// <param name="current">DateTime</param>
        /// <returns>String with Date String corresponding to the <paramref name="current"/>year.</returns>
        public static string ToDateString(this DateTime current)
        {
            return Time.ToDateString(Time.ToDate(current.Year, current.Month, current.Day));
        }

        /// <summary>
        /// Convert DateTime to an ISO Date String (yyyy-ww-dd)
        /// </summary>
        /// <param name="current">DateTime</param>
        /// <returns>String with ISO Date String corresponding to the <paramref name="current"/>year.</returns>
        public static string ToISODateString(this DateTime current)
        {
            return Time.ToISODateString(Time.ToDate(current.Year, current.Month, current.Day));
        }
    }

    /// Date and Time methods 
    public static class Time
    {

        const int SecondsPerMinute = 60;
        const int SecondsPerHour   = 60 * 60;       // SecondsPerMinute * MinutesPerHour
        const int SecondsPerDay    = 60 * 60 * 24;  // SecondsPerMinute * MinutesPerHour * HoursPerDay

        /// Jan1 - Calculate date value of 1st Jan for given year.
        public static long Jan1(int year)
        {
            int  divby4, divby100, divby400;
            long days;

            divby4   = (year-1) / 4;
            divby100 = (year-1) / 100;
            divby400 = (year-1) / 400;

            days = year*365 + divby4 - divby100 + divby400; 
            if (year > 0) ++days;

            return days;
        } 

        /// IsLeap - returns true if the year is a leap year.
        public static bool IsLeap(int year)
        {
            bool leap;
    
            if ((((year % 4) == 0) && ((year % 100) != 0)) || ((year % 400) == 0)) 
                 leap = true;
            else leap = false;

            return leap;
        }

        /// DaysInYear - returns number of days in a year.
        public static int DaysInYear(int year)
        {
            if (IsLeap(year)) return 366;
            else              return 365;
        }

        /// DaysBefore - returns number of days in the year before month.
        ///              n.b. adjusted for leap year.
        private static int DaysBefore(int month, int year)
        {
            //                         Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
            //                          31,  28,  31,  30,  31,  30,  31,  31,  30,  31,  30,  31
            int[] before = new int[] {   0,  31,  59,  90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
            int adjust;

            if (month < 1 || month > 13) throw new Exception("Month out of bounds in DaysBefore");
            adjust=0;
            if (IsLeap(year) && month > 2) adjust=1;
    
            return before[month-1]+adjust;
        }

        /// ToUnixTime - Derive UNIX Time from datetime.
        public static long ToUnixTime(long dt)
        {
            return dt - (Jan1(1970) * (long)SecondsPerDay);
        }

        /// FromUnixTime - Derive datetime from UNIX Time.
        public static long FromUnixTime(long ut)
        {
            return ut + (Jan1(1970) * (long)SecondsPerDay);
        }
        
        /// Now - Returns System Date and Time as datetime.
        public static long Now()
        {
            long d, t; 

            DateTime now = DateTime.Now;

            t = ToTime(now.Hour, now.Minute, now.Second);
            d = ToDate(now.Year, now.Month, now.Day);
            
            return d + t;
        }

        /// ToDate - Calculate datetime from as days since 1/1/0000 from year, month and day.
        public static long ToDate(int year, int month, int day)
        {
            return (Jan1(year) + DaysBefore(month, year) + day - 1) * (long)SecondsPerDay;
        }

        /// ToDays - Convert datetime to days since 1/1/0000
        public static long ToDays(long dt)
        {
            return (long)(dt / (long)SecondsPerDay);
        }

        /// Year - Calculate year from date.
        public static int Year(long dt)
        {
            int  year;
            long d, check;

            d = ToDays(dt);

            year = (int)((double)d / 365.2425);

            check = Jan1(year);
            while (d < check)
            {
                // If the first day of the calculated year is higher than the given days,
                // then we are a year in advance
                --year;
                check = Jan1(year);
            }
            while (d > ( check + (long)DaysInYear(year) -1L))
            {
                // If the last day of the calculated year is lower than the given days,
                // then we are a year behind
                ++year;
                check = Jan1(year);
            }
            if (d < check || d > ( check + (long)DaysInYear(year) -1L)) 
            {
                throw new Exception("Failed to calculate year from date in Year()");
            }
            return year;
        }

        /// DOY - Calculate Day in Year from date.
        public static int DOY(long dt)
        {
            return (int)(ToDays(dt) - Jan1(Year(dt)) + 1L);
        }

        /// Month - Calculate Month from date.
        public static int Month(long dt)
        {
            int  year, doy, month;

            year = Year(dt);
            doy  = DOY(dt);

            // Estimate month on assumption that every month has 31 days.
            // The estimate may be too low by at most one month, so adjust.
            month = ((doy-1) / 31) + 1;
            if (doy > DaysBefore(month+1, year)) month++;

            return month;
        }

        /// Day - Calculate Day from date.
        public static int Day(long dt)
        {
            int  year, doy, month;

 
            year = Year(dt);
            doy  = DOY(dt);
            month = Month(dt);

            return doy - DaysBefore(month, year);
        }


        /// AddDays - Add days to datetime.
        public static long AddDays(long d, int days)
        {
            return d + ((long)days * (long)SecondsPerDay);
        }

        /// Jan1WeekDay - returns the day number of Jan 1st for the year.
        public static int Jan1WeekDay(int year)
        {
            int i, j, k;

            i = (year - 1) % 100;
            j = (year - 1) - i;
            k = i + (i / 4);
    
            return (1 + (((((j / 100) % 4) * 5) + k) % 7));
        }

        /// DOW - Day of Week - 1 to 7, where 1 is Monday and 7 is Sunday.
        public static int DOW(long dt)
        {
            return (1 + (((DOY(dt) + Jan1WeekDay(Year(dt))) - 2) % 7));
        }

        /// ISO Week number, as defined by ISO 8601 returns 1 to 53, where 1 is week one
        public static int ISOWeekNum(long dt)
        {
            int  year;
            long week1;

            year = Year(dt);
    
            if (ToDays(dt) >= ToDays(ToDate(year, 12, 29)))
            {
                week1 = ISOWeekOne(year+1);
                if (ToDays(dt) < ToDays(week1))
                {
                    week1 = ISOWeekOne(year);
                }
            }
            else
            {
                week1 = ISOWeekOne(year);
                if (ToDays(dt) < ToDays(week1))
                {
                    week1 = ISOWeekOne(year-1);
                }
            }
            return (int)(((ToDays(dt) - ToDays(week1)) / 7L) + 1L);
        }

        ///***********************************************************************/
        ///*   ISOWeekOne - The date of the start of ISO week one.               */
        ///*                Weeks start with Monday. Each week's year is the     */
        ///*                Gregorian year in which the Thursday falls.          */
        ///*                The first week of the year, hence, always contains   */
        ///*                4 January. ISO week year numbering therefore         */
        ///*                slightly deviates from the Gregorian for some days   */
        ///*                close to 1 January.                                  */  
        ///***********************************************************************/
        public static long ISOWeekOne(int year)
        {
            long dt;

            dt = ToDate(year, 1, 4);

            return AddDays(dt, (1 - DOW(dt)));
        }

        ///***********************************************************************/
        ///*   ISOYear - The ISO year, which may differ from the Gregorian year  */
        ///*             for some days close to 1 January.                       */
        ///*             Weeks start with Monday. Each week's year is the        */
        ///*             Gregorian year in which the Thursday falls.             */ 
        ///*             The first week of the year, hence, always contains      */
        ///*             4 January. ISO week year numbering therefore slightly   */
        ///*             deviates from the Gregorian for some days close to 1    */
        ///*             January.                                                */
        ///***********************************************************************/
        public static int ISOYear(long dt)
        {
            int  year;
            long days;

            year = Year(dt);
            days = ToDays(dt);

            if (days >= ToDays(ToDate(year, 12, 29)))
            {
                if (days >= ToDays(ISOWeekOne(year+1)))
                {
                    year++;
                }
            }
            else
            {
                if (days < ToDays(ISOWeekOne(year)))
                {
                    year--;
                }        
            }
    
            return year;
        }

        /// ToTime - Calculate datetime as seconds past midnight from hour, minute and second.
        public static long ToTime(int hour, int minute, int second)
        {
            return (SecondsPerHour*hour) + (SecondsPerMinute*minute) + second;
        }

        /// Hour - Return Hour from time (range 0 to 23)
        public static int Hour(long t)
        {
            return (int)( t % SecondsPerDay)  / SecondsPerHour;
        }

        /// Minute - Return Minute from time (range 0 to 59)
        public static int Minute(long t)
        {
            return (int)( t % SecondsPerHour) / SecondsPerMinute;
        }

        /// Second - Return Second from time (range 0 to 59)
        public static int Second(long t)
        {
            return (int)( t % SecondsPerMinute);
        }

        /// ToDateString - Return string with date formatted as dd/MM/yyyy
        public static string ToDateString(long dt)
        {
            return String.Format("{0:d2}/{1:d2}/{2:d4}", Day(dt), Month(dt), Year(dt));
        }

        /// ToDateString - Return string with date as ISO date yyyy-ww-dd
        public static string ToISODateString(long dt)
        {
            return String.Format("{0:d4}-{1:d}-{2:d}", ISOYear(dt), ISOWeekNum(dt), DOW(dt));
        }

        /// ToDateStamp - Return string with date formatted as yyyyMMddhhmm
        public static string ToDateStamp(long dt)
        {
            return String.Format("{0:d4}{1:d2}{2:d2}{3:d2}{4:d2}", Year(dt), Month(dt), Day(dt), Hour(dt), Minute(dt));
        }

        /// ToLogStamp - Return string with date formatted as dd/MM hh:mm:ss
        public static string ToLogStamp(long dt)
        {
            return String.Format("{0:d2}/{1:d2} {2:d2}:{3:d2}:{4:d2}", Day(dt), Month(dt), Hour(dt), Minute(dt), Second(dt));
        }

    }
}
