using Dapper.Apex.Test.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Xunit;

namespace Dapper.Apex.Test
{
    public class GetParametersTests
    {
        [Fact(DisplayName = "Get Parameters with Tuple")]
        public void GetParametersTuple()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var tuple = (id1, id2);

            var parameters = DapperApex.GetParameters(type, tuple, typeInfo.PrimaryKeyProperties);

            Assert.Equal(2, parameters.ParameterNames.Count());
            Assert.Contains("Id1", parameters.ParameterNames);
            Assert.Contains("Id2", parameters.ParameterNames);
            Assert.Equal(id1, parameters.Get<Guid>("Id1"));
            Assert.Equal(id2, parameters.Get<Guid>("Id2"));
        }

        [Fact(DisplayName = "Get Parameters with Tuple missing values")]
        public void GetParametersTupleMissingValues()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();

            var tuple = ValueTuple.Create(id1);

            Action func = () => DapperApex.GetParameters(type, tuple, typeInfo.PrimaryKeyProperties);

            var ex = Assert.Throws<DapperApexException>(func);

            Assert.Equal("The tuple passed as key does not match the number of keys (2) in Model3.", ex.Message);
        }

        [Fact(DisplayName = "Get Parameters with Tuple containing more values")]
        public void GetParametersTupleContainingMoreValues()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();

            var tuple = (id1, id2, id3);

            Action func = () => DapperApex.GetParameters(type, tuple, typeInfo.PrimaryKeyProperties);

            var ex = Assert.Throws<DapperApexException>(func);

            Assert.Equal("The tuple passed as key does not match the number of keys (2) in Model3.", ex.Message);
        }

        [Fact(DisplayName = "Get Parameters with Value")]
        public void GetParametersValue()
        {
            var type = typeof(Model2);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id = 10;

            var parameters = DapperApex.GetParameters(type, id, typeInfo.PrimaryKeyProperties);

            Assert.Single(parameters.ParameterNames);
            Assert.Contains("Model2Id", parameters.ParameterNames);
            Assert.Equal(10, parameters.Get<int>("Model2Id"));
        }


        [Fact(DisplayName = "Get Parameters with Value for composite key entity")]
        public void GetParametersValueForCompositeKeyEntity()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();

            Action func = () => DapperApex.GetParameters(type, id1, typeInfo.PrimaryKeyProperties);

            var ex = Assert.Throws<DapperApexException>(func);

            Assert.Equal("The key passed does not match the number of keys (2) in Model3.", ex.Message);
        }

        [Fact(DisplayName = "Get Parameters with Array")]
        public void GetParametersArray()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var array = new Guid[] { id1, id2 };

            var parameters = DapperApex.GetParameters(type, array, typeInfo.PrimaryKeyProperties);

            Assert.Equal(2, parameters.ParameterNames.Count());
            Assert.Contains("Id1", parameters.ParameterNames);
            Assert.Contains("Id2", parameters.ParameterNames);
            Assert.Equal(id1, parameters.Get<Guid>("Id1"));
            Assert.Equal(id2, parameters.Get<Guid>("Id2"));
        }

        [Fact(DisplayName = "Get Parameters with Array missing values")]
        public void GetParametersArrayMissingValues()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();

            var array = new Guid[] { id1 };

            Action func = () => DapperApex.GetParameters(type, array, typeInfo.PrimaryKeyProperties);

            var ex = Assert.Throws<DapperApexException>(func);

            Assert.Equal("Key collection contains less values (1) than primary keys (2) in the target type (Model3).", ex.Message);
        }

        [Fact(DisplayName = "Get Parameters with Array containing more values")]
        public void GetParametersArrayContainingMoreValues()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();

            var array = new Guid[] { id1, id2, id3 };

            var parameters = DapperApex.GetParameters(type, array, typeInfo.PrimaryKeyProperties);

            Assert.Equal(2, parameters.ParameterNames.Count());
            Assert.Contains("Id1", parameters.ParameterNames);
            Assert.Contains("Id2", parameters.ParameterNames);
            Assert.Equal(id1, parameters.Get<Guid>("Id1"));
            Assert.Equal(id2, parameters.Get<Guid>("Id2"));
        }

        [Fact(DisplayName = "Get Parameters with List")]
        public void GetParametersList()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var list = new List<Guid> { id1, id2 };

            var parameters = DapperApex.GetParameters(type, list, typeInfo.PrimaryKeyProperties);

            Assert.Equal(2, parameters.ParameterNames.Count());
            Assert.Contains("Id1", parameters.ParameterNames);
            Assert.Contains("Id2", parameters.ParameterNames);
            Assert.Equal(id1, parameters.Get<Guid>("Id1"));
            Assert.Equal(id2, parameters.Get<Guid>("Id2"));
        }

        [Fact(DisplayName = "Get Parameters with List missing values")]
        public void GetParametersListMissingValues()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();

            var list = new List<Guid> { id1 };

            Action func = () => DapperApex.GetParameters(type, list, typeInfo.PrimaryKeyProperties);

            var ex = Assert.Throws<DapperApexException>(func);

            Assert.Equal("Key collection contains less values (1) than primary keys (2) in the target type (Model3).", ex.Message);
        }

        [Fact(DisplayName = "Get Parameters with List containing more values")]
        public void GetParametersListContainingMoreValues()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();

            var list = new List<Guid> { id1, id2, id3 };

            var parameters = DapperApex.GetParameters(type, list, typeInfo.PrimaryKeyProperties);

            Assert.Equal(2, parameters.ParameterNames.Count());
            Assert.Contains("Id1", parameters.ParameterNames);
            Assert.Contains("Id2", parameters.ParameterNames);
            Assert.Equal(id1, parameters.Get<Guid>("Id1"));
            Assert.Equal(id2, parameters.Get<Guid>("Id2"));
        }

        [Fact(DisplayName = "Get Parameters with Dictionary")]
        public void GetParametersDictionary()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var dic = new Dictionary<string, Guid>();
            dic.Add("Id1", id1);
            dic.Add("Id2", id2);

            var parameters = DapperApex.GetParameters(type, dic, typeInfo.PrimaryKeyProperties);

            Assert.Equal(2, parameters.ParameterNames.Count());
            Assert.Contains("Id1", parameters.ParameterNames);
            Assert.Contains("Id2", parameters.ParameterNames);
            Assert.Equal(id1, parameters.Get<Guid>("Id1"));
            Assert.Equal(id2, parameters.Get<Guid>("Id2"));
        }

        [Fact(DisplayName = "Get Parameters with Dictionary missing values")]
        public void GetParametersDictionaryMissingValues()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();

            var dic = new Dictionary<string, Guid>();
            dic.Add("Id1", id1);

            Action func = () => DapperApex.GetParameters(type, dic, typeInfo.PrimaryKeyProperties);

            var ex = Assert.Throws<DapperApexException>(func);

            Assert.Equal("Key dictionary contains less values (1) than primary keys (2) in the target type (Model3).", ex.Message);
        }

        [Fact(DisplayName = "Get Parameters with Dictionary missing correct key")]
        public void GetParametersDictionaryMissingCorrectKey()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var dic = new Dictionary<string, Guid>();
            dic.Add("Id1", id1);
            dic.Add("TEST", id2);

            Action func = () => DapperApex.GetParameters(type, dic, typeInfo.PrimaryKeyProperties);

            var ex = Assert.Throws<DapperApexException>(func);

            Assert.Equal("Key dictionary does not contain key name Id2 from target type (Model3).", ex.Message);
        }

        [Fact(DisplayName = "Get Parameters with Dictionary containing more values")]
        public void GetParametersDictionaryContainingMoreValues()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();

            var dic = new Dictionary<string, Guid>();
            dic.Add("Id1", id1);
            dic.Add("Id2", id2);
            dic.Add("Id3", id3);

            var parameters = DapperApex.GetParameters(type, dic, typeInfo.PrimaryKeyProperties);

            Assert.Equal(2, parameters.ParameterNames.Count());
            Assert.Contains("Id1", parameters.ParameterNames);
            Assert.Contains("Id2", parameters.ParameterNames);
            Assert.Equal(id1, parameters.Get<Guid>("Id1"));
            Assert.Equal(id2, parameters.Get<Guid>("Id2"));
        }

        [Fact(DisplayName = "Get Parameters with ExpandoObject")]
        public void GetParametersExpandoObject()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            dynamic expando = new ExpandoObject();
            expando.Id1 = id1;
            expando.Id2 = id2;

            var parameters = DapperApex.GetParameters(type, expando as ExpandoObject, typeInfo.PrimaryKeyProperties);

            Assert.Equal(2, parameters.ParameterNames.Count());
            Assert.Contains("Id1", parameters.ParameterNames);
            Assert.Contains("Id2", parameters.ParameterNames);
            Assert.Equal(id1, parameters.Get<Guid>("Id1"));
            Assert.Equal(id2, parameters.Get<Guid>("Id2"));
        }

        [Fact(DisplayName = "Get Parameters with ExpandoObject missing values")]
        public void GetParametersExpandoObjectMissingValues()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();

            dynamic expando = new ExpandoObject();
            expando.Id1 = id1;

            Action func = () => DapperApex.GetParameters(type, expando as ExpandoObject, typeInfo.PrimaryKeyProperties);

            var ex = Assert.Throws<DapperApexException>(func);

            Assert.Equal("ExpandoObject key contains less values (1) than primary keys (2) in the target type (Model3).", ex.Message);
        }

        [Fact(DisplayName = "Get Parameters with ExpandoObject missing correct key")]
        public void GetParametersExpandoObjectMissingCorrectKey()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            dynamic expando = new ExpandoObject();
            expando.Id1 = id1;
            expando.TEST = id2;

            Action func = () => DapperApex.GetParameters(type, expando as ExpandoObject, typeInfo.PrimaryKeyProperties);

            var ex = Assert.Throws<DapperApexException>(func);

            Assert.Equal("ExpandoObject key does not contain key name Id2 from target type (Model3).", ex.Message);
        }

        [Fact(DisplayName = "Get Parameters with ExpandoObject containing more values")]
        public void GetParametersExpandoObjectContainingMoreValues()
        {
            var type = typeof(Model3);

            var typeInfo = TypeHelper.GetTypeInfo(type);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();

            dynamic expando = new ExpandoObject();
            expando.Id1 = id1;
            expando.Id2 = id2;
            expando.Id3 = id3;

            var parameters = DapperApex.GetParameters(type, expando as ExpandoObject, typeInfo.PrimaryKeyProperties);

            Assert.Equal(2, parameters.ParameterNames.Count());
            Assert.Contains("Id1", parameters.ParameterNames);
            Assert.Contains("Id2", parameters.ParameterNames);
            Assert.Equal(id1, parameters.Get<Guid>("Id1"));
            Assert.Equal(id2, parameters.Get<Guid>("Id2"));
        }
    }
}
