using Newtonsoft.Json;
using Project.Client.Common.Exceptions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Project.Client;

// https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests

public class ApiHttpClient : IApiHttpClient
{
    private readonly HttpClient httpClient;
   
    public ApiHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        this.httpClient = httpClient;
        httpClient.BaseAddress = new Uri("http://localhost:5037/");
    }
    
    /// <summary>
    /// Sends a POST request to the specified <paramref name="endpoint"/> as an asynchronous operation and returns the result
    /// </summary>
    /// <param name="endpoint">The API endpoint where the request is being sent</param>
    /// <param name="data">The data to be serialized and sent to the API</param>
    /// <param name="token">The token used for authentication with the API</param>
    /// <returns>A string containing the result of the POST request</returns>
    public async Task<string> PostAsync<TDto>(string endpoint, TDto data, string? token = null)
    {
        if (!string.IsNullOrEmpty(token))
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(endpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            ApiErrorResponseDto? result = null;
            try
            {
                result = JsonConvert.DeserializeObject<ApiErrorResponseDto>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception(responseContent);
            }
            throw new ApiException(result, response.StatusCode);
        }
        return responseContent;
    }

    /// <summary>
    /// Sends a PUT request to the specified <paramref name="endpoint"/> as an asynchronous operation and returns the result
    /// </summary>
    /// <param name="endpoint">The API endpoint where the request is being sent</param>
    /// <param name="data">The data to be serialized and send to the API</param>
    /// <param name="token">The token used for authentication with the API</param>
   /// <returns>A string containing the result of the PUT request</returns>
    public async Task<string> PutAsync<TDto>(string endpoint, TDto data, string? token = null)
    {
        if (!string.IsNullOrEmpty(token))
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync(endpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            ApiErrorResponseDto? result = null;
            try
            {
                result = JsonConvert.DeserializeObject<ApiErrorResponseDto>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception(responseContent);
            }
            throw new ApiException(result, response.StatusCode);
        }
        return responseContent;
    }

    /// <summary>
    /// Sends a GET request to the specified <paramref name="endpoint"/> as an asynchronous operation and returns the result
    /// </summary>
    /// <param name="endpoint">The API endpoint where the request is being sent</param>
    /// <param name="token">The token used for authentication with the API</param>
    /// <returns>A string containing the result of the GET request</returns>
    public async Task<string> GetAsync(string endpoint, string? token = null)
    {
        if (!string.IsNullOrEmpty(token))
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await httpClient.GetAsync(endpoint);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            ApiErrorResponseDto? result = null;
            try
            {
                result = JsonConvert.DeserializeObject<ApiErrorResponseDto>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception(responseContent);
            }
            throw new ApiException(result, response.StatusCode);
        }
        return responseContent;
    }

    /// <summary>
    /// Sends a DELETE request to the specified <paramref name="endpoint"/> as an asynchronous operation and returns the result
    /// </summary>
    /// <param name="endpoint">The API endpoint where the request is being sent</param>
    /// <param name="token">The token used for authentication with the API</param>
    /// <returns>A string containing the result of the DELETE request</returns>
    public async Task<string> DeleteAsync(string endpoint, string? token = null)
    {
        if (!string.IsNullOrEmpty(token))
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await httpClient.DeleteAsync(endpoint);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            ApiErrorResponseDto? result = null;
            try
            {
                result = JsonConvert.DeserializeObject<ApiErrorResponseDto>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception(responseContent);
            }
            throw new ApiException(result, response.StatusCode);
        }
        return responseContent;
    }
}
