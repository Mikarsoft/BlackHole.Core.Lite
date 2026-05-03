using System.Linq.Expressions;

namespace BlackHole.Entities
{
    /// <summary>
    /// Fluent builder for configuring entity relationships, foreign keys, and indexes.
    /// Used within <see cref="BHEntity{T}.Congifure(RelationBuilder{T})"/> to define database constraints.
    /// </summary>
    /// <typeparam name="T">The entity type being configured. Must derive from <see cref="BHEntity"/>.</typeparam>
    /// <remarks>
    /// Provides methods to define one-to-one relationships via <see cref="HasOne{G}(Expression{Func{T, BHIncludeItem{G}}})"/>
    /// and indexes via <see cref="HasIndex{TKey}(Expression{Func{T, TKey}})"/>.
    /// </remarks>
    public class RelationBuilder<T> where T : BHEntity
    {
        internal List<BHEntityIndex> Indices = new();
        internal List<IBHRelation> Relations = new();

        /// <summary>
        /// Configures a database index on one or more properties of this entity.
        /// </summary>
        /// <typeparam name="TKey">The type of the property being indexed, or an anonymous type for composite indexes.</typeparam>
        /// <param name="property">Lambda expression selecting the property or properties to index.
        /// For a single property: <c>x => x.Email</c>. For composite: <c>x => new { x.Name, x.Department }</c>.</param>
        /// <returns>A <see cref="BHEntityIndex"/> object that can be configured with <see cref="BHEntityIndex.IsUnique(bool)"/>.</returns>
        /// <example>
        /// <code>
        /// modelBuilder.HasIndex(x => x.Email).IsUnique(true);
        /// modelBuilder.HasIndex(x => new { x.FirstName, x.LastName }).IsUnique(true);
        /// </code>
        /// </example>
        public BHEntityIndex HasIndex<TKey>(Expression<Func<T, TKey>> property)
        {
            Indices.Add(new BHEntityIndex(property.GetPropertyNames()));
            return Indices[^1];
        }

        /// <summary>
        /// Declares a one-to-one relationship from this entity to another entity.
        /// </summary>
        /// <typeparam name="G">The related entity type. Must derive from <see cref="BHEntity"/>.</typeparam>
        /// <param name="include">Lambda expression selecting the <see cref="BHIncludeItem{G}"/> navigation property.</param>
        /// <returns>A <see cref="BHIncludeKey{T, G}"/> that can be further configured with <see cref="BHIncludeKey{T, G}.WithOne(Expression{Func{G, BHIncludeItem{T}}})"/>
        /// or <see cref="BHIncludeKey{T, G}.WithMany(Expression{Func{G, BHIncludeList{T}}})"/> to establish the other side of the relationship.</returns>
        /// <remarks>
        /// After calling HasOne, you must chain either WithOne (for one-to-one) or WithMany (for one-to-many on the other side)
        /// to complete the relationship configuration.
        /// </remarks>
        /// <example>
        /// <code>
        /// modelBuilder.HasOne(x => x.Manager)
        ///     .WithMany(x => x.Employees)
        ///     .HasForeignKey(x => x.ManagerId)
        ///     .OnDelete(OnDeleteBehavior.SetNull);
        /// </code>
        /// </example>
        public BHIncludeKey<T, G> HasOne<G>(Expression<Func<T, BHIncludeItem<G>>> include) where G : BHEntity
        {
            var key = new BHIncludeKey<T, G>(include.GetPropertyName(), false);
            Relations.Add(key);
            return key;
        }
    }

    /// <summary>
    /// Represents a declared one-to-one relationship and provides methods to complete its configuration.
    /// Returned by <see cref="RelationBuilder{T}.HasOne{G}(Expression{Func{T, BHIncludeItem{G}}})"/>.
    /// </summary>
    /// <typeparam name="T">The entity type declaring the relationship.</typeparam>
    /// <typeparam name="G">The related entity type.</typeparam>
    /// <remarks>
    /// Chain with <see cref="WithOne(Expression{Func{G, BHIncludeItem{T}}})"/> or <see cref="WithMany(Expression{Func{G, BHIncludeList{T}}})"/>
    /// to configure the inverse relationship and access <see cref="BHForeignKey{T, G}"/> for foreign key configuration.
    /// </remarks>
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

        BHDeleteCase IBHRelation.OnDelete => ForeignKey.OnDelete;

        string IBHRelation.ForeignKeyPropertyName => ForeignKey.PropertyName;

        BHBackwardIncludeInfo? IBHRelation.BackwardInclude => _backwardsInclude;

        private BHBackwardIncludeInfo? _backwardsInclude { get; set; }

        /// <summary>
        /// Configures a one-to-one relationship by specifying the inverse navigation property on the related entity.
        /// </summary>
        /// <param name="include">Optional lambda expression selecting the <see cref="BHIncludeItem{T}"/> navigation property on entity G.
        /// If null, no backward navigation is defined.</param>
        /// <returns>A <see cref="BHForeignKey{T, G}"/> to configure the foreign key column and delete behavior.</returns>
        public BHForeignKey<T, G> WithOne(Expression<Func<G, BHIncludeItem<T>>>? include = null)
        {
            if (include != null)
            {
                string prop = include.GetPropertyName();
                _backwardsInclude = new(prop, false);
            }

            return ForeignKey;
        }

        /// <summary>
        /// Configures a one-to-many relationship by specifying the collection navigation property on the related entity.
        /// </summary>
        /// <param name="include">Optional lambda expression selecting the <see cref="BHIncludeList{T}"/> collection on entity G.
        /// If null, no backward navigation is defined.</param>
        /// <returns>A <see cref="BHForeignKey{T, G}"/> to configure the foreign key column and delete behavior.</returns>
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

