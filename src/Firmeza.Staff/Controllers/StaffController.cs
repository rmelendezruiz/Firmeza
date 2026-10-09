using Firmeza.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Staff.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = SessionScheme.Name, Roles = Roles.Staff)]
public abstract class StaffController : ControllerBase
{
}
