namespace Fitbit.Api.Portable
{
    using System.Threading.Tasks;

    using Fitbit.Api.Portable.OAuth2;

    public interface ITokenManager
    {
        Task<OAuth2AccessToken> RefreshTokenAsync(FitbitClient client);
    }
}