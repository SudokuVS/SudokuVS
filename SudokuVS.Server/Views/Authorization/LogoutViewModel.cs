using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SudokuVS.Server.Views.Authorization;

public class LogoutViewModel
{
    [BindNever]
    public string? RequestId { get; set; }
}
