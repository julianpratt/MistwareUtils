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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Mistware.Utils
{
    /// String extensions: Truncate, Left, Mid, Right, ThrowOnNullOrEmpty, HasValue, IsNull, IsInteger, 
    /// ToInteger, ToDouble, ToBool, ToEnum, CleanString, EscapeSingleQuote, StripQuotes, RemoveNonNumeric, 
    /// ToDateTime, AsciiCtlStrip, MailMerge, NormalPath, Padding, MultiPart, ToDict, DictToString, Wordise, 
    /// Match, ToStream, ToInList, ListTrim, ListToUpper, Validate, StringToList, ListToString.
    public static class StringExtensions
    {
        /// <summary>
        /// Returns <paramref name="value"/> truncated to <paramref name="maxLength"/>
        /// if the string is longer than it.
        /// </summary>
        /// <param name="value">The string to truncate</param>
        /// <param name="maxLength">The maximum length of <paramref name="value"/></param>
        /// <returns>A copy of the string, truncated to maxLength</returns>
        public static string Truncate(this string value, int maxLength)
        {
            if      (value == null)             return null;
            else if (value.Length <= maxLength) return value;
            else                                return value.Substring(0, maxLength);
            
        }

        /// <summary>
        /// Returns left hand portion of a string (synonym for Truncate).
        /// Avoid exception, by providing reasonable default behaviour. Most of the time 
        /// the count exceeds the string length, we just want the original string returned.
        /// </summary>
        /// <param name="value">The string to substring</param>
        /// <param name="count">Number of characters to return</param>
        /// <returns>The first <paramref name="count"/> characters in string s</returns>        
        public static string Left(this string value, int count)
        {
            return value.Truncate(count);
        }

        /// <summary>
        /// Returns middle portion of a string.
        /// Avoid exception, by providing reasonable default behaviour. Most of the time 
        /// the index exceeds the string length, we just want an empty string returned.
        /// </summary>
        /// <param name="value">The string to substring</param>
        /// <param name="index">Start of the string to return</param>
        /// <param name="count">Number of characters to return</param>
        /// <returns>Sub-string starting at <paramref name="index"/> from string <paramref name="value"/></returns>     
        public static string Mid(this string value, int index, int count)
        {
            if      (value == null)         return null;
            else if (index >= value.Length) return "";
            else
            {
              if ((index + count) >= value.Length) return value.Substring(index);
              else                                 return value.Substring(index, count);
            }
        }

        /// <summary>
        /// Returns right hand portion of a string.
        /// Avoid exception, by providing reasonable default behaviour. Most of the time 
        /// the count exceeds the string length, we just want the original string returned.
        /// </summary>
        /// <param name="value">The string to substring</param>
        /// <param name="count">Number of characters to return</param>
        /// <returns>The last <paramref name="count"/> characters in string <paramref name="value"/></returns>        
        public static string Right(this string value, int count)
        {
            if      (value == null)         return null;
            else if (count >= value.Length) return value;
            else                            return value.Substring(value.Length - count, count);
        }

        /// <summary>
        /// Throws an <see cref="T:System.ArgumentNullException"/> if <paramref name="str"/> is empty or null
        /// </summary>
        /// <param name="str">The item to expect</param>
        /// <param name="paramName">The name of the parameter to pass to the exception</param>        
        /// <param name="message">The message to pass into the exception</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="str"/> is null or empty</exception>   
        public static void ThrowOnNullOrEmpty(this string str, string paramName, string message)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(paramName, message);
            }
        }

        /// <summary>
        /// Checks if a string has a value assigned to it
        /// </summary>
        /// <param name="value">The string to check</param>
        /// <returns>true if the string is not null, and it has a length, even if we trim it</returns>
        public static bool HasValue(this string value)
        {
            return value != null && value.Length > 0 && value.Trim().Length > 0;
        }

        /// <summary>
        /// Replaces null string value with default (typically an empty string).
        /// </summary>
        /// <param name="s">The string to check and return</param>
        /// <param name="def">Default value, returned if <paramref name="s"/> is null</param>
        /// <returns>Either the string <paramref name="s"/>, or the default value <paramref name="def"/> if s is null.</returns>        
        public static string IsNull(this string s, string def) 
        {
            return s != null ? s : def;
        } 

        /// <summary>
        /// Tests whether string contains an integer
        /// </summary>
        /// <param name="value">The string to check</param>
        /// <returns>True if string <paramref name="value"/> contains an integer</returns>  
        public static bool IsInteger(this string value)
        {
            Regex regularExpression = new Regex("^-[0-9]+$|^[0-9]+$");
            return regularExpression.Match(value).Success;
        }

        /// <summary>
        /// Parse a string as an integer
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <returns>Either an integer, or zero if <paramref name="value"/> is null.</returns>  
        /// <exception cref="Exception">The value cannot be parsed as in int.</exception>
        public static int ToInteger(this string value)
        {
            return value.ToInteger(0);
        }

        /// <summary>
        /// Parse a string as an integer
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <param name="deflt">Default value, returned if <paramref name="value"/> is null</param>
        /// <returns>Either an integer, or the default value <paramref name="deflt"/> if <paramref name="value"/> is null.</returns> 
        /// <exception cref="Exception">The value cannot be parsed as in int.</exception> 
        public static int ToInteger(this string value, int deflt)
        {
            if (value == null) return deflt;

            int result = deflt;
            if (!int.TryParse(value, out result)) 
                throw new Exception("Unable to parse '" + value + "' as an int");
            
            return result;
        }

        /// <summary>
        /// Parse a string as a double
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <returns>Either a double, or 0.0 if <paramref name="value"/> is null.</returns>  
        /// <exception cref="Exception">The value cannot be parsed as a double.</exception>
        public static double ToDouble(this string value)
        {
            return value.ToDouble(0.0);
        }

        /// <summary>
        /// Parse a string as a double
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <param name="deflt">Default value, returned if <paramref name="value"/> is null</param>
        /// <returns>Either a double, or the default value <paramref name="deflt"/> if <paramref name="value"/> is null.</returns>  
        /// <exception cref="Exception">The value cannot be parsed as a double.</exception>
        public static double ToDouble(this string value, double deflt)
        {
            if (value == null) return deflt;

            double result = deflt;
            if (!double.TryParse(value, out result))
                throw new Exception("Unable to parse '" + value + "' as a double");

            return result;
        }

        /// <summary>
        /// Parse a string as a boolean.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <returns>Either a bool (true or false) or false if <paramref name="value"/> is null.</returns>
        /// <exception cref="Exception">The value cannot be parsed as a bool.</exception>
        public static bool ToBool(this string value)
        {           
            return value.ToBool(false);
        }

        /// <summary>
        /// Parses a string as a boolean.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="deflt">Default value, returned if <paramref name="value"/> is null</param>
        /// <returns>The value as a bool, or <paramref name="deflt"/>.</returns>
        /// <exception cref="Exception">The value cannot be parsed as a bool.</exception>
        public static bool ToBool(this string value, bool deflt)
        {
            if (value == null) return deflt;    

            bool result;
            if (!bool.TryParse(value, out result))
                throw new Exception("Unable to parse '" + value + "' as a bool");

            return result;
        }

        /// <summary>
        /// Converts a string to an enum value
        /// </summary>
        /// <typeparam name="T">The type of enum to convert to</typeparam>
        /// <param name="value">The string to convert</param>
        /// <param name="ignoreCase">True to ignore the case of <paramref name="value"/></param>
        /// <returns>An enumeration value (or the default of T, if <paramref name="value"/> is null).</returns>
        /// <exception cref="Exception">If <paramref name="value"/> is not an enum of <typeparamref name="T"/></exception>        
        public static T ToEnum<T>(this string value, bool ignoreCase)
        {
            if (value == null) return default(T);

            T result = default(T);

            try
            {
                result = (T)Enum.Parse(typeof(T), value, ignoreCase);
            }
            catch (Exception ex)
            {
                throw new Exception("Attempted to parse '" + value + "' to enum of type " + typeof(T).FullName + " failed, with error: " + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Remove control characters from string (anything with an ascii numeric value less than or equal to \r)
        /// </summary>
        /// <param name="sText">The string to process</param>
        /// <returns>String without hard returns, line feeds, etc</returns>  
        public static string CleanString(this string sText)
        {
            char[] to = new char[sText.Length];
            int j=0;

            foreach (char c in sText) if (c > '\r') to[j++]=c;

            if (j == 0 ) return string.Empty;

            return new string(to, 0, j);
        }

        /// <summary>
        /// Escape single quote characters in string (to avoid issues with SQL)
        /// </summary>
        /// <param name="sText">The string to process</param>
        /// <returns>String with single quote characters doubled up</returns>
        public static string EscapeSingleQuote(this string sText)
        {
            int n = 0;
            foreach (char c in sText) if (c == '\'') ++n;

            char[] to = new char[sText.Length+n];
            int j=0;
            foreach (char c in sText) 
            {
                if (c != '\'') to[j++]=c;
                else
                {
                    to[j++]='\'';
                    to[j++]='\'';
                }           
            }
            
            if (j == 0 ) return string.Empty;

            return new string(to, 0, j);
         }

        /// <summary>
        /// If a string is enclosed in double or single quotation marks, then remove them. Otherwise just pass on the string.
        /// </summary>
        /// <param name="s">The string to strip</param>
        /// <returns>The stripped string.</returns>
        public static string StripQuotes(this string s)
        {
            if      (s.Left(1) == "\"" && s.Right(1) == "\"") return s.Mid(1, s.Length-2);
            else if (s.Left(1) == "'"  && s.Right(1) == "'" ) return s.Mid(1, s.Length-2);            
            else                                              return s;
        }

        /// <summary>
        /// Strip any character from string that is not numeric
        /// </summary>
        /// <param name="s">The string to process</param>
        /// <returns>String without non-numeric characters</returns>  
        public static string RemoveNonNumeric(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                char[] result = new char[s.Length];
                int i = 0;
                foreach (char c in s)
                {
                    if (char.IsNumber(c))
                    {
                        result[i++] = c;
                    }
                }
                if (i == 0)
                {
                    s = string.Empty;
                    return s;
                }
                if (result.Length != i)
                {
                    s = new string(result, 0, i);
                }
            }
            return s;
        }

        /// <summary>
        /// Convert string to DateTime
        /// </summary>
        /// <param name="s">The string to process</param>
        /// <returns>DateTime value</returns>  
        public static DateTime ToDateTime(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    return Convert.ToDateTime(s);
                }
                catch
                {
                    return Convert.ToDateTime("1/1/0001");
                }
            }
            return Convert.ToDateTime("1/1/0001");
        }

        /// <summary>
        /// Strips ctl-B and ctl-D from string 
        /// </summary>
        /// <param name="s">String to be stripped</param>
        /// <returns>String without ctl-B or ctl-D chars</returns>
        public static string AsciiCtlStrip(this string s)
        {
            return Regex.Replace(s, @"[\u000B\u000D]", string.Empty);
        }

        /// <summary>
        /// Does a Mail Merge. 
        /// For each entry in dict, form search key with left and right brace around the dict key and do a replace in the template.
        /// Thus if delimiters = "{}" and a key/value pair is fred=bloggs, then this will look for all instances of {fred} in the 
        /// template and replace them with bloggs.
        /// </summary>
        /// <param name="s">Template as a string</param>
        /// <param name="dict">List of key/value pairs</param>
        /// <param name="braces">String with left and right brace</param>
        /// <returns>Expanded template</returns>  
        public static string MailMerge(this string s, Dictionary<string, string> dict, string braces)
        {
            // N.B. This algorithm is not necessarily the most efficient.
            //       It will work well when there are only a few 
            //       value pairs in the dictionary, but after that it might
            //       be better to read through the string looking for the 
            //       opening delimiter, doing a dictionary lookup and 
            //       replacing the contents using StringBuilder.
            if (braces.Length != 2) throw new Exception("MailMerge requires 2 chars in braces");      
            string leftBrace  = braces[0].ToString();    
            string rightBrace = braces[1].ToString();    

            foreach (KeyValuePair<string, string> item in dict)
            {
                string key = leftBrace + item.Key + rightBrace;
                s = s.Replace(key, item.Value);
            }
            return s;
        } 

        /// <summary>
        /// Converts a path string from Windows (with back slashes) to Unix (with forward slashes).
        /// </summary>
        /// <param name="path">The full path and filename</param>
        /// <returns>The normalised path.</returns>
        public static string NormalPath(this string path)
        {
            return path.Replace("\\", "/");
        } 

        /// <summary>
        /// Pad a string so its length is a multiple of n.
        /// </summary>
        /// <param name="s">The string to pad</param>
        /// <param name="n">The padding multiple</param>
        /// <param name="c">The character to pad with</param>        
        /// <returns>The padded string.</returns>
        public static string Padding(this string s, int n, char c)
        {
            string ret = s;
            string pad = c.ToString(); 
            while (ret.Length % n > 0) ret += pad;

            return ret;
        }  

        /// <summary>
        /// Parse multipart delimited string.
        /// </summary>
        /// <param name="s">The string to be split</param>
        /// <param name="delimiter">The delimiter character</param>
        /// <returns>An array of strings</returns>
        public static string[] MultiPart(this string s, char delimiter)
        {
            return s.Split(delimiter);
        } 

        /// <summary>
        /// Deserialise a dictionary, formatted as key1=value1,key2=value2,....
        /// </summary>
        /// <param name="definition">The string to be deserialised</param>
        /// <returns>A string,string dictionary.</returns>
        public static Dictionary<string,string> ToDict(this string definition)
        {
            Dictionary<string,string> dict = new Dictionary<string,string>();

            if (definition == null)    return dict;
            if (definition.Length== 0) return dict;

            string[] pairs = definition.Split(',');
            foreach (string entry in pairs)
            {
                string[] kv = entry.Split('=');
                dict.Add(kv[0], kv[1]);
            }

            return dict;
        } 

        /// <summary>
        /// Serialise a dictionary, formatted as key1=value1,key2=value2,....
        /// </summary>
        /// <param name="dict">The string,string dictionary to be serialised</param>
        /// <returns>Dictionary, formatted as key1=value1,key2=value2,...</returns>
        public static string DictToString(this Dictionary<string,string> dict)
        {
            string s = "";

            if (dict == null)    return s;
            if (dict.Count== 0)  return s;

            foreach (KeyValuePair<string,string> kv in dict)
            {
                if (s.Length > 0) s += ",";
                s += kv.Key + "=" + kv.Value;
            }

            return s;
        } 

        /// <summary>
        /// Convert a string into a list of strings. It either finds words delimited by spaces or double quoted strings (which are returned with their quotes).
        /// </summary>
        /// <param name="s">The string to be parsed.</param>
        /// <returns>A list of strings.</returns>
        public static List<string> Wordise(this string s)
        {
            List<string> l = new List<string>();

            string t = s.Trim();

            while (t.Length > 0)
            {
                t = t.Trim() + " ";
                if (t[0] == '"')
                {
                    // Double quote delimited string
                    t = t.Substring(1);
                    int i = t.IndexOf('"');
                    if (i <= 0) throw new Exception("Matching quote not found in Wordise.");
                    l.Add("\"" + t.Left(i) + "\"");
                    t = t.Substring(i+1).Trim();    
                }
                else
                {
                    // Find the next word
                    int i = t.IndexOf(' ');
                    if (i > 0) 
                    {
                        l.Add(t.Left(i));
                        t = t.Substring(i).Trim();  
                    }
                    else
                    {
                        l.Add(t);
                        t="";
                    }
                }
            }

            return l;
        }

        /// <summary>
        /// Determine whether 2 strings of differing lengths match, by comparing the shortest string with the same characters of the longest.
        /// </summary>
        /// <param name="s1">String to be matched</param>
        /// <param name="s2">String to be matched</param>
        /// <returns>True if the 2 strings match</returns>
        public static bool Match(this string s1, string s2)
        {
            int n = Min(s1.Length, s2.Length);
            return (s1.ToUpper().Left(n) == s2.ToUpper().Left(n));
        }

        private static int Min(int i1, int i2)
        {
            return i1 > i2 ? i2 : i1;
        }

        /// <summary>
        /// Convert a string into a Stream.
        /// </summary>
        /// <param name="s">String to be converted.</param>
        /// <returns>Stream</returns>
        public static Stream ToStream(this string s)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(s);
            MemoryStream stream = new MemoryStream( byteArray );
            return stream;
        }

        /// <summary>
        /// Convert a list of strings into a SQL IN list.
        /// e.g. 's1', 's2', 's3' 
        /// </summary>
        /// <param name="list">The list to be converted.</param>
        /// <returns>String with IN list.</returns>        
        public static string ToInList(this List<string> list)
        {
            if (list == null) throw new Exception("Null list in ToInList().");

            string s = "";
            foreach (string i in list)
            {
                s += (s == "") ? "" : ",";
                s += "'" + i + "'";
            }
            return s;
        }

        /// <summary>
        /// Trim each string in a list of strings
        /// </summary>
        /// <param name="list">The list to be converted.</param>
        /// <returns>The list with each string trimmed.</returns>           
        public static List<string> ListTrim(this List<string> list)
        {
            if (list == null) throw new Exception("Null list in ListTrim().");

            List<string> l = new List<string>();
            foreach (string s in list)
            {
                l.Add(s.Trim());
            }
            return l;
        }

        /// <summary>
        /// Convert each string in a list of strings to Upper.
        /// </summary>
        /// <param name="list">The list to be converted.</param>
        /// <returns>The list with each string converted to upper case.</returns>           
        public static List<string> ListToUpper(this List<string> list)
        {
            if (list == null) throw new Exception("Null list in ListToUpper().");

            List<string> l = new List<string>();
            foreach (string s in list)
            {
                l.Add(s.ToUpper());
            }
            return l;
        }

        /// <summary>
        /// Check a string against a list of valid values.
        /// </summary>
        /// <param name="value">The string to be checked against the list.</param>
        /// <param name="validList">The list of valid values.</param>        
        /// <returns>True if string is in list (or false is it is not).</returns> 
        public static bool Validate(this string value, List<string> validList)
        {
            if (value == null) throw new Exception("Null value in Validate().");
            if (validList == null) throw new Exception("Null validList in Validate().");            

            foreach (string s in validList) if (value == s) return true;

            return false;
        }

        /// <summary>
        /// Convert a string to a list of strings by splitting on a delimiter.
        /// </summary>
        /// <param name="s">The string to split.</param>
        /// <param name="delimiter">The delimiter to use.</param>
        /// <returns>The list of strings.</returns> 
        public static List<string> StringToList(this string s, char delimiter)
        {
            if (s == null) throw new Exception("Null string in StringToList().");

            string[] a = s.Split(delimiter);
            List<string> ret = new List<string>();
            foreach (string ss in a) ret.Add(ss.Trim());

            return ret;
        }

        /// <summary>
        /// Convert a list of strings to a string, where each string in the list is separated by the delimiter.
        /// e.g. s1/s2/s3  (if the delimiter is "/")         
        /// </summary>
        /// <param name="list">The list of strings to join.</param>
        /// <param name="delimiter">The delimiter(s) to use.</param>
        /// <returns>The resultant string</returns> 
        public static string ListToString(this List<string> list, string delimiter)
        {
            if (list == null) throw new Exception("Null list in ListToString().");
            if (delimiter == null) throw new Exception("Null delimiter in ListToString().");

            string ret = "";
            foreach (string s in list)
            {
                ret += (ret == "") ? s : delimiter + s;
            }
            return ret;
        }

    }

}