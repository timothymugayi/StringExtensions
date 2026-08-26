using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringExtensionLibrary;

namespace StringExtensions.Tests
{
    [TestClass]
    public class JsonAndQueryTests
    {
        [TestMethod]
        public void JsonToObject_StandardAndNewtonsoftQuotes()
        {
            const string standardJson =
                "{\"name\":\"Widget\",\"expiryDate\":\"2010-12-20T18:01Z\",\"price\":9.99,\"sizes\":[\"Small\",\"Medium\",\"Large\"]}";
            Product product = standardJson.JsonToObject<Product>();
            Assert.AreEqual("Widget", product.Name);
            Assert.AreEqual(9.99m, product.Price);
            CollectionAssert.AreEqual(new[] { "Small", "Medium", "Large" }, product.Sizes);

            const string newtonsoftQuotes = "{'name':'Gadget','price':1.5,'sizes':['S']}";
            Product gadget = newtonsoftQuotes.JsonToObject<Product>();
            Assert.AreEqual("Gadget", gadget.Name);
            Assert.ThrowsExactly<InvalidOperationException>(() => "null".JsonToObject<Product>());
        }

        [TestMethod]
        public void JsonToExpanderObject_ReadsNestedArrays()
        {
            const string productString = "{'name':'Widget','expiryDate':'2010-12-20T18:01Z'," +
                                         "'price':9.99,'sizes':['Small','Medium','Large']}";
            dynamic product = productString.JsonToExpanderObject();
            Assert.IsInstanceOfType(product, typeof(ExpandoObject));
            Assert.AreEqual("Widget", (string)product.name);
            var sizes = (List<object>)product.sizes;
            Assert.AreEqual(3, sizes.Count);
        }

        [TestMethod]
        public void JsonToDictionary_RequiresContent()
        {
            IDictionary<string, object> map = "{\"a\":\"b\"}".JsonToDictionary();
            Assert.AreEqual("b", map["a"]);
            Assert.ThrowsExactly<ArgumentNullException>(() => "".JsonToDictionary());
            Assert.ThrowsExactly<ArgumentNullException>(() => ((string)null).JsonToDictionary());
        }

        [TestMethod]
        public void QueryStringToDictionary_ParsesAndRejectsBadInput()
        {
            const string url = "?name=ferret&field1=value1&field2=value2&field3=value3";
            IDictionary<string, string> queryValues = url.QueryStringToDictionary();
            Assert.AreEqual("ferret", queryValues["name"]);
            Assert.AreEqual("value2", queryValues["field2"]);
            IDictionary<string, string> decoded = "?name=Ada%20Lovelace&q=hello+world".QueryStringToDictionary();
            Assert.AreEqual("Ada Lovelace", decoded["name"]);
            Assert.AreEqual("hello world", decoded["q"]);
            IDictionary<string, string> fromUrl = "https://example.com/search?name=Ada%20Lovelace#top".QueryStringToDictionary();
            Assert.AreEqual("Ada Lovelace", fromUrl["name"]);
            IDictionary<string, string> lastWins = "?a=1&a=2".QueryStringToDictionary();
            Assert.AreEqual("2", lastWins["a"]);
            IDictionary<string, string> malformed = "?x=%".QueryStringToDictionary();
            Assert.AreEqual("%", malformed["x"]);
            Assert.IsNull(((string)null).QueryStringToDictionary());
            Assert.IsNull("no-question".QueryStringToDictionary());
            Assert.IsNull("?onlykeys".QueryStringToDictionary());
        }

        internal class Product
        {
            public string Name { get; set; }
            public DateTime ExpiryDate { get; set; }
            public decimal Price { get; set; }
            public string[] Sizes { get; set; }
        }
    }
}
