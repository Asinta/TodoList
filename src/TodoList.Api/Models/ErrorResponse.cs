using System.Net;
using System.Text.Json;

namespace TodoList.Api.Models;


// 统一使用ApiResponse进行返回，该类不再继续使用
[Obsolete]
public class ErrorResponse
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
    public string Message { get; set; } = "An unexpected error occurred.";
    public string ToJsonString() => JsonSerializer.Serialize(this);
}