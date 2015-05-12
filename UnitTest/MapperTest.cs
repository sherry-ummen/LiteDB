using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LiteDB;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Specialized;

namespace UnitTest
{
    public enum MyEnum { First, Second }

    public class MyClass
    {
        [BsonId(false)]
        public int MyId { get; set; }
        [BsonField("MY-STRING")]
        public string MyString { get; set; }
        public Guid MyGuid { get; set; }
        public DateTime MyDateTime { get; set; }
        public DateTime? MyDateTimeNullable { get; set; }
        public int? MyIntNullable { get; set; }
        public MyEnum MyEnumProp { get; set; }
        public char MyChar { get; set; }
        public byte MyByte { get; set; }
        public decimal MyDecimal { get; set; }

        [BsonIndex(true)]
        public Uri MyUri { get; set; }

        // do not serialize this properties
        [BsonIgnore]
        public string MyIgnore { get; set; }
        public string MyReadOnly { get; private set; }
        public string MyWriteOnly { set; private get; }
        public string MyField = "DoNotSerializeThis";
        internal string MyInternalProperty { get; set; }

        // special types
        public NameValueCollection MyNameValueCollection { get; set; }

        // lists
        public string[] MyStringArray { get; set; }
        public List<string> MyStringList { get; set; }
        public Dictionary<int, string> MyDict { get; set; }

        // generic lists
        public IList<object> MyGenericList { get; set; }
        public IDictionary<int, string> MyGenericDict { get; set; }
    }

    [TestClass]
    public class MapperTest
    {
        private MyClass CreateModel()
        {
            var c = new MyClass
            {
                MyId = 123,
                MyString = "John",
                MyGuid = Guid.NewGuid(),
                MyDateTime = DateTime.Now,
                MyIgnore = "IgnoreTHIS",
                MyIntNullable = 999,
                MyStringList = new List<string>(),
                MyGenericList = new List<object>(),
                MyWriteOnly = "write-only",
                MyInternalProperty = "internal-field",
                MyNameValueCollection = new NameValueCollection(),
                MyDict = new Dictionary<int,string>(),
                MyGenericDict = new Dictionary<int, string>(),
                MyStringArray = new string[] { "One", "Two" },
                MyEnumProp = MyEnum.Second,
                MyChar = 'Y',
                MyUri = new Uri("http://www.numeria.com.br"),
                MyByte = 255,
                MyDecimal = 19.9m
            };

            c.MyStringList.Add("String-1");
            c.MyStringList.Add("String-2");

            c.MyGenericList.Add("John");
            c.MyGenericList.Add(28);
            
            c.MyNameValueCollection["key-1"] = "value-1";
            c.MyNameValueCollection["KeyNumber2"] = "value-2";

            c.MyDict[1] = "Row 1";
            c.MyDict[2] = "Row 2";

            c.MyGenericDict[1] = "John";
            c.MyGenericDict[2] = "Doe";

            return c;
        }

        [TestMethod]
        public void Mapper_Test()
        {
            var mapper = new BsonMapper();

            mapper.UseLowerCaseDelimiter('_');

            var obj = CreateModel();
            var doc = mapper.ToDocument(obj);
            var nobj = mapper.ToObject<MyClass>(doc);

            var json = JsonSerializer.Serialize(doc, true);

            // compare object to document
            Assert.AreEqual(doc["_id"].AsInt32, obj.MyId);
            Assert.AreEqual(doc["MY-STRING"].AsString, obj.MyString);
            Assert.AreEqual(doc["my_guid"].AsGuid, obj.MyGuid);

            // compare 2 objects
            Assert.AreEqual(obj.MyId, nobj.MyId);
            Assert.AreEqual(obj.MyString, nobj.MyString);
            Assert.AreEqual(obj.MyGuid, nobj.MyGuid);
            Assert.AreEqual(obj.MyDateTime, nobj.MyDateTime);
            Assert.AreEqual(obj.MyDateTimeNullable, nobj.MyDateTimeNullable);
            Assert.AreEqual(obj.MyIntNullable, nobj.MyIntNullable);
            Assert.AreEqual(obj.MyEnumProp, nobj.MyEnumProp);
            Assert.AreEqual(obj.MyChar, nobj.MyChar);
            Assert.AreEqual(obj.MyByte, nobj.MyByte);
            Assert.AreEqual(obj.MyDecimal, nobj.MyDecimal);
            Assert.AreEqual(obj.MyUri, nobj.MyUri);

            Assert.AreEqual(obj.MyStringArray[0], nobj.MyStringArray[0]);
            Assert.AreEqual(obj.MyStringArray[1], nobj.MyStringArray[1]);
            Assert.AreEqual(obj.MyStringList[1], nobj.MyStringList[1]);
            Assert.AreEqual(obj.MyGenericList[1], nobj.MyGenericList[1]);
            Assert.AreEqual(obj.MyDict[2], nobj.MyDict[2]);
            Assert.AreEqual(obj.MyGenericDict[2], nobj.MyGenericDict[2]);
        }
    }
}
