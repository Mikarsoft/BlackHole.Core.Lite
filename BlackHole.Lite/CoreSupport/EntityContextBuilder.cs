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

                    string propNames = sb.PNSb.ToString();
                    string paramNames = sb.PPSb.ToString();
                    string updateNames = sb.UPSb.ToString();

                    string propNamesFinal = propNames.Length > 1 ? $"{propNames.Remove(0,1)} " : string.Empty;
                    string paramNamesFinal = paramNames.Length > 1 ? $"{paramNames.Remove(0, 1)} " : string.Empty;
                    string updateNamesFinal = updateNames.Length > 1 ? $"{updateNames.Remove(0, 1)} " : string.Empty;

                    BHDataProvider.EntitiesContext.Add(new(tableInfo.TableType, tableInfo.TableType.CheckActivator(),
                        tableInfo.TableType.Name, Columns, propNamesFinal, paramNamesFinal, updateNamesFinal));
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
