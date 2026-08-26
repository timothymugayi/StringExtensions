using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringExtensionLibrary;

namespace StringExtensions.Tests
{
    [TestClass]
    public class MutationTests
    {
        [TestMethod]
        public void GetEmptyStringIfNull_And_GetNullIfEmpty()
        {
            Assert.AreEqual("", ((string)null).GetEmptyStringIfNull());
            Assert.AreEqual("hi", "  hi  ".GetEmptyStringIfNull());
            Assert.IsNull(((string)null).GetNullIfEmptyString());
            Assert.IsNull("   ".GetNullIfEmptyString());
            Assert.AreEqual("hi", "  hi  ".GetNullIfEmptyString());
        }

        [TestMethod]
        public void GetDefaultIfEmpty()
        {
            Assert.AreEqual("fallback", ((string)null).GetDefaultIfEmpty("fallback"));
            Assert.AreEqual("fallback", "  ".GetDefaultIfEmpty("fallback"));
            Assert.AreEqual("kept", " kept ".GetDefaultIfEmpty("fallback"));
        }

        [TestMethod]
        public void Capitalize_NullEmptyAndInvariant()
        {
            Assert.IsNull(((string)null).Capitalize());
            Assert.AreEqual("", "".Capitalize());
            Assert.AreEqual("Hello", "hELLo".Capitalize());
            CultureInfo original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
                Assert.AreEqual("Title", "TITLE".Capitalize());
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        [TestMethod]
        public void FirstAndLastCharacter()
        {
            Assert.AreEqual("H", "Hello".FirstCharacter());
            Assert.AreEqual("o", "Hello".LastCharacter());
            Assert.IsNull(((string)null).FirstCharacter());
            Assert.IsNull("".LastCharacter());
        }

        [TestMethod]
        public void ReplaceAndRemoveChars()
        {
            Assert.AreEqual("end", "Friends".Replace('F', 'r', 'i', 's'));
            Assert.AreEqual("end", "Friends".RemoveChars('F', 'r', 'i', 's'));
            Assert.ThrowsExactly<ArgumentNullException>(() => ((string)null).Replace('a'));
            Assert.ThrowsExactly<ArgumentNullException>(() => "a".RemoveChars(null));
        }

        [TestMethod]
        public void Truncate_Reverse_Csv()
        {
            Assert.AreEqual("Hel...", "Hello".Truncate(3));
            Assert.AreEqual("Hi", "Hi".Truncate(10));
            Assert.AreEqual("", ((string)null).Truncate(3));
            Assert.AreEqual("", "Hi".Truncate(0));
            Assert.AreEqual("cba", "abc".Reverse());
            Assert.ThrowsExactly<ArgumentNullException>(() => ((string)null).Reverse());
            Assert.AreEqual("\"a\"\"b\"", "a\"b".ParseStringToCsv());
            Assert.AreEqual("\"\"", ((string)null).ParseStringToCsv());
        }

        [TestMethod]
        public void PrefixAndSuffix()
        {
            Assert.AreEqual("bahaya", "berbahaya".RemovePrefix("ber", false));
            Assert.AreEqual("berbahaya", "berbahaya".RemovePrefix("xx"));
            Assert.AreEqual("masak", "masakan".RemoveSuffix("an"));
            Assert.AreEqual("masakan", "masakan".RemoveSuffix("zz"));
            Assert.AreEqual("file.txt", "file".AppendSuffixIfMissing(".txt"));
            Assert.AreEqual("file.txt", "file.txt".AppendSuffixIfMissing(".txt"));
            Assert.AreEqual("/tmp", "tmp".AppendPrefixIfMissing("/"));
            Assert.AreEqual("/tmp", "/tmp".AppendPrefixIfMissing("/"));
            Assert.IsNull(((string)null).RemovePrefix("a"));
        }

        [TestMethod]
        public void LeftRightAndByteSize()
        {
            Assert.AreEqual("He", "Hello".Left(2));
            Assert.AreEqual("lo", "Hello".Right(2));
            Assert.ThrowsExactly<ArgumentNullException>(() => ((string)null).Left(1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => "ab".Left(5));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => "ab".Right(-1));
            Assert.AreEqual(5, "Hello".GetByteSize(Encoding.ASCII));
            Assert.ThrowsExactly<ArgumentNullException>(() => "a".GetByteSize(null));
        }

        [TestMethod]
        public void ReverseSlash_And_LineFeeds()
        {
            Assert.AreEqual(@"a\b", "a/b".ReverseSlash(0));
            Assert.AreEqual("a/b", @"a\b".ReverseSlash(1));
            Assert.AreEqual("keep", "keep".ReverseSlash(9));
            Assert.ThrowsExactly<ArgumentNullException>(() => ((string)null).ReverseSlash(0));
            Assert.AreEqual("ab.", "\nab.\n".ReplaceLineFeeds());
            Assert.AreEqual("ab.cd", "ab.\ncd".ReplaceLineFeeds());
            Assert.ThrowsExactly<ArgumentNullException>(() => ((string)null).ReplaceLineFeeds());
        }

        [TestMethod]
        public void ToTextElements_EnumeratesCharacters()
        {
            string[] elements = "ab".ToTextElements().ToArray();
            CollectionAssert.AreEqual(new[] { "a", "b" }, elements);
            Assert.ThrowsExactly<ArgumentNullException>(() => ((string)null).ToTextElements().ToArray());
        }

        [TestMethod]
        public void CountOccurrences_TreatsNeedleAsLiteral()
        {
            const string sentence = "hey man! i went to the apple store, hey man! are you listening to me";
            Assert.AreEqual(2, sentence.CountOccurrences("HEY MAN!"));
            Assert.AreEqual(2, "a.a.a".CountOccurrences("."));
            Assert.AreEqual(0, "".CountOccurrences("a"));
            Assert.AreEqual(0, "abc".CountOccurrences(null));
        }

        [TestMethod]
        public void Hashes_AreStableAndRejectEmpty()
        {
            string sha256 = "hi".CreateHashSha256();
            string sha512 = "hi".CreateHashSha512();
            Assert.AreEqual(64, sha256.Length);
            Assert.AreEqual(128, sha512.Length);
            Assert.AreEqual(sha256, "hi".CreateHashSha256());
            Assert.ThrowsExactly<ArgumentException>(() => "".CreateHashSha256());
            Assert.ThrowsExactly<ArgumentException>(() => ((string)null).CreateHashSha512());
        }
    }
}
