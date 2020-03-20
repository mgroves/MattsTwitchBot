using System;
using System.Collections.Generic;
using MattsTwitchBot.Tests.Fakes;
using MattsTwitchBot.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Filters
{
    [TestFixture]
    public class BearerTokenFilterTests
    {
        private FakeConfiguration _config;
        private BearerTokenFilter _filter;
        private Mock<HttpContext> _mockHttpContext;
        private string _validToken;
        private Mock<HttpRequest> _mockHttpRequest;
        private ActionContext _actionContext;
        private AuthorizationFilterContext _actionExecutingContext;
        private Mock<HttpResponse> _mockHttpResponse;
        private Mock<IResponseCookies> _mockResponseCookies;
        private Mock<IRequestCookieCollection> _mockRequestCookiesCollection;

        [SetUp]
        public void Setup()
        {
            _validToken = Guid.NewGuid().ToString();
            _config = new FakeConfiguration();
            _config.FakeValues["BearerToken"] = _validToken;

            _filter = new BearerTokenFilter(_config);

            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpRequest = new Mock<HttpRequest>();
            _mockHttpResponse = new Mock<HttpResponse>();
            _mockResponseCookies = new Mock<IResponseCookies>();
            _mockRequestCookiesCollection = new Mock<IRequestCookieCollection>();

            _actionContext = new ActionContext(_mockHttpContext.Object, new RouteData(), new ActionDescriptor());
            _actionExecutingContext = new AuthorizationFilterContext(_actionContext, new List<IFilterMetadata>());
        }

        [Test]
        public void If_theres_no_query_in_context_return_401()
        {
            // act
            _filter.OnAuthorization(_actionExecutingContext);

            // assert
            Assert.That(_actionExecutingContext.Result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public void If_theres_query_context_but_no_token_or_cookie_then_401()
        {
            // arrange - there must be an HTTP request with no cookie
            _mockHttpContext.Setup(m => m.Request)
                .Returns(_mockHttpRequest.Object);
            _mockHttpRequest.Setup(m => m.Query)
                .Returns(new QueryCollection());
            _mockHttpRequest.Setup(m => m.Cookies)
                .Returns(_mockRequestCookiesCollection.Object);

            // act
            _filter.OnAuthorization(_actionExecutingContext);

            // assert
            Assert.That(_actionExecutingContext.Result, Is.TypeOf<UnauthorizedResult>());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("not valid")]
        public void If_theres_a_token_in_querystring_but_its_not_valid_then_401(string tokenValue)
        {
            // arrange - there must be a cookie but it has an invalid value
            var query = new QueryCollection(new Dictionary<string, StringValues> { ["token"] = tokenValue });
            _mockHttpContext.Setup(m => m.Request)
                .Returns(_mockHttpRequest.Object);
            _mockHttpRequest.Setup(m => m.Query)
                .Returns(query);

            // act
            _filter.OnAuthorization(_actionExecutingContext);

            // assert
            Assert.That(_actionExecutingContext.Result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public void If_theres_a_valid_token_in_querystring_put_into_cookie_and_no_result()
        {
            // arrange - the valid token must be in the query
            var query = new QueryCollection(new Dictionary<string, StringValues> { ["token"] = _validToken });
            _mockHttpContext.Setup(m => m.Request)
                .Returns(_mockHttpRequest.Object);
            _mockHttpRequest.Setup(m => m.Query)
                .Returns(query);

            // arrange - there must be a response for the token to put a cookie in
            _mockHttpContext.Setup(m => m.Response)
                .Returns(_mockHttpResponse.Object);
            _mockHttpResponse.Setup(m => m.Cookies)
                .Returns(_mockResponseCookies.Object);

            // act
            _filter.OnAuthorization(_actionExecutingContext);

            // assert - two things: cookie must be set and there is no 401 response
            _mockResponseCookies.Verify(m => m.Append("bearertoken", _validToken), Times.Once);
            Assert.That(_actionExecutingContext.Result, Is.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("not a valid value")]
        public void If_theres_an_invalid_token_in_cookie_return_401(string givenValue)
        {
            // arrange - http request
            _mockHttpContext.Setup(m => m.Request)
                .Returns(_mockHttpRequest.Object);

            // arrange - request cookies, bearertoken must exist but cannot be valid
            _mockHttpRequest.Setup(m => m.Cookies)
                .Returns(_mockRequestCookiesCollection.Object);
            _mockRequestCookiesCollection.Setup(m => m.ContainsKey("bearertoken"))
                .Returns(true);
            _mockRequestCookiesCollection.Setup(m => m["bearertoken"])
                .Returns(givenValue);

            // arrange - query (without token)
            var vals = new Dictionary<string, StringValues>();
            var query = new QueryCollection(vals);
            _mockHttpRequest.Setup(m => m.Query)
                .Returns(query);

            // act
            _filter.OnAuthorization(_actionExecutingContext);

            // assert
            Assert.That(_actionExecutingContext.Result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public void If_theres_a_valid_token_in_cookie_then_no_result()
        {
            // arrange - http request
            _mockHttpContext.Setup(m => m.Request)
                .Returns(_mockHttpRequest.Object);

            // arrange - request cookies, bearertoken must exist but cannot be valid
            _mockHttpRequest.Setup(m => m.Cookies)
                .Returns(_mockRequestCookiesCollection.Object);
            _mockRequestCookiesCollection.Setup(m => m.ContainsKey("bearertoken"))
                .Returns(true);
            _mockRequestCookiesCollection.Setup(m => m["bearertoken"])
                .Returns(_validToken);

            // arrange - query (without token)
            var vals = new Dictionary<string, StringValues>();
            var query = new QueryCollection(vals);
            _mockHttpRequest.Setup(m => m.Query)
                .Returns(query);

            // act
            _filter.OnAuthorization(_actionExecutingContext);

            // assert
            Assert.That(_actionExecutingContext.Result, Is.Null);
        }
    }
}