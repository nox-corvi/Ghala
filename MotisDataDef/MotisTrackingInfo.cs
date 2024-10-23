using Nox;
using Nox.Data.SqlServer;
using System.Data.SqlClient;

namespace MotisDataDef;

public class MotisTrackingInfo
{
    /// <summary>
    /// Die Packlistennummer aus eNVenta
    /// </summary>
    public string PackingListId { get; set; } = "";

    /// <summary>
    /// Feste Positionsnummer
    /// </summary>
    public int FixedItemId { get; set; } = -1;

    /// <summary>
    /// Versandanbieter
    /// </summary>
    public string ShippingProvider { get; set; } = "";

    /// <summary>
    /// Colli-Id
    /// </summary>
    public string ColliId { get; set; } = "";

    /// <summary>
    /// Nachverfolgungsnummer
    /// </summary>
    public string TrackingId { get; set; } = "";

    /// <summary>
    /// URI zur Sendungsverfolgung
    /// </summary>
    public string TrackingLink { get; set; } = "";
}