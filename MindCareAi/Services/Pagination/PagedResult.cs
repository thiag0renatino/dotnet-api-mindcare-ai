namespace MindCareAi.Services.Pagination;

public record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, long TotalItems);
