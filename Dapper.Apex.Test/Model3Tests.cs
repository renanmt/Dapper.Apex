﻿using Dapper.Apex.Query;
using Dapper.Apex.Test.Database;
using Dapper.Apex.Test.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dapper.Apex.Test
{
    [Collection("Database collection")]
    public class Model3Tests
    {
        private static Tuple<Guid, Guid> Entity1Id = Tuple.Create(new Guid("6018F3BE-3F6A-45D9-BE34-ACCA441AAB2F"), new Guid("8EB6501F-EC5A-4696-B579-BF73E9E2B914"));
        private static Tuple<Guid, Guid> Entity2Id = Tuple.Create(new Guid("F824BFFC-24FF-4932-8084-C7E91606141F"), new Guid("12B455AE-C111-4F8D-B333-60A8E4958B5F"));

        [Theory(DisplayName = "Get Entity by Id")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void GetEntityById(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model3 entity = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                entity = connection.Get<Model3>(new { Id1 = Entity1Id.Item1, Id2 = Entity1Id.Item2 });
                connection.Close();
            }

            Assert.NotNull(entity);
            Assert.Equal(Entity1Id.Item1, entity.Id1);
            Assert.Equal(Entity1Id.Item2, entity.Id2);
            Assert.Equal("ENTITY 1", entity.Prop1);
        }

        [Theory(DisplayName = "Get Non Existing Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void GetNonExistingEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model3 entity = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                entity = connection.Get<Model3>(new { Id1 = Guid.Empty, Id2 = Guid.Empty });
                connection.Close();
            }

            Assert.Null(entity);
        }


        [Theory(DisplayName = "Get All")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void GetAll(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            IEnumerable<Model3> entities = null;

            using (var connection = dbConnection)
            {
                connection.Open();
                entities = connection.GetAll<Model3>();
                connection.Close();
            }

            Assert.NotNull(entities);
            Assert.NotEmpty(entities);
            Assert.True(entities.Count() >= 2);

            Assert.Contains(entities, e => e.Id1 == Entity1Id.Item1 && e.Id2 == Entity1Id.Item2);
            Assert.Contains(entities, e => e.Id1 == Entity2Id.Item1 && e.Id2 == Entity2Id.Item2);
        }

        [Theory(DisplayName = "Update Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void UpdateEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model3 entity = null;
            bool found = true;

            using (var connection = dbConnection)
            {
                connection.Open();

                entity = connection.Get<Model3>(new { Id1 = Entity2Id.Item1, Id2 = Entity2Id.Item2 });
                entity.Prop1 = "ENTITY 2 UPDATED";

                found = connection.Update(entity);

                entity = connection.Get<Model3>(new { Id1 = Entity2Id.Item1, Id2 = Entity2Id.Item2 });

                connection.Close();
            }

            Assert.True(found);
            Assert.NotNull(entity);
            Assert.Equal("ENTITY 2 UPDATED", entity.Prop1);
        }

        [Theory(DisplayName = "Update Many")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void UpdateMany(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model3 entity1 = null;
            Model3 entity2 = null;

            bool found = true;

            using (var connection = dbConnection)
            {
                connection.Open();

                entity1 = connection.Get<Model3>(Entity1Id);
                entity2 = connection.Get<Model3>(Entity2Id);

                entity1.Prop1 = "ENTITY 1 UPDATED";

                entity2.Prop1 = "ENTITY 2 UPDATED";

                var entities = new List<Model3>()
                {
                    { entity1 },
                    { entity2 }
                };

                found = connection.UpdateMany<Model3>(entities);

                entity1 = connection.Get<Model3>(Entity1Id);
                entity2 = connection.Get<Model3>(Entity2Id);

                connection.Close();
            }

            Assert.True(found);
            Assert.NotNull(entity1);
            Assert.Equal("ENTITY 1 UPDATED", entity1.Prop1);

            Assert.NotNull(entity2);
            Assert.Equal("ENTITY 2 UPDATED", entity2.Prop1);
        }

        [Theory(DisplayName = "Update Non Existing Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void UpdateNonExistingEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model3 entity = new Model3();
            entity.Id1 = Guid.Empty;
            entity.Id2 = Guid.Empty;
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

        [Theory(DisplayName = "Insert Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void InsertEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            var keys = new
            {
                Id1 = Guid.NewGuid(),
                Id2 = Guid.NewGuid()
            };

            Model3 entity = new Model3()
            {
                Id1 = keys.Id1,
                Id2 = keys.Id2,
                Prop1 = "ENTITY 3"
            };

            using (var connection = dbConnection)
            {
                connection.Open();

                var count = connection.Insert(entity);

                Assert.Equal(1, count);

                entity = connection.Get<Model3>(keys);

                Assert.NotNull(entity);
                Assert.Equal("ENTITY 3", entity.Prop1);

                connection.Close();
            }
        }

        [Theory(DisplayName = "Insert Many One by One")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void InsertManyOneByOne(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            var keys1 = new { Id1 = Guid.NewGuid(), Id2 = Guid.NewGuid() };
            var keys2 = new { Id1 = Guid.NewGuid(), Id2 = Guid.NewGuid() };

            Model3 entity1 = new Model3() { Id1 = keys1.Id1, Id2 = keys1.Id2, Prop1 = "ENTITY 10" };
            Model3 entity2 = new Model3() { Id1 = keys2.Id1, Id2 = keys2.Id2, Prop1 = "ENTITY 11" };

            var entities = new List<Model3>() {
                { entity1 },
                { entity2 }
            };

            using (var connection = dbConnection)
            {
                connection.Open();

                var count = connection.InsertMany<Model3>(entities, insertMode: InsertMode.OneByOne);

                Assert.Equal(keys1.Id1, entity1.Id1);
                Assert.Equal(keys1.Id2, entity1.Id2);
                Assert.Equal(keys2.Id1, entity2.Id1);
                Assert.Equal(keys2.Id2, entity2.Id2);
                Assert.Equal(2, count);

                entity1 = connection.Get<Model3>(keys1);

                Assert.NotNull(entity1);
                Assert.Equal("ENTITY 10", entity1.Prop1);

                entity2 = connection.Get<Model3>(keys2);

                Assert.NotNull(entity2);
                Assert.Equal("ENTITY 11", entity2.Prop1);

                connection.Close();
            }
        }

        [Theory(DisplayName = "Insert Many Single Shot")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void InsertSingleShot(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            var keys1 = new { Id1 = Guid.NewGuid(), Id2 = Guid.NewGuid() };
            var keys2 = new { Id1 = Guid.NewGuid(), Id2 = Guid.NewGuid() };

            Model3 entity1 = new Model3() { Id1 = keys1.Id1, Id2 = keys1.Id2, Prop1 = "ENTITY 10" };
            Model3 entity2 = new Model3() { Id1 = keys2.Id1, Id2 = keys2.Id2, Prop1 = "ENTITY 11" };

            var entities = new List<Model3>() {
                { entity1 },
                { entity2 }
            };

            using (var connection = dbConnection)
            {
                connection.Open();

                var count = connection.InsertMany<Model3>(entities, insertMode: InsertMode.SingleShot);

                Assert.Equal(keys1.Id1, entity1.Id1);
                Assert.Equal(keys1.Id2, entity1.Id2);
                Assert.Equal(keys2.Id1, entity2.Id1);
                Assert.Equal(keys2.Id2, entity2.Id2);
                Assert.Equal(2, count);

                entity1 = connection.Get<Model3>(keys1);

                Assert.NotNull(entity1);
                Assert.Equal("ENTITY 10", entity1.Prop1);

                entity2 = connection.Get<Model3>(keys2);

                Assert.NotNull(entity2);
                Assert.Equal("ENTITY 11", entity2.Prop1);

                connection.Close();
            }
        }

        [Theory(DisplayName = "Delete Entity by Id")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void DeleteEntityById(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            var keys = new
            {
                Id1 = Guid.NewGuid(),
                Id2 = Guid.NewGuid()
            };

            Model3 entity = new Model3()
            {
                Id1 = keys.Id1,
                Id2 = keys.Id2,
                Prop1 = "ENTITY 10"
            };

            bool found = false;

            using (var connection = dbConnection)
            {
                connection.Open();
                connection.Insert(entity);

                found = connection.Delete(entity);
                entity = connection.Get<Model3>(keys);
                connection.Close();
            }

            Assert.True(found);
            Assert.Null(entity);
        }

        [Theory(DisplayName = "Delete Non Existing Entity")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void DeleteNonExistingEntity(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            Model3 entity = new Model3()
            {
                Id1 = Guid.Empty,
                Id2 = Guid.Empty
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
        public void DeleteMany(IDbConnection dbConnection)
        {
            QueryHelper.FlushCache();

            var keys1 = new { Id1 = Guid.NewGuid(), Id2 = Guid.NewGuid() };
            var keys2 = new { Id1 = Guid.NewGuid(), Id2 = Guid.NewGuid() };

            Model3 entity1 = new Model3() { Id1 = keys1.Id1, Id2 = keys1.Id2, Prop1 = "ENTITY 10" };
            Model3 entity2 = new Model3() { Id1 = keys2.Id1, Id2 = keys2.Id2, Prop1 = "ENTITY 11" };

            var entities = new List<Model3>() {
                { entity1 },
                { entity2 }
            };

            bool found = false;

            using (var connection = dbConnection)
            {
                connection.Open();
                connection.InsertMany<Model3>(entities);
                found = connection.DeleteMany<Model3>(entities);
                entity1 = connection.Get<Model3>(keys1);
                entity2 = connection.Get<Model3>(keys2);
                connection.Close();
            }

            Assert.True(found);
            Assert.Null(entity1);
            Assert.Null(entity2);

        }
    }
}
