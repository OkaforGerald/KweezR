using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Contract;
using SharedAPI;
using SharedAPI.TransferObjects;

namespace KweezR.Presentation.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
        private readonly IServiceManager manager;

        public AuthController(IServiceManager manager)
        {
            this.manager = manager;
        }

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] UserCreationDto newUser)
		{
			if (newUser is null)
			{
				return BadRequest("User Creation Object Can't be Null");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var result = await manager.Auth.RegisterUser(newUser);

			if (!result.Succeeded)
			{
				var errors = result.Errors.Select(e => e.Description).ToList();
				foreach (var error in result.Errors)
				{
					ModelState.TryAddModelError(error.Code, error.Description);
				}

				return BadRequest(ResponseDto<object>.Failure(errors: errors, StatusCode: 400));
			}

			return Ok(ResponseDto<string>.Success("Registration successful", 200));
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] UserLoginDto user)
		{
			try
			{
				if (user is null)
				{
					return BadRequest("User Login Object Can't be Null");
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var result = await manager.Auth.AuthenticateUser(user);
				if (!result)
				{
					return Unauthorized(ResponseDto<string>.Failure(error: "Invalid Username/Password", StatusCode: 401));
				}

				var token = await manager.Auth.CreateToken(RefreshTokenExpiry: true);

				return Ok(ResponseDto<TokenDto>.Success(token, 200));
			}
			catch (Exception e)
			{
				return BadRequest(new
				{
					StatusCode = 400,
					Message = e.Message
				});
			}
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto)
		{
			try
			{
				if (tokenDto is null)
				{
					return BadRequest("User Login Object Can't be Null");
				}

				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var token = await manager.Auth.RefreshToken(tokenDto.accessToken!, tokenDto.refreshToken!);

				return Ok(ResponseDto<TokenDto>.Success(token, 200));
			}
			catch (Exception e)
			{
				return BadRequest(new
				{
					StatusCode = 400,
					Message = e.Message
				});
			}
		}
	}
}
