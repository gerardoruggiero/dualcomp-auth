using Dualcomp.Auth.Application.Titles.GetTitles;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Moq;

namespace Dualcomp.Auth.UnitTests.Application.Titles;

public class GetTitlesQueryHandlerTests
{
	[Fact]
	public async Task Handle_Should_Return_All_Titles()
	{
		// Arrange
		var mockRepository = new Mock<ITitleRepository>();
		var expectedTypes = new List<TitleEntity>
		{
			TitleEntity.Create("Ingeniero", "Título de Ingeniero"),
			TitleEntity.Create("Doctor", "Título de Doctor"),
			TitleEntity.Create("Licenciado", "Título de Licenciado"),
			TitleEntity.Create("Técnico", "Título de Técnico")
		};
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedTypes);

		var handler = new GetTitlesQueryHandler(mockRepository.Object);
		var query = new GetTitlesQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(4, result.Titles.Count());
		Assert.Contains(result.Titles, t => t.Name == "Ingeniero" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.Titles, t => t.Name == "Doctor" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.Titles, t => t.Name == "Licenciado" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.Titles, t => t.Name == "Técnico" && !string.IsNullOrEmpty(t.Id));
	}

	[Fact]
	public async Task Handle_Should_Return_Empty_List_When_No_Types()
	{
		// Arrange
		var mockRepository = new Mock<ITitleRepository>();
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<TitleEntity>());

		var handler = new GetTitlesQueryHandler(mockRepository.Object);
		var query = new GetTitlesQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result.Titles);
	}

	[Fact]
	public void Constructor_With_Null_Repository_Should_Throw_ArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new GetTitlesQueryHandler(null!));
	}
}

