using FluentAssertions.Execution;
using NSubstitute.ReturnsExtensions;
using OStats.API.Commands;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.API.Commands;

public class CreateProjectHandlerTest
{
    public IUserRepository userRepository { get; }
    public IProjectRepository projectRepository { get; }

    public CreateProjectCommandValidator validator { get; }

    public CreateProjectHandlerTest()
    {
        userRepository = Substitute.For<IUserRepository>();
        projectRepository = Substitute.For<IProjectRepository>();
        validator = new CreateProjectCommandValidator();
    }

    [Fact]
    public async void Should_Fail_If_User_Is_Not_Found()
    {
        var command = new CreateProjectCommand("user_auth_id", "Test Title");
        userRepository.FindUserByAuthIdentityAsync(Arg.Any<string>()).ReturnsNullForAnyArgs();
        var handler = new CreateProjectCommandHandler(validator,
                                                      projectRepository,
                                                      userRepository);
        var result = await handler.Handle(command, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ValidationFailures.Should().NotBeNull().And.NotBeEmpty();
            result.Value.Should().BeNull();
        }
    }
}