    /// <summary>
    /// Internal metadata for backward/inverse navigation properties in a relationship.
    /// </summary>
    /// <remarks>
    /// This class is used internally to track the inverse navigation property name and whether it is a one-to-one or one-to-many relationship.
    /// </remarks>
    public class BHBackwardIncludeInfo
    {
        internal string PropertyName { get; }
        internal bool IsList { get; }

        internal BHBackwardIncludeInfo(string propertyName, bool isList)
        {
            PropertyName = propertyName;
            IsList = isList;
        }
    }

    /// <summary>
    /// Configures the foreign key column and delete behavior for a relationship.
    /// Returned by <see cref="BHIncludeKey{T, G}.WithOne(Expression{Func{G, BHIncludeItem{T}}})"/> or
    /// <see cref="BHIncludeKey{T, G}.WithMany(Expression{Func{G, BHIncludeList{T}}})"/>.
    /// </summary>
    /// <typeparam name="T">The entity type declaring the relationship.</typeparam>
    /// <typeparam name="G">The related entity type.</typeparam>
    public class BHForeignKey<T, G>
    {
        internal BHForeignKey()
        {

        }

        internal string PropertyName { get; set; } = null!;

        internal BHDeleteCase OnDelete { get; set; } = new();


        /// <summary>
        /// Specifies which property on this entity holds the foreign key value.
        /// </summary>
        /// <typeparam name="TKey">The type of the foreign key property (typically int or int?).</typeparam>
        /// <param name="foreignKey">Lambda expression selecting the foreign key property (e.g., <c>x => x.CustomerId</c>).</param>
        /// <returns>A <see cref="BHDeleteCase"/> to configure the delete behavior via <see cref="BHDeleteCase.OnDelete(OnDeleteBehavior)"/>.</returns>
        /// <remarks>
        /// If the property is nullable (TKey is int?), you can use <see cref="OnDeleteBehavior.SetNull"/>.
        /// If non-nullable (TKey is int), you can only use <see cref="OnDeleteBehavior.Restrict"/> or <see cref="OnDeleteBehavior.Cascade"/>.
        /// </remarks>
        public BHDeleteCase HasForeignKey<TKey>(Expression<Func<T, TKey?>> foreignKey)
        {
            var propertyType = typeof(TKey);
            bool isNullable = Nullable.GetUnderlyingType(propertyType) != null;

            OnDelete.IsNullable = isNullable;
            PropertyName = foreignKey.GetPropertyName();
            return OnDelete;
        }
    }

    /// <summary>
    /// Specifies the delete behavior for a foreign key relationship.
    /// Returned by <see cref="BHForeignKey{T, G}.HasForeignKey{TKey}(Expression{Func{T, TKey}})"/>.
    /// </summary>
    /// <remarks>
    /// The delete behavior determines what happens to child records when a parent record is deleted.
    /// If the foreign key is non-nullable, <see cref="OnDeleteBehavior.SetNull"/> is not allowed and will throw an exception.
    /// </remarks>
    public class BHDeleteCase
    {
        internal BHDeleteCase() { }

        internal OnDeleteBehavior DeleteAction {  get; set; }

        internal bool IsNullable { get; set; }

        /// <summary>
        /// Sets the delete behavior for this foreign key relationship.
        /// </summary>
        /// <param name="onDelete">The delete behavior: <see cref="OnDeleteBehavior.Restrict"/>, <see cref="OnDeleteBehavior.SetNull"/>, or <see cref="OnDeleteBehavior.Cascade"/>.</param>
        /// <remarks>
        /// If the foreign key property is non-nullable, <see cref="OnDeleteBehavior.SetNull"/> will throw an exception.
        /// </remarks>
        /// <exception cref="Exception">Thrown if <see cref="OnDeleteBehavior.SetNull"/> is used on a non-nullable foreign key.</exception>
        public void OnDelete(OnDeleteBehavior onDelete)
        {
            if (!IsNullable && onDelete == OnDeleteBehavior.SetNull)
            {
                throw new Exception("On Delete Behaviour Can't be 'SetNull' on a Non Nullable Column");
            }

            DeleteAction = onDelete;
        }
    }

    /// <summary>
    /// Specifies the behavior when a parent entity is deleted in a foreign key relationship.
    /// </summary>
    public enum OnDeleteBehavior
    {
        /// <summary>
        /// Prevents deletion of the parent if child records exist; raises an error.
        /// </summary>
        Restrict,

        /// <summary>
        /// Sets the foreign key to null on child records when the parent is deleted.
        /// Only valid if the foreign key property is nullable.
        /// </summary>
        SetNull,

        /// <summary>
        /// Automatically deletes child records when the parent is deleted.
        /// </summary>
        Cascade
    }

    /// <summary>
    /// Configures a database index on one or more columns.
    /// Returned by <see cref="RelationBuilder{T}.HasIndex{TKey}(Expression{Func{T, TKey}})"/>.
    /// </summary>
    public class BHEntityIndex
    {
        internal bool IsIndexUnique { get; private set; }

        internal string[] IndexColumns {  get; private set; }

        internal BHEntityIndex(IEnumerable<string> columns)
        {
            IndexColumns = columns.ToArray();
        }

        /// <summary>
        /// Marks this index as unique (or non-unique if passed false).
        /// </summary>
        /// <param name="isUnique">If true, creates a UNIQUE index; if false, creates a regular index. Defaults to true.</param>
        /// <remarks>
        /// Unique indexes enforce that no duplicate values (or combinations for composite indexes) can exist in the indexed columns.
        /// </remarks>

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
