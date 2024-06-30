using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SharedAPI.TransferObjects;

namespace Services.Contract
{
	public interface IAuthService
	{
		Task<IdentityResult> RegisterUser(UserCreationDto newUser);

		Task<bool> AuthenticateUser(UserLoginDto loginDetails);

		Task<TokenDto> CreateToken(bool RefreshTokenExpiry);

		Task<TokenDto> RefreshToken(string accessToken, string refreshToken);
	}
}
