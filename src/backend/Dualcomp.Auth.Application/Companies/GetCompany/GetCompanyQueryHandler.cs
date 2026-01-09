using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;

namespace Dualcomp.Auth.Application.Companies.GetCompany
{
    public class GetCompanyQueryHandler : IQueryHandler<GetCompanyQuery, GetCompanyResult>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetCompanyQueryHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }

        public async Task<GetCompanyResult> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIdAsync(request.CompanyId, cancellationToken);
            if (company == null)
            {
                throw new InvalidOperationException("Empresa no encontrada");
            }

            var result = new GetCompanyResult(
                company.Id,
                company.Name,
                company.TaxId.Value,
                company.Addresses.Select(a => new CompanyAddressResult(
                    a.Id.ToString(), 
                    a.AddressTypeId.ToString(), 
                    a.Address, 
                    a.IsPrimary)).ToList(),
                company.Emails.Select(e => new CompanyEmailResult(
                    e.Id.ToString(), 
                    e.EmailTypeId.ToString(), 
                    e.Email.Value, 
                    e.IsPrimary)).ToList(),
                company.Phones.Select(p => new CompanyPhoneResult(
                    p.Id.ToString(), 
                    p.PhoneTypeId.ToString(), 
                    p.Phone, 
                    p.IsPrimary)).ToList(),
                company.SocialMedias.Select(sm => new CompanySocialMediaResult(
                    sm.Id.ToString(), 
                    sm.SocialMediaTypeId.ToString(), 
                    sm.Url, 
                    sm.IsPrimary)).ToList(),
                company.Employees
                    .Where(e => e.IsActive)
                    .Select(e => new CompanyEmployeeResult(
                        e.Id.ToString(), 
                        e.FullName, 
                        e.Email, 
                        e.Phone, 
                        e.Position, 
                        e.HireDate)).ToList(),
                company.Modules.Select(m => m.ModuleId).ToList());

            return result;
        }
    }
}
