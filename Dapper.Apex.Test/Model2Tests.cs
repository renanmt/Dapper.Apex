using Dapper.Apex.Query;
using Dapper.Apex.Test.Database;
using Dapper.Apex.Test.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;

namespace Dapper.Apex.Test
{
    [Collection("Database collection")]
    [TestCaseOrderer("Dapper.Apex.Test.PriorityOrderer", "Dapper.Apex.Test")]
    public class Model2Tests
    {
        [Theory(DisplayName = "Get Count")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(0)]
        public void GetCount(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            long count = 0;

            using (var connection = dbConnection)
            {
                connection.Open();
                count = connection.GetCount<Model2>();
                connection.Close();
            }

            Assert.Equal(2, count);
        }

        [Theory(DisplayName = "Get Entity by Id")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(1)]
        public void GetEntityById(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model2 entity = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                entity = connection.Get<Model2>(1);
                connection.Close();
            }

            Assert.NotNull(entity);
            Assert.Equal(1, entity.Model2Id);
            Assert.Equal("ENTITY 1", entity.Prop1);
        }

        [Theory(DisplayName = "Get Non Existing Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(1)]
        public void GetNonExistingEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model2 entity = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                entity = connection.Get<Model2>(10);
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

            IEnumerable<Model2> entities = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                entities = connection.GetAll<Model2>();
                connection.Close();
            }

            Assert.NotNull(entities);
            Assert.NotEmpty(entities);
            Assert.True(entities.Count() >= 2);

            var entity = entities.First();

            Assert.Equal(1, entity.Model2Id);
            Assert.Equal("ENTITY 1", entity.Prop1);
        }

