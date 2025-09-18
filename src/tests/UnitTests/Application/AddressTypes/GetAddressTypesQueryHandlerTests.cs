using Dualcomp.Auth.Application.AddressTypes.GetAddressTypes;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Moq;

namespace Dualcomp.Auth.UnitTests.Application.AddressTypes;

public class GetAddressTypesQueryHandlerTests
{
	[Fact]
	public async Task Handle_Should_Return_All_AddressTypes()
	{
		// Arrange
		var mockRepository = new Mock<IAddressTypeRepository>();
		var expectedTypes = new List<AddressTypeEntity>
		{
			AddressTypeEntity.Create("Principal", "Dirección principal de la empresa"),
			AddressTypeEntity.Create("Sucursal", "Dirección de sucursal o filial"),
			AddressTypeEntity.Create("Facturación", "Dirección para facturación"),
			AddressTypeEntity.Create("Envío", "Dirección para envíos y entregas")
		};
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedTypes);

		var handler = new GetAddressTypesQueryHandler(mockRepository.Object);
		var query = new GetAddressTypesQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(4, result.AddressTypes.Count());
		Assert.Contains(result.AddressTypes, t => t.Name == "Principal" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.AddressTypes, t => t.Name == "Sucursal" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.AddressTypes, t => t.Name == "Facturación" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.AddressTypes, t => t.Name == "Envío" && !string.IsNullOrEmpty(t.Id));
	}

	[Fact]
	public async Task Handle_Should_Return_Empty_List_When_No_Types()
	{
		// Arrange
		var mockRepository = new Mock<IAddressTypeRepository>();
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<AddressTypeEntity>());

		var handler = new GetAddressTypesQueryHandler(mockRepository.Object);
		var query = new GetAddressTypesQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result.AddressTypes);
	}

	[Fact]
	public void Constructor_With_Null_Repository_Should_Throw_ArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new GetAddressTypesQueryHandler(null!));
	}
}
