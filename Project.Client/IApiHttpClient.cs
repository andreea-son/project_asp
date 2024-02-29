namespace Project.Client;

public interface IApiHttpClient
{
    Task<string> PostAsync<TDto>(string endpoint, TDto data, string? token = null);
    Task<string> PutAsync<TDto>(string endpoint, TDto data, string? token = null);
    Task<string> GetAsync(string endpoint, string? token = null);
    Task<string> DeleteAsync(string endpoint, string? token = null);
}
