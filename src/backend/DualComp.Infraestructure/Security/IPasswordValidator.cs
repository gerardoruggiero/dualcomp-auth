namespace DualComp.Infraestructure.Security
{
    public interface IPasswordValidator
    {
        bool IsValid(string password, out string errorMessage);
    }
}
