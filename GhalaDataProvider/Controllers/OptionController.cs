using GhalaDataAccess;
using GhalaDataDef;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nox;
using Nox.WebApi;
using XAuthPool;

namespace MotisWebApi1.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class OptionController(XAuth XAuth, GhalaDataAccess.Ghala Ghala, ILogger<OptionController> Logger)
    : ControllerBase
{
    private string Token { get => HttpContext.Request.Headers["Token"]!; }

    [HttpGet()]
    public Shell<GhalaDataDef.GhalaOption> GetOption(string KeyValue)
        => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read option"), (R) =>
                Ghala.GetOption(KeyValue).ToShell()));
}
           
