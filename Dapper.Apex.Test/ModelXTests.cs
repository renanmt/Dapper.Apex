using MySqlConnector;
using Dapper.Apex.Query;
using Dapper.Apex.Test.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;
using Dapper.Apex.Test.Models;

namespace Dapper.Apex.Test
{
    [Collection("Database collection")]
    [TestCaseOrderer("Dapper.Apex.Test.PriorityOrderer", "Dapper.Apex.Test")]
    public class ModelXTests
    {
        [Theory(DisplayName = "Get Entity by Id")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(1)]
        public void GetEntityById(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            ModelX entity = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                entity = connection.Get<ModelX>(1);
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
        public void GetNonExistingEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            ModelX entity = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                entity = connection.Get<ModelX>(10);
                connection.Close();
            }

            Assert.Null(entity);
        }

        [Theory(DisplayName = "Get All")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(1)]
        public void GetAll(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            IEnumerable<ModelX> entities = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                entities = connection.GetAll<ModelX>();
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
        public void UpdateEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            ModelX entity = null;
            bool found = true;

            using (var connection = dbConnection)
            {
                connection.Open();
                
                entity = connection.Get<ModelX>(2);
                entity.Prop1 = "ENTITY 2 UPDATED";
                entity.Prop2 = "XXX";
                entity.Prop4 = 10;

                found = connection.Update(entity);

                entity = connection.Get<ModelX>(2);

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
        public void UpdateNonExistingEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            ModelX entity = new ModelX();
            entity.Id = 10;
            entity.Prop1 = "ENTITY 10";
            entity.Prop2 = "XXX";
            entity.Prop4 = 10;

            bool found = true;

            using (var connection = dbConnection)
            {
                connection.Open();

                found = connection.Update(entity);

                connection.Close();
            }

            Assert.False(found);
        }

        [Theory(DisplayName = "Update Many")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(2)]
        public void UpdateMany(IDbConnection dbConnection)
        {

            QueryHelper.FlushCache();

            ModelX entity1 = null;
            ModelX entity2 = null;

            bool found = true;

            using (var connection = dbConnection)
            {
                connection.Open();

                entity1 = connection.Get<ModelX>(1);
                entity2 = connection.Get<ModelX>(2);

                entity1.Prop1 = "ENTITY 1 UPDATED";
                entity1.Prop2 = "XXX";
                entity1.Prop4 = 10;

                entity2.Prop1 = "ENTITY 2 UPDATED";
                entity2.Prop2 = "XXX";
                entity2.Prop4 = 10;

                var entities = new List<ModelX>()
                {
                    { entity1 },
                    { entity2 }
                };

                found = connection.UpdateMany(entities as IEnumerable<ModelX>);

                entity1 = connection.Get<ModelX>(1);
                entity2 = connection.Get<ModelX>(2);

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
        public void InsertEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            ModelX entity = new ModelX()
            {
                Prop1 = "ENTITY 3",
                Prop2 = "READONLY PROP",
                Prop4 = 10
            };

            using (var connection = dbConnection)
            {
                connection.Open();

                var count = connection.Insert(entity);

                Assert.Equal(3, entity.Id);
                Assert.Equal(1, count);

                entity = connection.Get<ModelX>(3);

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
        public void InsertManyOneByOne(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            ModelX entity1 = new ModelX() { Prop1 = "ENTITY 10" };
            ModelX entity2 = new ModelX() { Prop1 = "ENTITY 11" };

            var entities = new List<ModelX>() {
                { entity1 },
                { entity2 }
            };

            using (var connection = dbConnection)
            {
                connection.Open();

                var all = connection.GetAll<ModelX>();
                var lastId = all.Last().Id;

                var entity1Id = lastId + 1;
                var entity2Id = lastId + 2;

                var count = connection.InsertMany<ModelX>(entities, insertMode: InsertMode.OneByOne);

                Assert.Equal(entity1Id, entity1.Id);
                Assert.Equal(entity2Id, entity2.Id);
                Assert.Equal(2, count);

                entity1 = connection.Get<ModelX>(entity1Id);

                Assert.NotNull(entity1);
                Assert.Equal("ENTITY 10", entity1.Prop1);
                Assert.Equal("DEFAULT VALUE", entity1.Prop2);
                Assert.Equal(0, entity1.Prop4);

                entity2 = connection.Get<ModelX>(entity2Id);

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
        public void InsertManySingleShot(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            ModelX entity1 = new ModelX() { Prop1 = "ENTITY 10" };
            ModelX entity2 = new ModelX() { Prop1 = "ENTITY 11" };

            var entities = new List<ModelX>() {
                { entity1 },
                { entity2 }
            };

            using (var connection = dbConnection)
            {
                connection.Open();

                var all = connection.GetAll<ModelX>();
                var lastId = all.Last().Id;

                var entity1Id = lastId + 1;
                var entity2Id = lastId + 2;

                var count = connection.InsertMany<ModelX>(entities, insertMode: InsertMode.SingleShot);

                Assert.Equal(entity1Id, entity1.Id);
                Assert.Equal(entity2Id, entity2.Id);
                Assert.Equal(2, count);

                entity1 = connection.Get<ModelX>(entity1Id);

                Assert.NotNull(entity1);
                Assert.Equal("ENTITY 10", entity1.Prop1);
                Assert.Equal("DEFAULT VALUE", entity1.Prop2);
                Assert.Equal(0, entity1.Prop4);

                entity2 = connection.Get<ModelX>(entity2Id);

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
        public void DeleteEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            ModelX entity = new ModelX()
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
                found = connection.Delete(entity);
                entity = connection.Get<ModelX>(id);
                connection.Close();
            }

            Assert.True(found);
            Assert.Null(entity);
        }

        [Theory(DisplayName = "Delete Entity by Key Tuple")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(5)]
        public void DeleteEntityByKeyTuple(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            ModelX entity = new ModelX()
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
                found = connection.Delete<ModelX>(ValueTuple.Create(id));
                entity = connection.Get<ModelX>(id);
                connection.Close();
            }

            Assert.True(found);
            Assert.Null(entity);
        }

        [Theory(DisplayName = "Delete Non Existing Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(5)]
        public void DeleteNonExistingEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            ModelX entity = new ModelX()
            {
                Id = 10
            };

            bool found = false;

            using (var connection = dbConnection)
            {
                connection.Open();
                found = connection.Delete(entity);
                connection.Close();
            }

            Assert.False(found);
        }


        [Theory(DisplayName = "Delete Many")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(5)]
        public void DeleteMany(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            var entities = new List<ModelX>() {
                { new ModelX() { Prop1 = "ENTITY 10" } },
                { new ModelX() { Prop1 = "ENTITY 11" } }
            };

            bool found = false;

            ModelX entity1 = null;
            ModelX entity2 = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                connection.InsertMany<ModelX>(entities);
                found = connection.DeleteMany<ModelX>(entities);
                entity1 = connection.Get<ModelX>(entities.First().Id);
                entity2 = connection.Get<ModelX>(entities.Skip(1).First().Id);
                connection.Close();
            }

            Assert.True(found);
            Assert.Null(entity1);
            Assert.Null(entity2);

        }

        [Theory(DisplayName = "Delete All")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(6)]
        public void DeleteAll(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            long existing = 0;
            long deleted = 0;

            IEnumerable<ModelX> all = null;

            using (var connection = dbConnection)
            {
                all = connection.GetAll<ModelX>();
                
                existing = all.Count();

                deleted = connection.DeleteAll<ModelX>();

                all = connection.GetAll<ModelX>();

                connection.Close();
            }

            Assert.Equal(existing, deleted);
            Assert.Empty(all);
        }
    }
}
