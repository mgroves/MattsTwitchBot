using System;
using MattsTwitchBot.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.ViewModels.TriviaQuestionEditModelTests
{
    [TestFixture]
    public class ValidateTests
    {
        private TriviaQuestionEditModel _model;
        private ModelStateDictionary _modelState;

        [SetUp]
        public void Setup()
        {
            _model = new TriviaQuestionEditModel
            {
                Question = "This is a question " + Guid.NewGuid(),
                Option0 = "option 1 " + Guid.NewGuid(),
                Option1 = "option 2 " + Guid.NewGuid(),
                Option2 = "option 3 " + Guid.NewGuid(),
                Option3 = "option 4 " + Guid.NewGuid(),
                Answer = new Random().Next(0, 4)
            };
            _modelState = new ModelStateDictionary();
        }

        [TestCase(null)]
        [TestCase("")]
        public void Question_is_required(string emptyQuestion)
        {
            // arrange
            _model.Question = emptyQuestion;

            // act
            _model.Validate(_modelState);

            // assert
            Assert.That(_modelState.Count, Is.EqualTo(1));
            Assert.That(_modelState.ContainsKey("QuestionNullOrEmpty"), Is.True);
        }

        [TestCase(0, null)]
        [TestCase(0, "")]
        [TestCase(1, null)]
        [TestCase(1, "")]
        [TestCase(2, null)]
        [TestCase(2, "")]
        [TestCase(3, null)]
        [TestCase(3, "")]
        public void Answers_are_required(int num, string emptyOption)
        {
            // arrange
            if (num == 0) _model.Option0 = emptyOption;
            if (num == 1) _model.Option1 = emptyOption;
            if (num == 2) _model.Option2 = emptyOption;
            if (num == 3) _model.Option3 = emptyOption;

            // act
            _model.Validate(_modelState);

            // assert
            Assert.That(_modelState.ContainsKey($"OptionNullOrEmpty"), Is.True);
        }

        [TestCase(0,true)]
        [TestCase(1,true)]
        [TestCase(2,true)]
        [TestCase(3,true)]
        [TestCase(-1,false)]
        [TestCase(4,false)]
        [TestCase(4123412,false)]
        [TestCase(-123412,false)]
        [TestCase(null, false)]
        public void An_answer_number_must_be_selected(int? number, bool isValid)
        {
            // arrange
            _model.Answer = number;

            // act
            _model.Validate(_modelState);

            // assert
            Assert.That(_modelState.ContainsKey("AnswerNumberInvalid"), Is.EqualTo(!isValid));
        }
    }
}