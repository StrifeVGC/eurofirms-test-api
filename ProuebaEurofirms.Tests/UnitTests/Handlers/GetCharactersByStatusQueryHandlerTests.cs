using Microsoft.Extensions.Logging;
using Moq;
using PruebaEurofirms.Domain.Entities;
using PruebaEurofirms.Domain.Repositories.Contract;
using PruebaEurofirms.Infrastructure.Handlers;

namespace PruebaEurofirms.Tests.UnitTests.Handlers
{
    public class GetCharactersByStatusQueryHandlerTests
    {
        private readonly Mock<ICharacterRepository> _characterRepositoryMock = new();
        private readonly Mock<ILogger<GetCharactersByStatusQueryHandler>> _loggerMock = new();

        private GetCharactersByStatusQueryHandler CreateHandler()
            => new GetCharactersByStatusQueryHandler(_characterRepositoryMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_WhenCharactersExist_ReturnsListAndLogsInformation()
        {
            var status = "Alive";
            var expected = new List<Character>
            {
                new Character { Id = 1, ApiId = 100, Name = "Rick", Status = status },
                new Character { Id = 2, ApiId = 101, Name = "Morty", Status = status }
            };

            _characterRepositoryMock
                .Setup(r => r.GetByStatusAsync(status))
                .ReturnsAsync(expected);

            var handler = CreateHandler();

            var result = await handler.Handle(new GetCharactersByStatusQuery(status), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(expected.Count, result.Count);
            Assert.Equal(expected.Select(c => c.ApiId), result.Select(c => c.ApiId));

            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("Starting retrieval of characters")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("characters matched the status")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoCharacters_ReturnsEmptyListAndLogsZero()
        {
            var status = "Unknown";
            _characterRepositoryMock
                .Setup(r => r.GetByStatusAsync(status))
                .ReturnsAsync(new List<Character>());

            var handler = CreateHandler();

            var result = await handler.Handle(new GetCharactersByStatusQuery(status), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);

            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("Starting retrieval of characters")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString().Contains("0 characters matched the status") ||
                    state.ToString().Contains("characters matched the status 'Unknown'")
                ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrows_LogsErrorAndRethrows()
        {
            var status = "Dead";
            var ex = new InvalidOperationException("DB fail");
            _characterRepositoryMock
                .Setup(r => r.GetByStatusAsync(status))
                .ThrowsAsync(ex);

            var handler = CreateHandler();

            var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(new GetCharactersByStatusQuery(status), CancellationToken.None));

            Assert.Same(ex, thrown);

            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("An error occurred while retrieving characters")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
