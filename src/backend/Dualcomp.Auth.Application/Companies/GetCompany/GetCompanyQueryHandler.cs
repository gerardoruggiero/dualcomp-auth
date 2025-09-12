using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Domain.Domain.Common.Results;

namespace Dualcomp.Auth.Application.Companies.GetCompany
{
    public class GetCompanyQueryHandler : IQueryHandler<GetCompanyQuery, GetCompanyResult>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IAddressTypeRepository _addressTypeRepository;
        private readonly IEmailTypeRepository _emailTypeRepository;
        private readonly IPhoneTypeRepository _phoneTypeRepository;
        private readonly ISocialMediaTypeRepository _socialMediaTypeRepository;

        public GetCompanyQueryHandler(
            ICompanyRepository companyRepository,
            IAddressTypeRepository addressTypeRepository,
            IEmailTypeRepository emailTypeRepository,
            IPhoneTypeRepository phoneTypeRepository,
            ISocialMediaTypeRepository socialMediaTypeRepository)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _addressTypeRepository = addressTypeRepository ?? throw new ArgumentNullException(nameof(addressTypeRepository));
            _emailTypeRepository = emailTypeRepository ?? throw new ArgumentNullException(nameof(emailTypeRepository));
            _phoneTypeRepository = phoneTypeRepository ?? throw new ArgumentNullException(nameof(phoneTypeRepository));
            _socialMediaTypeRepository = socialMediaTypeRepository ?? throw new ArgumentNullException(nameof(socialMediaTypeRepository));
        }

        public async Task<GetCompanyResult> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIdAsync(request.CompanyId, cancellationToken);
            if (company == null)
            {
                throw new InvalidOperationException("Empresa no encontrada");
            }

            // Obtener todos los tipos de contacto
            var addressTypes = await _addressTypeRepository.GetAllAsync(cancellationToken);
            var emailTypes = await _emailTypeRepository.GetAllAsync(cancellationToken);
            var phoneTypes = await _phoneTypeRepository.GetAllAsync(cancellationToken);
            var socialMediaTypes = await _socialMediaTypeRepository.GetAllAsync(cancellationToken);

            // Crear diccionarios para mapear IDs a nombres
            var addressTypeNames = addressTypes.ToDictionary(at => at.Id, at => at.Name);
            var emailTypeNames = emailTypes.ToDictionary(et => et.Id, et => et.Name);
            var phoneTypeNames = phoneTypes.ToDictionary(pt => pt.Id, pt => pt.Name);
            var socialMediaTypeNames = socialMediaTypes.ToDictionary(smt => smt.Id, smt => smt.Name);

            return new GetCompanyResult(
                company.Id,
                company.Name,
                company.TaxId.Value,
                company.Addresses.Select(a => new CompanyAddressResult(addressTypeNames.GetValueOrDefault(a.AddressTypeId, "Unknown"), a.Address, a.IsPrimary)).ToList(),
                company.Emails.Select(e => new CompanyEmailResult(emailTypeNames.GetValueOrDefault(e.EmailTypeId, "Unknown"), e.Email.Value, e.IsPrimary)).ToList(),
                company.Phones.Select(p => new CompanyPhoneResult(phoneTypeNames.GetValueOrDefault(p.PhoneTypeId, "Unknown"), p.Phone, p.IsPrimary)).ToList(),
                company.SocialMedias.Select(sm => new CompanySocialMediaResult(socialMediaTypeNames.GetValueOrDefault(sm.SocialMediaTypeId, "Unknown"), sm.Url, sm.IsPrimary)).ToList(),
                company.Employees.Select(e => new CompanyEmployeeResult(e.FullName, e.Email, e.Phone, e.Position, e.HireDate)).ToList());
        }
    }
}
