using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Application.Abstractions.Queries;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;

namespace Dualcomp.Auth.Application.Titles.GetTitles;

public class GetTitlesQueryHandler : IQueryHandler<GetTitlesQuery, GetTitlesResult>
{
	private readonly ITitleRepository _titleRepository;

	public GetTitlesQueryHandler(ITitleRepository titleRepository)
	{
		_titleRepository = titleRepository ?? throw new ArgumentNullException(nameof(titleRepository));
	}

	public async Task<GetTitlesResult> Handle(GetTitlesQuery query, CancellationToken cancellationToken)
	{
		var titles = await _titleRepository.GetAllAsync(cancellationToken);
		
		var titleItems = titles.Select(title => new TitleItem(
			title.Id.ToString(),
			title.Name,
			title.Description,
			title.IsActive
		));

		return new GetTitlesResult(titleItems);
	}
}
