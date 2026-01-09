using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.WebApi.Controllers;
using Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes;
using Dualcomp.Auth.Application.PhoneTypes.CreatePhoneType;
using Dualcomp.Auth.Application.PhoneTypes.UpdatePhoneType;
using Dualcomp.Auth.Application.PhoneTypes.ActivatePhoneType;
using Dualcomp.Auth.Application.PhoneTypes.DeactivatePhoneType;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Moq;

namespace Dualcomp.Auth.UnitTests.WebApi.Controllers;

public class PhoneTypesControllerTests
{
	[Fact]
	public async Task GetPhoneTypes_Should_Return_Ok_With_PhoneTypes()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreatePhoneTypeCommand, CreatePhoneTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdatePhoneTypeCommand, UpdatePhoneTypeResult>>();
		var mockActivateHandler = new Mock<ICommandHandler<ActivatePhoneTypeCommand>>();
		var mockDeactivateHandler = new Mock<ICommandHandler<DeactivatePhoneTypeCommand>>();
		
		var expectedResult = new GetPhoneTypesResult(new List<PhoneTypeItem>
		{
			new PhoneTypeItem(Guid.NewGuid().ToString(), "Principal", "Teléfono principal", true),
			new PhoneTypeItem(Guid.NewGuid().ToString(), "Móvil", "Teléfono móvil", true),
			new PhoneTypeItem(Guid.NewGuid().ToString(), "Fax", "Número de fax", true),
			new PhoneTypeItem(Guid.NewGuid().ToString(), "WhatsApp", "Número de WhatsApp", true)
		});
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetPhoneTypesQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResult);

		var controller = new PhoneTypesController(
			mockQueryHandler.Object, 
			mockCreateHandler.Object, 
			mockUpdateHandler.Object,
			mockActivateHandler.Object,
			mockDeactivateHandler.Object);

		// Act
		var result = await controller.GetTypes(CancellationToken.None);

		// Assert
		Assert.IsType<OkObjectResult>(result);
		var okResult = result as OkObjectResult;
		Assert.Equal(expectedResult, okResult!.Value);
	}

	[Fact]
	public async Task GetPhoneTypes_Should_Return_BadRequest_On_Exception()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreatePhoneTypeCommand, CreatePhoneTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdatePhoneTypeCommand, UpdatePhoneTypeResult>>();
		var mockActivateHandler = new Mock<ICommandHandler<ActivatePhoneTypeCommand>>();
		var mockDeactivateHandler = new Mock<ICommandHandler<DeactivatePhoneTypeCommand>>();
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetPhoneTypesQuery>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		var controller = new PhoneTypesController(
			mockQueryHandler.Object, 
			mockCreateHandler.Object, 
			mockUpdateHandler.Object,
			mockActivateHandler.Object,
			mockDeactivateHandler.Object);

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
		var mockCreateHandler = new Mock<ICommandHandler<CreatePhoneTypeCommand, CreatePhoneTypeResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdatePhoneTypeCommand, UpdatePhoneTypeResult>>();
		var mockActivateHandler = new Mock<ICommandHandler<ActivatePhoneTypeCommand>>();
		var mockDeactivateHandler = new Mock<ICommandHandler<DeactivatePhoneTypeCommand>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new PhoneTypesController(
			null!, 
			mockCreateHandler.Object, 
			mockUpdateHandler.Object,
			mockActivateHandler.Object,
			mockDeactivateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_CreateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdatePhoneTypeCommand, UpdatePhoneTypeResult>>();
		var mockActivateHandler = new Mock<ICommandHandler<ActivatePhoneTypeCommand>>();
		var mockDeactivateHandler = new Mock<ICommandHandler<DeactivatePhoneTypeCommand>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new PhoneTypesController(
			mockQueryHandler.Object, 
			null!, 
			mockUpdateHandler.Object,
			mockActivateHandler.Object,
			mockDeactivateHandler.Object));
	}

	[Fact]
	public void Constructor_With_Null_UpdateHandler_Should_Throw_ArgumentNullException()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreatePhoneTypeCommand, CreatePhoneTypeResult>>();
		var mockActivateHandler = new Mock<ICommandHandler<ActivatePhoneTypeCommand>>();
		var mockDeactivateHandler = new Mock<ICommandHandler<DeactivatePhoneTypeCommand>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new PhoneTypesController(
			mockQueryHandler.Object, 
			mockCreateHandler.Object, 
			null!,
			mockActivateHandler.Object,
			mockDeactivateHandler.Object));
	}
}
