using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.WebApi.Controllers;
using Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Moq;

namespace Dualcomp.Auth.UnitTests.WebApi.Controllers;

public class PhoneTypesControllerTests
{
	[Fact]
	public async Task GetPhoneTypes_Should_Return_Ok_With_PhoneTypes()
	{
		// Arrange
		var mockHandler = new Mock<IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult>>();
		var expectedResult = new GetPhoneTypesResult(new List<PhoneTypeItem>
		{
			new PhoneTypeItem("Principal"),
			new PhoneTypeItem("Móvil"),
			new PhoneTypeItem("Fax"),
			new PhoneTypeItem("WhatsApp")
		});
		
		mockHandler.Setup(h => h.Handle(It.IsAny<GetPhoneTypesQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResult);

		var controller = new PhoneTypesController(mockHandler.Object);

		// Act
		var result = await controller.GetPhoneTypes(CancellationToken.None);

		// Assert
		Assert.IsType<OkObjectResult>(result);
		var okResult = result as OkObjectResult;
		Assert.Equal(expectedResult, okResult!.Value);
	}

	[Fact]
	public async Task GetPhoneTypes_Should_Return_BadRequest_On_Exception()
	{
		// Arrange
		var mockHandler = new Mock<IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult>>();
		mockHandler.Setup(h => h.Handle(It.IsAny<GetPhoneTypesQuery>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		var controller = new PhoneTypesController(mockHandler.Object);

		// Act
		var result = await controller.GetPhoneTypes(CancellationToken.None);

		// Assert
		Assert.IsType<BadRequestObjectResult>(result);
		var badRequestResult = result as BadRequestObjectResult;
		Assert.NotNull(badRequestResult!.Value);
	}

	[Fact]
	public void Constructor_With_Null_Handler_Should_Throw_ArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new PhoneTypesController(null!));
	}
}
