using Microsoft.EntityFrameworkCore;

namespace CollegeBackend.Extensions;

public static class DbSetExtension
{
    public static async Task<bool> IsExists<TEntity>(this IEnumerable<TEntity> set, Func<TEntity, bool> predicate)
        where TEntity : class
    {
        return await Task.FromResult(set.Where(predicate).Any());
    }
}