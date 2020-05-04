using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Services.Usage;
using TesterCall.Services.UtilsAndWrappers;

namespace TesterCall.Tests.Services.Usage.ObjectCreatorTests
{
    [TestClass]
    public class CreateTests
    {
        public enum TestEnum
        {
            TestFirst,
            TestSecond,
            TestThird
        }

        public class TestInnerClass
        {
            public int Id;
            public int? NullableId;
            public string Name;
            public TestEnum? NullableEnum;
            public DateTime? NullableDateTime;
        }

        public class TestClass
        {
            public int Id;
            public int? NullableId;
            public TestInnerClass InnerClass;
            public IEnumerable<TestInnerClass> InnerClasses;
            public IEnumerable<int> Ints;
            public IEnumerable<int?> NullableInts;
        }

        private EnumFromStringService _enumService;
        private ObjectCreator _service;
        private Hashtable _input;
        private Hashtable _innerHashTable;

        [TestInitialize]
        public void TestInitialise()
        {
            _enumService = new EnumFromStringService();

            _service = new ObjectCreator(_enumService);

            _innerHashTable = new Hashtable(new Dictionary<string, object>() 
            {
                { "Id", 2 },
                { "Name", "TestName" }
            });
            _input = new Hashtable(new Dictionary<string, object>()
            {
                { "Id", 15 },
                { "InnerClass", _innerHashTable }
            });

        }

        [TestMethod]
        public void CreatesCorrectlyWithReplacementPropertiesInExampleMode()
        {
            var output = (TestClass) _service.Create(typeof(TestClass),
                                                    _input,
                                                    true);

            output.Id.Should().Be(15);
            output.InnerClass.Id.Should().Be(2);
            output.InnerClass.Name.Should().Be("TestName");
        }

        [TestMethod]
        public void NullableFieldsFilledInExampleMode()
        {
            var output = (TestClass)_service.Create(typeof(TestClass),
                                                    null,
                                                    true);

            output.NullableId.HasValue.Should().BeTrue();
            output.InnerClass.NullableId.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public void SingleArrayMemberCreatedInExampleMode()
        {
            var output = (TestClass)_service.Create(typeof(TestClass),
                                                    null,
                                                    true);

            output.InnerClasses.Count().Should().Be(1);
            var member = output.InnerClasses.First();
            member.Id.Should().Be(0);
            member.NullableId.Should().Be(0);
            member.Name.Should().BeNull();
        }

        [TestMethod]
        public void MultipleArrayMembersCreatedInExampleModeWhenRequested()
        {
            var hashTableArray = new Hashtable[]
            {
                new Hashtable(new Dictionary<string, object>()
                {
                    { "Id", 12 },
                    { "Name", "ArrayElement1" }
                }),
                new Hashtable(new Dictionary<string, object>()
                {
                    { "Name", "ArrayElement2" },
                    { "NullableId", null }
                })
            };
            _input.Add("InnerClasses", hashTableArray);

            var output = (TestClass)_service.Create(typeof(TestClass),
                                                    _input,
                                                    true);

            output.InnerClasses.Count().Should().Be(2);
            var array = output.InnerClasses.ToArray();
            array[0].Id.Should().Be(12);
            array[0].Name.Should().Be("ArrayElement1");
            array[0].NullableId.Should().Be(0);
            array[1].Id.Should().Be(0);
            array[1].Name.Should().Be("ArrayElement2");
            array[1].NullableId.Should().BeNull();
        }

        [TestMethod]
        public void MultipleArrayMembersCreatedWithoutExampleModeWhenRequested()
        {
            var hashTableArray = new Hashtable[]
            {
                new Hashtable(new Dictionary<string, object>()
                {
                    { "Id", 12 },
                    { "Name", "ArrayElement1" }
                }),
                new Hashtable(new Dictionary<string, object>()
                {
                    { "Name", "ArrayElement2" },
                    { "NullableId", null }
                })
            };
            _input.Add("InnerClasses", hashTableArray);

            var output = (TestClass)_service.Create(typeof(TestClass),
                                                    _input,
                                                    false);

            output.InnerClasses.Count().Should().Be(2);
            var array = output.InnerClasses.ToArray();
            array[0].Id.Should().Be(12);
            array[0].Name.Should().Be("ArrayElement1");
            array[0].NullableId.Should().BeNull();
            array[1].Id.Should().Be(0);
            array[1].Name.Should().Be("ArrayElement2");
            array[1].NullableId.Should().BeNull();
        }

        [TestMethod]
        public void MultipleValueTypeArrayMembersCreatedWithoutExampleMode()
        {
            var innerIntsArray = new int[] { 1, 2 };
            _input["Ints"] = innerIntsArray;

            var output = (TestClass)_service.Create(typeof(TestClass),
                                                    _input,
                                                    false);

            output.Ints.Should().BeEquivalentTo(innerIntsArray);
        }

        [TestMethod]
        public void MultipleValueTypeArrayMembersCreatedWithExampleMode()
        {
            var innerIntsArray = new int[] { 1, 2 };
            _input["Ints"] = innerIntsArray;

            var output = (TestClass)_service.Create(typeof(TestClass),
                                                    _input,
                                                    true);

            output.Ints.Should().BeEquivalentTo(innerIntsArray);
        }

        [TestMethod]
        public void SingleValueTypeMembersCreatedInArraysWithExampleMode()
        {
            var output = (TestClass)_service.Create(typeof(TestClass),
                                                    _input,
                                                    true);

            output.Ints.Count().Should().Be(1);
            output.Ints.First().Should().Be(0);
            output.NullableInts.Count().Should().Be(1);
            output.NullableInts.First().Should().Be(0);
        }

        [TestMethod]
        public void NullableArrayTypeCanContainNulls()
        {
            var nullables = new int?[] { 1, null, 3 };
            _input["NullableInts"] = nullables;

            var output = (TestClass)_service.Create(typeof(TestClass),
                                                    _input,
                                                    true);

            output.NullableInts.Should().BeEquivalentTo(nullables);
        }

        [TestMethod]
        public void CreatesEnumsFromStrings()
        {
            _innerHashTable["NullableEnum"] = "TestThird";

            var output = (TestClass)_service.Create(typeof(TestClass),
                                                    _input,
                                                    false);

            output.InnerClass.NullableEnum.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public void CreatesExampleEnums()
        {
            var output = (TestClass)_service.Create(typeof(TestClass),
                                                    _input,
                                                    true);

            output.InnerClass.NullableEnum.HasValue.Should().BeTrue();
            output.InnerClass.NullableEnum.Value.Should().Be(TestEnum.TestFirst);
        }

        [TestMethod]
        public void CreateDateTimeFromStrings()
        {
            _innerHashTable["NullableDateTime"] = "2020-05-03T15:30:00";
            var expectedDateTime = 03.May(2020).At(15, 30);

            var output = (TestClass)_service.Create(typeof(TestClass),
                                                    _input,
                                                    false);

            output.InnerClass.NullableDateTime.HasValue.Should().BeTrue();
            output.InnerClass.NullableDateTime.Value.Should().Be(expectedDateTime);
        }
    }
}
