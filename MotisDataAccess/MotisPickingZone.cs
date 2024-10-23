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
    public class MotisPickingZone(Motis Motis)
    : Shell<MotisDataDef.MotisPickingZone>
    {
        public Shell<MotisDataDef.MotisPickingZone> GetPickingZones()
           => GetPickingZones(Motis);

        public static Shell<MotisDataDef.MotisPickingZone> GetPickingZones(Motis Motis)
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

        private static List<MotisDataDef.MotisPickingZone> ReaderToObjectList(SqlDataReader r)
        {
            var Result = new List<MotisDataDef.MotisPickingZone>();
            while (r.Read())
            {
                Result.Add(new MotisDataDef.MotisPickingZone()
                {
                    Id = r.GetInt16(0)
                });
            }

            return Result;
        }
    }
}
