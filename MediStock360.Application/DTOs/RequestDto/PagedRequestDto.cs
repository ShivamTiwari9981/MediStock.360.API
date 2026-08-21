namespace MediStock360.Application.DTOs.RequestDto
{
    public class PagedRequestDto
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? SearchText { get; set; }

        public string? SortColumn { get; set; }

        public string? SortDirection { get; set; } = "ASC";
    }
}
