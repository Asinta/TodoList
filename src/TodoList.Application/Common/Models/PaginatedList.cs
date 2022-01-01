using Microsoft.EntityFrameworkCore;

namespace TodoList.Application.Common.Models;

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    // 增加属性表示是否有前一页
    public bool HasPreviousPage => PageNumber > 1;
    // 增加属性表示是否有后一页
    public bool HasNextPage => PageNumber < TotalPages;

    // 分页结果构建辅助方法
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        // 注意我们给的请求中pageNumber是从1开始的
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}