        [Theory(DisplayName = "Update Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(2)]
        public void UpdateEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model2 entity = null;
            bool found = true;

            using (var connection = dbConnection)
            {
                connection.Open();

                entity = connection.Get<Model2>(2);
                entity.Prop1 = "ENTITY 2 UPDATED";

                found = connection.Update(entity);

                entity = connection.Get<Model2>(2);

                connection.Close();
            }

            Assert.True(found);
            Assert.NotNull(entity);
            Assert.Equal("ENTITY 2 UPDATED", entity.Prop1);
        }

        [Theory(DisplayName = "Update Non Existing Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(2)]
        public void UpdateNonExistingEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model2 entity = new Model2();
            entity.Model2Id = 10;
            entity.Prop1 = "ENTITY 10";

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

            Model2 entity1 = null;
            Model2 entity2 = null;

            bool found = true;

            using (var connection = dbConnection)
            {
                connection.Open();

                entity1 = connection.Get<Model2>(1);
                entity2 = connection.Get<Model2>(2);

                entity1.Prop1 = "ENTITY 1 UPDATED";

                entity2.Prop1 = "ENTITY 2 UPDATED";

                var entities = new List<Model2>()
                {
                    { entity1 },
                    { entity2 }
                };

                found = connection.UpdateMany(entities as IEnumerable<Model2>);

                entity1 = connection.Get<Model2>(1);
                entity2 = connection.Get<Model2>(2);

                connection.Close();
            }

            Assert.True(found);
            Assert.NotNull(entity1);
            Assert.Equal("ENTITY 1 UPDATED", entity1.Prop1);

            Assert.NotNull(entity2);
            Assert.Equal("ENTITY 2 UPDATED", entity2.Prop1);
        }

        [Theory(DisplayName = "Insert Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(3)]
        public void InsertEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model2 entity = new Model2()
            {
                Prop1 = "ENTITY 3"
            };

            using (var connection = dbConnection)
            {
                connection.Open();

                connection.Insert(entity);

                Assert.Equal(3, entity.Model2Id);

                entity = connection.Get<Model2>(3);

                Assert.NotNull(entity);
                Assert.Equal("ENTITY 3", entity.Prop1);

                connection.Close();
            }
        }

        [Theory(DisplayName = "Insert Many One by One")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(4)]
        public void InsertManyOneByOne(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model2 entity1 = new Model2() { Prop1 = "ENTITY 10" };
            Model2 entity2 = new Model2() { Prop1 = "ENTITY 11" };

            var entities = new List<Model2>() {
                { entity1 },
                { entity2 }
            };

            using (var connection = dbConnection)
            {
                connection.Open();

                var all = connection.GetAll<Model2>();
                var lastId = all.Last().Model2Id;

                var entity1Id = lastId + 1;
                var entity2Id = lastId + 2;

                var count = connection.InsertMany<Model2>(entities, operationMode: OperationMode.OneByOne);

                Assert.Equal(entity1Id, entity1.Model2Id);
                Assert.Equal(entity2Id, entity2.Model2Id);
                Assert.Equal(2, count);

                entity1 = connection.Get<Model2>(entity1Id);

                Assert.NotNull(entity1);
                Assert.Equal("ENTITY 10", entity1.Prop1);

                entity2 = connection.Get<Model2>(entity2Id);

                Assert.NotNull(entity2);
                Assert.Equal("ENTITY 11", entity2.Prop1);

                connection.Close();
            }
        }

        [Theory(DisplayName = "Insert Many Single Shot")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(4)]
        public void InsertManySingleShot(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model2 entity1 = new Model2() { Prop1 = "ENTITY 10" };
            Model2 entity2 = new Model2() { Prop1 = "ENTITY 11" };

            var entities = new List<Model2>() {
                { entity1 },
                { entity2 }
            };

            using (var connection = dbConnection)
            {
                connection.Open();

                var all = connection.GetAll<Model2>();
                var lastId = all.Last().Model2Id;

                var entity1Id = lastId + 1;
                var entity2Id = lastId + 2;

                var count = connection.InsertMany<Model2>(entities, operationMode: OperationMode.SingleShot);

                Assert.Equal(entity1Id, entity1.Model2Id);
                Assert.Equal(entity2Id, entity2.Model2Id);
                Assert.Equal(2, count);

                entity1 = connection.Get<Model2>(entity1Id);

                Assert.NotNull(entity1);
                Assert.Equal("ENTITY 10", entity1.Prop1);

                entity2 = connection.Get<Model2>(entity2Id);

                Assert.NotNull(entity2);
                Assert.Equal("ENTITY 11", entity2.Prop1);

                connection.Close();
            }
        }

        [Theory(DisplayName = "Insert Many One by One PERF")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(4)]
        public void InsertManyOneByOnePerf(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();
            Fixture fixture = new Fixture();

            var entities = new List<Model2>();
            fixture.AddManyTo(entities, 400);

            using (var connection = dbConnection)
            {
                connection.Open();

                var count = connection.InsertMany<Model2>(entities, operationMode: OperationMode.OneByOne);

                Assert.Equal(400, count);

                connection.Close();
            }
        }

        [Theory(DisplayName = "Insert Many Single Shot PERF")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(4)]
        public void InsertManySingleShotPerf(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();
            Fixture fixture = new Fixture();

            var entities = new List<Model2>();
            fixture.AddManyTo(entities, 400);

            using (var connection = dbConnection)
            {
                connection.Open();

                var count = connection.InsertMany<Model2>(entities, operationMode: OperationMode.SingleShot);

                Assert.Equal(400, count);
                
                connection.Close();
            }
        }

        [Theory(DisplayName = "Delete Entity by Id")]
        [ClassData(typeof(DbConnectionGenerator))]
        [TestPriority(5)]
        public void DeleteEntityById(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model2 entity = new Model2()
            {
                Prop1 = "ENTITY 10"
            };

            bool found = false;

            using (var connection = dbConnection)
            {
                connection.Open();
                connection.Insert(entity);
                var id = entity.Model2Id;
                found = connection.Delete(entity);
                entity = connection.Get<Model2>(id);
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

            Model2 entity = new Model2()
            {
                Model2Id = 99999
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

            var entities = new List<Model2>() {
                { new Model2() { Prop1 = "ENTITY 10" } },
                { new Model2() { Prop1 = "ENTITY 11" } }
            };

            bool found = false;

            Model2 entity1 = null;
            Model2 entity2 = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                connection.InsertMany<Model2>(entities);
                found = connection.DeleteMany<Model2>(entities);
                entity1 = connection.Get<Model2>(entities.First().Model2Id);
                entity2 = connection.Get<Model2>(entities.Skip(1).First().Model2Id);
                connection.Close();
            }

            Assert.True(found);
            Assert.Null(entity1);
            Assert.Null(entity2);

        }
    }
}
