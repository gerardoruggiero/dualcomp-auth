using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.WebApi.Controllers;
using Dualcomp.Auth.Application.DocumentTypes.GetDocumentTypes;
using Dualcomp.Auth.Application.DocumentTypes.CreateDocumentType;
using Dualcomp.Auth.Application.DocumentTypes.UpdateDocumentType;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Moq;

namespace Dualcomp.Auth.UnitTests.WebApi.Controllers;

public class DocumentTypesControllerTests
{
	[Fact]
	public async Task GetDocumentTypes_Should_Return_Ok_With_DocumentTypes()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetDocumentTypesQuery, GetDocumentTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateDocumentTypeCommand, CreateDocumentTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateDocumentTypeCommand, UpdateDocumentTypeResult>>();
		
		var expectedResult = new GetDocumentTypesResult(new List<DocumentTypeItem>
		{
			new DocumentTypeItem(Guid.NewGuid().ToString(), "DNI", "Documento Nacional de Identidad", true),
			new DocumentTypeItem(Guid.NewGuid().ToString(), "Pasaporte", "Pasaporte", true),
			new DocumentTypeItem(Guid.NewGuid().ToString(), "Cédula", "Cédula de Identidad", true),
			new DocumentTypeItem(Guid.NewGuid().ToString(), "RUC", "Registro Único de Contribuyentes", true)
		});
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetDocumentTypesQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResult);

		var controller = new DocumentTypesController(
			mockQueryHandler.Object, 
			mockCreateHandler.Object, 
			mockUpdateHandler.Object);

		// Act
		var result = await controller.GetTypes(CancellationToken.None);

		// Assert
		Assert.IsType<OkObjectResult>(result);
		var okResult = result as OkObjectResult;
		Assert.Equal(expectedResult, okResult!.Value);
	}

	[Fact]
	public async Task GetDocumentTypes_Should_Return_BadRequest_On_Exception()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetDocumentTypesQuery, GetDocumentTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateDocumentTypeCommand, CreateDocumentTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateDocumentTypeCommand, UpdateDocumentTypeResult>>();
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetDocumentTypesQuery>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		var controller = new DocumentTypesController(
			mockQueryHandler.Object, 
			mockCreateHandler.Object, 
			mockUpdateHandler.Object);

		// Act
		var result = await controller.GetTypes(CancellationToken.None);

		// Assert
		Assert.IsType<BadRequestObjectResult>(result);
		var badRequestResult = result as BadRequestObjectResult;
		Assert.NotNull(badRequestResult!.Value);
	}

	[Fact]
	public void Constructor_With_Null_QueryHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockCreateHandler = new Mock<ICommandHandler<CreateDocumentTypeCommand, CreateDocumentTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateDocumentTypeCommand, UpdateDocumentTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new DocumentTypesController(null!, mockCreateHandler.Object, mockUpdateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_CreateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetDocumentTypesQuery, GetDocumentTypesResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateDocumentTypeCommand, UpdateDocumentTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new DocumentTypesController(mockQueryHandler.Object, null!, mockUpdateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_UpdateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetDocumentTypesQuery, GetDocumentTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateDocumentTypeCommand, CreateDocumentTypeResult>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new DocumentTypesController(mockQueryHandler.Object, mockCreateHandler.Object, null!));
	}
}

