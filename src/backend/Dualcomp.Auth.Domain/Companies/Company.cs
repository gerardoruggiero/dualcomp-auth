using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies
{
	public class Company : AggregateRoot
	{
		private readonly List<Employee> _employees = [];
		private readonly List<CompanyAddress> _addresses = [];
		private readonly List<CompanyEmail> _emails = [];
		private readonly List<CompanyPhone> _phones = [];
		private readonly List<CompanySocialMedia> _socialMedias = [];
		private readonly List<CompanyModule> _modules = [];
		
		public string Name { get; private set; } = string.Empty;
		public TaxId TaxId { get; private set; } = null!;
		public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();
		public IReadOnlyCollection<CompanyAddress> Addresses => _addresses.AsReadOnly();
		public IReadOnlyCollection<CompanyEmail> Emails => _emails.AsReadOnly();
		public IReadOnlyCollection<CompanyPhone> Phones => _phones.AsReadOnly();
		public IReadOnlyCollection<CompanySocialMedia> SocialMedias => _socialMedias.AsReadOnly();
		public IReadOnlyCollection<CompanyModule> Modules => _modules.AsReadOnly();

		private Company() { }

		private Company(string name, TaxId taxId)
		{
			Id = Guid.NewGuid();
			Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required", nameof(name)) : name.Trim();
			TaxId = taxId ?? throw new ArgumentNullException(nameof(taxId));
		}

		public static Company Create(string name, TaxId taxId)
			=> new Company(name, taxId);

		public void AddEmployee(Employee employee)
		{
			if (employee is null) throw new ArgumentNullException(nameof(employee));
			_employees.Add(employee);
		}

		public void AddEmployees(IEnumerable<Employee> employees)
		{
			if (employees is null) throw new ArgumentNullException(nameof(employees));
			_employees.AddRange(employees.Where(e => e is not null));
		}

		public void UpdateInfo(string name, TaxId taxId)
		{
			Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required", nameof(name)) : name.Trim();
			TaxId = taxId ?? throw new ArgumentNullException(nameof(taxId));
		}

		// Métodos para agregar contactos
		public void AddAddress(CompanyAddress address)
		{
			if (address is null) throw new ArgumentNullException(nameof(address));
			_addresses.Add(address);
		}

		public void AddEmail(CompanyEmail email)
		{
			if (email is null) throw new ArgumentNullException(nameof(email));
			_emails.Add(email);
		}

		public void AddPhone(CompanyPhone phone)
		{
			if (phone is null) throw new ArgumentNullException(nameof(phone));
			_phones.Add(phone);
		}

		public void AddSocialMedia(CompanySocialMedia socialMedia)
		{
			if (socialMedia is null) throw new ArgumentNullException(nameof(socialMedia));
			_socialMedias.Add(socialMedia);
		}

		// Métodos para remover contactos
		public void RemoveAddress(CompanyAddress address)
		{
			if (address is null) throw new ArgumentNullException(nameof(address));
			_addresses.Remove(address);
		}

		public void RemoveEmail(CompanyEmail email)
		{
			if (email is null) throw new ArgumentNullException(nameof(email));
			_emails.Remove(email);
		}

		public void RemovePhone(CompanyPhone phone)
		{
			if (phone is null) throw new ArgumentNullException(nameof(phone));
			_phones.Remove(phone);
		}

		public void RemoveSocialMedia(CompanySocialMedia socialMedia)
		{
			if (socialMedia is null) throw new ArgumentNullException(nameof(socialMedia));
			_socialMedias.Remove(socialMedia);
		}

		public void ClearModules()
		{
			_modules.Clear();
		}

		public void AddModule(Guid moduleId)
		{
			if (moduleId == Guid.Empty) throw new ArgumentException("ModuleId is required", nameof(moduleId));
			if (!_modules.Any(m => m.ModuleId == moduleId))
			{
				_modules.Add(CompanyModule.Create(Id, moduleId));
			}
		}

		public void AddModules(IEnumerable<Guid> moduleIds)
		{
			if (moduleIds is null) return;
			foreach (var moduleId in moduleIds)
			{
				AddModule(moduleId);
			}
		}

		// Validaciones de negocio
		public bool HasAtLeastOneAddress() => _addresses.Any();
		public bool HasAtLeastOneEmail() => _emails.Any();
		public bool HasAtLeastOnePhone() => _phones.Any();
		public bool HasAtLeastOneSocialMedia() => _socialMedias.Any();
		public bool HasAtLeastOneEmployee() => _employees.Any();

		public bool IsValidForRegistration()
		{
			return HasAtLeastOneAddress() && 
				   HasAtLeastOneEmail() && 
				   HasAtLeastOnePhone() && 
				   HasAtLeastOneSocialMedia() && 
				   HasAtLeastOneEmployee();
		}
	}
}
