using Dualcomp.Auth.Application.Modulos.GetModulos;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Moq;

namespace Dualcomp.Auth.UnitTests.Application.Modulos;

public class GetModulosQueryHandlerTests
{
	[Fact]
	public async Task Handle_Should_Return_All_Modulos()
	{
		// Arrange
		var mockRepository = new Mock<IModuloRepository>();
		var expectedTypes = new List<ModuloEntity>
		{
			ModuloEntity.Create("Usuarios", "Módulo de gestión de usuarios"),
			ModuloEntity.Create("Empresas", "Módulo de gestión de empresas"),
			ModuloEntity.Create("Reportes", "Módulo de reportes y estadísticas"),
			ModuloEntity.Create("Configuración", "Módulo de configuración del sistema")
		};
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedTypes);

		var handler = new GetModulosQueryHandler(mockRepository.Object);
		var query = new GetModulosQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(4, result.Modulos.Count());
		Assert.Contains(result.Modulos, t => t.Name == "Usuarios" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.Modulos, t => t.Name == "Empresas" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.Modulos, t => t.Name == "Reportes" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.Modulos, t => t.Name == "Configuración" && !string.IsNullOrEmpty(t.Id));
	}

	[Fact]
	public async Task Handle_Should_Return_Empty_List_When_No_Types()
	{
		// Arrange
		var mockRepository = new Mock<IModuloRepository>();
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<ModuloEntity>());

		var handler = new GetModulosQueryHandler(mockRepository.Object);
		var query = new GetModulosQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result.Modulos);
	}

	[Fact]
	public void Constructor_With_Null_Repository_Should_Throw_ArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new GetModulosQueryHandler(null!));
	}
}
