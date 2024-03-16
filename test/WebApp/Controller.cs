using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Udup.WebApp;

[ApiController]
[Authorize]
[Route("api")]
public class Controller : ControllerBase
{
    [HttpPost("MakeActionBRoute")]
    public async Task<ActionResult> MakeActionBControllerMethod([FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var command = new DomainEventBHappened();
        await mediator.Send(command, cancellationToken);
        return Ok();
    }
}