using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Requests;
using MattsTwitchBot.Web.Controllers;
using MattsTwitchBot.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Controllers.TriviaControllerTests
{
    [TestFixture]
    public class GetRandomTriviaQuestionTests
    {
        private TriviaController _controller;
        private Mock<IMediator> _mockMediator;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new TriviaController(_mockMediator.Object);
        }

        [Test]
        public async Task Mediating_to_random_trivia_question_with_default_amount()
        {
            // arrange

            // act
            await _controller.GetRandomTriviaQuestions();

            // assert
            _mockMediator.Verify(m => m.Send(It.Is<GetRandomTriviaQuestions>(r => r.NumQuestions == 10), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Action_returns_a_trivia_view_model()
        {
            // arrange

            // act
            var result = await _controller.GetRandomTriviaQuestions();

            // assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.TypeOf<TriviaQuestionViewModel>());
        }
    }
}