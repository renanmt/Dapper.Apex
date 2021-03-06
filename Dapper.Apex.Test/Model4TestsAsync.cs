using MySqlConnector;
using Dapper.Apex.Query;
using Dapper.Apex.Test.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;
using Dapper.Apex.Test.Models;
using System.Threading.Tasks;

namespace Dapper.Apex.Test
{
    [Collection("Database collection")]
    [TestCaseOrderer("Dapper.Apex.Test.PriorityOrderer", "Dapper.Apex.Test")]
    public class Model4TestsAsync
    {
        [Theory(DisplayName = "Get Count")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task GetCount(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            long count = 0;

            using (var connection = dbConnection)
            {
                connection.Open();
                count = await connection.CountAsync<Model4>();
                connection.Close();
            }

            Assert.Equal(2, count);
        }

        [Theory(DisplayName = "Exists")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task Exists(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = false;

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync<Model4>(1);
                connection.Close();
            }

            Assert.True(exists);
        }

        [Theory(DisplayName = "Not Exists")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task NotExists(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = true;

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync<Model4>(-1);
                connection.Close();
            }

            Assert.False(exists);
        }

        [Theory(DisplayName = "Exists With Tuple Key")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task ExistsTuple(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = false;
            var key = ValueTuple.Create(1);

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync<Model4>(key);
                connection.Close();
            }

            Assert.True(exists);
        }

        [Theory(DisplayName = "Exists With Array Key")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task ExistsArray(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = false;
            var key = new int[] { 1 };

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync<Model4>(key);
                connection.Close();
            }

            Assert.True(exists);
        }

        [Theory(DisplayName = "Exists With List")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task ExistsList(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = false;
            var key = new List<int> { 1 };

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync<Model4>(key);
                connection.Close();
            }

            Assert.True(exists);
        }

        [Theory(DisplayName = "Exists With Dictionary")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task ExistsDictionary(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = false;
            var key = new Dictionary<string, int>();
            key.Add("Id", 1);

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync<Model4>(key);
                connection.Close();
            }

            Assert.True(exists);
        }

        [Theory(DisplayName = "Exists With Anonymous Object")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task ExistsAnonymousObject(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = false;
            var key = new { Id = 1 };

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync<Model4>(key);
                connection.Close();
            }

            Assert.True(exists);
        }

        [Theory(DisplayName = "Exists With Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task ExistsEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = false;
            var key = new Model4 { Id = 1 };

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync(key);
                connection.Close();
            }

            Assert.True(exists);
        }

        [Theory(DisplayName = "Not Exists With Tuple Key")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task NotExistsTuple(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = true;
            var key = ValueTuple.Create(-1);

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync<Model4>(key);
                connection.Close();
            }

            Assert.False(exists);
        }

        [Theory(DisplayName = "NotExists With Array Key")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task NotExistsArray(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = false;
            var key = new int[] { -1 };

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync<Model4>(key);
                connection.Close();
            }

            Assert.False(exists);
        }

        [Theory(DisplayName = "Not Exists With List")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task NotExistsList(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = false;
            var key = new List<int> { -1 };

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync<Model4>(key);
                connection.Close();
            }

            Assert.False(exists);
        }

        [Theory(DisplayName = "Not Exists With Dictionary")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task NotExistsDictionary(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = false;
            var key = new Dictionary<string, int>();
            key.Add("Id", -1);

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync<Model4>(key);
                connection.Close();
            }

            Assert.False(exists);
        }

        [Theory(DisplayName = "Not Exists With Anonymous Object")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task NotExistsAnonymousObject(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = false;
            var key = new { Id = -1 };

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync<Model4>(key);
                connection.Close();
            }

            Assert.False(exists);
        }

        [Theory(DisplayName = "Not Exists With Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public async Task NotExistsEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            bool exists = false;
            var key = new ModelX { Id = -1 };

            using (var connection = dbConnection)
            {
                connection.Open();
                exists = await connection.ExistsAsync(key);
                connection.Close();
            }

            Assert.False(exists);
        }

        [Theory(DisplayName = "Get Entity by Id")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(1)]
        public async Task GetEntityById(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model4 entity = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                entity = await connection.GetAsync<Model4>(1);
                connection.Close();
            }

            Assert.NotNull(entity);
            Assert.Equal(1, entity.Id);
            Assert.Equal("ENTITY 1", entity.Prop1);
            Assert.Equal("DEFAULT VALUE", entity.Prop2);
            Assert.Equal(0, entity.Prop4);
        }

        [Theory(DisplayName = "Get Non Existing Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(1)]
        public async Task GetNonExistingEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model4 entity = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                entity = await connection.GetAsync<Model4>(10);
                connection.Close();
            }

            Assert.Null(entity);
        }

        [Theory(DisplayName = "Get All")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(1)]
        public async Task GetAll(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            IEnumerable<Model4> entities = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                entities = await connection.GetAllAsync<Model4>();
                connection.Close();
            }

            Assert.NotNull(entities);
            Assert.NotEmpty(entities);
            Assert.True(entities.Count() >= 2);

            var entity = entities.First();

            Assert.Equal(1, entity.Id);
            Assert.Equal("ENTITY 1", entity.Prop1);
            Assert.Equal("DEFAULT VALUE", entity.Prop2);
            Assert.Equal(0, entity.Prop4);
        }


