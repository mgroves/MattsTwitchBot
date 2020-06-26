using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.KeyValue;
using Couchbase.Management.Buckets;
using Couchbase.Management.Query;
using MattsTwitchBot.Core;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.IntegrationTests.TestHelpers
{
    // this class will connect to a Couchbase instance
    // and provide helpers to test classes:
    // 1. It will use environment variables or fall back to defaults
    // 2. Any documents added to DocumentsToRemove will be removed after every test
    // 3. It will create a bucket for testing when necessary
    // 4. Bucket, Collection, BucketProvider are all made available to tests that inherit
    public abstract class UsesDatabase
    {
        protected ICluster TestCluster;
        protected List<string> DocumentsToRemove;
        protected ICouchbaseCollection Collection;
        protected IBucket Bucket;
        protected IntegrationTestBucketProvider BucketProvider;
        protected IKeyGenerator TestKeyGen;

        [SetUp]
        public async Task Setup()
        {
            var connectionString = Environment.GetEnvironmentVariable("COUCHBASE_CONNECTION_STRING") ?? "couchbase://localhost";
            var username = Environment.GetEnvironmentVariable("COUCHBASE_USERNAME") ?? "Administrator";
            var password = Environment.GetEnvironmentVariable("COUCHBASE_PASSWORD") ?? "password";
            var bucketName = Environment.GetEnvironmentVariable("COUCHBASE_BUCKET_NAME") ?? "tests";
            TestCluster = await Cluster.ConnectAsync(connectionString, username, password);
            try
            {
                await TestCluster.Buckets.CreateBucketAsync(new BucketSettings
                {
                    Name = bucketName,
                    BucketType = BucketType.Couchbase,
                    FlushEnabled = true,
                    RamQuotaMB = 100
                });

                await TestCluster.QueryIndexes.CreatePrimaryIndexAsync(bucketName);
            }
            catch
            {
                // assume bucket is already created
            }

            Bucket = await TestCluster.BucketAsync(bucketName);
            Collection = Bucket.DefaultCollection();
            DocumentsToRemove = new List<string>();
            BucketProvider = new IntegrationTestBucketProvider(Bucket);
            TestKeyGen = new TestKeyGenerator(DocumentsToRemove);
        }

        // use this method so that the document will get removed
        // when the test is over
        protected async Task InsertTestDocument<T>(string id, T content)
        {
            await Collection.InsertAsync(id, content);
            DocumentsToRemove.Add(id);
        }

        [TearDown]
        public async Task Teardown()
        {
            DocumentsToRemove.ForEach(async key => await Collection.RemoveAsync(key));
            await TestCluster.DisposeAsync();
        }
    }
}