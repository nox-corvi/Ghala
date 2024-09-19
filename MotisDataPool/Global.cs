using Microsoft.Extensions.Configuration;
using Nox;
using System.Diagnostics;

namespace MotisDataPool;

public class Global
    : Nox.Global
{
    #region Properties
    public static string BaseURL_GhalaDataProvider
    {
        get => Configuration["BaseURL:GhalaDataProvider"] ??
            throw new ArgumentNullException($"{nameof(BaseURL_GhalaDataProvider)} must not be null");
    }

    public static string BaseURL_MotisDataProvider
    {
        get => Configuration["BaseURL:MotisDataProvider"] ??
            throw new ArgumentNullException($"{nameof(BaseURL_MotisDataProvider)} must not be null");
    }

    public static string BaseURL_NVWebApi
    {
        get => Configuration["BaseURL:NVWebApi"] ??
            throw new ArgumentNullException($"{nameof(BaseURL_NVWebApi)} must not be null");
    }
    #endregion
}