        [Theory(DisplayName = "Update Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(2)]
        public async Task UpdateEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model4 entity = null;
            bool found = true;

            using (var connection = dbConnection)
            {
                connection.Open();
                
                entity = await connection.GetAsync<Model4>(2);
                entity.Prop1 = "ENTITY 2 UPDATED";
                entity.Prop2 = "XXX";
                entity.Prop4 = 10;

                found = await connection.UpdateAsync(entity);

                entity = await connection.GetAsync<Model4>(2);

                connection.Close();
            }

            Assert.True(found);
            Assert.NotNull(entity);
            Assert.Equal("ENTITY 2 UPDATED", entity.Prop1);
            Assert.Equal("CUSTOM VALUE", entity.Prop2);
            Assert.Equal(0, entity.Prop4);
        }

        [Theory(DisplayName = "Update Non Existing Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(2)]
        public async Task UpdateNonExistingEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model4 entity = new Model4();
            entity.Id = 10;
            entity.Prop1 = "ENTITY 10";
            entity.Prop2 = "XXX";
            entity.Prop4 = 10;

            bool found = true;

            using (var connection = dbConnection)
            {
                connection.Open();

                found = await connection.UpdateAsync(entity);

                connection.Close();
            }

            Assert.False(found);
        }

        [Theory(DisplayName = "Update Many")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(2)]
        public async Task UpdateMany(IDbConnection dbConnection)
        {

            QueryHelper.FlushCache();

            Model4 entity1 = null;
            Model4 entity2 = null;

            bool found = true;

            using (var connection = dbConnection)
            {
                connection.Open();

                entity1 = await connection.GetAsync<Model4>(1);
                entity2 = await connection.GetAsync<Model4>(2);

                entity1.Prop1 = "ENTITY 1 UPDATED";
                entity1.Prop2 = "XXX";
                entity1.Prop4 = 10;

                entity2.Prop1 = "ENTITY 2 UPDATED";
                entity2.Prop2 = "XXX";
                entity2.Prop4 = 10;

                var entities = new List<Model4>()
                {
                    { entity1 },
                    { entity2 }
                };

                found = await connection.UpdateManyAsync(entities as IEnumerable<Model4>);

                entity1 = await connection.GetAsync<Model4>(1);
                entity2 = await connection.GetAsync<Model4>(2);

                connection.Close();
            }

            Assert.True(found);
            Assert.NotNull(entity1);
            Assert.Equal("ENTITY 1 UPDATED", entity1.Prop1);
            Assert.Equal("DEFAULT VALUE", entity1.Prop2);
            Assert.Equal(0, entity1.Prop4);

            Assert.NotNull(entity2);
            Assert.Equal("ENTITY 2 UPDATED", entity2.Prop1);
            Assert.Equal("CUSTOM VALUE", entity2.Prop2);
            Assert.Equal(0, entity2.Prop4);
        }


        [Theory(DisplayName = "Insert Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(3)]
        public async Task InsertEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model4 entity = new Model4()
            {
                Prop1 = "ENTITY 3",
                Prop2 = "READONLY PROP",
                Prop4 = 10
            };

            using (var connection = dbConnection)
            {
                connection.Open();

                await connection.InsertAsync(entity);

                Assert.Equal(3, entity.Id);

                entity = await connection.GetAsync<Model4>(3);

                Assert.NotNull(entity);
                Assert.Equal("ENTITY 3", entity.Prop1);
                Assert.Equal("DEFAULT VALUE", entity.Prop2);
                Assert.Equal(0, entity.Prop4);

                connection.Close();
            }
        }

        [Theory(DisplayName = "Insert Many One by One")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(4)]
        public async Task InsertManyOneByOne(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model4 entity1 = new Model4() { Prop1 = "ENTITY 10" };
            Model4 entity2 = new Model4() { Prop1 = "ENTITY 11" };

            var entities = new List<Model4>() {
                { entity1 },
                { entity2 }
            };

            using (var connection = dbConnection)
            {
                connection.Open();

                var all = await connection.GetAllAsync<Model4>();
                var lastId = all.Last().Id;

                var entity1Id = lastId + 1;
                var entity2Id = lastId + 2;

                var count = await connection.InsertManyAsync(entities, insertMode: OperationMode.OneByOne);

                Assert.Equal(entity1Id, entity1.Id);
                Assert.Equal(entity2Id, entity2.Id);
                Assert.Equal(2, count);

                entity1 = await connection.GetAsync<Model4>(entity1Id);

                Assert.NotNull(entity1);
                Assert.Equal("ENTITY 10", entity1.Prop1);
                Assert.Equal("DEFAULT VALUE", entity1.Prop2);
                Assert.Equal(0, entity1.Prop4);

                entity2 = await connection.GetAsync<Model4>(entity2Id);

                Assert.NotNull(entity2);
                Assert.Equal("ENTITY 11", entity2.Prop1);
                Assert.Equal("DEFAULT VALUE", entity2.Prop2);
                Assert.Equal(0, entity2.Prop4);

                connection.Close();
            }
        }

        [Theory(DisplayName = "Insert Many Single Shot")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(4)]
        public async Task InsertManySingleShot(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model4 entity1 = new Model4() { Prop1 = "ENTITY 10" };
            Model4 entity2 = new Model4() { Prop1 = "ENTITY 11" };

            var entities = new List<Model4>() {
                { entity1 },
                { entity2 }
            };

            using (var connection = dbConnection)
            {
                connection.Open();

                var all = await connection.GetAllAsync<Model4>();
                var lastId = all.Last().Id;

                var entity1Id = lastId + 1;
                var entity2Id = lastId + 2;

                var count = await connection.InsertManyAsync(entities, insertMode: OperationMode.SingleShot);

                Assert.Equal(entity1Id, entity1.Id);
                Assert.Equal(entity2Id, entity2.Id);
                Assert.Equal(2, count);

                entity1 = await connection.GetAsync<Model4>(entity1Id);

                Assert.NotNull(entity1);
                Assert.Equal("ENTITY 10", entity1.Prop1);
                Assert.Equal("DEFAULT VALUE", entity1.Prop2);
                Assert.Equal(0, entity1.Prop4);

                entity2 = await connection.GetAsync<Model4>(entity2Id);

                Assert.NotNull(entity2);
                Assert.Equal("ENTITY 11", entity2.Prop1);
                Assert.Equal("DEFAULT VALUE", entity2.Prop2);
                Assert.Equal(0, entity2.Prop4);

                connection.Close();
            }
        }

        [Theory(DisplayName = "Delete Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(5)]
        public async Task DeleteEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model4 entity = new Model4()
            {
                Prop1 = "ENTITY 10",
                Prop2 = "READONLY PROP",
                Prop4 = 10
            };

            bool found = false;

            using (var connection = dbConnection)
            {
                connection.Open();
                await connection.InsertAsync(entity);
                var id = entity.Id;
                found = await connection.DeleteAsync(entity);
                entity = await connection.GetAsync<Model4>(id);
                connection.Close();
            }

            Assert.True(found);
            Assert.Null(entity);
        }

        [Theory(DisplayName = "Delete Entity by Key Tuple")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(5)]
        public async Task DeleteEntityByKeyTuple(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model4 entity = new Model4()
            {
                Prop1 = "ENTITY 10",
                Prop2 = "READONLY PROP",
                Prop4 = 10
            };

            bool found = false;

            using (var connection = dbConnection)
            {
                connection.Open();
                connection.Insert(entity);
                var id = entity.Id;
                found = await connection.DeleteAsync<Model4>(ValueTuple.Create(id));
                entity = await connection.GetAsync<Model4>(id);
                connection.Close();
            }

            Assert.True(found);
            Assert.Null(entity);
        }

        [Theory(DisplayName = "Delete Non Existing Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(5)]
        public async Task DeleteNonExistingEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model4 entity = new Model4()
            {
                Id = 10
            };

            bool found = false;

            using (var connection = dbConnection)
            {
                connection.Open();
                found = await connection.DeleteAsync(entity);
                connection.Close();
            }

            Assert.False(found);
        }


        [Theory(DisplayName = "Delete Many")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(5)]
        public async Task DeleteMany(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            var entities = new List<Model4>() {
                { new Model4() { Prop1 = "ENTITY 10" } },
                { new Model4() { Prop1 = "ENTITY 11" } }
            };

            bool found = false;

            Model4 entity1 = null;
            Model4 entity2 = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                await connection .InsertManyAsync<Model4>(entities);
                found = await connection.DeleteManyAsync<Model4>(entities);
                entity1 = await connection.GetAsync<Model4>(entities.First().Id);
                entity2 = await connection.GetAsync<Model4>(entities.Skip(1).First().Id);
                connection.Close();
            }

            Assert.True(found);
            Assert.Null(entity1);
            Assert.Null(entity2);

        }

        [Theory(DisplayName = "Delete All")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(6)]
        public async Task DeleteAll(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            long existing = 0;
            long deleted = 0;

            IEnumerable<Model4> all = null;

            using (var connection = dbConnection)
            {
                all = await connection.GetAllAsync<Model4>();
                
                existing = all.Count();

                deleted = await connection.DeleteAllAsync<Model4>();

                all = await connection.GetAllAsync<Model4>();

                connection.Close();
            }

            Assert.Equal(existing, deleted);
            Assert.Empty(all);
        }
    }
}
