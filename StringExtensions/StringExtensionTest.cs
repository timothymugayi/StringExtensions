using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringExtensionLibrary;

namespace StringExtensions
{
    [TestClass]
    public class StringExtensionTest
    {
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
