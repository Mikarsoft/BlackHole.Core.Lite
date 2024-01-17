using BlackHole.Core;
using System.Reflection;

namespace BlackHole.CoreSupport
{
    internal class EntityContextBuilder
    {
        internal void InsertEntityContext(Type EntityType)
        {
            using (TripleStringBuilder sb = new())
            {
                List<string> Columns = new();
                foreach (PropertyInfo prop in EntityType.GetProperties())
                {
                    if (prop.Name != "Inactive")
                    {
                        if (prop.Name != "Id")
                        {
                            sb.PNSb.Append($", {prop.Name}");
                            sb.PPSb.Append($", @{prop.Name}");
                            sb.UPSb.Append($",{prop.Name} = @{prop.Name}");
                        }
                        Columns.Add(prop.Name);
                    }
                }

                BHDataProvider.EntitiesContext.Add(new(EntityType, EntityType.CheckActivator(),
                    EntityType.Name, Columns, $"{sb.PNSb.ToString().Remove(0, 1)} ",
                    $"{sb.PPSb.ToString().Remove(0, 1)} ",
                    $"{sb.UPSb.ToString().Remove(0, 1)} "));
            }
        }
    }
}
