namespace SudokuVS.Server.Areas.Guides;

public class Guide
{
    public required string Page { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }
    public bool Interactive { get; set; }
}
