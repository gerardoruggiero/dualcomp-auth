using Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes;
using Dualcomp.Auth.Domain.Companies;
using Moq;

namespace Dualcomp.Auth.UnitTests.Application.PhoneTypes;

public class GetPhoneTypesQueryHandlerTests
{
	[Fact]
	public async Task Handle_Should_Return_All_PhoneTypes()
	{
		// Arrange
		var mockRepository = new Mock<IPhoneTypeRepository>();
		var expectedTypes = new List<PhoneTypeEntity>
		{
			PhoneTypeEntity.Create("Principal", "Teléfono principal de contacto"),
			PhoneTypeEntity.Create("Móvil", "Teléfono móvil"),
			PhoneTypeEntity.Create("Fax", "Número de fax"),
			PhoneTypeEntity.Create("WhatsApp", "Número de WhatsApp")
		};
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedTypes);

		var handler = new GetPhoneTypesQueryHandler(mockRepository.Object);
		var query = new GetPhoneTypesQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(4, result.PhoneTypes.Count());
		Assert.Contains(result.PhoneTypes, t => t.Value == "Principal");
		Assert.Contains(result.PhoneTypes, t => t.Value == "Móvil");
		Assert.Contains(result.PhoneTypes, t => t.Value == "Fax");
		Assert.Contains(result.PhoneTypes, t => t.Value == "WhatsApp");
	}

	[Fact]
	public async Task Handle_Should_Return_Empty_List_When_No_Types()
	{
		// Arrange
		var mockRepository = new Mock<IPhoneTypeRepository>();
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<PhoneTypeEntity>());

		var handler = new GetPhoneTypesQueryHandler(mockRepository.Object);
		var query = new GetPhoneTypesQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result.PhoneTypes);
	}

	[Fact]
	public void Constructor_With_Null_Repository_Should_Throw_ArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new GetPhoneTypesQueryHandler(null!));
	}
}
