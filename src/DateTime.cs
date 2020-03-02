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
            return (1 + (((current.DOY() + current.Jan1WeekDay()) - 2) % 7));
        }

        /// <summary>
        /// Returns the day number of Jan 1st for the year in current.
        /// </summary>
        /// <returns>Integer 1 to 7, where 1 is Monday and 7 is Sunday</returns>
        public static int Jan1WeekDay(this DateTime current)
        {
            int iYear = current.Year;
            int i = (iYear - 1) % 100;
            int j = (iYear - 1) - i;
            int k = i + (i / 4);
            return (1 + (((((j / 100) % 4) * 5) + k) % 7));
        }

        /// <summary>
        /// Day of Year (chained: e.g. int doy = Now().DOY(); ) 
        /// </summary>
        /// <param name="current">DateTime</param>
        /// <returns>Integer 1 to 366, where 1 is 1st Jan</returns>
        public static int DOY(this DateTime current)
        {
            int iAdjust;
            int iMonth = current.Month;
            //                              Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
            //                               31,  28,  31,  30,  31,  30,  31,  31,  30,  31,  30,  31
            int[] months = new int[] {   0,  31,  59,  90, 120, 151, 181, 212, 243, 273, 304, 334 };
            if ((iMonth > 2) && current.IsLeapYear())
            {
                iAdjust = 1;
            }
            else
            {
                iAdjust = 0;
            }
            return ((current.Day + months[iMonth - 1]) + iAdjust);
        }

        /// <summary>
        /// Is a Leap Year (chained: e.g. bool test = Now().IsLeapYear(); )
        /// </summary> 
        /// <param name="current">DateTime</param>
        /// <returns>True if the date is a leap year.</returns>
        public static bool IsLeapYear(this DateTime current)
        {
            int iYear = current.Year;
            return ((((iYear % 4) == 0) && ((iYear % 100) != 0)) || ((iYear % 400) == 0));
        }

        /// <summary>
        /// ISO Week number, as defined by ISO 8601 (chained: e.g. int weeknum = Now().ISOWeekNum(); )
        /// </summary> 
        /// <param name="current">DateTime</param>
        /// <returns>Integer 1 to 53, where 1 is week one.</returns>
        public static int ISOWeekNum(this DateTime current)
        {
            DateTime week1;
            int iYear = current.Year;
            if (current >= new DateTime(iYear, 12, 29))
            {
                week1 = current.AddYears(1).ISOWeekOne();
                if (current < week1)
                {
                    week1 = current.ISOWeekOne();
                }
            }
            else
            {
                week1 = current.ISOWeekOne();
                if (current < week1)
                {
                    week1 = current.AddYears(-1).ISOWeekOne();
                }
            }
            TimeSpan ts = (TimeSpan) (current - week1);
            return ((ts.Days / 7) + 1);
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
            int iYear = current.Year;
            DateTime dt = new DateTime(iYear, 1, 4);
            int iDay = (int) dt.DayOfWeek;
            if (iDay == 0)
            {
                iDay = 7;
            }
            return dt.AddDays((double) (1 - iDay));
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
            int iYear = current.Year;
            if (current >= new DateTime(iYear, 12, 29))
            {
                if (current >= current.AddYears(1).ISOWeekOne())
                {
                    iYear++;
                }
                return iYear;
            }
            if (current < current.ISOWeekOne())
            {
                iYear--;
            }
            return iYear;
        }

        /// <summary>
        /// Convert DateTime to a Date Stamp (yyyyMMddhhmm)
        /// </summary>
        /// <param name="current">DateTime</param>
        /// <returns>String with Date Stamp corresponding to the <paramref name="current"/>year.</returns>
        public static string ToDateStamp(this DateTime current)
        {
            if (current.Year > 1900)
            {
                return (current.Year.ToString("d4") + current.Month.ToString("d2") + current.Day.ToString("d2") + current.Hour.ToString("d2") + current.Minute.ToString("d2"));
            }
            return "";
        }

        /// <summary>
        /// Convert DateTime to a Log Stamp (dd/MM hh:mm:ss)
        /// </summary>
        /// <param name="current">DateTime</param>
        /// <returns>String with Log Stamp corresponding to the <paramref name="current"/>year.</returns>
        public static string ToLogStamp(this DateTime current)
        {
            if (current.Year > 1900)
            {
                return (current.Day.ToString("d2") + "/" + current.Month.ToString("d2") + " " + current.Hour.ToString("d2") + ":" + current.Minute.ToString("d2") + ":" + current.Second.ToString("d2"));
            }
            return "";
        }

        /// <summary>
        /// Convert DateTime to a Date String (dd/MM/yyyy)
        /// </summary>
        /// <param name="current">DateTime</param>
        /// <returns>String with Date String corresponding to the <paramref name="current"/>year.</returns>
        public static string ToDateString(this DateTime current)
        {
            if (current.Year > 1900)
            {
                return current.ToString("dd/MM/yyyy");
            }
            return "";
        }

        /// <summary>
        /// Convert DateTime to an ISO Date String (yyyy-ww-dd)
        /// </summary>
        /// <param name="current">DateTime</param>
        /// <returns>String with ISO Date String corresponding to the <paramref name="current"/>year.</returns>
        public static string ToISODateString(this DateTime current)
        {
            int iYear = current.ISOYear();
            int iWeek = current.ISOWeekNum();
            int iDay = current.DOW();
            if (current.Year > 1900)
            {
                return (iYear.ToString("d0") + "-" + iWeek.ToString("d0") + "-" + iDay.ToString("d0"));
            }
            return "";
        }

    }
}
