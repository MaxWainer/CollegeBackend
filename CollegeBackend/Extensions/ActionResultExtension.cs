using Microsoft.AspNetCore.Mvc;

namespace CollegeBackend.Extensions;

public static class ActionResultExtension
{
    public static JsonResult ToActionResult<TValue>(this TValue it)
    {
        return new JsonResult(new ResultValue<TValue> { Result = it });
    }

    private class ResultValue<TValue>
    {
        public TValue Result { get; set; }
    }
}