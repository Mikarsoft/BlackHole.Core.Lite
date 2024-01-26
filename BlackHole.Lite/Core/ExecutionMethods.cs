using BlackHole.CoreSupport;
using BlackHole.DataProviders;
using BlackHole.Entities;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// Methods that extend the BHEntityContext and contain 
    /// all the required commands to interact with the database
    /// </summary>
    public static class ExecutionMethods
    {
        internal static SqliteDataProvider _dataProvider = BHDataProviderSelector.GetDataProvider();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool Any<T>(this BHEntityContext<T> context) where T : BlackHoleEntity
        {
            string limit = 1.GetLimiter();
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select Id ,{context.PropertyNames} from {context.ThisTable} where Inactive = 0 {limit}", null, context.ConnectionString) != null;
            }
            return _dataProvider.QueryFirst<T>($"select Id ,{context.PropertyNames} from {context.ThisTable} {limit}", null, context.ConnectionString) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool Any<T>(this BHEntityContext<T> context, Expression<Func<T,bool>> predicate) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(string.Empty, null, 0);
            string limit = 1.GetLimiter();
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select Id ,{context.PropertyNames} from {context.ThisTable} where Inactive = 0 and {sql.Columns} {limit}", sql.Parameters, context.ConnectionString) != null;
            }
            return _dataProvider.QueryFirst<T>($"select Id ,{context.PropertyNames} from {context.ThisTable} where {sql.Columns} {limit}", sql.Parameters, context.ConnectionString) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool Any<T>(this BHEntityContext<T> context, BHTransaction bHTransaction) where T : BlackHoleEntity
        {
            string limit = 1.GetLimiter();
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select Id ,{context.PropertyNames} from {context.ThisTable} where Inactive = 0 {limit}", null, bHTransaction.transaction) != null;
            }
            return _dataProvider.QueryFirst<T>($"select Id ,{context.PropertyNames} from {context.ThisTable} {limit}", null, bHTransaction.transaction) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool Any<T>(this BHEntityContext<T> context, Expression<Func<T, bool>> predicate, BHTransaction bHTransaction) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(string.Empty, null, 0);
            string limit = 1.GetLimiter();
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select Id ,{context.PropertyNames} from {context.ThisTable} where Inactive = 0 and {sql.Columns} {limit}", sql.Parameters, bHTransaction.transaction) != null;
            }
            return _dataProvider.QueryFirst<T>($"select Id ,{context.PropertyNames} from {context.ThisTable} where {sql.Columns} {limit}", sql.Parameters, bHTransaction.transaction) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int Count<T>(this BHEntityContext<T> context) where T : BlackHoleEntity
        {
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<int>($"select count(Id) from {context.ThisTable} where Inactive = 0", null, context.ConnectionString);
            }
            return _dataProvider.QueryFirst<int>($"select count(Id) from {context.ThisTable}", null, context.ConnectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int Count<T>(this BHEntityContext<T> context, Expression<Func<T, bool>> predicate) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(string.Empty, null, 0);

            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<int>($"select count(Id) from {context.ThisTable} where Inactive = 0 and {sql.Columns}", sql.Parameters, context.ConnectionString);
            }
            return _dataProvider.QueryFirst<int>($"select count(Id) from {context.ThisTable} where {sql.Columns}", sql.Parameters, context.ConnectionString);
        }

        /// <summary>
        /// Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <returns>List of All Table's Entities</returns>
        public static List<T> GetAllEntries<T>(this BHEntityContext<T> context) where T : BlackHoleEntity
        {
            if (context.WithActivator)
            {
                return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Inactive = 0", null, context.ConnectionString);
            }
            return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable}", null, context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b> Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>List of All Table's Entities</returns>
        public static List<T> GetAllEntries<T>(this BHEntityContext<T> context, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            if (context.WithActivator)
            {
                return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Inactive = 0", null, bhTransaction.transaction);
            }
            return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable}", null, bhTransaction.transaction);
        }

        /// <summary>
        /// In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return a List of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <returns>List of Inactive Entities</returns>
        public static List<T> GetAllInactiveEntries<T>(this BHEntityContext<T> context) where T : BlackHoleEntity
        {
            if (context.WithActivator)
            {
                return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Inactive = 1", null, context.ConnectionString);
            }
            return new List<T>();
        }

        /// <summary>
        /// <b>Transaction.</b> In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return an IList of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>List of Inactive Entities</returns>
        public static List<T> GetAllInactiveEntries<T>(this BHEntityContext<T> context, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            if (context.WithActivator)
            {
                return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Inactive = 1", null, bhTransaction.transaction);
            }
            return new List<T>();
        }

        /// <summary>
        /// Returns the Entity from this Table that has the
        /// specified Id
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="Id">Id of the Entry</param>
        /// <returns>Entity</returns>
        public static T? GetEntryById<T>(this BHEntityContext<T> context, int Id) where T : BlackHoleEntity
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Id = @Id and Inactive = 0", Params.Parameters, context.ConnectionString);
            }
            return _dataProvider.QueryFirst<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Id = @Id", Params.Parameters, context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b> Returns the Entity from this Table that has the
        /// specified Id
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="Id">Id of the Entry</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>Entity</returns>
        public static T? GetEntryById<T>(this BHEntityContext<T> context, int Id, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Id = @Id and Inactive = 0", Params.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.QueryFirst<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Id = @Id", Params.Parameters, bhTransaction.transaction);
        }

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Entity</returns>
        public static T? GetEntryWhere<T>(this BHEntityContext<T> context, Expression<Func<T, bool>> predicate) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(string.Empty, null, 0);
            string limit = 1.GetLimiter();
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Inactive = 0 and {sql.Columns} {limit}", sql.Parameters, context.ConnectionString);
            }
            return _dataProvider.QueryFirst<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where {sql.Columns} {limit}", sql.Parameters, context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>Entity</returns>
        public static T? GetEntryWhere<T>(this BHEntityContext<T> context, Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(string.Empty, null, 0);
            string limit = 1.GetLimiter();
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Inactive = 0 and {sql.Columns} {limit}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.QueryFirst<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where {sql.Columns} {limit}", sql.Parameters, bhTransaction.transaction);
        }

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of Entities</returns>
        public static List<T> GetEntriesWhere<T>(this BHEntityContext<T> context, Expression<Func<T, bool>> predicate) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(string.Empty, null, 0);
            if (context.WithActivator)
            {
                return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Inactive = 0 and {sql.Columns}", sql.Parameters, context.ConnectionString);
            }
            return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where {sql.Columns}", sql.Parameters, context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters 
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate"></param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>List of Entities</returns>
        public static List<T> GetEntriesWhere<T>(this BHEntityContext<T> context, Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(string.Empty, null, 0);
            if (context.WithActivator)
            {
                return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Inactive = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        /// <summary>
        /// Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="entry">Entity to Insert</param>
        /// <returns>Inserted Id</returns>
        public static int InsertEntry<T>(this BHEntityContext<T> context, T entry) where T : BlackHoleEntity
        {
            return _dataProvider.InsertScalar($"insert into {context.ThisTable} ({context.PropertyNames},Inactive", $"values ({context.PropertyParams}, 0", entry, context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b> Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="entry">Entity to Insert</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>Inserted Id</returns>
        public static int InsertEntry<T>(this BHEntityContext<T> context, T entry, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            return _dataProvider.InsertScalar($"insert into {context.ThisTable} ({context.PropertyNames}, Inactive", $"values ({context.PropertyParams}, 0", entry, bhTransaction.transaction);
        }

        /// <summary>
        /// Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="entries">Entities to Insert</param>
        /// <returns>List of Inserted Ids</returns>
        public static List<int> InsertEntries<T>(this BHEntityContext<T> context, List<T> entries) where T : BlackHoleEntity
        {
            List<int> Ids = new();
            using (BlackHoleTransaction bhTransaction = new(context.ConnectionString))
            {
                Ids = _dataProvider.MultiInsertScalar($"insert into {context.ThisTable} ({context.PropertyNames},Inactive", $"values ({context.PropertyParams}, 0", entries, bhTransaction);
            }
            return Ids;
        }

        /// <summary>
        /// <b>Transaction.</b> Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="entries">Entities to Insert</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>List of Inserted Ids</returns>
        public static List<int> InsertEntries<T>(this BHEntityContext<T> context, List<T> entries, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            return _dataProvider.MultiInsertScalar<T>($"insert into {context.ThisTable} ({context.PropertyNames},Inactive", $"values ({context.PropertyParams}, 0", entries, bhTransaction.transaction);
        }

        /// <summary>
        /// Finds the entry in the table that has
        /// the same Id with the input's Entity and updates all
        /// the columns based on the Entity's property values. 
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="entry">Entry to Update</param>
        /// <returns>Success</returns>
        public static bool UpdateEntryById<T>(this BHEntityContext<T> context, T entry) where T : BlackHoleEntity
        {
            return _dataProvider.ExecuteEntry($"update {context.ThisTable} set {context.UpdateParams} where Id = @Id", entry, context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b> Finds the entry in the table that has
        /// the same Id with the input's Entity and updates all
        /// the columns based on the Entity's property values. 
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="entry">Entry to Update</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>Success</returns>
        public static bool UpdateEntryById<T>(this BHEntityContext<T> context, T entry, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            return _dataProvider.ExecuteEntry($"update {context.ThisTable} set {context.UpdateParams} where Id = @Id", entry, bhTransaction.transaction);
        }

        /// <summary>
        /// Finds the entries in the table that have
        /// the same Id with the input's Entities and updates all
        /// the columns based on each Entity's property values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para> 
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="entries">Entries to Update</param>
        /// <returns>Success</returns>
        public static bool UpdateEntriesById<T>(this BHEntityContext<T> context, List<T> entries) where T : BlackHoleEntity
        {
            return UpdateMany(entries, $"update {context.ThisTable} set {context.UpdateParams} where Id = @Id", context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b> Finds the entries in the table that have
        /// the same Id with the input's Entities and updates all
        /// the columns based on each Entity's property values. 
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="entries">Entries to Update</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>Success</returns>
        public static bool UpdateEntriesById<T>(this BHEntityContext<T> context, List<T> entries, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            return UpdateMany(entries, $"update {context.ThisTable} set {context.UpdateParams} where Id = @Id", bhTransaction.transaction);
        }

        /// <summary>
        /// Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Entry to Update</param>
        /// <returns>Success</returns>
        public static bool UpdateEntriesWhere<T>(this BHEntityContext<T> context, Expression<Func<T, bool>> predicate, T entry) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            if (context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {context.ThisTable} set {context.UpdateParams} where Inactive = 0 and {sql.Columns}", sql.Parameters, context.ConnectionString);
            }
            return _dataProvider.JustExecute($"update {context.ThisTable} set {context.UpdateParams} where {sql.Columns}", sql.Parameters, context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b>Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Entry to Update</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>Success</returns>
        public static bool UpdateEntriesWhere<T>(this BHEntityContext<T> context, Expression<Func<T, bool>> predicate, T entry, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            if (context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {context.ThisTable} set {context.UpdateParams} where Inactive = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.JustExecute($"update {context.ThisTable} set {context.UpdateParams} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        /// <summary>
        /// Deletes All entires of the database table.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <returns>Success</returns>
        public static bool DeleteAllEntries<T>(this BHEntityContext<T> context) where T : BlackHoleEntity
        {
            if (context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {context.ThisTable} set Inactive = 1 where Inactive = 0", null, context.ConnectionString);
            }
            return _dataProvider.JustExecute($"delete from {context.ThisTable}", null, context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b> Deletes All entires of the database table.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>Success</returns>
        public static bool DeleteAllEntries<T>(this BHEntityContext<T> context, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            if (context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {context.ThisTable} set Inactive = 1 where Inactive = 0", null, bhTransaction.transaction);
            }
            return _dataProvider.JustExecute($"delete from {context.ThisTable}", null, bhTransaction.transaction);
        }

        /// <summary>
        /// Finds and deletes the entry of the database table
        /// that has the same Id as the input.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entry gets deactivated instead of deleted and it can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="Id">Id of the Entry</param>
        /// <returns>Success</returns>
        public static bool DeleteEntryById<T>(this BHEntityContext<T> context, int Id) where T : BlackHoleEntity
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (context.WithActivator)
            {
                return _dataProvider.JustExecute($"Update {context.ThisTable} set Inactive = 1 where Id = @Id", Params.Parameters, context.ConnectionString);
            }
            return _dataProvider.JustExecute($"delete from {context.ThisTable} where Id = @Id", Params.Parameters, context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b>Finds and deletes the entry of the database table
        /// that has the same Id as the input.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entry gets deactivated instead of deleted and it can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="Id">Id of the Entry</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>Success</returns>
        public static bool DeleteEntryById<T>(this BHEntityContext<T> context, int Id, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {context.ThisTable} set Inactive = 1 where Id = @Id", Params.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.JustExecute($"delete from {context.ThisTable} where Id = @Id", Params.Parameters, bhTransaction.transaction);
        }

        /// <summary>
        /// If you are using a 'UseActivator' Attribute on this Entity
        /// It finds the entry in the database table that is Inactive and has the same
        /// Id as the input and permanently deletes it from the database.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="Id">Id of the Entry</param>
        /// <returns>Success</returns>
        public static bool DeleteInactiveEntryById<T>(this BHEntityContext<T> context, int Id) where T : BlackHoleEntity
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return _dataProvider.JustExecute($"delete from {context.ThisTable} where Id= @Id and Inactive = 1", Params.Parameters, context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b> If you are using a 'UseActivator' Attribute on this Entity
        /// It finds the entry in the database table that is Inactive and has the same
        /// Id as the input and permanently deletes it from the database.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="Id">Id of the Entry</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>Success</returns>
        public static bool DeleteInactiveEntryById<T>(this BHEntityContext<T> context, int Id, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return _dataProvider.JustExecute($"delete from {context.ThisTable} where Id = @Id and Inactive = 1", Params.Parameters, bhTransaction.transaction);
        }

        /// <summary>
        /// <b>Transaction.</b>Activates again an Inactive Entry
        /// in the database.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="Id">Id of the Entry</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>Success</returns>
        public static bool ReactivateEntryById<T>(this BHEntityContext<T> context, int Id, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return _dataProvider.JustExecute($"update {context.ThisTable} set Inactive = 0 where Id = @Id and Inactive = 1", Params.Parameters, bhTransaction.transaction);
        }

        /// <summary>
        /// Activates again an Inactive Entry
        /// in the database.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="Id">Id of the Entry</param>
        /// <returns>Success</returns>
        public static bool ReactivateEntryById<T>(this BHEntityContext<T> context, int Id) where T : BlackHoleEntity
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return _dataProvider.JustExecute($"update {context.ThisTable} set Inactive = 0 where Id = @Id and Inactive = 1", Params.Parameters, context.ConnectionString);
        }

        /// <summary>
        /// Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Success</returns>
        public static bool DeleteEntriesWhere<T>(this BHEntityContext<T> context, Expression<Func<T, bool>> predicate) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers(string.Empty, null, 0);
            if (context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {context.ThisTable} set Inactive = 1 where {sql.Columns}", sql.Parameters, context.ConnectionString);
            }
            return _dataProvider.JustExecute($"delete from {context.ThisTable} where {sql.Columns}", sql.Parameters, context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b>Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>Success</returns>
        public static bool DeleteEntriesWhere<T>(this BHEntityContext<T> context, Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers(string.Empty, null, 0);
            if (context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {context.ThisTable} set Inactive=1 where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.JustExecute($"delete from {context.ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        /// <summary>
        /// Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns their Ids
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of Ids</returns>
        public static List<int> GetIdsWhere<T>(this BHEntityContext<T> context, Expression<Func<T, bool>> predicate) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers(string.Empty, null, 0);
            if (context.WithActivator)
            {
                return _dataProvider.Query<int>($"select Id from {context.ThisTable} where Inactive = 0 and {sql.Columns}", sql.Parameters, context.ConnectionString);
            }
            return _dataProvider.Query<int>($"select Id from {context.ThisTable} where {sql.Columns}", sql.Parameters, context.ConnectionString);
        }

        /// <summary>
        /// <b>Transaction.</b> Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns their Ids
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>List of Ids</returns>
        public static List<int> GetIdsWhere<T>(this BHEntityContext<T> context, Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where T : BlackHoleEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers(string.Empty, null, 0);
            if (context.WithActivator)
            {
                return _dataProvider.Query<int>($"select Id from {context.ThisTable} where Inactive = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.Query<int>($"select Id from {context.ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        /// <summary>
        /// Combines the Data of the Join and let's you start
        /// another Join or Execute the Joins Data or Store them as View
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <param name="data">All data of the previous Joins sequence</param>
        /// <returns>Updated Joins Data</returns>
        public static JoinsData Then<T, TOther>(this JoinsData<T, TOther> data)
             where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return data.NextAction();
        }

        /// <summary>
        /// Starts an Inner Join Between two Entities which can 
        /// extend with additional joins.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <param name="previousData">All data of the previous Joins sequence</param>
        /// <returns>Updated PreJoins Data</returns>
        public static PreJoinsData<T, TOther> InnerJoin<T, TOther>(this JoinsData previousData)
             where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return previousData.NextJoin<T, TOther>("inner");
        }

        /// <summary>
        /// Starts an Inner Join Between two Entities which can 
        /// extend with additional joins.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <param name="previousData">All data of the previous Joins sequence</param>
        /// <returns>Updated PreJoins Data</returns>
        public static PreJoinsData<T, TOther> OuterJoin<T, TOther>(this JoinsData previousData)
             where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return previousData.NextJoin<T, TOther>("full outer");
        }

        /// <summary>
        /// Starts an Inner Join Between two Entities which can 
        /// extend with additional joins.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <param name="previousData">All data of the previous Joins sequence</param>
        /// <returns>Updated PreJoins Data</returns>
        public static PreJoinsData<T, TOther> RightJoin<T, TOther>(this JoinsData previousData)
             where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return previousData.NextJoin<T, TOther>("right");
        }

        /// <summary>
        /// Starts an Inner Join Between two Entities which can 
        /// extend with additional joins.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <param name="previousData">All data of the previous Joins sequence</param>
        /// <returns>Updated PreJoins Data</returns>
        public static PreJoinsData<T, TOther> LeftJoin<T, TOther>(this JoinsData previousData)
             where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return previousData.NextJoin<T, TOther>("left");
        }

        /// <summary>
        /// Sets the columns that the two tables will be joined on
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <typeparam name="TKey">Type of Property</typeparam>
        /// <param name="data">All data of the previous Joins sequence</param>
        /// <param name="key">Selected Property of First Entity</param>
        /// <param name="otherkey">Selected Property of Second Entity</param>
        /// <returns>Updated Joins Data</returns>
        public static JoinsData<T, TOther> On<T, TOther, TKey>(this PreJoinsData<T, TOther> data, Expression<Func<T, TKey>> key, Expression<Func<TOther, TKey>> otherkey)
            where T : BlackHoleEntity where TOther : BlackHoleEntity where TKey : IComparable<TKey>
        {
            return data.CreateJoin(key, otherkey);
        }

        /// <summary>
        /// Sets additional columns that the two tables will be joined on
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <typeparam name="TKey">Type of Property</typeparam>
        /// <param name="currentData">All data of the previous Joins sequence</param>
        /// <param name="key">Selected Property of First Entity</param>
        /// <param name="otherkey">Selected Property of Second Entity</param>
        /// <returns>Updated Joins Data</returns>
        public static JoinsData<T, TOther> And<T, TOther, TKey>(this JoinsData<T, TOther> currentData, Expression<Func<T, TKey>> key, Expression<Func<TOther, TKey>> otherkey)
             where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return currentData.AdditionalJoint(key, otherkey);
        }

        /// <summary>
        /// Sets optional columns that the two tables can be joined on
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <typeparam name="TKey">Type of Property</typeparam>
        /// <param name="currentData">All data of the previous Joins sequence</param>
        /// <param name="key">Selected Property of First Entity</param>
        /// <param name="otherkey">Selected Property of Second Entity</param>
        /// <returns>Updated Joins Data</returns>
        public static JoinsData<T, TOther> Or<T, TOther, TKey>(this JoinsData<T, TOther> currentData, Expression<Func<T, TKey>> key, Expression<Func<TOther, TKey>> otherkey)
             where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return currentData.OptionalJoint(key, otherkey);
        }

        /// <summary>
        /// Creates where statement on the first table of the join, to filter 
        /// the lines of that table that will be used in join
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <param name="currentData">All data of the previous Joins sequence</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Updated Joins Data</returns>
        public static JoinsData<T, TOther> WhereFirst<T, TOther>(this JoinsData<T, TOther> currentData, Expression<Func<T, bool>> predicate)
             where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return currentData.AddWhereStatementOne(predicate);
        }

        /// <summary>
        /// Creates where statement on the second table of the join, to filter 
        /// the lines of that table that will be used in join
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <param name="currentData">All data of the previous Joins sequence</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Updated Joins Data</returns>
        public static JoinsData<T, TOther> WhereSecond<T, TOther>(this JoinsData<T, TOther> currentData, Expression<Func<TOther, bool>> predicate)
             where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return currentData.AddWhereStatementTwo(predicate);
        }

        /// <summary>
        /// Gives Mapping Priority to a property of the second entity of the join.
        /// If other entities in the join sequence have also the same property, the 
        /// property of this entity will be mapped with priority in the final Dto result.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <typeparam name="TKey">Type of Property</typeparam>
        /// <param name="data">All data of the previous Joins sequence</param>
        /// <param name="key">Selected Property</param>
        /// <returns>Updated Joins Data</returns>
        public static JoinsData<T, TOther> GiveMapPriorityToSecond<T, TOther, TKey>(this JoinsData<T, TOther> data, Expression<Func<TOther, TKey>> key)
             where T : BlackHoleEntity where TOther : BlackHoleEntity where TKey : IComparable
        {
            if (data.Ignore)
            {
                return data;
            }
            data.AllProps.GivePriority(key);
            return data;
        }

        /// <summary>
        /// Gives Mapping Priority to a property of the first entity of the join.
        /// If other entities in the join sequence have also the same property, the 
        /// property of this entity will be mapped with priority in the final Dto result.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <typeparam name="TKey">Type of Property</typeparam>
        /// <param name="data">All data of the previous Joins sequence</param>
        /// <param name="key">Selected Property</param>
        /// <returns>Updated Joins Data</returns>
        public static JoinsData<T, TOther> GiveMapPriorityToFirst<T, TOther, TKey>(this JoinsData<T, TOther> data, Expression<Func<T, TKey>> key)
            where T : BlackHoleEntity where TOther : BlackHoleEntity where TKey : IComparable
        {
            if (data.Ignore)
            {
                return data;
            }
            data.AllProps.GivePriority(key);
            return data;
        }

        /// <summary>
        /// Executes the whole joins sequence and returns the result mapped 
        /// on a List of the specified Dto. Only the properties of the Entities 
        /// that match with the properties of the Dto will be mapped
        /// </summary>
        /// <typeparam name="Dto">BlackHoleDto</typeparam>
        /// <param name="data">All data of the previous Joins sequence</param>
        /// <returns>List of the Specified Dto</returns>
        public static List<Dto> ExecuteQuery<Dto>(this JoinsData data) where Dto : BlackHoleDto
        {
            List<PropertyOccupation> OccupiedProperties = data.AllProps.MapPropertiesToDto<Dto>();
            data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates);
            TableLetters tL = data.TablesToLetters.First(x => x.Table == data.BaseTable);
            string commandText = $"{OccupiedProperties.BuildCommand()} from {tL.Table?.Name} {tL.Letter} {data.Joins} {data.WherePredicates}";
            return _dataProvider.Query<Dto>(commandText, data.DynamicParams, data.DatabaseName.BuildConnectionString());
        }

        /// <summary>
        /// <b>Transaction.</b> Executes the whole joins sequence and returns the result mapped 
        /// on a List of the specified Dto. Only the properties of the Entities 
        /// that match with the properties of the Dto will be mapped
        /// </summary>
        /// <typeparam name="Dto">BlackHoleDto</typeparam>
        /// <param name="data">All data of the previous Joins sequence</param>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>List of the Specified Dto</returns>
        public static List<Dto> ExecuteQuery<Dto>(this JoinsData data, BHTransaction bhTransaction) where Dto : BlackHoleDto
        {
            List<PropertyOccupation> OccupiedProperties = data.AllProps.MapPropertiesToDto<Dto>();
            data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates);
            TableLetters tL = data.TablesToLetters.First(x => x.Table == data.BaseTable);
            string commandText = $"{OccupiedProperties.BuildCommand()} from {tL.Table?.Name} {tL.Letter} {data.Joins} {data.WherePredicates}";
            return _dataProvider.Query<Dto>(commandText, data.DynamicParams, bhTransaction.transaction);
        }

        internal static List<Dto> ExecuteView<Dto>(this StoredView storedV)
        {
            return _dataProvider.Query<Dto>(storedV.CommandText, storedV.DynamicParams, storedV.DatabaseName.BuildConnectionString());
        }

        internal static List<Dto> ExecuteView<Dto>(this StoredView storedV, BHTransaction bhTransaction)
        {
            return _dataProvider.Query<Dto>(storedV.CommandText, storedV.DynamicParams, bhTransaction.transaction);
        }

        /// <summary>
        /// Gathers and calculates the Data of the join sequence and then 
        /// stores a simplified form of them into a static list using the Dto as Identifier.
        /// Then using the BHDataProvider and the Dto you can execute this join at 
        /// any point with higher efficiency.
        /// </summary>
        /// <typeparam name="Dto">BlackHoleDto</typeparam>
        /// <param name="data">All data of the previous Joins sequence</param>
        public static void StoreAsView<Dto>(this JoinsData data) where Dto : BlackHoleDto
        {
            List<PropertyOccupation> OccupiedProperties = data.AllProps.MapPropertiesToDto<Dto>();
            data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates);
            TableLetters tL = data.TablesToLetters.First(x => x.Table == data.BaseTable);
            string commandText = $"{OccupiedProperties.BuildCommand()} from {tL.Table?.Name} {tL.Letter} {data.Joins} {data.WherePredicates}";
            BHDataProvider.AddStoredView<Dto>(new StoredView { DtoType = typeof(Dto), CommandText = commandText, DynamicParams = data.DynamicParams }, data.DatabaseName);
        }

        private static string RejectInactiveEntities(this List<TableLetters> involvedTables, string whereCommand)
        {
            string command = string.Empty;
            string inactiveColumn = "Inactive";
            string anD = "and";

            if (whereCommand == string.Empty)
            {
                anD = "where";
            }

            foreach (TableLetters table in involvedTables)
            {
                if (command != string.Empty)
                {
                    anD = "and";
                }

                command += $" {anD} {table.Letter}.{inactiveColumn} = 0 ";
            }

            return whereCommand + command;
        }

        private static string BuildCommand(this List<PropertyOccupation> usedProperties)
        {
            string sqlCommand = "select ";

            foreach (PropertyOccupation prop in usedProperties.Where(x => x.Occupied))
            {
                sqlCommand += $" {prop.TableLetter}.{prop.PropName},";
            }

            return sqlCommand.Substring(0, sqlCommand.Length - 1);
        }

        private static bool UpdateMany<T>(List<T> entries, string updateCommand, string connectionString)
        {
            BlackHoleTransaction bhTransaction = new(connectionString);

            foreach (T entry in entries)
            {
                _dataProvider.ExecuteEntry(updateCommand, entry, bhTransaction);
            }

            bool result = bhTransaction.Commit();
            bhTransaction.Dispose();

            return result;
        }

        internal static string GetLimiter(this int rowsCount)
        {
            return $" limit {rowsCount} ";
        }

        private static bool UpdateMany<T>(List<T> entries, string updateCommand, BlackHoleTransaction bhTransaction)
        {
            bool result = true;

            foreach (T entry in entries)
            {
                if (!_dataProvider.ExecuteEntry(updateCommand, entry, bhTransaction))
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
