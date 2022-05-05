using Microsoft.AspNetCore.Mvc;

namespace CollegeBackend.Extensions;

public static class ActionResultExtension
{
    public static JsonResult ToActionResult<TEnum>(this TEnum it) where TEnum : Enum
    {
        return new JsonResult(new ResultValue<TEnum> { Result = it });
    }

    private class ResultValue<TEnum> where TEnum : Enum
    {
        public TEnum Result { get; set; }
    }
}