using KweezR.Client.HttpRepository;
using Microsoft.AspNetCore.Components;
using SharedAPI.TransferObjects;

namespace KweezR.Client.Pages
{
    public partial class Register
    {
        private UserCreationDto _userForRegistration = new UserCreationDto();

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public bool ShowRegistrationErrors { get; set; }
        public IEnumerable<string> Errors { get; set; }

        public async Task RegisterUser()
        {
            ShowRegistrationErrors = false;

            var result = await AuthenticationService.RegisterUser(_userForRegistration);
            if (!result.IsSuccessful)
            {
                Errors = result.Errors;
                ShowRegistrationErrors = true;
            }
            else
            {
                NavigationManager.NavigateTo("/login");
            }
        }
    }
}
