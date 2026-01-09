using Dualcomp.Auth.Application.Companies.UpdateCompany;
using Dualcomp.Auth.Application.Companies;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
using Moq;
using Dualcomp.Auth.Application.Companies.GetCompany;

namespace Dualcomp.Auth.UnitTests.Application.Companies
{
    public class UpdateCompanyCommandHandlerTests
    {
        private readonly Mock<ICompanyRepository> _companyRepositoryMock;
        private readonly Mock<ICompanyContactService> _contactServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateCompanyCommandHandler _handler;

        public UpdateCompanyCommandHandlerTests()
        {
            _companyRepositoryMock = new Mock<ICompanyRepository>();
            _contactServiceMock = new Mock<ICompanyContactService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            
            _handler = new UpdateCompanyCommandHandler(
                _companyRepositoryMock.Object,
                _contactServiceMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WithDeletedEmployee_ShouldDeactivateEmployeeAndUser()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            // Crear empresa con un empleado y contactos básicos
            var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
            // Asignar el ID específico para el test
            company.GetType().GetProperty("Id")?.SetValue(company, companyId);
            var user = User.Create("John", "Doe", Email.Create("john@test.com"), HashedPassword.Create("hashedpassword"), companyId);
            var employee = Employee.Create("John Doe", "john@test.com", "+56912345678", company.Id, "Developer", DateTime.UtcNow, user);
            
            // Agregar empleado a la empresa
            company.AddEmployee(employee);
            
            // Agregar contactos básicos para que la empresa sea válida
            var address = CompanyAddress.Create(company.Id, Guid.NewGuid(), "Test Address", true);
            var email = CompanyEmail.Create(company.Id, Guid.NewGuid(), Email.Create("test@company.com"), true);
            var phone = CompanyPhone.Create(company.Id, Guid.NewGuid(), "+56912345678", true);
            var socialMedia = CompanySocialMedia.Create(company.Id, Guid.NewGuid(), "https://linkedin.com/company/test", true);
            
            company.AddAddress(address);
            company.AddEmail(email);
            company.AddPhone(phone);
            company.AddSocialMedia(socialMedia);

             var command = new UpdateCompanyCommand(
                 companyId,
                 "Updated Company",
                 TaxId.Create("12345678-9"),
                 new List<UpdateCompanyAddressDto>
                 {
                     new(Guid.NewGuid(), Guid.NewGuid(), "Test Address", true)
                 },
                 new List<UpdateCompanyEmailDto>
                 {
                     new(Guid.NewGuid(), Guid.NewGuid(), "test@company.com", true)
                 },
                 new List<UpdateCompanyPhoneDto>
                 {
                     new(Guid.NewGuid(), Guid.NewGuid(), "+56912345678", true)
                 },
                 new List<UpdateCompanySocialMediaDto>
                 {
                     new(Guid.NewGuid(), Guid.NewGuid(), "https://linkedin.com/company/test", true)
                 },
                 new List<UpdateCompanyEmployeeDto>
                 {
                     new(Guid.NewGuid(), "Jane Doe", "jane@test.com", "+56987654321", "Manager", null)
                 }, // Lista con un empleado diferente - el empleado original fue eliminado
                 new List<Guid>()
             );

            _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);
            _companyRepositoryMock.Setup(x => x.ExistsByTaxIdForOtherCompanyAsync(It.IsAny<string>(), companyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _contactServiceMock.Setup(x => x.ValidateRequiredContactsForUpdate(It.IsAny<IEnumerable<UpdateCompanyAddressDto>>(), It.IsAny<IEnumerable<UpdateCompanyEmailDto>>(), It.IsAny<IEnumerable<UpdateCompanyPhoneDto>>(), It.IsAny<IEnumerable<UpdateCompanySocialMediaDto>>()));
            _contactServiceMock.Setup(x => x.RemoveDeletedContactsAsync(It.IsAny<Company>(), It.IsAny<IEnumerable<UpdateCompanyAddressDto>>(), It.IsAny<IEnumerable<UpdateCompanyEmailDto>>(), It.IsAny<IEnumerable<UpdateCompanyPhoneDto>>(), It.IsAny<IEnumerable<UpdateCompanySocialMediaDto>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _contactServiceMock.Setup(x => x.ProcessAllContactsForUpdateAsync(It.IsAny<Company>(), It.IsAny<IEnumerable<UpdateCompanyAddressDto>>(), It.IsAny<IEnumerable<UpdateCompanyEmailDto>>(), It.IsAny<IEnumerable<UpdateCompanyPhoneDto>>(), It.IsAny<IEnumerable<UpdateCompanySocialMediaDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContactTypeNames(new Dictionary<Guid, string>(), new Dictionary<Guid, string>(), new Dictionary<Guid, string>(), new Dictionary<Guid, string>()));
            _contactServiceMock.Setup(x => x.ProcessEmployeesForUpdateAsync(It.IsAny<Company>(), It.IsAny<IEnumerable<UpdateCompanyEmployeeDto>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _contactServiceMock.Setup(x => x.BuildAddressResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
                .Returns(new List<CompanyAddressResult>());
            _contactServiceMock.Setup(x => x.BuildEmailResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
                .Returns(new List<CompanyEmailResult>());
            _contactServiceMock.Setup(x => x.BuildPhoneResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
                .Returns(new List<CompanyPhoneResult>());
            _contactServiceMock.Setup(x => x.BuildSocialMediaResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
                .Returns(new List<CompanySocialMediaResult>());
            _contactServiceMock.Setup(x => x.BuildEmployeeResults(It.IsAny<Company>()))
                .Returns(new List<CompanyEmployeeResult>());
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
             Assert.Equal(companyId, result.Id);
            Assert.Equal("Updated Company", result.Name);
            
            // Verificar que se llamó al método de desactivación de empleados
            _contactServiceMock.Verify(x => x.DeactivateDeletedEmployeesAsync(
                It.IsAny<Company>(), 
                It.IsAny<IEnumerable<UpdateCompanyEmployeeDto>>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldUpdateCompanySuccessfully()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
            // Asignar el ID específico para el test
            company.GetType().GetProperty("Id")?.SetValue(company, companyId);
            
            // Agregar contactos básicos para que la empresa sea válida
            var address = CompanyAddress.Create(company.Id, Guid.NewGuid(), "Test Address", true);
            var email = CompanyEmail.Create(company.Id, Guid.NewGuid(), Email.Create("test@company.com"), true);
            var phone = CompanyPhone.Create(company.Id, Guid.NewGuid(), "+56912345678", true);
            var socialMedia = CompanySocialMedia.Create(company.Id, Guid.NewGuid(), "https://linkedin.com/company/test", true);
            
            company.AddAddress(address);
            company.AddEmail(email);
            company.AddPhone(phone);
            company.AddSocialMedia(socialMedia);
            
            // Agregar un empleado para que la empresa sea válida
            var user = User.Create("John", "Doe", Email.Create("john@test.com"), HashedPassword.Create("hashedpassword"), company.Id);
            var employee = Employee.Create("John Doe", "john@test.com", "+56912345678", company.Id, "Developer", DateTime.UtcNow, user);
            company.AddEmployee(employee);
            
             var command = new UpdateCompanyCommand(
                 companyId,
                 "Updated Company",
                 TaxId.Create("12345678-9"),
                 new List<UpdateCompanyAddressDto>
                 {
                     new(Guid.NewGuid(), Guid.NewGuid(), "Test Address", true)
                 },
                 new List<UpdateCompanyEmailDto>
                 {
                     new(Guid.NewGuid(), Guid.NewGuid(), "test@company.com", true)
                 },
                 new List<UpdateCompanyPhoneDto>
                 {
                     new(Guid.NewGuid(), Guid.NewGuid(), "+56912345678", true)
                 },
                 new List<UpdateCompanySocialMediaDto>
                 {
                     new(Guid.NewGuid(), Guid.NewGuid(), "https://linkedin.com/company/test", true)
                 },
                 new List<UpdateCompanyEmployeeDto>
                 {
                     new(Guid.NewGuid(), "John Doe", "john@test.com", "+56912345678", "Developer", null)
                 },
                 new List<Guid>()
             );

            _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);
            _companyRepositoryMock.Setup(x => x.ExistsByTaxIdForOtherCompanyAsync(It.IsAny<string>(), companyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _contactServiceMock.Setup(x => x.ValidateRequiredContactsForUpdate(It.IsAny<IEnumerable<UpdateCompanyAddressDto>>(), It.IsAny<IEnumerable<UpdateCompanyEmailDto>>(), It.IsAny<IEnumerable<UpdateCompanyPhoneDto>>(), It.IsAny<IEnumerable<UpdateCompanySocialMediaDto>>()));
            _contactServiceMock.Setup(x => x.RemoveDeletedContactsAsync(It.IsAny<Company>(), It.IsAny<IEnumerable<UpdateCompanyAddressDto>>(), It.IsAny<IEnumerable<UpdateCompanyEmailDto>>(), It.IsAny<IEnumerable<UpdateCompanyPhoneDto>>(), It.IsAny<IEnumerable<UpdateCompanySocialMediaDto>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _contactServiceMock.Setup(x => x.DeactivateDeletedEmployeesAsync(It.IsAny<Company>(), It.IsAny<IEnumerable<UpdateCompanyEmployeeDto>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _contactServiceMock.Setup(x => x.ProcessAllContactsForUpdateAsync(It.IsAny<Company>(), It.IsAny<IEnumerable<UpdateCompanyAddressDto>>(), It.IsAny<IEnumerable<UpdateCompanyEmailDto>>(), It.IsAny<IEnumerable<UpdateCompanyPhoneDto>>(), It.IsAny<IEnumerable<UpdateCompanySocialMediaDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContactTypeNames(new Dictionary<Guid, string>(), new Dictionary<Guid, string>(), new Dictionary<Guid, string>(), new Dictionary<Guid, string>()));
            _contactServiceMock.Setup(x => x.ProcessEmployeesForUpdateAsync(It.IsAny<Company>(), It.IsAny<IEnumerable<UpdateCompanyEmployeeDto>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _contactServiceMock.Setup(x => x.BuildAddressResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
                .Returns(new List<CompanyAddressResult>());
            _contactServiceMock.Setup(x => x.BuildEmailResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
                .Returns(new List<CompanyEmailResult>());
            _contactServiceMock.Setup(x => x.BuildPhoneResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
                .Returns(new List<CompanyPhoneResult>());
            _contactServiceMock.Setup(x => x.BuildSocialMediaResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
                .Returns(new List<CompanySocialMediaResult>());
            _contactServiceMock.Setup(x => x.BuildEmployeeResults(It.IsAny<Company>()))
                .Returns(new List<CompanyEmployeeResult>());
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
             Assert.Equal(companyId, result.Id);
            Assert.Equal("Updated Company", result.Name);
            
            // Verificar que se llamaron todos los métodos necesarios
            _contactServiceMock.Verify(x => x.RemoveDeletedContactsAsync(
                It.IsAny<Company>(), 
                It.IsAny<IEnumerable<UpdateCompanyAddressDto>>(), 
                It.IsAny<IEnumerable<UpdateCompanyEmailDto>>(), 
                It.IsAny<IEnumerable<UpdateCompanyPhoneDto>>(), 
                It.IsAny<IEnumerable<UpdateCompanySocialMediaDto>>(), 
                It.IsAny<CancellationToken>()), Times.Once);
            
            _contactServiceMock.Verify(x => x.DeactivateDeletedEmployeesAsync(
                It.IsAny<Company>(), 
                It.IsAny<IEnumerable<UpdateCompanyEmployeeDto>>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
