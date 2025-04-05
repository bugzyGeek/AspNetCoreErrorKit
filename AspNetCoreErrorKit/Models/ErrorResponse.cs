namespace AspNetCoreErrorKit.Models;

internal class ErrorResponse
{
    public string ErrorCode { get; set; } = null!;
    public string ReferenceId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string UserInstructions { get; set; } = null!;
}
