using Dualcomp.Auth.Application.EmailTypes.GetEmailTypes;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Moq;

namespace Dualcomp.Auth.UnitTests.Application.EmailTypes;

public class GetEmailTypesQueryHandlerTests
{
	[Fact]
	public async Task Handle_Should_Return_All_EmailTypes()
	{
		// Arrange
		var mockRepository = new Mock<IEmailTypeRepository>();
		var expectedTypes = new List<EmailTypeEntity>
		{
			EmailTypeEntity.Create("Principal", "Email principal de contacto"),
			EmailTypeEntity.Create("Facturación", "Email para facturación"),
			EmailTypeEntity.Create("Soporte", "Email para soporte técnico"),
			EmailTypeEntity.Create("Comercial", "Email para ventas y comercial")
		};
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedTypes);

		var handler = new GetEmailTypesQueryHandler(mockRepository.Object);
		var query = new GetEmailTypesQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(4, result.EmailTypes.Count());
		Assert.Contains(result.EmailTypes, t => t.Name == "Principal" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.EmailTypes, t => t.Name == "Facturación" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.EmailTypes, t => t.Name == "Soporte" && !string.IsNullOrEmpty(t.Id));
		Assert.Contains(result.EmailTypes, t => t.Name == "Comercial" && !string.IsNullOrEmpty(t.Id));
	}

	[Fact]
	public async Task Handle_Should_Return_Empty_List_When_No_Types()
	{
		// Arrange
		var mockRepository = new Mock<IEmailTypeRepository>();
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<EmailTypeEntity>());

		var handler = new GetEmailTypesQueryHandler(mockRepository.Object);
		var query = new GetEmailTypesQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result.EmailTypes);
	}

	[Fact]
	public void Constructor_With_Null_Repository_Should_Throw_ArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new GetEmailTypesQueryHandler(null!));
	}
}
