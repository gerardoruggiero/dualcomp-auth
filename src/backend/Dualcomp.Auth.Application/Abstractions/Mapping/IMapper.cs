namespace Dualcomp.Auth.Application.Abstractions.Mapping
{
	public interface IMapper
	{
		TDestination Map<TDestination>(object source);
	}
}


