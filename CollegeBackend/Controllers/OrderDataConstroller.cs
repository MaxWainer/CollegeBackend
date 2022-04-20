using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace CollegeBackend.Controllers;

[Authorize]
[Route("order/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class OrderDataController : Controller
{
    // [HttpPost]
    // [ProducesResponseType(StatusCodes.Status201Created)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // public ActionResult<int> HandleOrder(OrderData orderData)
    // {
    //     
    // }
}

public class OrderData
{
    public User User { get; set; }
    public Station EndStation { get; set; }
    public Action RelatedActive { get; set; }
    public Direction RelatedDirection { get; set; }
    public Sitting Sitting { get; set; }
}