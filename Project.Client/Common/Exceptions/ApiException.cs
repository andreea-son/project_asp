using System.Net;

namespace Project.Client.Common.Exceptions;

public class ApiException : Exception
{
    public HttpStatusCode HttpStatusCode { get; set; }
    public ApiErrorResponseDto? Error { get; }


    public ApiException(ApiErrorResponseDto? error, HttpStatusCode httpStatusCode) : base(error?.Title)
    {
        Error = error;
        HttpStatusCode = httpStatusCode;
    }
}