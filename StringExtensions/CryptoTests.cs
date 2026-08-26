using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringExtensionLibrary;

namespace StringExtensions.Tests
{
    [TestClass]
    public class CryptoTests
    {
        [TestMethod]
        public void EncryptDecrypt_RoundTripAndWrongKey()
        {
            const string key = "1234567890!@#$%^&*()_+";
            const string stringToEncrypt = "In my opinion best movie released 2014 is prometheus";
            string encryptedString = stringToEncrypt.Encrypt(key);
            Assert.AreEqual(stringToEncrypt, encryptedString.Decrypt(key));
            Assert.ThrowsExactly<CryptographicException>(() => encryptedString.Decrypt("wrongkey"));
        }

        [TestMethod]
        public void Encrypt_RejectsEmptyInputs()
        {
            Assert.ThrowsExactly<ArgumentException>(() => "".Encrypt("key"));
            Assert.ThrowsExactly<ArgumentException>(() => "data".Encrypt(""));
            Assert.ThrowsExactly<ArgumentException>(() => ((string)null).Encrypt("key"));
            Assert.ThrowsExactly<ArgumentException>(() => "AA".Decrypt(""));
            Assert.ThrowsExactly<ArgumentException>(() => "".Decrypt("key"));
        }

        [TestMethod]
        public void Decrypt_RejectsTruncatedAndTamperedPayload()
        {
            string encrypted = "hello".Encrypt("secret-key");
            Assert.ThrowsExactly<CryptographicException>(() => "00-11".Decrypt("secret-key"));
            Assert.ThrowsExactly<CryptographicException>(() => "ZZ-not-hex".Decrypt("secret-key"));

            string[] parts = encrypted.Split('-');
            parts[parts.Length - 1] = parts[parts.Length - 1] == "00" ? "01" : "00";
            string tampered = string.Join("-", parts);
            Assert.ThrowsExactly<CryptographicException>(() => tampered.Decrypt("secret-key"));
        }

        [TestMethod]
        public void Pbkdf2HmacSha256_MatchesFrameworkAndPublishedVectors()
        {
            byte[] password = Encoding.UTF8.GetBytes("password");
            byte[] salt = Encoding.UTF8.GetBytes("salt");

            CollectionAssert.AreEqual(
                ParseHex("120fb6cffcf8b32c43e7225256c4f837a86548c92ccc35480805987cb70be17b"),
                StringExtensionLibrary.StringExtensions.Pbkdf2HmacSha256(password, salt, 1, 32));
            CollectionAssert.AreEqual(
                ParseHex("ae4d0c95af6b46d32d0adff928f06dd02a303f8ef3c251dfd6e2d85a95474c43"),
                StringExtensionLibrary.StringExtensions.Pbkdf2HmacSha256(password, salt, 2, 32));
            CollectionAssert.AreEqual(
                ParseHex("c5e478d59288c841aa530db6845c4c8d962893a001ce4e11a4963873aa98134a"),
                StringExtensionLibrary.StringExtensions.Pbkdf2HmacSha256(password, salt, 4096, 32));

            byte[] framework = Rfc2898DeriveBytes.Pbkdf2(password, salt, 4096, HashAlgorithmName.SHA256, 64);
            byte[] custom = StringExtensionLibrary.StringExtensions.Pbkdf2HmacSha256(password, salt, 4096, 64);
            CollectionAssert.AreEqual(framework, custom);
        }

        [TestMethod]
        public void CreateParameters_IsNotPartOfPublicApi()
        {
            Assert.IsNull(typeof(StringExtensionLibrary.StringExtensions).GetMethod(
                "CreateParameters",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance));
        }

        private static byte[] ParseHex(string hex)
        {
            return Convert.FromHexString(hex);
        }
    }
}
