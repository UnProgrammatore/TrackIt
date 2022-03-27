using System.Data;
using Dapper;

namespace TrackItAPI.Database.TypeMappers;

public class DateTimeTypeMapper : SqlMapper.TypeHandler<DateTime> 
{
    public override DateTime Parse(object value)
    {
        return DateTime.Parse(value?.ToString() ?? "");
    }

    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = value.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public static void Set() 
    {
        SqlMapper.RemoveTypeMap(typeof(DateTime));
        SqlMapper.AddTypeHandler(new DateTimeTypeMapper());
    }
}