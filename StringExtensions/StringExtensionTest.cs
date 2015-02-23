using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringExtensionLibrary;

namespace StringExtensions
{
    [TestClass]
    public class StringExtensionTest
    {
        [TestMethod]
        public void TestJsonToExpandoObject()
        {
            const string productString = "{'name':'Widget','expiryDate':'2010-12-20T18:01Z'," +
                                         "'price':9.99,'sizes':['Small','Medium','Large']}";

            dynamic product = productString.JsonToExpanderObject();
            Assert.IsInstanceOfType(product, typeof (ExpandoObject));

            Assert.IsNotNull(product.name);
            Assert.IsNotNull(product.expiryDate);
            Assert.IsNotNull(product.price);
            Assert.IsNotNull(product.sizes);

            var sizes = (List<object>) product.sizes;
            foreach (string item in sizes)
            {
                Assert.IsNotNull(item);
            }
        }

        [TestMethod]
        public void TestLength()
        {
            string sample = "There is currently no easy way to update all packages within a solution";
            Assert.IsTrue(sample.IsMinLength(2));
            sample = "The running";
            Assert.IsFalse(sample.IsMinLength(50));
            sample = null;
            Assert.IsFalse(sample.IsMinLength(1));
            sample = "One";
            Assert.IsTrue(sample.IsMaxLength(3));
            sample = "three";
            Assert.IsFalse(sample.IsMaxLength(3));
        }

        [TestMethod]
        public void TestDoesNotStartWith()
        {
            Assert.IsTrue("test".DoesNotStartWith("a"));
            Assert.IsFalse("test".DoesNotStartWith("t"));
            Assert.IsTrue("".DoesNotStartWith("t"));
            string NULL = null;
            Assert.IsTrue(NULL.DoesNotStartWith("t"));
        }

        [TestMethod]
        public void ToTextElements()
        {
            string testing = "asdfasdf aasdflk asdfasdf";
            IEnumerable<string> a = testing.ToTextElements();
            foreach (string k in a)
            {
                Console.WriteLine(k);
            }
        }

        [TestMethod]
        public void TestIPv4Address()
        {
            Assert.IsFalse("64.233.161.1470".IsValidIPv4());
            Assert.IsTrue("64.233.161.147".IsValidIPv4());
        }

        [TestMethod]
        public void TestQueryStringToDictionary()
        {
            const string url = "?name=ferret&field1=value1&field2=value2&field3=value3";
            IDictionary<string, string> queryValues = url.QueryStringToDictionary();
            Assert.IsNotNull(queryValues);
            foreach (var obj in queryValues)
            {
                Console.WriteLine("key={0},value={1}", obj.Key, obj.Value);
            }
        }

        [TestMethod]
        public void TestIsAlphaOrNumeric()
        {
            Assert.IsTrue("Burning bridges as we go".IsAlpha());
            Assert.IsFalse("Burning bridges as we go!".IsAlpha());
            Assert.IsTrue("10 minutes left to code".IsAlphaNumeric());
            Assert.IsTrue("123456".IsAlphaNumeric());
        }

        [TestMethod]
        public void TestRemovePrefixSufix()
        {
            Assert.AreEqual("berbahaya".RemovePrefix("ber", false), "bahaya");
            Assert.AreEqual("masakan".RemoveSuffix("an"), "masak");
        }

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
            return string.Format("Name: {0}, ExpiryDate: {1}, Price: {2}, Sizes: [{3}]", Name, ExpiryDate, Price,
                string.Join(",", Sizes));
        }
    }


    internal class Address
    {
        public Address()
        {
            CreatedOn = DateTime.UtcNow;
            Active = true;
        }

        public string Street1 { set; get; }
        public string Street2 { set; get; }
        public string Street3 { set; get; }
        public string City { set; get; }
        public string Country { set; get; }
        public DateTime CreatedOn { set; get; }
        public bool Active { set; get; }

        public override string ToString()
        {
            return
                string.Format(
                    "Active: {0}, City: {1}, Country: {2}, CreatedOn: {3}, Street1: {4}, Street2: {5}, Street3: {6}",
                    Active, City, Country, CreatedOn, Street1, Street2, Street3);
        }
    }

    internal class Person
    {
        public Person()
        {
            CreatedOn = DateTime.UtcNow;
            Addresses = new HashSet<Address>();
        }

        public string Name { set; get; }
        public int Age { set; get; }
        public DateTime Dob { set; get; }
        public string Email { set; get; }
        public string SocialSecurityNo { set; get; }
        public string MobileNo { set; get; }
        public string OfficeNo { set; get; }
        public string PassportNo { set; get; }
        public ICollection<Address> Addresses { set; get; }
        public string BirthPlace { set; get; }
        public string Nationality { set; get; }
        public DateTime CreatedOn { set; get; }

        public override string ToString()
        {
            return
                string.Format(
                    "Name: {0}, Age: {1}, Dob: {2}, Email: {3}, SocialSecurityNo: {4}, MobileNo: {5}, OfficeNo: {6}, Address: {7}, PassportNo: {8}, BirthPlace: {9}, Nationality: {10}",
                    Name, Age, Dob, Email, SocialSecurityNo, MobileNo, OfficeNo, Addresses, PassportNo, BirthPlace,
                    Nationality);
        }
    }
}