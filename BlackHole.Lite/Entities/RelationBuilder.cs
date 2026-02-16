using System.Linq.Expressions;

namespace BlackHole.Lite.Entities
{
    public class RelationBuilder<T> where T : BHEntity
    {
        internal List<BHEntityIndex> Indices = new();
        internal List<IBHRelation> Relations = new();

        public BHEntityIndex HasIndex<TKey>(Expression<Func<T, TKey>> property)
        {
            Indices.Add(new BHEntityIndex(property.GetPropertyNames()));
            return Indices[^1];
        }

        public BHIncludeKey<T, G> HasOne<G>(Expression<Func<T, BHIncludeItem<G>>> include) where G : BHEntity
        {
            var key = new BHIncludeKey<T, G>(include.GetPropertyName(), false);
            Relations.Add(key);
            return key;
        }

        public BHIncludeKey<T, G> HasMany<G>(Expression<Func<T, BHIncludeList<G>>> include) where G : BHEntity
        {
            var key = new BHIncludeKey<T, G>(include.GetPropertyName(), true);
            Relations.Add(key);
            return key;
        }

    }

    public class BHIncludeKey<T, G> : IBHRelation where T : BHEntity where G : BHEntity
    {
        internal string PropertyName { get; set; }

        internal bool IsList { get; set; }

        internal BHIncludeKey(string propertyName, bool many)
        {
            PropertyName = propertyName;
            IsList = many;
        }

        internal BHForeignKey<T, G> ForeignKey { get; set; } = new();

        Type IBHRelation.MainType => typeof(T);

        Type IBHRelation.IncludedType => typeof(G);

        string IBHRelation.PropertyName => PropertyName;

        bool IBHRelation.IsList => IsList;

        BHDeleteCase IBHRelation.OnDelete => _onDelete;

        string IBHRelation.ForeignKeyPropertyName => ForeignKey.PropertyName;

        bool IBHRelation.ForeignKeyOnMain => ForeignKey.KeyOnMain;

        BHBackwardIncludeInfo? IBHRelation.BackwardInclude => _backwardsInclude;

        private BHDeleteCase _onDelete { get; set; } = new();

        private BHBackwardIncludeInfo? _backwardsInclude { get; set; }

        public BHForeignKey<T, G> WithOne(Expression<Func<G, BHIncludeItem<T>>>? include = null)
        {
            if (include != null)
            {
                string prop = include.GetPropertyName();
                _backwardsInclude = new(prop, false);
            }

            return ForeignKey;
        }

        public BHForeignKey<T, G> WithMany(Expression<Func<G, BHIncludeList<T>>>? include = null)
        {
            if (include != null)
            {
                string prop = include.GetPropertyName();
                _backwardsInclude = new(prop, true);
            }

            return ForeignKey;
        }
    }

    public class BHBackwardIncludeInfo
    {
        public string PropertyName { get; }
        public bool IsList { get; }

        public BHBackwardIncludeInfo(string propertyName, bool isList)
        {
            PropertyName = propertyName;
            IsList = isList;
        }
    }

    public class BHForeignKey<T, G>
    {
        internal BHForeignKey()
        {

        }

        internal bool KeyOnMain { get; set; }

        internal string PropertyName { get; set; } = null!;

        internal BHDeleteCase OnDelete { get; set; } = new();

        public BHDeleteCase HasForeignKeyTo<GKey>(Expression<Func<G, GKey>> foreignKey)
        {
            KeyOnMain = false;
            PropertyName = foreignKey.GetPropertyName();
            return OnDelete;
        }

        public BHDeleteCase HasForeignKeyFrom<TKey>(Expression<Func<T, TKey>> foreignKey)
        {
            KeyOnMain = true;
            PropertyName = foreignKey.GetPropertyName();
            return OnDelete;
        }
    }

    public class BHDeleteCase
    {
        internal BHDeleteCase() { }

        internal OnDeleteBehavior DeleteAction {  get; set; }
        public void OnDelete(OnDeleteBehavior onDelete)
        {
            DeleteAction = onDelete;
        }
    }

    public enum OnDeleteBehavior
    {
        Restrict,
        SetNull,
        Cascade
    }

    public class BHEntityIndex
    {
        internal bool IsIndexUnique { get; private set; }

        internal string[] IndexColumns {  get; private set; }

        internal BHEntityIndex(IEnumerable<string> columns)
        {
            IndexColumns = columns.ToArray();
        }

        public void IsUnique(bool isUnique = true)
        {
            IsIndexUnique = isUnique;
        }
    }

    internal interface IBHRelation
    {
        Type MainType { get; }
        Type IncludedType { get; }
        string PropertyName { get; }
        bool IsList { get; }
        BHDeleteCase OnDelete { get; }
        string ForeignKeyPropertyName { get; }
        bool ForeignKeyOnMain { get; }
        BHBackwardIncludeInfo? BackwardInclude { get; }
    }


    internal static class RelationExtensions
    {
        internal static IEnumerable<string> GetPropertyNames<T, TKey>(this Expression<Func<T, TKey>> expression)
        {
            var body = expression.Body;

            // Unwrap Convert(...) for nullable/object casting
            if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
                body = unary.Operand;

            if (body is NewExpression newExpr)
                return newExpr.Members?.Select(m => m.Name) ?? [];

            if (body is MemberExpression memberExpr)
                return new[] { memberExpr.Member.Name };

            throw new ArgumentException("Unsupported expression type");
        }

        internal static string GetPropertyName<T, TKey>(this Expression<Func<T, TKey>> expression)
        {
            if (expression.Body is MemberExpression member)
                return member.Member.Name;

            if (expression.Body is UnaryExpression unary &&
                unary.Operand is MemberExpression unaryMember)
                return unaryMember.Member.Name;

            throw new ArgumentException("Invalid expression");
        }
    }
}
