using Dualcomp.Auth.Domain.Companies;
using System;

namespace Dualcomp.Auth.Domain.Companies
{
    public class CompanyModule
    {
        public Guid CompanyId { get; private set; }
        public Guid ModuleId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private CompanyModule() { }

        private CompanyModule(Guid companyId, Guid moduleId)
        {
            CompanyId = companyId;
            ModuleId = moduleId;
            CreatedAt = DateTime.UtcNow;
        }

        public static CompanyModule Create(Guid companyId, Guid moduleId)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId is required", nameof(companyId));
            if (moduleId == Guid.Empty) throw new ArgumentException("ModuleId is required", nameof(moduleId));
            
            return new CompanyModule(companyId, moduleId);
        }
    }
}
