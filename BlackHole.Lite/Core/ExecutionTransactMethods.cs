using BlackHole.CoreSupport;
using BlackHole.DataProviders;
using BlackHole.Entities;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    internal static class ExecutionTransactMethods
    {
        internal static SqliteDataProvider _dataProvider = BHDataProviderSelector.GetDataProvider();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <param name="context"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public static BHTransactEntityContext<T, G> Include<T, G>(this BHTransactEntityContext<T> context,
            Expression<Func<T, BHIncludeItem<G>>> include)
            where T : BHEntity where G : BHEntity
        {
            var propName = include.GetPropertyName();

            var keyMain = $"{typeof(T).Name}_{propName}";

            string fkPropName;
            bool reversed;

            if (BHDataProvider.FkMap.ContainsKey(keyMain))
            {
                fkPropName = BHDataProvider.FkMap[keyMain];
                reversed = false;
            }
            else
            {
                fkPropName = BHDataProvider.FkReverseMap[keyMain];
                reversed = true;
            }

            context.Includes.Add(new IncludePart
            {
                TableType = typeof(G),
                ForeignKeyProperty = fkPropName,
                IsReversed = reversed,
                IsList = false,
                NavigationPropertyName = propName,
                ParentIndex = -1
            });

            return context.MapEntityTransact<T, G>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <param name="context"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public static BHTransactEntityContext<T, G> Include<T, G>(this BHTransactEntityContext<T> context,
            Expression<Func<T, BHIncludeList<G>>> include)
            where T : BHEntity where G : BHEntity
        {
            var propName = include.GetPropertyName();

            string fkPropName = BHDataProvider.FkReverseMap[$"{typeof(T).Name}_{propName}"];

            context.Includes.Add(new IncludePart
            {
                IsReversed = true,
                TableType = typeof(G),
                ForeignKeyProperty = fkPropName,
                IsList = true,
                NavigationPropertyName = propName,
                ParentIndex = -1
            });

            return context.MapEntityTransact<T, G>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <param name="context"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public static BHTransactEntityContext<T, G> Include<T, H, G>(this BHTransactEntityContext<T, H> context,
          Expression<Func<T, BHIncludeItem<G>>> include)
          where T : BHEntity where G : BHEntity where H : BHEntity
        {
            var propName = include.GetPropertyName();

            var keyMain = $"{typeof(T).Name}_{propName}";

            string fkPropName;
            bool reversed;

            if (BHDataProvider.FkMap.ContainsKey(keyMain))
            {
                fkPropName = BHDataProvider.FkMap[keyMain];
                reversed = false;
            }
            else
            {
                fkPropName = BHDataProvider.FkReverseMap[keyMain];
                reversed = true;
            }

            context.Includes.Add(new IncludePart
            {
                TableType = typeof(G),
                ForeignKeyProperty = fkPropName,
                IsReversed = reversed,
                IsList = false,
                NavigationPropertyName = propName,
                ParentIndex = -1
            });

            return context.MapEntityTransact<T, H, G>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <param name="context"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public static BHTransactEntityContext<T, G> Include<T, H, G>(this BHTransactEntityContext<T, H> context,
            Expression<Func<T, BHIncludeList<G>>> include)
            where T : BHEntity where G : BHEntity where H : BHEntity
        {
            var propName = include.GetPropertyName();

            string fkPropName = BHDataProvider.FkReverseMap[$"{typeof(T).Name}_{propName}"];

            context.Includes.Add(new IncludePart
            {
                IsReversed = true,
                TableType = typeof(G),
                ForeignKeyProperty = fkPropName,
                IsList = true,
                NavigationPropertyName = propName,
                ParentIndex = -1
            });

            return context.MapEntityTransact<T, H, G>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <param name="context"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public static BHTransactEntityContext<T, H> ThenInclude<T, G, H>(this BHTransactEntityContext<T, G> context,
            Expression<Func<G, BHIncludeList<H>>> include)
            where T : BHEntity where G : BHEntity where H : BHEntity
        {
            var propName = include.GetPropertyName();

            string fkPropName = BHDataProvider.FkReverseMap[$"{typeof(G).Name}_{propName}"];

            context.Includes.Add(new IncludePart
            {
                IsReversed = true,
                TableType = typeof(H),
                ForeignKeyProperty = fkPropName,
                IsList = true,
                NavigationPropertyName = propName,
                ParentTableType = typeof(G),
                ParentIndex = context.Includes.Count - 1
            });

            return context.MapEntityTransact<T, G, H>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="H"></typeparam>
        /// <param name="context"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public static BHTransactEntityContext<T, H> ThenInclude<T, G, H>(this BHTransactEntityContext<T, G> context,
            Expression<Func<G, BHIncludeItem<H>>> include)
            where T : BHEntity where G : BHEntity where H : BHEntity
        {
            var propName = include.GetPropertyName();

            var keyMain = $"{typeof(G).Name}_{propName}";

            string fkPropName;
            bool reversed;

            if (BHDataProvider.FkMap.ContainsKey(keyMain))
            {
                fkPropName = BHDataProvider.FkMap[keyMain];
                reversed = false;
            }
            else
            {
                fkPropName = BHDataProvider.FkReverseMap[keyMain];
                reversed = true;
            }

            context.Includes.Add(new IncludePart
            {
                TableType = typeof(H),
                ForeignKeyProperty = fkPropName,
                IsReversed = reversed,
                IsList = false,
                NavigationPropertyName = propName,
                ParentTableType = typeof(G),
                ParentIndex = context.Includes.Count - 1
            });

            return context.MapEntityTransact<T, G, H>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <param name="context"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static T? GetEntryById<T, G>(this BHTransactEntityContext<T, G> context, int Id)
            where T : BHEntity where G : BHEntity
        {
            var inc = context.Includes.BuildIncludeSql<T>(context.Columns);

            BHParameters Params = new();
            Params.Add("Id", Id);

            if (context.WithActivator)
            {
                return _dataProvider.QueryWithIncludes<T>($"select {inc.Query} from {context.ThisTable} r {inc.Joins} where {inc.RootLetter}.Id = @Id and {inc.RootLetter}.Inactive = 0",
                    Params.Parameters, context._transatcion, context.Includes).FirstOrDefault();
            }
            return _dataProvider.QueryWithIncludes<T>($"select {inc.Query} from {context.ThisTable} r {inc.Joins} where {inc.RootLetter}.Id = @Id",
                Params.Parameters, context._transatcion, context.Includes).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<T> GetAllEntries<T, G>(this BHTransactEntityContext<T, G> context) where T : BHEntity where G : BHEntity
        {
            var inc = context.Includes.BuildIncludeSql<T>(context.Columns);

            if (context.WithActivator)
            {
                return _dataProvider.QueryWithIncludes<T>($"select {inc.Query} from {context.ThisTable} r {inc.Joins} where {inc.RootLetter}.Inactive = 0",
                    null, context._transatcion, context.Includes);
            }
            return _dataProvider.QueryWithIncludes<T>($"select {inc.Query} from {context.ThisTable} r {inc.Joins}",
                null, context._transatcion, context.Includes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <param name="context"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T? GetEntryWhere<T, G>(this BHTransactEntityContext<T, G> context,
            Expression<Func<T, bool>> predicate)
            where T : BHEntity where G : BHEntity
        {
            var inc = context.Includes.BuildIncludeSql<T>(context.Columns);

            ColumnsAndParameters sql = predicate.SplitMembers(inc.RootLetter, null, 0);

            string limit = 1.GetLimiter();

            if (context.WithActivator)
            {
                return _dataProvider.QueryWithIncludes<T>($"select {inc.Query} from {context.ThisTable} r {inc.Joins} where {inc.RootLetter}.Inactive = 0 and {sql.Columns} {limit}",
                    sql.Parameters, context._transatcion, context.Includes).FirstOrDefault();
            }
            return _dataProvider.QueryWithIncludes<T>($"select {inc.Query} from {context.ThisTable} r {inc.Joins} where {sql.Columns} {limit}",
                sql.Parameters, context._transatcion, context.Includes).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <param name="context"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<T> GetEntriesWhere<T, G>(this BHTransactEntityContext<T, G> context,
            Expression<Func<T, bool>> predicate)
            where T : BHEntity where G : BHEntity
        {
            var inc = context.Includes.BuildIncludeSql<T>(context.Columns);

            ColumnsAndParameters sql = predicate.SplitMembers(inc.RootLetter, null, 0);

            if (context.WithActivator)
            {
                return _dataProvider.QueryWithIncludes<T>($"select {inc.Query} from {context.ThisTable} r {inc.Joins} where {inc.RootLetter}.Inactive = 0 and {sql.Columns}",
                    sql.Parameters, context._transatcion, context.Includes);
            }

            return _dataProvider.QueryWithIncludes<T>($"select {inc.Query} from {context.ThisTable} r {inc.Joins} where {sql.Columns}",
                sql.Parameters, context._transatcion, context.Includes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool Any<T>(this BHTransactEntityContext<T> context) where T : BHEntity
        {
            string limit = 1.GetLimiter();
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select Id ,{context.PropertyNames} from {context.ThisTable} where Inactive = 0 {limit}", null, context._transatcion) != null;
            }
            return _dataProvider.QueryFirst<T>($"select Id ,{context.PropertyNames} from {context.ThisTable} {limit}", null, context._transatcion) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Any<T>(this BHTransactEntityContext<T> context, Expression<Func<T, bool>> predicate) where T : BHEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(string.Empty, null, 0);
            string limit = 1.GetLimiter();
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select Id ,{context.PropertyNames} from {context.ThisTable} where Inactive = 0 and {sql.Columns} {limit}",
                    sql.Parameters, context._transatcion) != null;
            }
            return _dataProvider.QueryFirst<T>($"select Id ,{context.PropertyNames} from {context.ThisTable} where {sql.Columns} {limit}",
                sql.Parameters, context._transatcion) != null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int Count<T>(this BHTransactEntityContext<T> context) where T : BHEntity
        {
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<int>($"select count(Id) from {context.ThisTable} where Inactive = 0", null, context._transatcion);
            }
            return _dataProvider.QueryFirst<int>($"select count(Id) from {context.ThisTable}", null, context._transatcion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int Count<T>(this BHTransactEntityContext<T> context, Expression<Func<T, bool>> predicate) where T : BHEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(string.Empty, null, 0);

            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<int>($"select count(Id) from {context.ThisTable} where Inactive = 0 and {sql.Columns}",
                    sql.Parameters, context._transatcion);
            }
            return _dataProvider.QueryFirst<int>($"select count(Id) from {context.ThisTable} where {sql.Columns}",
                sql.Parameters, context._transatcion);
        }

        /// <summary>
        /// Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <returns>List of All Table's Entities</returns>
        public static List<T> GetAllEntries<T>(this BHTransactEntityContext<T> context) where T : BHEntity
        {
            if (context.WithActivator)
            {
                return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Inactive = 0",
                    null, context._transatcion);
            }
            return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable}",
                null, context._transatcion);
        }

        /// <summary>
        /// In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return a List of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <returns>List of Inactive Entities</returns>
        public static List<T> GetAllInactiveEntries<T>(this BHTransactEntityContext<T> context) where T : BHEntity
        {
            if (context.WithActivator)
            {
                return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Inactive = 1",
                    null, context._transatcion);
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
        public static T? GetEntryById<T>(this BHTransactEntityContext<T> context, int Id) where T : BHEntity
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Id = @Id and Inactive = 0",
                    Params.Parameters, context._transatcion);
            }
            return _dataProvider.QueryFirst<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Id = @Id",
                Params.Parameters, context._transatcion);
        }

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Entity</returns>
        public static T? GetEntryWhere<T>(this BHTransactEntityContext<T> context, Expression<Func<T, bool>> predicate) where T : BHEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers(string.Empty, null, 0);
            string limit = 1.GetLimiter();
            if (context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Inactive = 0 and {sql.Columns} {limit}",
                    sql.Parameters, context._transatcion);
            }
            return _dataProvider.QueryFirst<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where {sql.Columns} {limit}",
                sql.Parameters, context._transatcion);
        }

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of Entities</returns>
        public static List<T> GetEntriesWhere<T>(this BHTransactEntityContext<T> context, Expression<Func<T, bool>> predicate) where T : BHEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers(string.Empty, null, 0);
            if (context.WithActivator)
            {
                return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where Inactive = 0 and {sql.Columns}",
                    sql.Parameters, context._transatcion);
            }
            return _dataProvider.Query<T>($"select Id,{context.PropertyNames} from {context.ThisTable} where {sql.Columns}",
                sql.Parameters, context._transatcion);
        }

        /// <summary>
        /// Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="entry">Entity to Insert</param>
        /// <returns>Inserted Id</returns>
        public static int InsertEntry<T>(this BHTransactEntityContext<T> context, T entry) where T : BHEntity
        {
            entry.Id = _dataProvider.InsertScalar($"insert into {context.ThisTable} ({context.PropertyNames},Inactive", $"values ({context.PropertyParams}, 0",
                entry, context._transatcion);

            return entry.Id;
        }

        /// <summary>
        /// <b>Transaction.</b> Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="entries">Entities to Insert</param>
        /// <returns>List of Inserted Ids</returns>
        public static List<int> InsertEntries<T>(this BHTransactEntityContext<T> context, List<T> entries) where T : BHEntity
        {
            return _dataProvider.MultiInsertScalar<T>($"insert into {context.ThisTable} ({context.PropertyNames},Inactive", $"values ({context.PropertyParams}, 0",
                entries, context._transatcion);
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
        public static bool UpdateEntryById<T>(this BHTransactEntityContext<T> context, T entry) where T : BHEntity
        {
            return _dataProvider.ExecuteEntry($"update {context.ThisTable} set {context.UpdateParams} where Id = @Id", entry, context._transatcion);
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
        public static bool UpdateEntriesById<T>(this BHTransactEntityContext<T> context, List<T> entries) where T : BHEntity
        {
            return entries.UpdateMany($"update {context.ThisTable} set {context.UpdateParams} where Id = @Id", context._transatcion);
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
        public static bool UpdateEntriesWhere<T>(this BHTransactEntityContext<T> context, Expression<Func<T, bool>> predicate, T entry) where T : BHEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers(string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            if (context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {context.ThisTable} set {context.UpdateParams} where Inactive = 0 and {sql.Columns}",
                    sql.Parameters, context._transatcion);
            }
            return _dataProvider.JustExecute($"update {context.ThisTable} set {context.UpdateParams} where {sql.Columns}",
                sql.Parameters, context._transatcion);
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
        public static bool DeleteAllEntries<T>(this BHTransactEntityContext<T> context) where T : BHEntity
        {
            if (context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {context.ThisTable} set Inactive = 1 where Inactive = 0", null, context._transatcion);
            }
            return _dataProvider.JustExecute($"delete from {context.ThisTable}", null, context._transatcion);
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
        public static bool DeleteEntryById<T>(this BHTransactEntityContext<T> context, int Id) where T : BHEntity
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (context.WithActivator)
            {
                return _dataProvider.JustExecute($"Update {context.ThisTable} set Inactive = 1 where Id = @Id", Params.Parameters, context._transatcion);
            }
            return _dataProvider.JustExecute($"delete from {context.ThisTable} where Id = @Id", Params.Parameters, context._transatcion);
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
        public static bool DeleteInactiveEntryById<T>(this BHTransactEntityContext<T> context, int Id) where T : BHEntity
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return _dataProvider.JustExecute($"delete from {context.ThisTable} where Id = @Id and Inactive = 1", Params.Parameters, context._transatcion);
        }

        /// <summary>
        /// Activates again an Inactive Entry
        /// in the database.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="Id">Id of the Entry</param>
        /// <returns>Success</returns>
        public static bool ReactivateEntryById<T>(this BHTransactEntityContext<T> context, int Id) where T : BHEntity
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return _dataProvider.JustExecute($"update {context.ThisTable} set Inactive = 0 where Id = @Id and Inactive = 1",
                Params.Parameters, context._transatcion);
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
        public static bool DeleteEntriesWhere<T>(this BHTransactEntityContext<T> context, Expression<Func<T, bool>> predicate) where T : BHEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers(string.Empty, null, 0);
            if (context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {context.ThisTable} set Inactive = 1 where {sql.Columns}",
                    sql.Parameters, context._transatcion);
            }
            return _dataProvider.JustExecute($"delete from {context.ThisTable} where {sql.Columns}",
                sql.Parameters, context._transatcion);
        }

        /// <summary>
        /// Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns their Ids
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <param name="context">Entity's Context</param>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of Ids</returns>
        public static List<int> GetIdsWhere<T>(this BHTransactEntityContext<T> context, Expression<Func<T, bool>> predicate) where T : BHEntity
        {
            ColumnsAndParameters sql = predicate.SplitMembers(string.Empty, null, 0);
            if (context.WithActivator)
            {
                return _dataProvider.Query<int>($"select Id from {context.ThisTable} where Inactive = 0 and {sql.Columns}",
                    sql.Parameters, context._transatcion);
            }
            return _dataProvider.Query<int>($"select Id from {context.ThisTable} where {sql.Columns}",
                sql.Parameters, context._transatcion);
        }
    }
}
