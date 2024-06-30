using Blazored.LocalStorage;
using KweezR.Client.AuthProvider;
using Microsoft.AspNetCore.Components.Authorization;
using SharedAPI.TransferObjects;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using SharedAPI;

namespace KweezR.Client.HttpRepository
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly HttpClient _client;
		private readonly JsonSerializerOptions _options;
		private readonly AuthenticationStateProvider _authStateProvider;
		private readonly ILocalStorageService _localStorage;

		public AuthenticationService(HttpClient client, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
		{
			_client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			_authStateProvider = authStateProvider;
			_localStorage = localStorage;
		}

		public async Task<ResponseDto<object>> RegisterUser(UserCreationDto userForRegistration)
		{
			var content = JsonSerializer.Serialize(userForRegistration);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

			var registrationResult = await _client.PostAsync("auth/register", bodyContent);
			var registrationContent = await registrationResult.Content.ReadAsStringAsync();

			var result = JsonSerializer.Deserialize<ResponseDto<object>>(registrationContent, _options);

			return result!;
		}

		public async Task<ResponseDto<object>> Login(UserLoginDto userForAuthentication)
		{
			var content = JsonSerializer.Serialize(userForAuthentication);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

			var authResult = await _client.PostAsync("auth/login", bodyContent);
			var authContent = await authResult.Content.ReadAsStringAsync();
			var result = JsonSerializer.Deserialize<ResponseDto<object>>(authContent, _options);

			if (!authResult.IsSuccessStatusCode)
				return result!;
			var token = (TokenDto) result.Data;
			await _localStorage.SetItemAsync("authToken", token?.accessToken);
			await _localStorage.SetItemAsync("refreshToken", token?.refreshToken);
			((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(token?.accessToken!);
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token?.accessToken);

			return result;
		}

		public async Task<string> RefreshToken()
		{
			var token = await _localStorage.GetItemAsync<string>("authToken");
			var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

			var tokenDto = JsonSerializer.Serialize(new TokenDto { accessToken = token, refreshToken = refreshToken });
			var bodyContent = new StringContent(tokenDto, Encoding.UTF8, "application/json");

			var refreshResult = await _client.PostAsync("auth/refresh-token", bodyContent);
			var refreshContent = await refreshResult.Content.ReadAsStringAsync();
			var result = JsonSerializer.Deserialize<ResponseDto<object>>(refreshContent, _options);

			if (!refreshResult.IsSuccessStatusCode)
				throw new ApplicationException("Something went wrong during the refresh token action");

			var newToken = (TokenDto)result.Data;
			await _localStorage.SetItemAsync("authToken", newToken!.accessToken);
			await _localStorage.SetItemAsync("refreshToken", newToken.refreshToken);

			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", newToken.accessToken);

			return newToken.accessToken!;
		}

		public async Task Logout()
		{
			await _localStorage.RemoveItemAsync("authToken");
			await _localStorage.RemoveItemAsync("refreshToken");
			((AuthStateProvider)_authStateProvider).NotifyUserLogout();
			_client.DefaultRequestHeaders.Authorization = null;
		}
	}
}
