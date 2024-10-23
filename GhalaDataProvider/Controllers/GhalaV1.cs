using GhalaDataAccess;
using GhalaDataDef;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nox;
using Nox.WebApi;
using XAuthPool;

namespace MotisWebApi1.Controllers;

[ApiController]
[Route("v1/[action]")]
public class GhalaV1(XAuth XAuth, GhalaDataAccess.Ghala Ghala, ILogger<GhalaV1> Logger)
    : Nox.Web.WebApi(Logger)
{
    protected override string RootKey => "ghala";

    [HttpGet()]
    public SingleDataResponseShell GetOption(string KeyValue)
       => Logger.LogShell(Shell.SuccessHandler(
           XAuth.ValidateToken(Token, "read option"), (t) =>
               Shell.SuccessHandler(Ghala.GetOption(KeyValue),
                   (o) => new SingleDataResponseShell(o.State, Shell.OK, o.First().Data))));
}
           
