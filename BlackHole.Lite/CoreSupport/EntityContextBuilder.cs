using BlackHole.Core;
using BlackHole.Internal;

namespace BlackHole.CoreSupport
{
    internal class EntityContextBuilder
    {
        internal void InsertEntityContext(TableCompleteInfo tableInfo)
        {
            BHEntityContext? existingContext = BHDataProvider.EntitiesContext.FirstOrDefault(x => x.EntityType == tableInfo.TableType);

            if (existingContext == null)
            {
                using (TripleStringBuilder sb = new())
                {
                    List<string> Columns = new();
                    foreach (ColumnInfo prop in tableInfo.Columns)
                    {
                        if (prop.PropertyName != "Inactive")
                        {
                            if (!prop.IsPrimaryKey)
                            {
                                sb.PNSb.Append($", {prop.PropertyName}");
                                sb.PPSb.Append($", @{prop.PropertyName}");
                                sb.UPSb.Append($",{prop.PropertyName} = @{prop.PropertyName}");
                            }
                            Columns.Add(prop.PropertyName);
                        }
                    }

                    BHDataProvider.EntitiesContext.Add(new(tableInfo.TableType, tableInfo.TableType.CheckActivator(),
                        tableInfo.TableType.Name, Columns, $"{sb.PNSb.ToString().Remove(0, 1)} ",
                        $"{sb.PPSb.ToString().Remove(0, 1)} ",
                        $"{sb.UPSb.ToString().Remove(0, 1)} "));
                }

                foreach(var fk in tableInfo.ForeignKeys)
                {
                    if (!string.IsNullOrEmpty(fk.MapPropertyName))
                    {
                        BHDataProvider.FkMap.Add($"{tableInfo.TableType.Name}_{fk.MapPropertyName}", fk.PropertyName);

                        if (!string.IsNullOrEmpty(fk.BackwardsIncludeProp))
                        {
                            BHDataProvider.FkReverseMap.Add($"{fk.ReferencedTable}_{fk.BackwardsIncludeProp}", fk.PropertyName);
                        }
                    }
                }
            }
        }
    }

    internal static class BHAllowedTypes
    {
        internal static HashSet<Type> AllowedTypes = new()
        {
            typeof(int), typeof(long), typeof(short), typeof(byte),
            typeof(bool), typeof(char), typeof(float), typeof(double),
            typeof(decimal), typeof(string), typeof(Guid),
            typeof(DateTime), typeof(DateTimeOffset), typeof(byte[])
        };

        internal static bool IsAllowedType(this Type type)
        {
            var underlying = Nullable.GetUnderlyingType(type) ?? type;
            return AllowedTypes.Contains(underlying);
        }
    }
}
