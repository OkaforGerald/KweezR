using KweezR.Client.HttpRepository;
using Microsoft.AspNetCore.Components;
using SharedAPI.TransferObjects;

namespace KweezR.Client.Pages
{
	public partial class Login
	{
		private UserLoginDto _userForAuthentication = new UserLoginDto();

		[Inject]
		public IAuthenticationService AuthenticationService { get; set; }
		[Inject]
		public NavigationManager NavigationManager { get; set; }
		public bool ShowAuthenticationErrors { get; set; }
		public string Errors { get; set; }

		public async Task ExecuteLogin()
		{
			ShowAuthenticationErrors = false;

			var result = await AuthenticationService.Login(_userForAuthentication);
			if (!result.IsSuccessful)
			{
				Errors = result.Errors.First();
				ShowAuthenticationErrors = true;
			}
			else
			{
				NavigationManager.NavigateTo("/");
			}
		}
	}
}
