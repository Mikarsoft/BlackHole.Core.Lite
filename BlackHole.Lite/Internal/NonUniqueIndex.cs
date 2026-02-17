

namespace BlackHole.Lite.Internal
{
    internal class NonUniqueIndex
    {
        internal NonUniqueIndex(string[] propNames, int groupId)
        {
            PropertyNames = propNames;
            GroupId = groupId;
        }

        internal string[] PropertyNames { get; set; }
        internal int GroupId { get; set; }
    }
}
