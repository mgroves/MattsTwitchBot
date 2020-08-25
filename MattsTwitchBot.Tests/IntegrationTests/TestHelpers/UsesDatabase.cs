using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.KeyValue;
using MattsTwitchBot.Core;
using MattsTwitchBot.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.IntegrationTests.TestHelpers
{
    // this class will connect to a Couchbase instance
    // and provide helpers to test classes:
    // 1. It will use environment variables or fall back to defaults
    // 2. Any documents added to DocumentsToRemove will be removed after every test
    // 3. It will create a bucket for testing when necessary
    // 4. Bucket, Collection, BucketProvider are all made available to tests that inherit
    [Category("SkipWhenLiveUnitTesting")]
    public abstract class UsesDatabase
    {
        protected ICluster TestCluster;
        protected List<string> DocumentsToRemove;
        protected ICouchbaseCollection Collection;
        protected IBucket Bucket;
        protected IntegrationTestBucketProvider BucketProvider;
        protected IKeyGenerator TestKeyGen;
        protected FakeConfiguration TestConfiguration;

        [SetUp]
        public virtual async Task Setup()
        {
            var connectionString = Environment.GetEnvironmentVariable("COUCHBASE_CONNECTION_STRING") ?? "couchbase://localhost";
            var username = Environment.GetEnvironmentVariable("COUCHBASE_USERNAME") ?? "Administrator";
            var password = Environment.GetEnvironmentVariable("COUCHBASE_PASSWORD") ?? "password";
            var bucketName = Environment.GetEnvironmentVariable("COUCHBASE_BUCKET_NAME") ?? "tests";
            TestCluster = await Cluster.ConnectAsync(connectionString, username, password);

            // from this point on, any test using this base class assumes:
            // - that a bucket with name in bucketName exists
            // - that a primary index in that bucket exists
            // Locally, you'll need to make sure this is setup manually
            // Also see the .github folder for how this is setup for Github Actions CI/CD

            Bucket = await TestCluster.BucketAsync(bucketName);
            Collection = Bucket.DefaultCollection();
            DocumentsToRemove = new List<string>();
            BucketProvider = new IntegrationTestBucketProvider(Bucket);
            TestKeyGen = new TestKeyGenerator(DocumentsToRemove);
            TestConfiguration = new FakeConfiguration();
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
            DocumentsToRemove.ForEach(async key =>
            {
                try
                {
                    await Collection.RemoveAsync(key);
                }
                catch
                {
                    // if there's an exception throw because that key doesn't exist
                    // that's fine, but there many be others in DocumentsToRemove
                    // so that's why this exception is being swallowed
                }
            });
            await TestCluster.DisposeAsync();
        }
    }
}