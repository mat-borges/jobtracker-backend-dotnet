using System.Net.Http.Headers;
using System.Net.Http.Json;
using JobTracker.Api;
using JobTracker.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace JobTracker.IntegrationTests;
public class IntegrationTestBase(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
	protected readonly HttpClient _client = factory.CreateClient();

	// Cria usuário via rota de autenticação
	protected async Task<HttpResponseMessage> RegisterUserAsync(string email, string password)
	{
		var registerDto = new UserRegisterDto
		{
			Email = email,
			Password = password
		};

		return await _client.PostAsJsonAsync("/api/auth/register", registerDto);
	}

	// Faz login via rota de autenticação e retorna o token
	protected async Task<string> LoginAndGetTokenAsync(string email, string password)
	{
		var loginDto = new UserLoginDto
		{
			Email = email,
			Password = password
		};

		var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
		response.EnsureSuccessStatusCode();

		var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>()
			?? throw new InvalidOperationException("Login response was null");
		return result.Token;
	}

	// Configura o HttpClient para usar autenticação JWT
	protected void AuthenticateClient(string token)
	{
		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
	}
}
