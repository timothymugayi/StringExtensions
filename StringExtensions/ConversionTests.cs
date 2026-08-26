using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringExtensionLibrary;

namespace StringExtensions.Tests
{
    [TestClass]
    public class ConversionTests
    {
        [TestMethod]
        public void ToInt32_ParsesValidAndInvalid()
        {
            Assert.AreEqual(42, "42".ToInt32());
            Assert.AreEqual(0, "nope".ToInt32());
            Assert.AreEqual(0, ((string)null).ToInt32());
            Assert.AreEqual(0, "".ToInt32());
        }

        [TestMethod]
        public void ToInt16_And_ToInt64_Parse()
        {
            Assert.AreEqual((short)12, "12".ToInt16());
            Assert.AreEqual(0, "x".ToInt16());
            Assert.AreEqual(9L, "9".ToInt64());
            Assert.AreEqual(0L, ((string)null).ToInt64());
        }

        [TestMethod]
        public void ToDecimal_Parses()
        {
            Assert.AreEqual(1.5m, "1.5".ToDecimal());
            Assert.AreEqual(0m, "abc".ToDecimal());
        }

        [TestMethod]
        [DataRow("true", true)]
        [DataRow("T", true)]
        [DataRow("yes", true)]
        [DataRow("Y", true)]
        [DataRow("false", false)]
        [DataRow("f", false)]
        [DataRow("no", false)]
        [DataRow("N", false)]
        public void ToBoolean_RecognizedValues(string input, bool expected)
        {
            Assert.AreEqual(expected, input.ToBoolean());
        }

        [TestMethod]
        public void ToBoolean_RejectsEmptyAndUnknown()
        {
            Assert.ThrowsExactly<ArgumentException>(() => "".ToBoolean());
            Assert.ThrowsExactly<ArgumentException>(() => "  ".ToBoolean());
            Assert.ThrowsExactly<ArgumentException>(() => ((string)null).ToBoolean());
            Assert.ThrowsExactly<ArgumentException>(() => "maybe".ToBoolean());
        }

        [TestMethod]
        public void SplitTo_SplitsAndConverts()
        {
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, "1,2,3".SplitTo<int>(',').ToArray());
            CollectionAssert.AreEqual(new[] { "a", "b" }, "a,,b".SplitTo<string>(StringSplitOptions.RemoveEmptyEntries, ',').ToArray());
            Assert.ThrowsExactly<ArgumentNullException>(() => ((string)null).SplitTo<int>(',').ToArray());
        }

        internal enum Color
        {
            Unknown,
            Red,
            Blue
        }

        [TestMethod]
        public void ToEnum_ParsesIgnoresCaseAndFallsBack()
        {
            Assert.AreEqual(Color.Red, "red".ToEnum(Color.Unknown));
            Assert.AreEqual(Color.Unknown, "pink".ToEnum(Color.Unknown));
            Assert.AreEqual(Color.Blue, "Blue".ToEnum<Color>());
            Assert.ThrowsExactly<ArgumentException>(() => "red".ToEnum<int>());
        }

        [TestMethod]
        public void Format_ReplacesItems()
        {
            Assert.AreEqual("Hello world", "Hello {0}".Format("world"));
            Assert.AreEqual("1-2", "{0}-{1}".Format(1, 2));
        }

        [TestMethod]
        public void ToBytes_CopiesUtf16Payload()
        {
            const string value = "ab";
            byte[] bytes = value.ToBytes();
            Assert.AreEqual(value.Length * sizeof(char), bytes.Length);
            Assert.ThrowsExactly<ArgumentNullException>(() => ((string)null).ToBytes());
        }
    }
}
