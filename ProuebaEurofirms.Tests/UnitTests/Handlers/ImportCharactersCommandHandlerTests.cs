using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PruebaEurofirms.Domain.Entities;
using PruebaEurofirms.Domain.Repositories.Contract;
using PruebaEurofirms.Infrastructure.Handlers;
using PruebaEurofirms.Infrastructure.Models.DTO;

namespace PruebaEurofirms.Tests.UnitTests.Handlers
{
    public class ImportCharactersCommandHandlerTests
    {
        private readonly Mock<ICharacterRepository> _characterRepoMock = new();
        private readonly Mock<IEpisodeRepository> _episodeRepoMock = new();
        private readonly Mock<IRickAndMortyApiClient> _apiClientMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<ImportCharactersCommandHandler>> _loggerMock = new();

        private ImportCharactersCommandHandler CreateHandler(HttpClient? httpClient = null)
        {
            return new ImportCharactersCommandHandler(
                _characterRepoMock.Object,
                _episodeRepoMock.Object,
                httpClient ?? new HttpClient(),
                _apiClientMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_NoNewCharacters_ReturnsZero_AndDoesNotAdd()
        {
            var dto = new RickAndMortyCharacterDto { Id = 1, Name = "Rick", Episode = new List<string> { "ep/1" } };
            _apiClientMock.Setup(c => c.GetAllCharactersAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new List<RickAndMortyCharacterDto> { dto });

            _mapperMock.Setup(m => m.Map<Character>(It.IsAny<RickAndMortyCharacterDto>()))
                       .Returns((RickAndMortyCharacterDto s) => new Character { ApiId = s.Id, Name = s.Name });

            _mapperMock.Setup(m => m.Map<List<Episode>>(It.IsAny<List<string>>()))
                       .Returns((List<string> src) => src.Select((s, i) => new Episode { ApiId = i + 1, Url = s }).ToList());

            _characterRepoMock.Setup(r => r.GetAllAsync(false, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(new List<Character> { new Character { ApiId = 1 } });

            var handler = CreateHandler();

            var result = await handler.Handle(new ImportCharactersCommand(), CancellationToken.None);

            Assert.Equal(0, result);
            _episodeRepoMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Episode>>()), Times.Never);
            _characterRepoMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Character>>()), Times.Never);
            _characterRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_NewCharactersAndNewEpisodes_AddsAndReturnsCount()
        {
            var dto1 = new RickAndMortyCharacterDto { Id = 10, Name = "Morty", Episode = new List<string> { "ep/100", "ep/200" } };
            var dto2 = new RickAndMortyCharacterDto { Id = 11, Name = "Summer", Episode = new List<string> { "ep/200" } };

            _apiClientMock.Setup(c => c.GetAllCharactersAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new List<RickAndMortyCharacterDto> { dto1, dto2 });

            _mapperMock.Setup(m => m.Map<Character>(It.IsAny<RickAndMortyCharacterDto>()))
                       .Returns((RickAndMortyCharacterDto s) => new Character { ApiId = s.Id, Name = s.Name });

            _mapperMock.Setup(m => m.Map<List<Episode>>(It.IsAny<List<string>>()))
                       .Returns((List<string> src) =>
                           src.Select(s =>
                           {
                               var seg = s.Split('/').Last();
                               if (int.TryParse(seg.Replace("ep", "").Trim(), out var v))
                                   return new Episode { ApiId = v, Url = s };
                               return new Episode { ApiId = s.GetHashCode(), Url = s };
                           }).ToList());

            _characterRepoMock.Setup(r => r.GetAllAsync(false, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(new List<Character>());

            _episodeRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new List<Episode>());

            _episodeRepoMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Episode>>()))
                            .Returns(Task.CompletedTask);

            _characterRepoMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Character>>()))
                              .Returns(Task.CompletedTask);

            _characterRepoMock.Setup(r => r.SaveChangesAsync())
                              .Returns(Task.CompletedTask);

            var handler = CreateHandler();

            var result = await handler.Handle(new ImportCharactersCommand(), CancellationToken.None);

            Assert.Equal(2, result);

            _episodeRepoMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Episode>>(eps => eps.Count() == 2)), Times.Once);

            _characterRepoMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Character>>(cs => cs.Count() == 2)), Times.Once);
            _characterRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ApiThrows_ThrowsApplicationException()
        {
            _apiClientMock.Setup(c => c.GetAllCharactersAsync(It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new InvalidOperationException("api fail"));

            var handler = CreateHandler();

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => handler.Handle(new ImportCharactersCommand(), CancellationToken.None));
            Assert.Contains("Failed to import characters", ex.Message);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("An error occurred while importing characters")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_DuplicateEpisodeReferencedByMultipleCharacters_AddsEpisodeOnlyOnce()
        {
            var dto1 = new RickAndMortyCharacterDto { Id = 21, Name = "A", Episode = new List<string> { "ep/5" } };
            var dto2 = new RickAndMortyCharacterDto { Id = 22, Name = "B", Episode = new List<string> { "ep/5" } };

            _apiClientMock.Setup(c => c.GetAllCharactersAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new List<RickAndMortyCharacterDto> { dto1, dto2 });

            _mapperMock.Setup(m => m.Map<Character>(It.IsAny<RickAndMortyCharacterDto>()))
                       .Returns((RickAndMortyCharacterDto s) => new Character { ApiId = s.Id, Name = s.Name });

            _mapperMock.Setup(m => m.Map<List<Episode>>(It.IsAny<List<string>>()))
                       .Returns((List<string> src) => src.Select(s =>
                       {
                           var seg = s.Split('/').Last();
                           if (int.TryParse(seg.Replace("ep", "").Trim(), out var v))
                               return new Episode { ApiId = v, Url = s };
                           return new Episode { ApiId = s.GetHashCode(), Url = s };
                       }).ToList());

            _characterRepoMock.Setup(r => r.GetAllAsync(false, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(new List<Character>());

            _episodeRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new List<Episode>());

            IEnumerable<Episode>? passedEpisodes = null;
            _episodeRepoMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Episode>>()))
                            .Callback<IEnumerable<Episode>>(eps => passedEpisodes = eps)
                            .Returns(Task.CompletedTask);

            _characterRepoMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Character>>()))
                              .Returns(Task.CompletedTask);

            _characterRepoMock.Setup(r => r.SaveChangesAsync())
                              .Returns(Task.CompletedTask);

            var handler = CreateHandler();

            var result = await handler.Handle(new ImportCharactersCommand(), CancellationToken.None);

            Assert.Equal(2, result);
            Assert.NotNull(passedEpisodes);
            Assert.Single(passedEpisodes!.Select(e => e.ApiId).Distinct());
            _episodeRepoMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Episode>>(eps => eps.Count() == 1)), Times.Once);
            _characterRepoMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Character>>(cs => cs.Count() == 2)), Times.Once);
        }
    }
}