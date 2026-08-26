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
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StringExtensionLibrary
{
    /// <summary>
    ///     Provides extension methods to the <see cref="string">System.string</see> object.
    /// </summary>
    public static class StringExtensions
    {
        private const int AesSaltSize = 16;
        private const int AesIvSize = 16;
        private const int AesKeySizeBytes = 32;
        private const int AesHmacKeySizeBytes = 32;
        private const int AesHmacTagSize = 32;
        private const int AesKeyIterations = 100000;
        private static readonly TimeSpan RegexMatchTimeout = TimeSpan.FromMilliseconds(250);

        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9][\w\.\+\-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.\-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$",
            RegexOptions.CultureInvariant | RegexOptions.Compiled,
            RegexMatchTimeout);

        private static readonly Regex LineFeedRegex = new Regex(
            @"[\r\n]+",
            RegexOptions.CultureInvariant | RegexOptions.Compiled,
            RegexMatchTimeout);

        private static readonly char[] HexByteSeparator = { '-' };
        private static readonly char[] QueryEqualsSeparator = { '=' };

        private static byte[] DeriveBytesFromPassphrase(string password, byte[] salt, int byteCount)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            try
            {
                return Pbkdf2HmacSha256(passwordBytes, salt, AesKeyIterations, byteCount);
            }
            finally
            {
                Array.Clear(passwordBytes, 0, passwordBytes.Length);
            }
        }

        internal static byte[] Pbkdf2HmacSha256(byte[] password, byte[] salt, int iterations, int byteCount)
        {
            using (var hmac = new HMACSHA256(password))
            {
                int hashLength = hmac.HashSize / 8;
                int blockCount = (byteCount + hashLength - 1) / hashLength;
                var derived = new byte[byteCount];
                var saltAndBlock = new byte[salt.Length + 4];
                Buffer.BlockCopy(salt, 0, saltAndBlock, 0, salt.Length);

                int offset = 0;
                for (int block = 1; block <= blockCount; block++)
                {
                    saltAndBlock[salt.Length] = (byte)(block >> 24);
                    saltAndBlock[salt.Length + 1] = (byte)(block >> 16);
                    saltAndBlock[salt.Length + 2] = (byte)(block >> 8);
                    saltAndBlock[salt.Length + 3] = (byte)block;

                    byte[] u = hmac.ComputeHash(saltAndBlock);
                    var t = (byte[])u.Clone();
                    for (int i = 1; i < iterations; i++)
                    {
                        u = hmac.ComputeHash(u);
                        for (int j = 0; j < t.Length; j++)
                        {
                            t[j] ^= u[j];
                        }
                    }

                    int toCopy = Math.Min(hashLength, byteCount - offset);
                    Buffer.BlockCopy(t, 0, derived, offset, toCopy);
                    offset += toCopy;
                }

                return derived;
            }
        }

        /// <summary>
        ///     Checks if date with dateFormat is parse-able to System.DateTime format returns boolean value if true else false
        /// </summary>
        /// <param name="data">String date</param>
        /// <param name="dateFormat">date format example dd/MM/yyyy HH:mm:ss</param>
        /// <returns>boolean True False if is valid System.DateTime</returns>
        public static bool IsDateTime(this string? data, string dateFormat)
        {
            // ReSharper disable once RedundantAssignment
            DateTime dateVal = default(DateTime);
            return DateTime.TryParseExact(data, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out dateVal);
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
        public static int ToInt32(this string? value)
        {
            int number;
            if (!int.TryParse(value, out number))
            {
                return 0;
            }
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
        public static long ToInt64(this string? value)
        {
            long number;
            if (!long.TryParse(value, out number))
            {
                return 0;
            }
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
        public static short ToInt16(this string? value)
        {
            short number;
            if (!short.TryParse(value, out number))
            {
                return 0;
            }
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
        public static decimal ToDecimal(this string? value)
        {
            decimal number;
            if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out number))
            {
                return 0;
            }
            return number;
        }

        /// <summary>
        ///     Converts string to its boolean equivalent
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>boolean equivalent</returns>
        /// <remarks>
        ///     <exception cref="ArgumentException">
        ///         thrown in the event no boolean equivalent found or an empty or whitespace
        ///         string is passed
        ///     </exception>
        /// </remarks>
        public static bool ToBoolean(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value is null, empty, or whitespace.", nameof(value));
            }
            string val = value.ToLowerInvariant().Trim();
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
                    throw new ArgumentException("Value is not a recognized boolean.", nameof(value));
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
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            return str.Split(separator, StringSplitOptions.None).Select(s => (T) Convert.ChangeType(s, typeof (T), CultureInfo.InvariantCulture));
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
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            return str.Split(separator, options).Select(s => (T) Convert.ChangeType(s, typeof (T), CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Converts string to its Enum type
        ///     Checks of string is a member of type T enum before converting
        ///     if fails returns default enum
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="value"> The string representation of the enumeration name or underlying value to convert</param>
        /// <param name="defaultValue"></param>
        /// <returns>Enum object</returns>
        /// <remarks>
        ///     <exception cref="ArgumentException">
        ///         enumType is not an System.Enum.-or- value is either an empty string ("") or
        ///         only contains white space.-or- value is a name, but not one of the named constants defined for the enumeration
        ///     </exception>
        /// </remarks>
        public static T ToEnum<T>(this string value, T defaultValue = default(T)) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Type T must be an enum.", nameof(T));
            }

            T result;
            bool isParsed = Enum.TryParse(value, true, out result);
            return isParsed ? result : defaultValue;
        }

        /// <summary>
        ///     Replaces one or more format items in a specified string with the string representation of a specified object.
        ///     Named <c>Format</c> as an extension; call as <c>"{0}".Format(arg)</c> to avoid ambiguity with <see cref="string.Format(string,object)" />.
        /// </summary>
        /// <param name="value">A composite format string</param>
        /// <param name="arg0">An System.Object to format</param>
        /// <returns>A copy of format in which any format items are replaced by the string representation of arg0</returns>
        /// <exception cref="ArgumentNullException">format or args is null.</exception>
        /// <exception cref="System.FormatException">
        ///     format is invalid.-or- The index of a format item is less than zero, or
        ///     greater than or equal to the length of the args array.
        /// </exception>
        public static string Format(this string value, object arg0)
        {
            return string.Format(CultureInfo.InvariantCulture, value, arg0);
        }

        /// <summary>
        ///     Replaces the format item in a specified string with the string representation of a corresponding object in a
        ///     specified array.
        /// </summary>
        /// <param name="value">A composite format string</param>
        /// <param name="args">An object array that contains zero or more objects to format</param>
        /// <returns>
        ///     A copy of format in which the format items have been replaced by the string representation of the
        ///     corresponding objects in args
        /// </returns>
        /// <exception cref="ArgumentNullException">format or args is null.</exception>
        /// <exception cref="System.FormatException">
        ///     format is invalid.-or- The index of a format item is less than zero, or
        ///     greater than or equal to the length of the args array.
        /// </exception>
        public static string Format(this string value, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, value, args);
        }

        /// <summary>
        ///     Gets empty String if passed value is of type Null/Nothing
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>System.String</returns>
        /// <remarks></remarks>
        public static string GetEmptyStringIfNull(this string? val)
        {
            return (val != null ? val.Trim() : "");
        }

        /// <summary>
        ///     Checks if a string is null and returns String if not Empty else returns null/Nothing
        /// </summary>
        /// <param name="myValue">String value</param>
        /// <returns>null/nothing if String IsEmpty</returns>
        /// <remarks></remarks>
        public static string? GetNullIfEmptyString(this string? myValue)
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
        public static bool IsInteger(this string? val)
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
        ///     Capitalizes the first character and lowercases the remainder. Null and empty strings are returned unchanged.
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>Word with capitalization</returns>
        public static string? Capitalize(this string? s)
        {
            if (s is null || s.Length == 0)
            {
                return s;
            }
            return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1).ToLowerInvariant();
        }

        /// <summary>
        ///     Gets first character in string
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>System.string</returns>
        public static string? FirstCharacter(this string? val)
        {
            if (val is null || val.Length == 0)
            {
                return null;
            }
            return val.Substring(0, 1);
        }

        /// <summary>
        ///     Gets last character in string
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>System.string</returns>
        public static string? LastCharacter(this string? val)
        {
            if (val is null || val.Length == 0)
            {
                return null;
            }
            return val.Substring(val.Length - 1, 1);
        }

        /// <summary>
        ///     Check a String ends with another string ignoring the case.
        /// </summary>
        /// <param name="val">string</param>
        /// <param name="suffix">suffix</param>
        /// <returns>true or false</returns>
        public static bool EndsWithIgnoreCase(this string val, string suffix)
        {
            if (val == null)
            {
                throw new ArgumentNullException(nameof(val));
            }
            if (suffix == null)
            {
                throw new ArgumentNullException(nameof(suffix));
            }
            if (val.Length < suffix.Length)
            {
                return false;
            }
            return val.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Check a String starts with another string ignoring the case.
        /// </summary>
        /// <param name="val">string</param>
        /// <param name="prefix">prefix</param>
        /// <returns>true or false</returns>
        public static bool StartsWithIgnoreCase(this string val, string prefix)
        {
            if (val == null)
            {
                throw new ArgumentNullException(nameof(val));
            }
            if (prefix == null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }
            if (val.Length < prefix.Length)
            {
                return false;
            }
            return val.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Replace specified characters with an empty string. This overload is distinct from
        ///     <see cref="string.Replace(char,char)" />: it deletes each listed character rather than substituting a replacement.
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
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }
            if (chars == null)
            {
                throw new ArgumentNullException(nameof(chars));
            }
            return chars.Aggregate(s, (current, c) => current.Replace(c.ToString(CultureInfo.InvariantCulture), ""));
        }

        /// <summary>
        ///     Remove Characters from string
        /// </summary>
        /// <param name="s">string to remove characters</param>
        /// <param name="chars">array of chars</param>
        /// <returns>System.string</returns>
        public static string RemoveChars(this string s, params char[] chars)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }
            if (chars == null)
            {
                throw new ArgumentNullException(nameof(chars));
            }
            var sb = new StringBuilder(s.Length);
            foreach (char c in s.Where(c => !chars.Contains(c)))
            {
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Practical email check (allows plus-tags). Not RFC 5322-complete; do not use as the sole
        ///     gate for security-sensitive identity.
        /// </summary>
        /// <param name="email">string email address</param>
        /// <returns>true or false if email if valid</returns>
        public static bool IsEmailAddress(this string? email)
        {
            if (email is null)
            {
                return false;
            }

            string candidate = email.Trim();
            if (candidate.Length == 0)
            {
                return false;
            }

            try
            {
                return EmailRegex.IsMatch(candidate);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        ///     IsNumeric checks if a string is a valid floating value
        /// </summary>
        /// <param name="val"></param>
        /// <returns>Boolean True if isNumeric else False</returns>
        /// <remarks></remarks>
        public static bool IsNumeric(this string? val)
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
        public static string Truncate(this string? s, int maxLength)
        {
            if (s is null || s.Length == 0 || maxLength <= 0)
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
        public static string GetDefaultIfEmpty(this string? myValue, string defaultValue)
        {
            if (myValue is null || myValue.Length == 0)
            {
                return defaultValue;
            }

            myValue = myValue.Trim();
            return myValue.Length > 0 ? myValue : defaultValue;
        }

        /// <summary>
        ///     Convert a string to its equivalent UTF-16 little-endian byte array (each char is two bytes via
        ///     <see cref="Buffer.BlockCopy(Array,int,Array,int,int)" />). This is not UTF-8. Hash helpers use the same encoding.
        /// </summary>
        /// <param name="val">string to convert</param>
        /// <returns>System.byte array</returns>
        public static byte[] ToBytes(this string val)
        {
            if (val == null)
            {
                throw new ArgumentNullException(nameof(val));
            }
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
            if (val == null)
            {
                throw new ArgumentNullException(nameof(val));
            }
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
        public static string ParseStringToCsv(this string? val)
        {
            return '"' + GetEmptyStringIfNull(val).Replace("\"", "\"\"") + '"';
        }

        /// <summary>
        ///     Encrypt a string using the supplied passphrase. Encoding uses AES-256-CBC with HMAC-SHA256
        ///     integrity and a PBKDF2-HMAC-SHA256-derived key (100,000 iterations).
        ///     The result is a hyphen-separated hex string (salt + IV + ciphertext + tag).
        ///     Passphrase-based authenticated encryption for application data; not a key-management system.
        /// </summary>
        /// <param name="stringToEncrypt">String that must be encrypted.</param>
        /// <param name="key">Passphrase used to derive the encryption key.</param>
        /// <returns>A hyphen-separated hex string representing salt, IV, ciphertext, and HMAC tag.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToEncrypt or key is null or empty.</exception>
        public static string Encrypt(this string stringToEncrypt, string key)
        {
            if (string.IsNullOrEmpty(stringToEncrypt))
            {
                throw new ArgumentException("Value is null or empty.", nameof(stringToEncrypt));
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Value is null or empty.", nameof(key));
            }

            var salt = new byte[AesSaltSize];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] derivedBytes = DeriveBytesFromPassphrase(key, salt, AesKeySizeBytes + AesHmacKeySizeBytes);
            var aesKey = new byte[AesKeySizeBytes];
            var hmacKey = new byte[AesHmacKeySizeBytes];
            try
            {
                Buffer.BlockCopy(derivedBytes, 0, aesKey, 0, aesKey.Length);
                Buffer.BlockCopy(derivedBytes, aesKey.Length, hmacKey, 0, hmacKey.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = aesKey;
                    aes.GenerateIV();

                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(stringToEncrypt);
                        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        var payload = new byte[salt.Length + aes.IV.Length + cipherBytes.Length];
                        Buffer.BlockCopy(salt, 0, payload, 0, salt.Length);
                        Buffer.BlockCopy(aes.IV, 0, payload, salt.Length, aes.IV.Length);
                        Buffer.BlockCopy(cipherBytes, 0, payload, salt.Length + aes.IV.Length, cipherBytes.Length);

                        byte[] tag;
                        using (var hmac = new HMACSHA256(hmacKey))
                        {
                            tag = hmac.ComputeHash(payload);
                        }

                        var result = new byte[payload.Length + tag.Length];
                        Buffer.BlockCopy(payload, 0, result, 0, payload.Length);
                        Buffer.BlockCopy(tag, 0, result, payload.Length, tag.Length);
                        return BitConverter.ToString(result);
                    }
                }
            }
            finally
            {
                Array.Clear(derivedBytes, 0, derivedBytes.Length);
                Array.Clear(aesKey, 0, aesKey.Length);
                Array.Clear(hmacKey, 0, hmacKey.Length);
            }
        }

        /// <summary>
        ///     Decrypt a string using the supplied passphrase. Decoding uses AES-256-CBC with HMAC-SHA256
        ///     and a PBKDF2-HMAC-SHA256-derived key.
        /// </summary>
        /// <param name="stringToDecrypt">Hyphen-separated hex string produced by <see cref="Encrypt" />.</param>
        /// <param name="key">Passphrase used to derive the decryption key.</param>
        /// <returns>The decrypted string.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToDecrypt or key is null or empty.</exception>
        /// <exception cref="CryptographicException">Occurs when the payload is invalid or the key is wrong.</exception>
        public static string Decrypt(this string stringToDecrypt, string key)
        {
            if (string.IsNullOrEmpty(stringToDecrypt))
            {
                throw new ArgumentException("Value is null or empty.", nameof(stringToDecrypt));
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Value is null or empty.", nameof(key));
            }

            byte[] fullBytes;
            try
            {
                fullBytes = Array.ConvertAll(stringToDecrypt.Split(HexByteSeparator, StringSplitOptions.None),
                    hex => byte.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException || ex is ArgumentException)
            {
                throw new CryptographicException("Invalid encrypted string.", ex);
            }
            int minLength = AesSaltSize + AesIvSize + AesHmacTagSize;
            if (fullBytes.Length < minLength)
            {
                throw new CryptographicException("Invalid encrypted string.");
            }

            var payload = new byte[fullBytes.Length - AesHmacTagSize];
            var tag = new byte[AesHmacTagSize];
            Buffer.BlockCopy(fullBytes, 0, payload, 0, payload.Length);
            Buffer.BlockCopy(fullBytes, payload.Length, tag, 0, tag.Length);

            var salt = new byte[AesSaltSize];
            Buffer.BlockCopy(payload, 0, salt, 0, salt.Length);

            byte[] derivedBytes = DeriveBytesFromPassphrase(key, salt, AesKeySizeBytes + AesHmacKeySizeBytes);
            var aesKey = new byte[AesKeySizeBytes];
            var hmacKey = new byte[AesHmacKeySizeBytes];
            try
            {
                Buffer.BlockCopy(derivedBytes, 0, aesKey, 0, aesKey.Length);
                Buffer.BlockCopy(derivedBytes, aesKey.Length, hmacKey, 0, hmacKey.Length);

                byte[] expectedTag;
                using (var hmac = new HMACSHA256(hmacKey))
                {
                    expectedTag = hmac.ComputeHash(payload);
                }

                if (!FixedTimeEquals(tag, expectedTag))
                {
                    throw new CryptographicException("Invalid encrypted string or key.");
                }

                var iv = new byte[AesIvSize];
                var cipherBytes = new byte[payload.Length - AesSaltSize - AesIvSize];
                Buffer.BlockCopy(payload, salt.Length, iv, 0, iv.Length);
                Buffer.BlockCopy(payload, salt.Length + iv.Length, cipherBytes, 0, cipherBytes.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = aesKey;
                    aes.IV = iv;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    {
                        byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return Encoding.UTF8.GetString(plainBytes);
                    }
                }
            }
            finally
            {
                Array.Clear(derivedBytes, 0, derivedBytes.Length);
                Array.Clear(aesKey, 0, aesKey.Length);
                Array.Clear(hmacKey, 0, hmacKey.Length);
            }
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        /// <summary>
        ///     Count number of occurrences in string
        /// </summary>
        /// <param name="val">string containing text</param>
        /// <param name="stringToMatch">string or pattern find</param>
        /// <returns></returns>
        public static int CountOccurrences(this string? val, string? stringToMatch)
        {
            if (string.IsNullOrEmpty(val) || string.IsNullOrEmpty(stringToMatch))
            {
                return 0;
            }

            return Regex.Matches(val, Regex.Escape(stringToMatch), RegexOptions.IgnoreCase, RegexMatchTimeout).Count;
        }

        /// <summary>
        ///     Converts a Json string to dictionary object method applicable for single hierarchy objects i.e
        ///     no parent child relationships, for parent child relationships <see cref="JsonToExpanderObject" />
        /// </summary>
        /// <param name="val">string formated as Json</param>
        /// <returns>IDictionary Json object</returns>
        /// <remarks>
        ///     <exception cref="ArgumentNullException">if string parameter is null or empty</exception>
        /// </remarks>
        public static IDictionary<string, object> JsonToDictionary(this string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException(nameof(val));
            }
            Dictionary<string, object>? result = JsonConvert.DeserializeObject<Dictionary<string, object>>(val);
            if (result == null)
            {
                throw new InvalidOperationException("JSON did not deserialize to a dictionary.");
            }
            return result;
        }

        /// <summary>
        ///     Converts a Json string to ExpandoObject method applicable for multi hierarchy objects i.e
        ///     having zero or many parent child relationships
        /// </summary>
        /// <param name="json">string formated as Json</param>
        /// <returns>System.Dynamic.ExpandoObject Json object<see cref="ExpandoObject" />ExpandoObject</returns>
        public static ExpandoObject JsonToExpanderObject(this string json)
        {
            var converter = new ExpandoObjectConverter();
            return JsonConvert.DeserializeObject<ExpandoObject>(json, converter)
                   ?? throw new InvalidOperationException("JSON did not deserialize to an ExpandoObject.");
        }

        /// <summary>
        ///     Converts a Json string to object of type T method applicable for multi hierarchy objects i.e
        ///     having zero or many parent child relationships, Ignore loop references and do not serialize if cycles are detected.
        /// </summary>
        /// <typeparam name="T">object to convert to</typeparam>
        /// <param name="json">json</param>
        /// <returns>object</returns>
        public static T JsonToObject<T>(this string json)
        {
            var settings = new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
            return JsonConvert.DeserializeObject<T>(json, settings)
                   ?? throw new InvalidOperationException("JSON did not deserialize to the requested type.");
        }

        /// <summary>
        ///     Removes the first part of the string, if no match found return original string
        /// </summary>
        /// <param name="val">string to remove prefix</param>
        /// <param name="prefix">prefix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns>trimmed string with no prefix or original string</returns>
        public static string? RemovePrefix(this string? val, string prefix, bool ignoreCase = true)
        {
            if (val is null || val.Length == 0)
            {
                return val;
            }
            if (ignoreCase ? val.StartsWithIgnoreCase(prefix) : val.StartsWith(prefix, StringComparison.Ordinal))
            {
                return val.Substring(prefix.Length, val.Length - prefix.Length);
            }
            return val;
        }

        /// <summary>
        ///     Removes the end part of the string, if no match found return original string
        /// </summary>
        /// <param name="val">string to remove suffix</param>
        /// <param name="suffix">suffix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns>trimmed string with no suffix or original string</returns>
        public static string? RemoveSuffix(this string? val, string suffix, bool ignoreCase = true)
        {
            if (val is null || val.Length == 0)
            {
                return val;
            }
            if (ignoreCase ? val.EndsWithIgnoreCase(suffix) : val.EndsWith(suffix, StringComparison.Ordinal))
            {
                return val.Substring(0, val.Length - suffix.Length);
            }
            return val;
        }

        /// <summary>
        ///     Appends the suffix to the end of the string if the string does not already end in the suffix.
        /// </summary>
        /// <param name="val">string to append suffix</param>
        /// <param name="suffix">suffix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns>The original string, or the string with suffix appended.</returns>
        public static string? AppendSuffixIfMissing(this string? val, string suffix, bool ignoreCase = true)
        {
            if (val is null || val.Length == 0)
            {
                return val;
            }
            if (ignoreCase ? val.EndsWithIgnoreCase(suffix) : val.EndsWith(suffix, StringComparison.Ordinal))
            {
                return val;
            }
            return val + suffix;
        }

        /// <summary>
        ///     Appends the prefix to the start of the string if the string does not already start with prefix.
        /// </summary>
        /// <param name="val">string to append prefix</param>
        /// <param name="prefix">prefix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns></returns>
        public static string? AppendPrefixIfMissing(this string? val, string prefix, bool ignoreCase = true)
        {
            if (val is null || val.Length == 0)
            {
                return val;
            }
            if (ignoreCase ? val.StartsWithIgnoreCase(prefix) : val.StartsWith(prefix, StringComparison.Ordinal))
            {
                return val;
            }
            return prefix + val;
        }

        /// <summary>
        ///     Checks if the String contains only Unicode letters.
        ///     null will return false. An empty String ("") will return false.
        /// </summary>
        /// <param name="val">string to check if is Alpha</param>
        /// <returns>true if only contains letters, and is non-null</returns>
        public static bool IsAlpha(this string? val)
        {
            if (val is null || val.Length == 0)
            {
                return false;
            }
            return val.Trim().Replace(" ", "").All(Char.IsLetter);
        }

        /// <summary>
        ///     Checks if the String contains only Unicode letters, digits.
        ///     null will return false. An empty String ("") will return false.
        /// </summary>
        /// <param name="val">string to check if is Alpha or Numeric</param>
        /// <returns></returns>
        public static bool IsAlphaNumeric(this string? val)
        {
            if (val is null || val.Length == 0)
            {
                return false;
            }
            return val.Trim().Replace(" ", "").All(Char.IsLetterOrDigit);
        }

        /// <summary>
        ///     Convert string to Hash using Sha512
        /// </summary>
        /// <param name="val">string to hash</param>
        /// <returns>Hashed string</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string CreateHashSha512(this string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentException("Value is null or empty.", nameof(val));
            }
            var sb = new StringBuilder();
            using (SHA512 hash = SHA512.Create())
            {
                byte[] data = hash.ComputeHash(val.ToBytes());
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Convert string to Hash using Sha256
        /// </summary>
        /// <param name="val">string to hash</param>
        /// <returns>Hashed string</returns>
        public static string CreateHashSha256(this string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentException("Value is null or empty.", nameof(val));
            }
            var sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                byte[] data = hash.ComputeHash(val.ToBytes());
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Convert url query string to IDictionary value key pair. Keys and values are URL-decoded
        ///     (<see cref="Uri.UnescapeDataString" />; <c>+</c> is treated as a space).
        /// </summary>
        /// <param name="queryString">query string value</param>
        /// <returns>IDictionary value key pair</returns>
        public static IDictionary<string, string>? QueryStringToDictionary(this string? queryString)
        {
            if (queryString is null || queryString.Trim().Length == 0)
            {
                return null;
            }

            int queryStart = queryString.IndexOf('?');
            if (queryStart < 0 || queryStart == queryString.Length - 1)
            {
                return null;
            }

            string query = queryString.Substring(queryStart + 1);
            int fragment = query.IndexOf('#');
            if (fragment >= 0)
            {
                query = query.Substring(0, fragment);
            }

            if (query.IndexOf('=') < 0)
            {
                return null;
            }

            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (string pair in query.Split('&'))
            {
                string[] parts = pair.Split(QueryEqualsSeparator, 2);
                if (parts.Length != 2 || string.IsNullOrEmpty(parts[0]))
                {
                    continue;
                }

                string key = DecodeQueryComponent(parts[0]).ToLowerInvariant().Trim();
                if (key.Length == 0)
                {
                    continue;
                }

                result[key] = DecodeQueryComponent(parts[1]);
            }

            return result;
        }

        private static string DecodeQueryComponent(string value)
        {
            try
            {
                return Uri.UnescapeDataString(value.Replace("+", " "));
            }
            catch (UriFormatException)
            {
                return value.Replace("+", " ");
            }
        }

        /// <summary>
        ///     Reverse back or forward slashes
        /// </summary>
        /// <param name="val">string</param>
        /// <param name="direction">
        ///     0 - replace forward slash with back
        ///     1 - replace back with forward slash
        /// </param>
        /// <returns></returns>
        public static string ReverseSlash(this string val, int direction)
        {
            if (val == null)
            {
                throw new ArgumentNullException(nameof(val));
            }
            switch (direction)
            {
                case 0:
                    return val.Replace(@"/", @"\");
                case 1:
                    return val.Replace(@"\", @"/");
                default:
                    return val;
            }
        }

        /// <summary>
        ///     Replace CR/LF sequences with an empty string. Periods and other characters are left unchanged.
        /// </summary>
        /// <param name="val">string to remove line feeds</param>
        /// <returns>System.string</returns>
        public static string ReplaceLineFeeds(this string val)
        {
            if (val == null)
            {
                throw new ArgumentNullException(nameof(val));
            }
            return LineFeedRegex.Replace(val, "");
        }

        /// <summary>
        ///     Validates if a string is a dotted-quad IPv4 address.
        /// </summary>
        /// <param name="val">string IP address</param>
        /// <returns>true if string matches valid IP address else false</returns>
        public static bool IsValidIPv4(this string? val)
        {
            if (val is null)
            {
                return false;
            }

            string trimmed = val.Trim();
            if (trimmed.Length == 0)
            {
                return false;
            }

            string[] octets = trimmed.Split('.');
            if (octets.Length != 4)
            {
                return false;
            }

            return IPAddress.TryParse(trimmed, out IPAddress? address)
                   && address != null
                   && address.AddressFamily == AddressFamily.InterNetwork
                   && string.Equals(address.ToString(), trimmed, StringComparison.Ordinal);
        }

        /// <summary>
        ///     Calculates the amount of bytes occupied by the input string encoded as the encoding specified
        /// </summary>
        /// <param name="val">The input string to check</param>
        /// <param name="encoding">The encoding to use</param>
        /// <returns>The total size of the input string in bytes</returns>
        /// <exception cref="System.ArgumentNullException">input is null</exception>
        /// <exception cref="System.ArgumentNullException">encoding is null</exception>
        public static int GetByteSize(this string val, Encoding encoding)
        {
            if (val == null)
            {
                throw new ArgumentNullException(nameof(val));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }
            return encoding.GetByteCount(val);
        }

        /// <summary>
        ///     Extracts the left part of the input string limited with the length parameter
        /// </summary>
        /// <param name="val">The input string to take the left part from</param>
        /// <param name="length">The total number characters to take from the input string</param>
        /// <returns>The substring starting at startIndex 0 until length</returns>
        /// <exception cref="System.ArgumentNullException">input is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Length is smaller than zero or higher than the length of input</exception>
        public static string Left(this string val, int length)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException(nameof(val));
            }
            if (length < 0 || length > val.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    "length cannot be higher than total string length or less than 0");
            }
            return val.Substring(0, length);
        }

        /// <summary>
        ///     Extracts the right part of the input string limited with the length parameter
        /// </summary>
        /// <param name="val">The input string to take the right part from</param>
        /// <param name="length">The total number characters to take from the input string</param>
        /// <returns>The substring taken from the input string</returns>
        /// <exception cref="System.ArgumentNullException">input is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Length is smaller than zero or higher than the length of input</exception>
        public static string Right(this string val, int length)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException(nameof(val));
            }
            if (length < 0 || length > val.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    "length cannot be higher than total string length or less than 0");
            }
            return val.Substring(val.Length - length);
        }

        /// <summary>
        ///     ToTextElements
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static IEnumerable<string> ToTextElements(this string val)
        {
            if (val == null)
            {
                throw new ArgumentNullException(nameof(val));
            }
            TextElementEnumerator elementEnumerator = StringInfo.GetTextElementEnumerator(val);
            while (elementEnumerator.MoveNext())
            {
                string textElement = elementEnumerator.GetTextElement();
                yield return textElement;
            }
        }

        /// <summary>
        ///     Check if a string does not start with prefix
        /// </summary>
        /// <param name="val">string to evaluate</param>
        /// <param name="prefix">prefix</param>
        /// <returns>true if string does not match prefix else false; null <paramref name="val"/> or <paramref name="prefix"/> evaluates to true.</returns>
        public static bool DoesNotStartWith(this string? val, string? prefix)
        {
            return val == null || prefix == null ||
                   !val.StartsWith(prefix, StringComparison.InvariantCulture);
        }

        /// <summary>
        ///     Check if a string does not end with prefix
        /// </summary>
        /// <param name="val">string to evaluate</param>
        /// <param name="suffix">suffix</param>
        /// <returns>true if string does not match suffix else false; null <paramref name="val"/> or <paramref name="suffix"/> evaluates to true.</returns>
        public static bool DoesNotEndWith(this string? val, string? suffix)
        {
            return val == null || suffix == null ||
                   !val.EndsWith(suffix, StringComparison.InvariantCulture);
        }

        /// <summary>
        ///     Checks if a string is null
        /// </summary>
        /// <param name="val">string to evaluate</param>
        /// <returns>true if string is null else false</returns>
        public static bool IsNull(this string? val)
        {
            return val == null;
        }

        /// <summary>
        ///     Checks if a string is null or empty. Instance-style wrapper around
        ///     <see cref="string.IsNullOrEmpty" />; prefer the static method in new code if both are in scope.
        /// </summary>
        /// <param name="val">string to evaluate</param>
        /// <returns>true if string is null or is empty else false</returns>
        public static bool IsNullOrEmpty(this string? val)
        {
            return String.IsNullOrEmpty(val);
        }

        /// <summary>
        ///     Checks if string length is a certain minimum number of characters, does not ignore leading and trailing
        ///     white-space.
        ///     null strings will always evaluate to false.
        /// </summary>
        /// <param name="val">string to evaluate minimum length</param>
        /// <param name="minCharLength">minimum allowable string length</param>
        /// <returns>true if string is of specified minimum length</returns>
        public static bool IsMinLength(this string? val, int minCharLength)
        {
            return val != null && val.Length >= minCharLength;
        }

        /// <summary>
        ///     Checks if string length is consists of specified allowable maximum char length. does not ignore leading and
        ///     trailing white-space.
        ///     null strings will always evaluate to false.
        /// </summary>
        /// <param name="val">string to evaluate maximum length</param>
        /// <param name="maxCharLength">maximum allowable string length</param>
        /// <returns>true if string has specified maximum char length</returns>
        public static bool IsMaxLength(this string? val, int maxCharLength)
        {
            return val != null && val.Length <= maxCharLength;
        }

        /// <summary>
        ///     Checks if string length satisfies minimum and maximum allowable char length. does not ignore leading and
        ///     trailing white-space
        /// </summary>
        /// <param name="val">string to evaluate</param>
        /// <param name="minCharLength">minimum char length</param>
        /// <param name="maxCharLength">maximum char length</param>
        /// <returns>true if string satisfies minimum and maximum allowable length</returns>
        public static bool IsLength(this string? val, int minCharLength, int maxCharLength)
        {
            return val != null && val.Length >= minCharLength && val.Length <= maxCharLength;
        }

        /// <summary>
        ///     Gets the number of characters in string checks if string is null
        /// </summary>
        /// <param name="val">string to evaluate length</param>
        /// <returns>total number of chars or null if string is null</returns>
        public static int? GetLength(this string? val)
        {
            return val == null ? (int?) null : val.Length;
        }
    }
}
