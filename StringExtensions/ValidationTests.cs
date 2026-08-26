using System;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringExtensionLibrary;

namespace StringExtensions.Tests
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void IsDateTime_RespectsFormat()
        {
            Assert.IsTrue("26/08/2026 10:00:00".IsDateTime("dd/MM/yyyy HH:mm:ss"));
            Assert.IsFalse("not-a-date".IsDateTime("dd/MM/yyyy"));
            Assert.IsFalse(((string)null).IsDateTime("dd/MM/yyyy"));
            Assert.IsFalse("".IsDateTime("dd/MM/yyyy"));
        }

        [TestMethod]
        public void IsInteger_And_IsNumeric()
        {
            Assert.IsTrue("42".IsInteger());
            Assert.IsFalse("4.2".IsInteger());
            Assert.IsFalse(((string)null).IsInteger());
            Assert.IsTrue("4.2".IsNumeric());
            Assert.IsFalse("x".IsNumeric());
        }

        [TestMethod]
        public void IsAlpha_And_IsAlphaNumeric()
        {
            Assert.IsTrue("Burning bridges as we go".IsAlpha());
            Assert.IsFalse("Burning bridges as we go!".IsAlpha());
            Assert.IsFalse("".IsAlpha());
            Assert.IsFalse(((string)null).IsAlpha());
            Assert.IsTrue("10 minutes left to code".IsAlphaNumeric());
            Assert.IsTrue("123456".IsAlphaNumeric());
            Assert.IsFalse(((string)null).IsAlphaNumeric());
        }

        [TestMethod]
        [DataRow("64.233.161.147", true)]
        [DataRow("64.233.161.1470", false)]
        [DataRow("256.1.1.1", false)]
        [DataRow("1", false)]
        [DataRow("http://127.0.0.1", false)]
        [DataRow("127.00.0.1", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        [DataRow("  127.0.0.1  ", true)]
        public void IsValidIPv4_UsesAddressParse(string value, bool expected)
        {
            Assert.AreEqual(expected, value.IsValidIPv4());
        }

        [TestMethod]
        public void IsValidIPv4_RejectsIPv6()
        {
            Assert.IsFalse("::1".IsValidIPv4());
            Assert.AreEqual(AddressFamilyOf("127.0.0.1"), System.Net.Sockets.AddressFamily.InterNetwork);
        }

        private static System.Net.Sockets.AddressFamily AddressFamilyOf(string ip)
        {
            return IPAddress.Parse(ip).AddressFamily;
        }

        [TestMethod]
        [DataRow("user@example.com", true)]
        [DataRow("user+tag@example.com", true)]
        [DataRow(" user@example.com ", true)]
        [DataRow("not-an-email", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        [DataRow("  ", false)]
        public void IsEmailAddress_NullSafeAndPlusTags(string email, bool expected)
        {
            Assert.AreEqual(expected, email.IsEmailAddress());
        }

        [TestMethod]
        public void LengthChecks_IncludeIsLengthMax()
        {
            const string sample = "There is currently no easy way to update all packages within a solution";
            Assert.IsTrue(sample.IsMinLength(2));
            Assert.IsFalse("The running".IsMinLength(50));
            Assert.IsFalse(((string)null).IsMinLength(1));
            Assert.IsTrue("One".IsMaxLength(3));
            Assert.IsFalse("three".IsMaxLength(3));
            Assert.IsTrue("abcd".IsLength(2, 6));
            Assert.IsFalse("abcd".IsLength(5, 6));
            Assert.IsFalse(((string)null).IsLength(0, 1));
            Assert.AreEqual(4, "abcd".GetLength());
            Assert.IsNull(((string)null).GetLength());
        }

        [TestMethod]
        public void NullHelpers()
        {
            Assert.IsTrue(((string)null).IsNull());
            Assert.IsFalse("x".IsNull());
            Assert.IsTrue(((string)null).IsNullOrEmpty());
            Assert.IsTrue("".IsNullOrEmpty());
            Assert.IsFalse("x".IsNullOrEmpty());
        }

        [TestMethod]
        public void StartsAndEndsWithIgnoreCase()
        {
            Assert.IsTrue("Hello".StartsWithIgnoreCase("he"));
            Assert.IsFalse("Hello".StartsWithIgnoreCase("x"));
            Assert.IsTrue("Hello".EndsWithIgnoreCase("LO"));
            Assert.ThrowsExactly<ArgumentNullException>(() => ((string)null).StartsWithIgnoreCase("a"));
            Assert.ThrowsExactly<ArgumentNullException>(() => "a".EndsWithIgnoreCase(null));
        }

        [TestMethod]
        public void DoesNotStartOrEndWith_NullIsTrue()
        {
            Assert.IsTrue("test".DoesNotStartWith("a"));
            Assert.IsFalse("test".DoesNotStartWith("t"));
            Assert.IsTrue("".DoesNotStartWith("t"));
            Assert.IsTrue(((string)null).DoesNotStartWith("t"));
            Assert.IsTrue("test".DoesNotEndWith("a"));
            Assert.IsFalse("test".DoesNotEndWith("t"));
            Assert.IsTrue(((string)null).DoesNotEndWith("t"));
        }
    }
}
