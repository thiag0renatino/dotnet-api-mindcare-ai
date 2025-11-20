using challenge_api_dotnet.Hateoas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MindCareAi.Controllers;

internal static class HateoasControllerHelper
{
    public static int TotalPages<T>(MindCareAi.Services.Pagination.PagedResult<T> paged)
        => paged.PageSize == 0 ? 0 : (int)Math.Ceiling((double)paged.TotalItems / paged.PageSize);

    public static PagedResult<Resource<TDto>> BuildPagedResult<TDto>(
        IUrlHelper urlHelper,
        MindCareAi.Services.Pagination.PagedResult<TDto> paged,
        IEnumerable<Resource<TDto>> items,
        IEnumerable<HateoasLink> collectionLinks)
        => new(items, paged.Page, paged.PageSize, paged.TotalItems, collectionLinks);

    public static IEnumerable<HateoasLink> FilteredPagingLinks(
        IUrlHelper urlHelper,
        string actionName,
        object routeValues,
        int page,
        int size,
        int totalPages,
        IEnumerable<HateoasLink>? extra = null)
    {
        var links = new List<HateoasLink>
        {
            urlHelper.CreateLink("self", actionName, Merge(routeValues, new { page, size })),
            urlHelper.CreateLink("first", actionName, Merge(routeValues, new { page = 1, size })),
            urlHelper.CreateLink("last", actionName,
                Merge(routeValues, new { page = totalPages > 0 ? totalPages : 1, size })),
        };

        if (page > 1)
        {
            links.Add(urlHelper.CreateLink("prev", actionName, Merge(routeValues, new { page = page - 1, size })));
        }

        if (totalPages > 0 && page < totalPages)
        {
            links.Add(urlHelper.CreateLink("next", actionName, Merge(routeValues, new { page = page + 1, size })));
        }

        if (extra is not null)
        {
            links.AddRange(extra);
        }

        return links;
    }

    private static RouteValueDictionary Merge(object baseValues, object pagingValues)
    {
        var result = new RouteValueDictionary(baseValues);
        foreach (var kvp in new RouteValueDictionary(pagingValues))
        {
            result[kvp.Key] = kvp.Value;
        }

        return result;
    }
}
