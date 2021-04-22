using Dapper.Apex.Query;
using Dapper.Apex.Test.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dapper.Apex.Test
{
    public class InitializeTests
    {
        [Fact(DisplayName = "Initialize Model Infos")]
        public void InitializeModelInfos()
        {
            TypeHelper.FlushCache();

            Type typeModelX = typeof(ModelX);
            Type typeModel2 = typeof(Model2);

            var typeList = new List<Type>() { { typeModelX }, { typeModel2 } };

            DapperApex.Initialize(typeList);

            Assert.Equal(2, TypeHelper.TypeInfos.Count());
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModelX.TypeHandle));
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModel2.TypeHandle));
        }

        [Fact(DisplayName = "Initialize Model Infos and Queries")]
        public void InitializeModelInfosAndQueries()
        {
            TypeHelper.FlushCache();
            QueryHelper.FlushCache();

            Type typeModelX = typeof(ModelX);
            Type typeModel2 = typeof(Model2);

            var typeList = new List<Type>() { { typeModelX }, { typeModel2 } };

            DapperApex.Initialize(typeList, new SqlConnection());

            Assert.Equal(2, TypeHelper.TypeInfos.Count());
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModelX.TypeHandle));
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModel2.TypeHandle));

            Assert.Equal(2, QueryHelper.QueryInfos.Count());
            Assert.True(QueryHelper.QueryInfos.ContainsKey(typeModelX.TypeHandle));
            Assert.True(QueryHelper.QueryInfos.ContainsKey(typeModel2.TypeHandle));
        }

        [Fact(DisplayName = "Initialize Model Infos for Namespace")]
        public void InitializeModelInfosForNamespace()
        {
            TypeHelper.FlushCache();

            Type typeModelX = typeof(ModelX);
            Type typeModel2 = typeof(Model2);
            Type typeModel3 = typeof(Model3);
            Type typeModel4 = typeof(Model4);

            DapperApex.Initialize(Assembly.GetExecutingAssembly(), "Dapper.Apex.Test.Models");

            Assert.Equal(4, TypeHelper.TypeInfos.Count());
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModelX.TypeHandle));
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModel2.TypeHandle));
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModel3.TypeHandle));
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModel4.TypeHandle));
        }

        [Fact(DisplayName = "Initialize Model Infos and Queries for Namespace")]
        public void InitializeModelInfosAndQueriesForNamespace()
        {
            TypeHelper.FlushCache();
            QueryHelper.FlushCache();

            Type typeModelX = typeof(ModelX);
            Type typeModel2 = typeof(Model2);
            Type typeModel3 = typeof(Model3);
            Type typeModel4 = typeof(Model4);

            DapperApex.Initialize(Assembly.GetExecutingAssembly(), "Dapper.Apex.Test.Models", new SqlConnection());

            Assert.Equal(4, TypeHelper.TypeInfos.Count);
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModelX.TypeHandle));
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModel2.TypeHandle));
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModel3.TypeHandle));
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModel4.TypeHandle));

            Assert.Equal(4, QueryHelper.QueryInfos.Count);
            Assert.True(QueryHelper.QueryInfos.ContainsKey(typeModelX.TypeHandle));
            Assert.True(QueryHelper.QueryInfos.ContainsKey(typeModel2.TypeHandle));
            Assert.True(QueryHelper.QueryInfos.ContainsKey(typeModel3.TypeHandle));
            Assert.True(QueryHelper.QueryInfos.ContainsKey(typeModel4.TypeHandle));
        }

        [Fact(DisplayName = "Initialize Model Infos for Base Type in Namespace")]
        public void InitializeModelInfosForBaseTypeInNamespace()
        {
            TypeHelper.FlushCache();

            Type typeModelX = typeof(ModelX);
            Type typeModel2 = typeof(Model2);
            Type typeModel3 = typeof(Model3);
            Type typeModel4 = typeof(Model4);

            DapperApex.Initialize(Assembly.GetExecutingAssembly(), "Dapper.Apex.Test.Models", typeof(BaseModel));

            Assert.Equal(2, TypeHelper.TypeInfos.Count);
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModelX.TypeHandle));
            Assert.False(TypeHelper.TypeInfos.ContainsKey(typeModel2.TypeHandle));
            Assert.False(TypeHelper.TypeInfos.ContainsKey(typeModel3.TypeHandle));
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModel4.TypeHandle));
        }

        [Fact(DisplayName = "Initialize Model Infos and Queries for Base Type in Namespace")]
        public void InitializeModelInfosAndQueriesForBaseTypeInNamespace()
        {
            TypeHelper.FlushCache();
            QueryHelper.FlushCache();

            Type typeModelX = typeof(ModelX);
            Type typeModel2 = typeof(Model2);
            Type typeModel3 = typeof(Model3);
            Type typeModel4 = typeof(Model4);

            DapperApex.Initialize(Assembly.GetExecutingAssembly(), "Dapper.Apex.Test.Models", typeof(BaseModel), new SqlConnection());

            Assert.Equal(2, TypeHelper.TypeInfos.Count);
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModelX.TypeHandle));
            Assert.False(TypeHelper.TypeInfos.ContainsKey(typeModel2.TypeHandle));
            Assert.False(TypeHelper.TypeInfos.ContainsKey(typeModel3.TypeHandle));
            Assert.True(TypeHelper.TypeInfos.ContainsKey(typeModel4.TypeHandle));

            Assert.Equal(2, QueryHelper.QueryInfos.Count);
            Assert.True(QueryHelper.QueryInfos.ContainsKey(typeModelX.TypeHandle));
            Assert.False(QueryHelper.QueryInfos.ContainsKey(typeModel2.TypeHandle));
            Assert.False(QueryHelper.QueryInfos.ContainsKey(typeModel3.TypeHandle));
            Assert.True(QueryHelper.QueryInfos.ContainsKey(typeModel4.TypeHandle));
        }
    }
}
