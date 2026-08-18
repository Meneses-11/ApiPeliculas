using ApiPeliculas.Models;
using System.Net;

namespace ApiPeliculas.Helpers;

public static class RespuestaAPIHelper
{
    public static RespuestaAPI Success(Object datos = null, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
    {
        return new RespuestaAPI
        {
            IsSuccess = true,
            Result = datos,
            StatusCode = httpStatusCode,
            ErrorMessage = []
        };
    }

    public static RespuestaAPI Error(string error, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
    {
        return new RespuestaAPI
        {
            IsSuccess = false,
            Result = null,
            StatusCode = httpStatusCode,
            ErrorMessage = [error]
        };
    }

    public static RespuestaAPI Error(List<string> error, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
    {
        return new RespuestaAPI
        {
            IsSuccess = false,
            Result = null,
            StatusCode = httpStatusCode,
            ErrorMessage = error
        };
    }
}
