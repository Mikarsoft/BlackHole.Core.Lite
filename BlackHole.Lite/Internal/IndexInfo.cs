

namespace BlackHole.Internal
{
    internal class IndexInfo
    {
        internal IndexInfo(string[] propNames, int groupId)
        {
            PropertyNames = propNames;
            GroupId = groupId;
        }

        internal string[] PropertyNames { get; set; }
        internal int GroupId { get; set; }
    }
}
