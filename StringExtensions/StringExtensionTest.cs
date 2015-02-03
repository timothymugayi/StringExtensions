using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringExtensionLibrary;

namespace StringExtensions
{
    /// <summary>
    ///     Temperature enum default values in c# are always 0 or first enum element
    /// </summary>
    internal enum Temperature
    {
        Unknown,
        Low,
        Medium,
        High,
    };

    [TestClass]
    public class StringExtensionTest
    {
        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TestToEnum()
        {
            Temperature guage = "VeryHigh".ToEnum(Temperature.High);
            Assert.IsTrue(guage.Equals(Temperature.High));
            guage = "low".ToEnum(Temperature.Unknown);
            Assert.IsTrue(guage.Equals(Temperature.Low));
            guage = "veryHigh".ToEnum<Temperature>();
            Assert.IsTrue(guage.Equals(Temperature.Unknown));
            guage = "Medium".ToEnum<Temperature>();
            Assert.IsTrue(guage.Equals(Temperature.Medium));
            Assert.IsInstanceOfType("high".ToEnum<int>(), typeof (Temperature));
        }

        [TestMethod]
        public void TestCountOccurrences()
        {
            const string sentence = "hey man! i went to the apple store, hey man! are you listening to me";
            Assert.IsTrue(sentence.CountOccurrences("HEY MAN!") == 2);
        }

        [TestMethod]
        [ExpectedException(typeof (CryptographicException))]
        public void TestEncryptDecrypt()
        {
            const string key = "1234567890!@#$%^&*()_+";
            const string stringToEncrypt = "In my opinion best movie released 2014 is prometheus";
            string encryptedString = stringToEncrypt.Encrypt(key);
            string decryptedString = encryptedString.Decrypt(key);
            Assert.AreEqual(stringToEncrypt, decryptedString);
            encryptedString.Decrypt("wrongkey");
        }
    }
}
