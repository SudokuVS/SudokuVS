using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SudokuVS.Server.ViewModels.Authorization;

public class LogoutViewModel
{
    [BindNever]
    public string? RequestId { get; set; }
}
