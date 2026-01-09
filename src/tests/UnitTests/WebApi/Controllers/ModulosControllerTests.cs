using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.WebApi.Controllers;
using Dualcomp.Auth.Application.Modulos.GetModulos;
using Dualcomp.Auth.Application.Modulos.CreateModulo;
using Dualcomp.Auth.Application.Modulos.UpdateModulo;
using Dualcomp.Auth.Application.Modulos.ActivateModulo;
using Dualcomp.Auth.Application.Modulos.DeactivateModulo;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Moq;

namespace Dualcomp.Auth.UnitTests.WebApi.Controllers;

public class ModulosControllerTests
{
	[Fact]
	public async Task GetModulos_Should_Return_Ok_With_Modulos()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetModulosQuery, GetModulosResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateModuloCommand, CreateModuloResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateModuloCommand, UpdateModuloResult>>();
		var mockActivateHandler = new Mock<ICommandHandler<ActivateModuloCommand>>();
		var mockDeactivateHandler = new Mock<ICommandHandler<DeactivateModuloCommand>>();
		
		var expectedResult = new GetModulosResult(new List<ModuloItem>
		{
			new ModuloItem(Guid.NewGuid().ToString(), "Usuarios", "Módulo de gestión de usuarios", true),
			new ModuloItem(Guid.NewGuid().ToString(), "Empresas", "Módulo de gestión de empresas", true),
			new ModuloItem(Guid.NewGuid().ToString(), "Reportes", "Módulo de reportes y estadísticas", true),
			new ModuloItem(Guid.NewGuid().ToString(), "Configuración", "Módulo de configuración del sistema", true)
		});
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetModulosQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResult);

		var controller = new ModulosController(
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
	public async Task GetModulos_Should_Return_BadRequest_On_Exception()
	{
		// Arrange
		var mockQueryHandler = new Mock<IQueryHandler<GetModulosQuery, GetModulosResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateModuloCommand, CreateModuloResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateModuloCommand, UpdateModuloResult>>();
		var mockActivateHandler = new Mock<ICommandHandler<ActivateModuloCommand>>();
		var mockDeactivateHandler = new Mock<ICommandHandler<DeactivateModuloCommand>>();
		
		mockQueryHandler.Setup(h => h.Handle(It.IsAny<GetModulosQuery>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		var controller = new ModulosController(
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
		var mockCreateHandler = new Mock<ICommandHandler<CreateModuloCommand, CreateModuloResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateModuloCommand, UpdateModuloResult>>();
		var mockActivateHandler = new Mock<ICommandHandler<ActivateModuloCommand>>();
		var mockDeactivateHandler = new Mock<ICommandHandler<DeactivateModuloCommand>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new ModulosController(
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
		var mockQueryHandler = new Mock<IQueryHandler<GetModulosQuery, GetModulosResult>>();
		var mockUpdateHandler = new Mock<ICommandHandler<UpdateModuloCommand, UpdateModuloResult>>();
		var mockActivateHandler = new Mock<ICommandHandler<ActivateModuloCommand>>();
		var mockDeactivateHandler = new Mock<ICommandHandler<DeactivateModuloCommand>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new ModulosController(
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
		var mockQueryHandler = new Mock<IQueryHandler<GetModulosQuery, GetModulosResult>>();
		var mockCreateHandler = new Mock<ICommandHandler<CreateModuloCommand, CreateModuloResult>>();
		var mockActivateHandler = new Mock<ICommandHandler<ActivateModuloCommand>>();
		var mockDeactivateHandler = new Mock<ICommandHandler<DeactivateModuloCommand>>();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new ModulosController(
			mockQueryHandler.Object, 
			mockCreateHandler.Object, 
			null!,
			mockActivateHandler.Object,
			mockDeactivateHandler.Object));
	}
}
