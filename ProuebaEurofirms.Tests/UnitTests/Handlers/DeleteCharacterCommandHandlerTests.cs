using Microsoft.Extensions.Logging;
using Moq;
using PruebaEurofirms.Domain.Entities;
using PruebaEurofirms.Domain.Repositories.Contract;
using PruebaEurofirms.Infrastructure.Handlers;

namespace PruebaEurofirms.Tests.UnitTests.Handlers
{
    public class DeleteCharacterCommandHandlerTests
    {
        private readonly Mock<ICharacterRepository> _characterRepoMock = new();
        private readonly Mock<ILogger<DeleteCharacterCommandHandler>> _loggerMock = new();

        private DeleteCharacterCommandHandler CreateHandler()
            => new(_characterRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_CharacterExists_RemovesAndReturnsTrue()
        {
            var apiId = 42;
            var character = new Character { ApiId = apiId, Name = "Test" };

            _characterRepoMock
                .Setup(r => r.GetByApiIdAsync(apiId, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(character);

            _characterRepoMock.Setup(r => r.Remove(It.IsAny<Character>()));
            _characterRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var handler = CreateHandler();

            var result = await handler.Handle(new DeleteCharacterCommand(apiId), CancellationToken.None);

            Assert.True(result);
            _characterRepoMock.Verify(r => r.Remove(It.Is<Character>(c => c.ApiId == apiId)), Times.Once);
            _characterRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("Attempting to delete character")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("deleted successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CharacterNotFound_ThrowsKeyNotFoundException_AndLogsError()
        {
            var apiId = 1000;

            _characterRepoMock
                .Setup(r => r.GetByApiIdAsync(apiId, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Character?)null);

            var handler = CreateHandler();

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new DeleteCharacterCommand(apiId), CancellationToken.None));
            Assert.Contains(apiId.ToString(), ex.Message);

            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("not found")),
                It.Is<Exception>(e => e == null),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

            _characterRepoMock.Verify(r => r.Remove(It.IsAny<Character>()), Times.Never);
            _characterRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_SaveChangesThrows_LogsAndThrowsWrappedException()
        {
            var apiId = 7;
            var character = new Character { ApiId = apiId, Name = "WillFail" };
            var inner = new InvalidOperationException("DB failed");

            _characterRepoMock
                .Setup(r => r.GetByApiIdAsync(apiId, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(character);

            _characterRepoMock.Setup(r => r.Remove(It.IsAny<Character>()));
            _characterRepoMock.Setup(r => r.SaveChangesAsync()).ThrowsAsync(inner);

            var handler = CreateHandler();

            var thrown = await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(new DeleteCharacterCommand(apiId), CancellationToken.None));

            Assert.Equal("An unexpected error occurred while deleting the character. Please try again later.", thrown.Message);
            Assert.Same(inner, thrown.InnerException);

            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("An unexpected error occurred while deleting character")),
                It.Is<Exception>(e => e == inner),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

            _characterRepoMock.Verify(r => r.Remove(It.Is<Character>(c => c.ApiId == apiId)), Times.Once);
            _characterRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
