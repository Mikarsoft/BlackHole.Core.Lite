
namespace BlackHole.Entities
{
    /// <summary>
    /// Sets Foreign Key for this Column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ForeignKey<T> : Attribute
    {
        /// <summary>
        /// Name of the Foreign Table
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Name of the column
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// On Delete
        /// </summary>
        public string CascadeInfo { get; set; }

        /// <summary>
        /// Nullability boolean
        /// </summary>
        public bool Nullability { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OnDeleteBehavior OnDelete {  get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onDelete"></param>
        /// <param name="isNullable"></param>
        public ForeignKey(OnDeleteBehavior onDelete, bool isNullable)
        {
            Type table = typeof(T);
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
        /// This Overload of the Constructor Sets by Default the corresponding column
        /// on the Primary Table as Id. You Can choose the Primary Table and
        /// if the Foreign Key is Nullable
        /// </summary>
        /// <param name="isNullable">Is this Column Nullable?</param>
        public ForeignKey(bool isNullable)
        {
            Type table = typeof(T);
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
        /// This Overload of the Constructor Sets by Default the corresponding column
        /// on the Primary Table as Id and makes the Foreign Key Column Nullable.
        /// You Can choose the Primary Table
        /// </summary>
        public ForeignKey()
        {
            Type table = typeof(T);
            TableName = table.Name;
            Column = "Id";
            CascadeInfo = "on delete set null";
            Nullability = true;
            OnDelete = OnDeleteBehavior.SetNull;
        }
    }
}
