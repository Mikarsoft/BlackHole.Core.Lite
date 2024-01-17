
namespace BlackHole.CoreSupport
{
    internal class StoredView
    {
        internal Type? DtoType { get; set; }
        internal string CommandText { get; set; } = string.Empty;
        internal List<BlackHoleParameter>? DynamicParams { get; set; }
    }
}
