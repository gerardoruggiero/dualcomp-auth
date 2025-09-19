namespace Dualcomp.Auth.Application.Modulos.GetModulos
{
	public record ModuloItem(string Id, string Name, string? Description, bool IsActive);

	public record GetModulosResult(IEnumerable<ModuloItem> Modulos);
}
