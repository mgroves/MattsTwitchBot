using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Query;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Trivia;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class GetRandomTriviaQuestionsHandlerTests : UnitTest
    {
        private GetRandomTriviaQuestionsHandler _handler;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _handler = new GetRandomTriviaQuestionsHandler(MockBucketProvider.Object);
        }

        [Test]
        public async Task Query_is_execute_to_get_questions()
        {
            // arrange
            var queryResult = new FakeQueryResult<TriviaQuestion>();
            queryResult.FakeRows = new List<TriviaQuestion>();
            var req = new GetRandomTriviaQuestions(numQuestions: 13);
            MockCluster.Setup(m => m.QueryAsync<TriviaQuestion>(It.IsAny<string>(), It.IsAny<QueryOptions>()))
                .ReturnsAsync(queryResult);

            // act
            var result = await _handler.Handle(req, CancellationToken.None);

            // assert
            MockCluster.Verify(m => m.QueryAsync<TriviaQuestion>(It.IsAny<string>(),It.IsAny<QueryOptions>()),
                Times.Once);
            var resultList = await result.ToListAsync();
            Assert.That(resultList, Is.Not.Null);
        }
    }
}