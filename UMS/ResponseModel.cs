using System.Net;

namespace UMS;

public class ResponseModel
{
   public HttpStatusCode  StatusCode { get; set; }
   public bool  IsSuccess { get; set; }
   public string SuccessMessage { get; set; } = string.Empty;
   public string ErrorMessage { get; set; } = string.Empty;
}