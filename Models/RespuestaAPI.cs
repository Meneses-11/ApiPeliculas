using System.Net;

namespace ApiPeliculas.Models;

public class RespuestaAPI
{
    public RespuestaAPI()
    {
        ErrorMessage = new List<string>();
    }
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; } = true;
    public List<string> ErrorMessage { get; set; }
    public object? Result { get; set; }
}
