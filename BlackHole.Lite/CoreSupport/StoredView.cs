
namespace BlackHole.CoreSupport
{
    internal class StoredView
    {
        internal string DatabaseName { get; set; } = string.Empty;
        internal Type? DtoType { get; set; }
        internal string CommandText { get; set; } = string.Empty;
        internal List<BlackHoleParameter>? DynamicParams { get; set; }
    }
}
