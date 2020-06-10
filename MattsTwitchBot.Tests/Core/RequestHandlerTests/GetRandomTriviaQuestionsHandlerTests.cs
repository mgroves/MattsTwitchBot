using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.N1QL;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.RequestHandlers.Trivia;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class GetRandomTriviaQuestionsHandlerTests
    {
        private GetRandomTriviaQuestionsHandler _handler;
        private Mock<IBucket> _mockBucket;
        private Mock<ITwitchBucketProvider> _mockBucketProvider;

        [SetUp]
        public void Setup()
        {
            _mockBucket = new Mock<IBucket>();
            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(m => m.GetBucket()).Returns(_mockBucket.Object);
            _handler = new GetRandomTriviaQuestionsHandler(_mockBucketProvider.Object);
        }

        [Test]
        public async Task Query_is_execute_to_get_questions()
        {
            // arrange
            var queryResult = new FakeQueryResult<TriviaQuestion>();
            queryResult.FakeRows = new List<TriviaQuestion>();
            var req = new GetRandomTriviaQuestions(numQuestions: 13);
            _mockBucket.Setup(m => m.QueryAsync<TriviaQuestion>(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            // act
            var result = await _handler.Handle(req, CancellationToken.None);

            // assert
            _mockBucket.Verify(m => m.QueryAsync<TriviaQuestion>(It.IsAny<IQueryRequest>(),It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}