using Dapper.Apex.Test.BadModels;
using Dapper.Apex.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dapper.Apex.Test
{
    public class TypeHelperTest
    {
        [Fact(DisplayName = "Flush Cache")]
        public void FlushCache()
        {
            var typeInfo = TypeHelper.GetTypeInfo(typeof(ModelX));

            Assert.NotNull(typeInfo);
            Assert.NotEmpty(TypeHelper.TypeInfos);

            TypeHelper.FlushCache();

            Assert.Empty(TypeHelper.TypeInfos);
        }

        [Fact(DisplayName = "Get Type Info ModelX")]
        public void GetTypeInfoModelX()
        {
            var type = typeof(ModelX);
            var typeInfo = TypeHelper.GetTypeInfo(type);

            Assert.NotNull(typeInfo);
            Assert.Single(typeInfo.PrimaryKeyProperties);
            Assert.Equal("Id", typeInfo.PrimaryKeyProperties.First().Name);
            Assert.Equal(4, typeInfo.ReadableProperties.Count());
            Assert.Equal("Id", typeInfo.ReadableProperties.First().Name);
            Assert.Equal("Prop1", typeInfo.ReadableProperties.Skip(1).First().Name);
            Assert.Equal("Prop2", typeInfo.ReadableProperties.Skip(2).First().Name);
            Assert.Equal("Prop3", typeInfo.ReadableProperties.Skip(3).First().Name);
            Assert.Single(typeInfo.ComputedProperties);
            Assert.Equal("Prop4", typeInfo.ComputedProperties.First().Name);
            Assert.Equal(2, typeInfo.WritableProperties.Count());
            Assert.Equal("Prop1", typeInfo.WritableProperties.First().Name);
            Assert.Equal("Prop3", typeInfo.WritableProperties.Skip(1).First().Name);
            Assert.NotEmpty(typeInfo.TableName);
            Assert.Equal("Model1", typeInfo.TableName);
            Assert.Equal(type.TypeHandle, typeInfo.TypeHandle);
            Assert.Equal(KeyType.Surrogate, typeInfo.KeyType);

            var typeInfo2 = TypeHelper.GetTypeInfo(type);

            Assert.Same(typeInfo, typeInfo2);
        }

        [Fact(DisplayName = "Get Type Info Model2")]
        public void GetTypeInfoModel2()
        {
            var type = typeof(Model2);
            var typeInfo = TypeHelper.GetTypeInfo(type);

            Assert.NotNull(typeInfo);
            Assert.Single(typeInfo.PrimaryKeyProperties);
            Assert.Equal("Model2Id", typeInfo.PrimaryKeyProperties.First().Name);
            Assert.Equal(2, typeInfo.ReadableProperties.Count());
            Assert.Equal("Model2Id", typeInfo.ReadableProperties.First().Name);
            Assert.Equal("Prop1", typeInfo.ReadableProperties.Skip(1).First().Name);
            Assert.Empty(typeInfo.ComputedProperties);
            Assert.Single(typeInfo.WritableProperties);
            Assert.Equal("Prop1", typeInfo.WritableProperties.First().Name);
            Assert.NotEmpty(typeInfo.TableName);
            Assert.Equal("Model2", typeInfo.TableName);
            Assert.Equal(type.TypeHandle, typeInfo.TypeHandle);
            Assert.Equal(KeyType.Surrogate, typeInfo.KeyType);

            var typeInfo2 = TypeHelper.GetTypeInfo(type);

            Assert.Same(typeInfo, typeInfo2);
        }

        [Fact(DisplayName = "Get Type Info Model3")]
        public void GetTypeInfoModel3()
        {
            var type = typeof(Model3);
            var typeInfo = TypeHelper.GetTypeInfo(type);

            Assert.NotNull(typeInfo);
            Assert.Equal(2, typeInfo.PrimaryKeyProperties.Count());
            Assert.Equal("Id1", typeInfo.PrimaryKeyProperties.First().Name);
            Assert.Equal("Id2", typeInfo.PrimaryKeyProperties.Skip(1).First().Name);
            Assert.Equal(3, typeInfo.ReadableProperties.Count());
            Assert.Equal("Id1", typeInfo.ReadableProperties.First().Name);
            Assert.Equal("Id2", typeInfo.ReadableProperties.Skip(1).First().Name);
            Assert.Equal("Prop1", typeInfo.ReadableProperties.Skip(2).First().Name);
            Assert.Empty(typeInfo.ComputedProperties);
            Assert.Single(typeInfo.WritableProperties);
            Assert.Equal("Prop1", typeInfo.WritableProperties.First().Name);
            Assert.NotEmpty(typeInfo.TableName);
            Assert.Equal("Model3", typeInfo.TableName);
            Assert.Equal(type.TypeHandle, typeInfo.TypeHandle);
            Assert.Equal(KeyType.Natural, typeInfo.KeyType);

            var typeInfo2 = TypeHelper.GetTypeInfo(type);

            Assert.Same(typeInfo, typeInfo2);
        }

        [Fact(DisplayName = "Get Type Info with No Primary Key Model")]
        public void GetTypeInfoNoPrimartKeyModel()
        {
            var type = typeof(NoPrimaryKeyModel);
            Action action = () => TypeHelper.GetTypeInfo(type);

            var ex = Assert.Throws<DapperApexException>(action);
            Assert.Contains("No keys found", ex.Message);
        }

        [Fact(DisplayName = "Get Type Info with Dual Primary Key Model")]
        public void GetTypeInfoDualPrimartKeyModel()
        {
            var type = typeof(DualPrimaryKeyModel);
            Action action = () => TypeHelper.GetTypeInfo(type);

            var ex = Assert.Throws<DapperApexException>(action);
            Assert.Contains("has mixed [Key] and [ExplicitKey] attributes", ex.Message);
        }

        [Fact(DisplayName = "Get Type Info with Dual Primary Key Model Using Key Attribute")]
        public void GetTypeInfoDualPrimartKeyModelUsingKeyAttr()
        {
            var type = typeof(DualPrimaryKeyModelUsingKeyAttr);
            Action action = () => TypeHelper.GetTypeInfo(type);

            var ex = Assert.Throws<DapperApexException>(action);
            Assert.Contains("has mixed [Key] and [ExplicitKey] attributes", ex.Message);
        }

        [Fact(DisplayName = "Get Type Info with Multiple Surrogate Key Model")]
        public void GetTypeInfoMultipleSurrogateKeyModel()
        {
            var type = typeof(MultipleSurrogateKeyModel);
            Action action = () => TypeHelper.GetTypeInfo(type);

            var ex = Assert.Throws<DapperApexException>(action);
            Assert.Contains("has multiple [Key] attributes", ex.Message);
        }

        [Fact(DisplayName = "Is Collection List")]
        public void IsCollectionList()
        {
            var type = typeof(List<ModelX>);
            var isCollection = TypeHelper.IsCollection(ref type);

            Assert.True(isCollection);
            Assert.Equal(typeof(ModelX), type);

            var newType = typeof(List<ModelX>);
            TypeHelper.IsCollection(ref newType);

            Assert.Same(type, newType);
        }

        [Fact(DisplayName = "Is Collection Array")]
        public void IsCollectionArray()
        {
            var type = typeof(ModelX[]);
            var isCollection = TypeHelper.IsCollection(ref type);

            Assert.True(isCollection);
            Assert.Equal(typeof(ModelX), type);

            var newType = typeof(ModelX[]);
            TypeHelper.IsCollection(ref newType);

            Assert.Same(type, newType);
        }

        [Fact(DisplayName = "Is Not Collection")]
        public void IsNotCollection()
        {
            var type = typeof(ModelX);
            var originalType = type;
            var isCollection = TypeHelper.IsCollection(ref type);

            Assert.False(isCollection);
            Assert.Same(originalType, type);
        }
    }
}
