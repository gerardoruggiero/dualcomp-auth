using Dualcomp.Auth.Application.DocumentTypes.GetDocumentTypes;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Moq;

namespace Dualcomp.Auth.UnitTests.Application.DocumentTypes;

public class GetDocumentTypesQueryHandlerTests
{
	[Fact]
	public async Task Handle_Should_Return_All_DocumentTypes()
	{
		// Arrange
		var mockRepository = new Mock<IDocumentTypeRepository>();
		var expectedTypes = new List<DocumentTypeEntity>
		{
			DocumentTypeEntity.Create("DNI", "Documento Nacional de Identidad"),
			DocumentTypeEntity.Create("Pasaporte", "Pasaporte"),
			DocumentTypeEntity.Create("Cédula", "Cédula de Identidad"),
			DocumentTypeEntity.Create("RUC", "Registro Único de Contribuyentes")
		};
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedTypes);

		var handler = new GetDocumentTypesQueryHandler(mockRepository.Object);
		var query = new GetDocumentTypesQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(4, result.DocumentTypes.Count());
		Assert.Contains(result.DocumentTypes, t => t.Name == "DNI" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.DocumentTypes, t => t.Name == "Pasaporte" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.DocumentTypes, t => t.Name == "Cédula" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.DocumentTypes, t => t.Name == "RUC" && !string.IsNullOrEmpty(t.Id));
	}

	[Fact]
	public async Task Handle_Should_Return_Empty_List_When_No_Types()
	{
		// Arrange
		var mockRepository = new Mock<IDocumentTypeRepository>();
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<DocumentTypeEntity>());

		var handler = new GetDocumentTypesQueryHandler(mockRepository.Object);
		var query = new GetDocumentTypesQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result.DocumentTypes);
	}

	[Fact]
	public void Constructor_With_Null_Repository_Should_Throw_ArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new GetDocumentTypesQueryHandler(null!));
	}
}

