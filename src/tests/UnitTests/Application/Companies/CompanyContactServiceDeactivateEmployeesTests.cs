using Dualcomp.Auth.Application.Companies;
using Dualcomp.Auth.Application.Companies.UpdateCompany;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Security;
using Moq;

namespace Dualcomp.Auth.UnitTests.Application.Companies
{
    public class CompanyContactServiceDeactivateEmployeesTests
    {
        private readonly Mock<IAddressTypeRepository> _addressTypeRepositoryMock;
        private readonly Mock<IEmailTypeRepository> _emailTypeRepositoryMock;
        private readonly Mock<IPhoneTypeRepository> _phoneTypeRepositoryMock;
        private readonly Mock<ISocialMediaTypeRepository> _socialMediaTypeRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IPasswordGenerator> _passwordGeneratorMock;
        private readonly CompanyContactService _service;

        public CompanyContactServiceDeactivateEmployeesTests()
        {
            _addressTypeRepositoryMock = new Mock<IAddressTypeRepository>();
            _emailTypeRepositoryMock = new Mock<IEmailTypeRepository>();
            _phoneTypeRepositoryMock = new Mock<IPhoneTypeRepository>();
            _socialMediaTypeRepositoryMock = new Mock<ISocialMediaTypeRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _passwordGeneratorMock = new Mock<IPasswordGenerator>();
            
            _service = new CompanyContactService(
                _addressTypeRepositoryMock.Object,
                _emailTypeRepositoryMock.Object,
                _phoneTypeRepositoryMock.Object,
                _socialMediaTypeRepositoryMock.Object,
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _passwordGeneratorMock.Object);
        }

        [Fact]
        public async Task DeactivateDeletedEmployeesAsync_WithDeletedEmployee_ShouldDeactivateEmployeeAndUser()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            // Crear empresa con un empleado
            var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
            var user = User.Create("John", "Doe", Email.Create("john@test.com"), HashedPassword.Create("hashedpassword"));
            var employee = Employee.Create("John Doe", "john@test.com", "+56912345678", company.Id, "Developer", DateTime.UtcNow, user);
            
            // Agregar empleado a la empresa
            company.AddEmployee(employee);

            // Lista vacía de empleados (el empleado fue eliminado del frontend)
            var employees = new List<UpdateCompanyEmployeeDto>();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            await _service.DeactivateDeletedEmployeesAsync(company, employees, CancellationToken.None);

            // Assert
            // Verificar que el empleado fue desactivado
            Assert.False(employee.IsActive);
            
            // Verificar que el usuario fue desactivado
            Assert.False(user.IsActive);
            
            // Verificar que se llamó al repositorio para obtener el usuario
            _userRepositoryMock.Verify(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeactivateDeletedEmployeesAsync_WithActiveEmployee_ShouldNotDeactivateEmployee()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            // Crear empresa con un empleado
            var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
            var user = User.Create("John", "Doe", Email.Create("john@test.com"), HashedPassword.Create("hashedpassword"));
            var employee = Employee.Create("John Doe", "john@test.com", "+56912345678", company.Id, "Developer", DateTime.UtcNow, user);
            
            // Agregar empleado a la empresa
            company.AddEmployee(employee);

            // Lista con el empleado existente (no fue eliminado)
            var employees = new List<UpdateCompanyEmployeeDto>
            {
                new(employee.Id, "John Doe", "john@test.com", "+56912345678", "Developer", null)
            };

            // Act
            await _service.DeactivateDeletedEmployeesAsync(company, employees, CancellationToken.None);

            // Assert
            // Verificar que el empleado sigue activo
            Assert.True(employee.IsActive);
            
            // Verificar que el usuario sigue activo
            Assert.True(user.IsActive);
            
            // Verificar que NO se llamó al repositorio para obtener el usuario
            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeactivateDeletedEmployeesAsync_WithMultipleEmployees_ShouldDeactivateOnlyDeletedOnes()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var employee1Id = Guid.NewGuid();
            var employee2Id = Guid.NewGuid();
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            
            // Crear empresa con dos empleados
            var company = Company.Create("Test Company", TaxId.Create("12345678-9"));
            
            var user1 = User.Create("John", "Doe", Email.Create("john@test.com"), HashedPassword.Create("hashedpassword"));
            var user2 = User.Create("Jane", "Smith", Email.Create("jane@test.com"), HashedPassword.Create("hashedpassword"));
            
            var employee1 = Employee.Create("John Doe", "john@test.com", "+56912345678", company.Id, "Developer", DateTime.UtcNow, user1);
            var employee2 = Employee.Create("Jane Smith", "jane@test.com", "+56987654321", company.Id, "Designer", DateTime.UtcNow, user2);
            
            // Agregar empleados a la empresa
            company.AddEmployee(employee1);
            company.AddEmployee(employee2);

            // Lista con solo el primer empleado (el segundo fue eliminado)
            var employees = new List<UpdateCompanyEmployeeDto>
            {
                new(employee1.Id, "John Doe", "john@test.com", "+56912345678", "Developer", null)
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(user2.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user2);

            // Act
            await _service.DeactivateDeletedEmployeesAsync(company, employees, CancellationToken.None);

            // Assert
            // Verificar que el primer empleado sigue activo
            Assert.True(employee1.IsActive);
            Assert.True(user1.IsActive);
            
            // Verificar que el segundo empleado fue desactivado
            Assert.False(employee2.IsActive);
            Assert.False(user2.IsActive);
            
            // Verificar que se llamó al repositorio solo para el usuario del empleado eliminado
            _userRepositoryMock.Verify(x => x.GetByIdAsync(user2.Id, It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(user1.Id, It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
