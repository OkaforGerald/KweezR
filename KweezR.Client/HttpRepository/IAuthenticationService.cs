using SharedAPI;
using SharedAPI.TransferObjects;

namespace KweezR.Client.HttpRepository
{
	public interface IAuthenticationService
	{
		Task<ResponseDto<object>> RegisterUser(UserCreationDto userForRegistration);

		Task<ResponseDto<object>> Login(UserLoginDto userForAuthentication);

		Task<string> RefreshToken();

		Task Logout();
	}
}
