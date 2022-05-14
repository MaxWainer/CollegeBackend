using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeBackend.Controllers;

// dummy method to check heartbeat
[Route("dummy/[controller]")]
[ApiController]
[AllowAnonymous]
public class DummyController : Controller
{

    [HttpPost("check")]
    [AllowAnonymous]
    public void Check()
    {
        // empty
    }
    
}