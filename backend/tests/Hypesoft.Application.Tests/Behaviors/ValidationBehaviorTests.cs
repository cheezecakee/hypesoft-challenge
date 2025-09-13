using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Hypesoft.Application.Common.Behaviors;
using MediatR;
using Moq;
using Xunit;

namespace Hypesoft.Application.Tests.Behaviors
{
    public class ValidationBehaviorTests
    {
        // Make this public so Moq can create a proxy
        public class TestRequest : IRequest<string>
        {
            public string? Name { get; set; }
        }

        [Fact]
        public async Task Should_Throw_When_Validation_Fails()
        {
            var validatorMock = new Mock<IValidator<TestRequest>>();
            validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Name", "Name is required") }));

            var behavior = new ValidationBehavior<TestRequest, string>(new[] { validatorMock.Object });

            var request = new TestRequest();

            await Assert.ThrowsAsync<ValidationException>(() =>
                behavior.Handle(request, ct => Task.FromResult("next called"), CancellationToken.None));
        }

        [Fact]
        public async Task Should_Call_Next_When_Validation_Passes()
        {
            var validatorMock = new Mock<IValidator<TestRequest>>();
            validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            bool nextCalled = false;
            RequestHandlerDelegate<string> next = ct =>
            {
                nextCalled = true;
                return Task.FromResult("success");
            };

            var behavior = new ValidationBehavior<TestRequest, string>(new[] { validatorMock.Object });

            var response = await behavior.Handle(new TestRequest(), next, CancellationToken.None);

            Assert.True(nextCalled);
            Assert.Equal("success", response);
        }
    }
}
