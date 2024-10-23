using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nox;
using Nox.Data.SqlServer;
using Nox.WebApi;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace MotisDataAccess;

public class MotisTrackingInfo(Motis Motis, IConfiguration Configuration, ILogger Logger)
    : Shell<MotisDataDef.MotisTrackingInfo>
{
    public const string SHIPPING_PICKUP = "ABHOLUNG";
    public const string SHIPPING_FORWARDER = "SPEDITION";
    public const string SHIPPING_TOUR = "TOUR";
    public const string SHIPPING_SEV = "SONSTIGES";

    public const string SHIPPING_DPD = "DPD";
    public const string SHIPPING_UPS = "UPS";
    public const string SHIPPING_GLS = "GLS";

    public Shell<MotisDataDef.MotisTrackingInfo> GetTrackingByPackingListId(string OrderId, int FixedItemPos, string Branch = "001")
       => GetTrackingByPackingListId(Motis, OrderId, FixedItemPos, Branch);

    public static Shell<MotisDataDef.MotisTrackingInfo> GetTrackingByPackingListId(Motis Motis, string OrderId, int FixedItemPos, string Branch = "001")
    {
        try
        {
            using (var DbA = new SqlDbAccess(Motis.ConnectionString))
            using (var Reader = DbA.GetReader(
                @"SELECT DISTINCT 
	                APos.AuftragNr, 
	                APos.MasterAuftragNr, 
	                APos.MasterPosNr, 
	                PackPos.PackstueckNr, 
	                Pack.ColliNr, 
	                Pack.TrackingNr, 
	                PackList.Versandart
                FROM 
	                Auftragsposition AS APos 
                LEFT JOIN Fahrauftrag AS FAuf on FAuf.AuftragNr = apos.AuftragNr AND fauf.AuftragMandant = apos.Mandant AND fauf.PosNr = apos.PosNr 
                LEFT JOIN PackstueckPosition AS PackPos on PackPos.FahrAuftragNr = FAuf.FahrAuftragNr 
                LEFT JOIN PackStueck AS Pack on	Pack.PackstueckNr = PackPos.PackstueckNr 
                LEFT JOIN Versandliste AS PackList on PackList.VersandListenNr = Pack.Versandliste 
                WHERE 
	                APos.MasterMandant = @Branch AND 
	                APos.MasterAuftragNr = @MasterOrderId AND 
	                APos.MasterPosNr = @MasterOrderPosId",
                new SqlParameter("Branch", Branch),
                new SqlParameter("MasterOrderId", OrderId),
                new SqlParameter("MasterOrderPosId", FixedItemPos)))
                if (Reader.Read())
                    return new(StateEnum.Success)
                    {
                        Data = ReaderToObjectList(Motis, Reader)
                    };
                else
                    return new(StateEnum.Failure, "not found");
        }
        catch (Exception ex)
        {
            return new(StateEnum.Error, Helpers.SerializeException(ex));
        }
    }

    private static List<MotisDataDef.MotisTrackingInfo> ReaderToObjectList(Motis Motis, SqlDataReader r)
    {
        var ResultItem = new MotisDataDef.MotisTrackingInfo()
        {
            PackingListId = Helpers.NZ(r["MasterAuftragNr"]),
            FixedItemId = int.Parse(Helpers.N<int>(r["MasterPosNr"]).ToString()),
            ColliId = Helpers.NZ(r["collinr"]),
            TrackingId = Helpers.NZ(r["trackingnr"]),
        };

        string Token = Motis.Configuration["GhalaDataProvider:Token"] ??
            throw new ArgumentNullException("GhalaDataProvider:Token");

        var Option = new GhalaDataPool.Ghala(Motis.Configuration, Motis.Logger);

        // get wildcard ...
        var OptionItem = Option.GetOption("TRACKING URL WILDCARD");
        if (OptionItem.State == StateEnum.Success)
        {
            var Wildcard = OptionItem.AdditionalData1;
            string ShippingProvider = string.Concat(Helpers.NZ(r["Versandart"]), ' ').Split(' ').First();
            switch (ShippingProvider.ToUpper())
            {
                case SHIPPING_PICKUP:
                case SHIPPING_TOUR:
                case SHIPPING_FORWARDER:
                    ResultItem.ShippingProvider = ShippingProvider;
                    ResultItem.TrackingLink = "";
                    break;
                case SHIPPING_DPD:
                    OptionItem = Option.GetOption("TRACKING URL DPD");
                    if (OptionItem.State == StateEnum.Success)
                    {
                        var DPDTrackingUrl = OptionItem.AdditionalData1;
                        ResultItem.ShippingProvider = ShippingProvider;
                        if (string.IsNullOrWhiteSpace(ResultItem.ColliId))
                            ResultItem.TrackingLink = "";
                        else
                            ResultItem.TrackingLink = DPDTrackingUrl.Replace(Wildcard, ResultItem.ColliId);
                    }
                    else
                        throw new Exception("option not found");
                    break;
                case SHIPPING_UPS:
                    OptionItem = Option.GetOption("TRACKING URL UPS");
                    if (OptionItem.State == StateEnum.Success)
                    {
                        var UPSTrackingUrl = OptionItem.AdditionalData1;
                        ResultItem.ShippingProvider = ShippingProvider;
                        if (string.IsNullOrWhiteSpace(ResultItem.TrackingId))
                            ResultItem.TrackingLink = "";
                        else
                            ResultItem.TrackingLink = UPSTrackingUrl.Replace(Wildcard, ResultItem.TrackingId);
                    }
                    else
                        throw new Exception("option not found");
                    break;
                case SHIPPING_GLS:
                    OptionItem = Option.GetOption("TRACKING URL GLS");
                    if (OptionItem.State == StateEnum.Success)
                    {
                        var GLSTrackingUrl = OptionItem.AdditionalData1;
                        ResultItem.ShippingProvider = ShippingProvider;
                        if (string.IsNullOrWhiteSpace(ResultItem.TrackingId))
                            ResultItem.TrackingLink = "";
                        else
                            ResultItem.TrackingLink = GLSTrackingUrl.Replace(Wildcard, ResultItem.TrackingId);
                    }
                    else
                        throw new Exception("option not found");
                    break;
                default:
                    ResultItem.ShippingProvider = SHIPPING_SEV;
                    ResultItem.TrackingLink = "";
                    break;
            }

            return new List<MotisDataDef.MotisTrackingInfo>() { ResultItem };
        } else
            throw new Exception("option not found");
    }

    // DI-Constructor
    public MotisTrackingInfo(Motis Motis, IConfiguration Configuration, ILogger<MotisTrackingInfo> Logger)
        : this(Motis, Configuration, (ILogger)Logger) { }
}
