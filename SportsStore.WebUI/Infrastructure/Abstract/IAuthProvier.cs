namespace SportsStore.WebUI.Infrastructure.Abstract
{
    public interface IAuthProvier
    {
        bool Authenticate(string userName, string password);
    }
}