using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies
{
	public class CompanySocialMedia : Entity
	{
		public Guid CompanyId { get; private set; }
		public Guid SocialMediaTypeId { get; private set; }
		public string Url { get; private set; } = string.Empty;
		public bool IsPrimary { get; private set; }

		private CompanySocialMedia() { }

		private CompanySocialMedia(Guid companyId, Guid socialMediaTypeId, string url, bool isPrimary = false)
		{
			Id = Guid.NewGuid();
			CompanyId = companyId;
			SocialMediaTypeId = socialMediaTypeId == Guid.Empty ? throw new ArgumentException("SocialMediaTypeId cannot be empty", nameof(socialMediaTypeId)) : socialMediaTypeId;
			Url = string.IsNullOrWhiteSpace(url) ? throw new ArgumentException("Url is required", nameof(url)) : url.Trim();
			IsPrimary = isPrimary;
		}

		public static CompanySocialMedia Create(Guid companyId, Guid socialMediaTypeId, string url, bool isPrimary = false)
			=> new CompanySocialMedia(companyId, socialMediaTypeId, url, isPrimary);

		public void UpdateUrl(string url)
		{
			Url = string.IsNullOrWhiteSpace(url) ? throw new ArgumentException("Url is required", nameof(url)) : url.Trim();
		}

		public void UpdateInfo(Guid socialMediaTypeId, string url, bool isPrimary)
		{
			SocialMediaTypeId = socialMediaTypeId == Guid.Empty ? throw new ArgumentException("SocialMediaTypeId cannot be empty", nameof(socialMediaTypeId)) : socialMediaTypeId;
			Url = string.IsNullOrWhiteSpace(url) ? throw new ArgumentException("Url is required", nameof(url)) : url.Trim();
			IsPrimary = isPrimary;
		}

		public void SetAsPrimary()
		{
			IsPrimary = true;
		}

		public void SetAsSecondary()
		{
			IsPrimary = false;
		}
	}
}
