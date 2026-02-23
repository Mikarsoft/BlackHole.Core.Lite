using BlackHole.Entities;

namespace BlackHole.Internal
{
    internal class FKInfo
    {
        internal string? MapPropertyName { get; set; }
        internal string? BackwardsIncludeProp { get; set; }
        internal string PropertyName { get; set; } = string.Empty;
        internal string ReferencedTable { get; set; } = string.Empty;
        internal string ReferencedColumn { get; set; } = string.Empty;
        internal bool IsNullable { get; set; }
        internal OnDeleteBehavior OnDelete {  get; set; }
    }
}
