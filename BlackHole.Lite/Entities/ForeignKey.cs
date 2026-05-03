
namespace BlackHole.Entities
{
    /// <summary>
    /// Attribute to declare a foreign key relationship on a property.
    /// Alternative to using fluent configuration via <see cref="RelationBuilder{T}.HasOne{G}(Expression{Func{T, BHIncludeItem{G}}})"/>.
    /// </summary>
    /// <remarks>
    /// Apply this attribute to a foreign key property (typically an int or int?) to establish a relationship to another entity.
    /// Use one of the constructors to specify the target entity type and delete behavior.
    /// If the property is non-nullable and you attempt to use <see cref="OnDeleteBehavior.SetNull"/>, an exception is thrown.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class Order : BHEntity&lt;Order&gt;
    /// {
    ///     [ForeignKey(typeof(Customer), OnDeleteBehavior.SetNull, true)]
    ///     public int? CustomerId { get; set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ForeignKey : Attribute
    {
        /// <summary>
        /// Name of the target entity (foreign table).
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Name of the primary key column in the target table (typically "Id").
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// SQL cascade clause string (e.g., "on delete cascade" or "on delete set null").
        /// </summary>
        public string CascadeInfo { get; set; }

        /// <summary>
        /// Indicates whether the foreign key column is nullable.
        /// </summary>
        public bool Nullability { get; set; }

        /// <summary>
        /// The delete behavior when the parent entity is deleted.
        /// </summary>
        public OnDeleteBehavior OnDelete {  get; set; }

        /// <summary>
        /// Initializes a new instance with explicit delete behavior and nullability.
        /// </summary>
        /// <param name="table">The target entity type (e.g., typeof(Customer)).</param>
        /// <param name="onDelete">The delete behavior.</param>
        /// <param name="isNullable">Whether the foreign key column is nullable.</param>
        /// <exception cref="Exception">Thrown if isNullable is false and onDelete is SetNull.</exception>
        public ForeignKey(Type table, OnDeleteBehavior onDelete, bool isNullable)
        {
            TableName = table.Name;
            Column = "Id";
            Nullability = isNullable;
            OnDelete = onDelete;

            if (isNullable)
            {
                CascadeInfo = "on delete set null";
            }
            else
            {
                if (OnDelete == OnDeleteBehavior.SetNull)
                {
                    throw new Exception("On Delete Behaviour Can't be 'SetNull' on a Non Nullable Column");
                }

                OnDelete = OnDeleteBehavior.Cascade;
                CascadeInfo = "on delete cascade";
            }
        }

        /// <summary>
        /// Initializes a new instance with nullability specified; delete behavior is inferred from nullability.
        /// If nullable, delete behavior defaults to SetNull; if non-nullable, defaults to Cascade.
        /// </summary>
        /// <param name="table">The target entity type (e.g., typeof(Customer)).</param>
        /// <param name="isNullable">Whether the foreign key column is nullable.</param>
        public ForeignKey(Type table, bool isNullable)
        {
            TableName = table.Name;
            Column = "Id";
            Nullability = isNullable;

            if (isNullable)
            {
                CascadeInfo = "on delete set null";
                OnDelete = OnDeleteBehavior.SetNull;
            }
            else
            {
                OnDelete = OnDeleteBehavior.Cascade;
                CascadeInfo = "on delete cascade";
            }
        }

        /// <summary>
        /// Initializes a new instance with the target table only; both nullability and delete behavior default to SetNull.
        /// </summary>
        /// <param name="table">The target entity type (e.g., typeof(Customer)).</param>
        public ForeignKey(Type table)
        {
            TableName = table.Name;
            Column = "Id";
            CascadeInfo = "on delete set null";
            Nullability = true;
            OnDelete = OnDeleteBehavior.SetNull;
        }
    }
}
