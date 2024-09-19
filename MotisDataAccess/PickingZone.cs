using Nox;
using Nox.Data.SqlServer;
using Nox.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotisDataAccess
{
    public class PickingZone(Motis Motis)
    : Shell<MotisDataDef.PickingZone>
    {
        public Shell<MotisDataDef.PickingZone> GetPickingZones()
           => GetPickingZones(Motis);

        public static Shell<MotisDataDef.PickingZone> GetPickingZones(Motis Motis)
        {
            try
            {
                using (var DbA = new SqlDbAccess(Motis.ConnectionString))
                using (var Reader = DbA.GetReader(
                    @"select kommizone from kommizone;"))
                    if (Reader.Read())
                        return new(StateEnum.Success)
                        {
                            Data = ReaderToObjectList(Reader)
                        };
                    else
                        return new(StateEnum.Failure, "not found");
            }
            catch (Exception ex)
            {
                return new(StateEnum.Error, Helpers.SerializeException(ex));
            }
        }

        private static List<MotisDataDef.PickingZone> ReaderToObjectList(SqlDataReader r)
        {
            var Result = new List<MotisDataDef.PickingZone>();
            while (r.Read())
            {
                Result.Add(new MotisDataDef.PickingZone()
                {
                    Id = r.GetInt16(0)
                });
            }

            return Result;
        }
    }
}
