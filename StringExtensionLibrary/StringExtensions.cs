/* StringExtensions Library provides comprehensive string extension methods that go behold 
 * just the common string validation methods extending the .Net System.string class. 
 * The idea to create such a library was motivated by the lack of such a StringUtil library such as 
 * org.apache.commons.lang3.StringUtils in the .Net realm. The aim of this library is to serve as a goto library 
 * for those wishing to have such a library readily available to incorporate in to new or existing projects. 
 * 
 * Copyright (C) 2015  Timothy Mugayi
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by the Free Software Foundation, 
 * either version 3 of the License, or (at your option) any later version.
 * This program is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY; 
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.You should have received a copy of the GNU General Public License along with this program.  
 * If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace StringExtensionLibrary
{
    /// <summary>
    ///     Provides extension methods to the <see cref="string">System.string</see> object.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Checks if date with dateFormat is parse-able to System.DateTime format returns boolean value if true else false
        /// </summary>
        /// <param name="data">String date</param>
        /// <param name="dateFormat">date format example dd/MM/yyyy HH:mm:ss</param>
        /// <returns>boolean True False if is valid System.DateTime</returns>
        public static bool IsDateTime(this string data, string dateFormat)
        {
            // ReSharper disable once RedundantAssignment
            DateTime dateVal = default(DateTime);
            if (DateTime.TryParseExact(data, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateVal))
            {
                if (dateVal.Year < 1946)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Converts the string representation of a number to its 32-bit signed integer equivalent
        /// </summary>
        /// <param name="value">string containing a number to convert</param>
        /// <returns>System.Int32</returns>
        /// <remarks>
        ///     The conversion fails if the string parameter is null, is not of the correct format, or represents a number
        ///     less than System.Int32.MinValue or greater than System.Int32.MaxValue
        /// </remarks>
        public static int ToInt32(this string value)
        {
            int number;
            Int32.TryParse(value, out number);
            return number;
        }

        /// <summary>
        ///     Converts the string representation of a number to its 64-bit signed integer equivalent
        /// </summary>
        /// <param name="value">string containing a number to convert</param>
        /// <returns>System.Int64</returns>
        /// <remarks>
        ///     The conversion fails if the string parameter is null, is not of the correct format, or represents a number
        ///     less than System.Int64.MinValue or greater than System.Int64.MaxValue
        /// </remarks>
        public static long ToInt64(this string value)
        {
            long number;
            Int64.TryParse(value, out number);
            return number;
        }

        /// <summary>
        ///     Converts the string representation of a number to its 16-bit signed integer equivalent
        /// </summary>
        /// <param name="value">string containing a number to convert</param>
        /// <returns>System.Int16</returns>
        /// <remarks>
        ///     The conversion fails if the string parameter is null, is not of the correct format, or represents a number
        ///     less than System.Int16.MinValue or greater than System.Int16.MaxValue
        /// </remarks>
        public static short ToInt16(this string value)
        {
            short number;
            Int16.TryParse(value, out number);
            return number;
        }

        /// <summary>
        ///     Converts the string representation of a number to its System.Decimal equivalent
        /// </summary>
        /// <param name="value">string containing a number to convert</param>
        /// <returns>System.Decimal</returns>
        /// <remarks>
        ///     The conversion fails if the s parameter is null, is not a number in a valid format, or represents a number
        ///     less than System.Decimal.MinValue or greater than System.Decimal.MaxValue
        /// </remarks>
        public static Decimal ToDecimal(this string value)
        {
            Decimal number;
            Decimal.TryParse(value, out number);
            return number;
        }

        /// <summary>
        ///     Converts string to its boolean equivalent
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>boolean equivalent</returns>
        /// <remarks>
        ///     <exception cref="ArgumentException">thrown in the event no boolean equivalent found</exception>
        /// </remarks>
        public static bool ToBoolean(this string value)
        {
            string val = value.ToLower().Trim();

            switch (val)
            {
                case "false":
                    return false;
                case "f":
                    return false;
                case "true":
                    return true;
                case "t":
                    return true;
                case "yes":
                    return true;
                case "no":
                    return false;
                case "y":
                    return true;
                case "n":
                    return false;
                default:
                    throw new ArgumentException("Invalid boolean");
            }
        }

        /// <summary>
        ///     Returns an enumerable collection of the specified type containing the substrings in this instance that are
        ///     delimited by elements of a specified Char array
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="separator">
        ///     An array of Unicode characters that delimit the substrings in this instance, an empty array containing no
        ///     delimiters, or null.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the element to return in the collection, this type must implement IConvertible.
        /// </typeparam>
        /// <returns>
        ///     An enumerable collection whose elements contain the substrings in this instance that are delimited by one or more
        ///     characters in separator.
        /// </returns>
        public static IEnumerable<T> SplitTo<T>(this string str, params char[] separator) where T : IConvertible
        {
            return str.Split(separator, StringSplitOptions.None).Select(s => (T) Convert.ChangeType(s, typeof (T)));
        }

        /// <summary>
        ///     Returns an enumerable collection of the specified type containing the substrings in this instance that are
        ///     delimited by elements of a specified Char array
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="options">StringSplitOptions <see cref="StringSplitOptions" /></param>
        /// <param name="separator">
        ///     An array of Unicode characters that delimit the substrings in this instance, an empty array containing no
        ///     delimiters, or null.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the element to return in the collection, this type must implement IConvertible.
        /// </typeparam>
        /// <returns>
        ///     An enumerable collection whose elements contain the substrings in this instance that are delimited by one or more
        ///     characters in separator.
        /// </returns>
        public static IEnumerable<T> SplitTo<T>(this string str, StringSplitOptions options, params char[] separator)
            where T : IConvertible
        {
            return str.Split(separator, options).Select(s => (T) Convert.ChangeType(s, typeof (T)));
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="value">string enum value</param>
        /// <returns>
        ///     Enum object
        /// </returns>
        /// <remarks>
        ///     <exception cref="ArgumentNullException">enumType or value is null</exception>
        ///     <exception cref="ArgumentException">
        ///         enumType is not an System.Enum.-or- value is either an empty string ("") or
        ///         only contains white space.-or- value is a name, but not one of the named constants defined for the enumeration
        ///     </exception>
        ///     <exception cref="OverflowException">value is outside the range of the underlying type of enumTypel</exception>
        /// </remarks>
        public static T ToEnum<T>(this string value)
        {
            if (!typeof (T).IsEnum)
            {
                throw new ArgumentException("Type T Must an enum");
            }
            return (T) Enum.Parse(typeof (T), value, true);
        }

        /// <summary>
        ///     Replaces one or more format items in a specified string with the string representation of a specified object.
        /// </summary>
        /// <param name="value">A composite format string</param>
        /// <param name="arg0">An System.Object to format</param>
        /// <returns>A copy of format in which any format items are replaced by the string representation of arg0</returns>
        public static string Format(this string value, object arg0)
        {
            return string.Format(value, arg0);
        }

        /// <summary>
        ///     Gets empty String if passed value is of type Null/Nothing
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>System.String</returns>
        /// <remarks></remarks>
        public static string GetEmptyStringIfNull(this string val)
        {
            return (val != null ? val.Trim() : "");
        }

        /// <summary>
        ///     Checks if a string is null and returns String if not Empty else returns null/Nothing
        /// </summary>
        /// <param name="myValue">String value</param>
        /// <returns>null/nothing if String IsEmpty</returns>
        /// <remarks></remarks>
        public static string GetNullIfEmptyString(this string myValue)
        {
            if (myValue == null || myValue.Length <= 0)
            {
                return null;
            }
            myValue = myValue.Trim();
            if (myValue.Length > 0)
            {
                return myValue;
            }
            return null;
        }

        /// <summary>
        ///     IsInteger Function checks if a string is a valid int32 value
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>Boolean True if isInteger else False</returns>
        public static bool IsInteger(this string val)
        {
            // Variable to collect the Return value of the TryParse method.

            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            int retNum;

            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            bool isNum = Int32.TryParse(val, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        ///     Read in a sequence of words from standard input and capitalize each
        ///     one (make first letter uppercase; make rest lowercase).
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>Word with capitalization</returns>
        public static string Capitalize(this string s)
        {
            if (s.Length == 0)
            {
                return s;
            }
            return s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower();
        }

        /// <summary>
        ///     Gets first character in string
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>System.string</returns>
        public static string FristCharacter(this string val)
        {
            return (!string.IsNullOrEmpty(val))
                ? (val.Length >= 1)
                    ? val.Substring(0, 1)
                    : val
                : null;
        }

        /// <summary>
        ///     Gets last character in string
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>System.string</returns>
        public static string LastCharacter(this string val)
        {
            return (!string.IsNullOrEmpty(val))
                ? (val.Length >= 1)
                    ? val.Substring(val.Length - 1, 1)
                    : val
                : null;
        }

        /// <summary>
        ///     Check a String ends with another string ignoring the case.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="suffix">suffix</param>
        /// <returns>true or false</returns>
        public static bool EndsWithIgnoreCase(this string str, string suffix)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str", "str parameter is null");
            }
            if (suffix == null)
            {
                throw new ArgumentNullException("suffix", "suffix parameter is null");
            }
            if (str.EndsWith(suffix))
            {
                return true;
            }
            if (str.Length < suffix.Length)
            {
                return false;
            }
            return str.ToLower().EndsWith(suffix.ToLower());
        }

        /// <summary>
        ///     Check a String starts with another string ignoring the case.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="prefix">prefix</param>
        /// <returns>true or false</returns>
        public static bool StartsWithIgnoreCase(this string str, string prefix)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str", "str parameter is null");
            }
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix", "prefix parameter is null");
            }
            if (str.StartsWith(prefix))
            {
                return true;
            }
            if (str.Length < prefix.Length)
            {
                return false;
            }
            return str.ToLower().StartsWith(prefix.ToLower());
        }

        /// <summary>
        ///     Replace specified characters with an empty string.
        /// </summary>
        /// <param name="s">the string</param>
        /// <param name="chars">list of characters to replace from the string</param>
        /// <example>
        ///     string s = "Friends";
        ///     s = s.Replace('F', 'r','i','s');  //s becomes 'end;
        /// </example>
        /// <returns>System.string</returns>
        public static string Replace(this string s, params char[] chars)
        {
            return chars.Aggregate(s, (current, c) => current.Replace(c.ToString(CultureInfo.InvariantCulture), ""));
        }

        /// <summary>
        ///     Validate email address
        /// </summary>
        /// <param name="email">string email address</param>
        /// <returns>true or false if email if valid</returns>
        public static bool IsEmailAddress(this string email)
        {
            string pattern =
                "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$";
            return Regex.Match(email, pattern).Success;
        }

        /// <summary>
        ///     IsNumeric checks if a string is a valid floating value
        /// </summary>
        /// <param name="val"></param>
        /// <returns>Boolean True if isNumeric else False</returns>
        /// <remarks></remarks>
        public static bool IsNumeric(this string val)
        {
            // Variable to collect the Return value of the TryParse method.

            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            double retNum;

            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            bool isNum = Double.TryParse(val, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        ///     Truncate String and append ... at end
        /// </summary>
        /// <param name="s">String to be truncated</param>
        /// <param name="maxLength">number of chars to truncate</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Truncate(this string s, int maxLength)
        {
            if (String.IsNullOrEmpty(s) || maxLength <= 0)
            {
                return String.Empty;
            }
            if (s.Length > maxLength)
            {
                return s.Substring(0, maxLength) + "...";
            }
            return s;
        }


        /// <summary>
        ///     Function returns a default String value if given value is null or empty
        /// </summary>
        /// <param name="myValue">String value to check if isEmpty</param>
        /// <param name="defaultValue">default value to return if String value isEmpty</param>
        /// <returns>returns either String value or default value if IsEmpty</returns>
        /// <remarks></remarks>
        public static string GetDefaultIfEmpty(this string myValue, string defaultValue)
        {
            if (!String.IsNullOrEmpty(myValue))
            {
                myValue = myValue.Trim();
                return myValue.Length > 0 ? myValue : defaultValue;
            }
            return defaultValue;
        }

        /// <summary>
        ///     Convert a string to its equivalent byte array
        /// </summary>
        /// <param name="val">string to convert</param>
        /// <returns>System.byte array</returns>
        public static byte[] ToBytes(this string val)
        {
            var bytes = new byte[val.Length*sizeof (char)];
            Buffer.BlockCopy(val.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        ///     Reverse string
        /// </summary>
        /// <param name="val">string to reverse</param>
        /// <returns>System.string</returns>
        public static string Reverse(this string val)
        {
            var chars = new char[val.Length];
            for (int i = val.Length - 1, j = 0; i >= 0; --i, ++j)
            {
                chars[j] = val[i];
            }
            val = new String(chars);
            return val;
        }


        /// <summary>
        ///     Appends String quotes for type CSV data
        /// </summary>
        /// <param name="val">val</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string ParseStringToCsv(this string val)
        {
            return '"' + GetEmptyStringIfNull(val).Replace("\"", "\"\"") + '"';
        }
    }
}
