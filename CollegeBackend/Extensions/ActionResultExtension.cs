using Microsoft.AspNetCore.Mvc;

namespace CollegeBackend.Extensions;

public static class ActionResultExtension
{
    public static ActionResult<string> ToActionResult<TEnum>(this TEnum it) where TEnum : Enum
    {
        return new ActionResult<string>(it.ToString());
    }
}