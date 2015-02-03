using System;
using System.Collections.Generic;
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

    internal class Product
    {
        public string Name { set; get; }
        public DateTime ExpiryDate { set; get; }
        public decimal Price { set; get; }
        public string[] Sizes { set; get; }

        public override string ToString()
        {
            return string.Format("Name={0},ExpiryDate={1},Price={2},Sizes=[{3}]", Name, ExpiryDate, Price,
                string.Join(",", Sizes));
        }
    }

    [TestClass]
    public class StringExtensionTest
    {
        [TestMethod]
        public void TestJsonStringToObject()
        {
            const string productString = "{'name':'Widget','expiryDate':'2010-12-20T18:01Z'," +
                                         "'price':9.99,'sizes':['Small','Medium','Large']}";
            var product = productString.JsonToObject<Product>();
            Assert.IsNotNull(product);
            Console.WriteLine(product);

            const string productListString =
                "[{'name':'Widget','expiryDate':'2010-12-20T18:01Z','price':9.99,'sizes':['Small','Medium','Large']}," +
                "{'name':'Image','expiryDate':'2015-12-20T18:01Z','price':20.50,'sizes':['Small','Medium','Large','Extra Large']}]";
            var products = productListString.JsonToObject<List<Product>>();
            Assert.IsNotNull(products);
            foreach (Product obj in products)
            {
                Console.WriteLine(obj);
            }
        }

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
