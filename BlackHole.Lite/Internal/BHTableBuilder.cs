using BlackHole.CoreSupport;
using BlackHole.DataProviders;
using BlackHole.Entities;
using BlackHole.Statics;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace BlackHole.Internal
{
    internal class BHTableBuilder
    {
        private readonly BHDatabaseSelector _multiDatabaseSelector = new();
        private readonly SqliteExecutionProvider connection;
        private List<TableParsingInfo> DbConstraints { get; set; } = new();
        private readonly string[] SqlDatatypes;
        internal BHDatabaseInfoReader dbInfoReader { get; set; }
        internal bool IsTrashOracleProduct { get; set; }
        private List<string> CreateTablesTransaction { get; set; } = new();
        private List<string> CustomTransaction { get; set; } = new();
        private List<string> AfterMath { get; set; } = new();
        private EntityContextBuilder entityRegistrator { get; } = new();
        private string CurrentConnectionString { get; set; } 

        public bool IsDeveloperMode { get; set; }

        internal BHTableBuilder()
        {
            CurrentConnectionString = DatabaseStatics.DefaultConnectionString;
            connection = _multiDatabaseSelector.GetExecutionProvider(CurrentConnectionString);
            dbInfoReader = new BHDatabaseInfoReader(connection);
            SqlDatatypes = _multiDatabaseSelector.SqlDatatypesTranslation();
        }

        internal void SwitchConnectionString(string connectionString)
        {
            CurrentConnectionString = connectionString;
            connection.SwitchConnectionString(CurrentConnectionString);
            dbInfoReader.SwitchConnection(CurrentConnectionString);
        }

        internal void BuildMultipleTables(List<Type> TableTypes)
        {
            DatabaseStatics.InitializeData = true;
            TableCompleteInfo[] Built = new TableCompleteInfo[TableTypes.Count];

            //DbConstraints = dbInfoReader.GetDatabaseParsingInfo();

            for (int i = 0; i < Built.Length; i++)
            {
                Built[i] = CreateTable(TableTypes[i]);
                entityRegistrator.InsertEntityContext(TableTypes[i]);
            }

            for (int j = 0; j < Built.Length; j++)
            {
                ForeignKeyLiteAssignment(Built[j]);
            }

            if (!ExecuteTableCreation())
            {
                Thread.Sleep(2000);
                throw ProtectDbAndThrow("Something went wrong with the Update of the Database.The Database is not changed. Please check the BlackHole logs to detect and fix the problem.");
            }
            ClearCommands();
        }

        internal void CleanupConstraints()
        {
            DbConstraints.Clear();
        }

        TableCompleteInfo CreateTable(Type TableType)
        {
            TableInfoExtractor extractor = new TableInfoExtractor(TableType);
            TableCompleteInfo entityInfo = extractor.ExtractData();

            if (!CheckTable(TableType.Name))
            {
                PropertyInfo[] Properties = TableType.GetProperties();
                StringBuilder tableCreator = new();
                tableCreator.Append($"CREATE TABLE {TableType.Name} (");

                foreach (var column in entityInfo.Columns.OrderByDescending(c => c.IsPrimaryKey))
                {
                    if (column.IsPrimaryKey)
                    {
                        tableCreator.Append(_multiDatabaseSelector.GetPrimaryKeyCommand());
                    }
                    else
                    {
                        tableCreator.Append(GetDatatypeCommand(column, null, TableType.Name, true));
                    }
                }

                string creationCommand = tableCreator.ToString();
                creationCommand = $"{creationCommand.Substring(0, creationCommand.Length - 2)})";
                CreateTablesTransaction.Add(creationCommand);
            }

            DatabaseStatics.InitializeData = false;
            return entityInfo;
        }

        bool CheckTable(string TableName)
        {
            return connection.ExecuteScalar<string>($@"SELECT name FROM SQLite_master WHERE type='table' AND name='" + TableName + "'", null) == TableName;
        }

        //void UpdateLiteTableSchema(Type TableType)
        //{
        //    List<string> ColumnNames = new();
        //    List<string> NewColumnNames = new();

        //    foreach (PropertyInfo Property in TableType.GetProperties())
        //    {
        //        NewColumnNames.Add(Property.Name);
        //    }

        //    foreach (SQLiteTableInfo column in connection.Query<SQLiteTableInfo>($"PRAGMA table_info({TableType.Name}); ", null))
        //    {
        //        ColumnNames.Add(column.name);
        //    }

        //    List<string> CommonList = ColumnNames.Intersect(NewColumnNames).ToList();

        //    if (CommonList.Count != NewColumnNames.Count || CommonList.Count != ColumnNames.Count)
        //    {
        //        //ForeignKeyLiteAssignment(TableType, false);
        //    }
        //}

        void ForeignKeyLiteAssignment(TableCompleteInfo entityInfo)
        {
            Type TableType = entityInfo.TableType;

            TableInfoComparator comparator = new TableInfoComparator(TableType);

            TableCompleteInfo dbInfo = comparator.GatherExistingInfo(connection);

            var exactColumns = entityInfo.Columns
                .Where(e => dbInfo.Columns.Any(d =>
                    d.PropertyName == e.PropertyName &&
                    d.PropertyType == e.PropertyType &&
                    d.IsNullable == e.IsNullable))
                .Count();

            if (exactColumns == entityInfo.Columns.Count) return;

            string OldTablename = $"{TableType.Name}_Old";

            var commonColumns = entityInfo.Columns
                .Where(e => dbInfo.Columns.Any(d =>
                    d.PropertyName == e.PropertyName &&
                    d.PropertyBaseType == e.PropertyBaseType))
                .ToList();

            List<string> oldColumns = dbInfo.Columns.Select(x => x.PropertyName).ToList();
            List<string> commonColumnNames = commonColumns.Select(x => x.PropertyName).ToList();
            List<string> columnsToDrop = oldColumns.Except(commonColumnNames).ToList();

            StringBuilder alterTable = new();
            StringBuilder foreignKeys = new();
            StringBuilder closingCommand = new();

            alterTable.Append($"ALTER TABLE {TableType.Name} RENAME TO {OldTablename}; CREATE TABLE {TableType.Name} (");

            foreach (var column in entityInfo.Columns.OrderByDescending(c => c.IsPrimaryKey))
            {
                if (column.IsPrimaryKey)
                {
                    alterTable.Append(_multiDatabaseSelector.GetPrimaryKeyCommand());
                }
                else
                {
                    ColumnInfo? oldColumn = commonColumns.FirstOrDefault(x => x.PropertyName == column.PropertyName);
                    bool firstTime = oldColumn == null;
                    alterTable.Append(GetDatatypeCommand(column, oldColumn, TableType.Name, firstTime));
                }
            }

            string fkCommand = CreateForeignKeyConstraintLite(entityInfo.ForeignKeys, TableType.Name);
            string uniqueCommand = CreateUniqueConstraintLite(entityInfo.UniqueIndices, TableType.Name);
            string indicesCommand = CreateIndicesLite(entityInfo.Indices, TableType.Name);

            string tableCommand = $"{alterTable}{fkCommand}";
            string transferCommand = TransferOldTableData(commonColumnNames, TableType.Name, OldTablename);

            if (tableCommand.Length > 1)
            {
                tableCommand = $"{tableCommand.Substring(0, tableCommand.Length - 2)}); ";
            }

            alterTable.Clear();
            closingCommand.Append(tableCommand);
            closingCommand.Append($"{transferCommand} DROP TABLE {OldTablename};");
            closingCommand.Append($"ALTER TABLE {TableType.Name} RENAME TO {OldTablename}; ALTER TABLE {OldTablename} RENAME TO {TableType.Name};");
            closingCommand.Append($" DROP INDEX IF EXISTS {OldTablename};{uniqueCommand}{indicesCommand}");
            string final = closingCommand.ToString();
            CustomTransaction.Add(final);
            closingCommand.Clear();
        }

        //void ForeignKeyLiteAssignment(Type TableType, bool firstTime)
        //{
        //    string Tablename = TableType.Name;
        //    string OldTablename = $"{TableType.Name}_Old";

        //    Type FkType = typeof(ForeignKey);
        //    Type UQType = typeof(Unique);

        //    List<UniqueInfo> UniqueOptions = new();
        //    List<NonUniqueIndex> Indices = new();
        //    List<FKInfo> FkOptions = new();

        //    List<NonUniqueIndex> tempIndices = new();
        //    List<UniqueInfo> tempUniqueOptions = new();
        //    List<FKInfo> tempFkOptions = new();

        //    var relationType = GetRelationEntityType(TableType);

        //    if (relationType != null)
        //    {
        //        var builderType = typeof(RelationBuilder<>).MakeGenericType(relationType);
        //        var builder = Activator.CreateInstance(builderType);

        //        var instance = Activator.CreateInstance(TableType);
        //        var configureMethod = TableType.GetMethod("Congifure");
        //        configureMethod!.Invoke(instance, new[] { builder });

        //        var flags = BindingFlags.NonPublic | BindingFlags.Instance;
        //        var indices = (List<BHEntityIndex>)builderType.GetField("Indices", flags)!.GetValue(builder)!;
        //        var relations = (List<IBHRelation>)builderType.GetField("Relations", flags)!.GetValue(builder)!;

        //        ushort uniqueId = 256;

        //        foreach (var index in indices)
        //        {
        //            if (index.IsIndexUnique)
        //            {
        //                tempIndices.Add(new NonUniqueIndex(index.IndexColumns, uniqueId));
        //            }
        //            else
        //            {
        //                foreach (var prop in index.IndexColumns)
        //                {
        //                    tempUniqueOptions.Add(AddUniqueConstraint(prop, uniqueId));
        //                }
        //            }

        //            uniqueId++;
        //        }

        //        foreach (var relation in relations)
        //        {
        //            tempFkOptions.Add(new FKInfo()
        //            {
        //                ReferencedColumn = "Id",
        //                ReferencedTable = relation.IncludedType.Name,
        //                OnDelete = relation.OnDelete.DeleteAction,
        //                IsNullable = relation.OnDelete.IsNullable,
        //                PropertyName = relation.ForeignKeyPropertyName
        //            });
        //        }
        //    }

        //    List<string> ColumnNames = new();
        //    List<string> NewColumnNames = new()
        //    {
        //        "Inactive"
        //    };

        //    List<SQLiteForeignKeySchema> SchemaInfo = connection.Query<SQLiteForeignKeySchema>($"PRAGMA foreign_key_list({Tablename});", null);
        //    List<SQLiteTableInfo> ColumnsInfo = connection.Query<SQLiteTableInfo>($"PRAGMA table_info({Tablename});", null);

        //    foreach (SQLiteTableInfo column in ColumnsInfo)
        //    {
        //        ColumnNames.Add(column.name);
        //    }

        //    PropertyInfo[] Properties = TableType.GetProperties();
        //    foreach (PropertyInfo Property in Properties)
        //    {
        //        NewColumnNames.Add(Property.Name);
        //    }

        //    List<string> ColumnsToDrop = ColumnNames.Except(NewColumnNames).ToList();

        //    if (ColumnsToDrop.Any())
        //    {
        //        throw ProtectDbAndThrow($"Error at Table '{TableType.Name}' on Dropping Columns. You CAN ONLY Drop Columns of a Table in Developer Mode, or by using the CLI 'update' command with the '--force' argument => 'bhl update --force'");
        //    }

        //    List<string> ColumnsToAdd = NewColumnNames.Except(ColumnNames).ToList();
        //    bool missingInactiveColumn = ColumnsToAdd.Contains("Inactive");
        //    List<string> CommonColumns = ColumnNames.Intersect(NewColumnNames).ToList();

        //    StringBuilder alterTable = new();
        //    StringBuilder foreignKeys = new();
        //    StringBuilder closingCommand = new();

        //    alterTable.Append($"ALTER TABLE {Tablename} RENAME TO {OldTablename}; CREATE TABLE {Tablename} (");
        //    alterTable.Append(GetDatatypeCommand(typeof(int), Array.Empty<object>(), "Inactive", TableType.Name));
        //    alterTable.Append(" NULL, ");

        //    foreach (string AddColumn in CommonColumns.Where(x => x != "Inactive"))
        //    {
        //        PropertyInfo Property = Properties.First(x => x.Name == AddColumn);
        //        object[] attributes = Property.GetCustomAttributes(true);

        //        if (Property.Name != "Id")
        //        {
        //            alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name, TableType.Name));
        //            SQLiteTableInfo existingCol = ColumnsInfo.First(x => x.name == AddColumn);
        //            alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, Property.Name, TableType.Name, existingCol.notnull));
        //        }
        //        else
        //        {
        //            alterTable.Append(_multiDatabaseSelector.GetPrimaryKeyCommand());
        //        }

        //        if (attributes.Length > 0)
        //        {
        //            object? FK_attribute = attributes.FirstOrDefault(x => x.GetType() == FkType);
        //            bool nullability = true;
        //            if (FK_attribute != null)
        //            {
        //                var tName = FkType.GetProperty("TableName")?.GetValue(FK_attribute, null);
        //                var tColumn = FkType.GetProperty("Column")?.GetValue(FK_attribute, null);
        //                if (FkType.GetProperty("Nullability")?.GetValue(FK_attribute, null) is bool isNullable)
        //                {
        //                    nullability = isNullable;
        //                    FkOptions.Add(AddForeignKey(Property.Name, tName, tColumn, isNullable));
        //                }
        //            }
        //            else
        //            {
        //                object? NN_attribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));
        //                if (NN_attribute != null)
        //                {
        //                    nullability = false;
        //                }
        //            }

        //            object? UQ_attribute = attributes.FirstOrDefault(x => x.GetType() == UQType);

        //            if (UQ_attribute != null)
        //            {
        //                if (UQType.GetProperty("UniqueGroupId")?.GetValue(UQ_attribute, null) is int groupId)
        //                {
        //                    if (!nullability)
        //                    {
        //                        UniqueOptions.Add(AddUniqueConstraint(Property.Name, groupId));
        //                    }
        //                    else
        //                    {
        //                        throw ProtectDbAndThrow($"Property '{Property.Name}' of Entity '{TableType.Name}' is marked with UNIQUE CONSTRAINT and it Requires to be 'NOT NULL'.");
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    foreach (string AddColumn in ColumnsToAdd.Where(x => x != "Inactive"))
        //    {
        //        if (AddColumn != "Id")
        //        {
        //            PropertyInfo Property = Properties.First(x => x.Name == AddColumn);
        //            object[] attributes = Property.GetCustomAttributes(true);
        //            alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name, TableType.Name));
        //            alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, Property.Name, TableType.Name, false));

        //            if (attributes.Length > 0)
        //            {
        //                object? FK_attribute = attributes.FirstOrDefault(x => x.GetType() == FkType);
        //                bool nullability = true;
        //                if (FK_attribute != null)
        //                {
        //                    var tName = FkType.GetProperty("TableName")?.GetValue(FK_attribute, null);
        //                    var tColumn = FkType.GetProperty("Column")?.GetValue(FK_attribute, null);
        //                    if (FkType.GetProperty("Nullability")?.GetValue(FK_attribute, null) is bool isNullable)
        //                    {
        //                        nullability = isNullable;
        //                        FkOptions.Add(AddForeignKey(Property.Name, tName, tColumn, isNullable));
        //                    }
        //                }
        //                else
        //                {
        //                    object? NN_attribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));
        //                    if (NN_attribute != null)
        //                    {
        //                        nullability = false;
        //                    }
        //                }

        //                object? UQ_attribute = attributes.FirstOrDefault(x => x.GetType() == UQType);

        //                if (UQ_attribute != null)
        //                {
        //                    if (UQType.GetProperty("UniqueGroupId")?.GetValue(UQ_attribute, null) is int groupId)
        //                    {
        //                        if (!nullability)
        //                        {
        //                            UniqueOptions.Add(AddUniqueConstraint(Property.Name, groupId));
        //                        }
        //                        else
        //                        {
        //                            throw ProtectDbAndThrow($"Property '{Property.Name}' of Entity '{TableType.Name}' is marked with UNIQUE CONSTRAINT and it Requires to be 'NOT NULL'.");
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            alterTable.Append(_multiDatabaseSelector.GetPrimaryKeyCommand());
        //        }
        //    }

        //    string FkCommand = $"{alterTable}{CreateForeignKeyConstraintLite(FkOptions, TableType.Name)}{CreateUniqueConstraintLite(UniqueOptions, TableType.Name)}";

        //    if (FkCommand.Length > 1)
        //    {
        //        FkCommand = $"{FkCommand.Substring(0, FkCommand.Length - 2)}); ";
        //    }

        //    alterTable.Clear(); foreignKeys.Clear();
        //    closingCommand.Append(FkCommand);
        //    closingCommand.Append($"{TransferOldTableData(CommonColumns, Tablename, OldTablename)} DROP TABLE {OldTablename};");
        //    closingCommand.Append($"ALTER TABLE {Tablename} RENAME TO {OldTablename}; ALTER TABLE {OldTablename} RENAME TO {Tablename};");
        //    closingCommand.Append($" DROP INDEX IF EXISTS {OldTablename}");
        //    CustomTransaction.Add(closingCommand.ToString());
        //    closingCommand.Clear();

        //    if (missingInactiveColumn)
        //    {
        //        string updateInactiveCol = $"Update {Tablename} set Inactive = 0 where Inactive is null;";
        //        AfterMath.Add(updateInactiveCol);
        //    }
        //}

        Type? GetRelationEntityType(Type entityType)
        {
            var type = entityType.BaseType;
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BHEntity<>))
                    return type.GenericTypeArguments[0]; // the T in BHEntity<T>
                type = type.BaseType;
            }
            return null;
        }

        string GetSqlColumn(object[] attributes, Type PropertyType, string PropName, string TableName)
        {
            bool mandatoryNull = false;
            bool isNullable = true;
            string nullPhase = "NULL, ";

            if (PropertyType.Name.Contains("Nullable"))
            {
                if (PropertyType.GenericTypeArguments != null && PropertyType.GenericTypeArguments.Length > 0)
                {
                    mandatoryNull = true;
                }
            }

            object? fkAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(ForeignKey));

            if (fkAttribute != null)
            {
                if (typeof(ForeignKey).GetProperty("Nullability")?.GetValue(fkAttribute, null) is bool Nullability)
                {
                    isNullable = Nullability;
                }

                nullPhase = isNullable ? "NULL, " : "NOT NULL, ";

                if (mandatoryNull && !isNullable)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{PropName}' of Entity '{TableName}' CAN NOT become a NOT NULL column in the Database." +
                        $"Please change the Nullability on the '[ForeignKey]' Attribute or Remove the (?) from the Property's Type.");
                }

                return nullPhase;
            }

            object? nnAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));

            if (nnAttribute != null)
            {
                isNullable = false;
                nullPhase = "NOT NULL, ";

                if (mandatoryNull)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{PropName}' of Entity '{TableName}' CAN NOT become a 'NOT NULL' column in the Database." +
                        $"Please remove the (?) from the Property's Type or Remove the [NotNullable] Attribute.");
                }

                return nullPhase;
            }

            return "NULL, ";
        }

        private string SQLiteColumn(object[] attributes, bool firstTime, Type PropertyType, string PropName, string TableName, bool wasNotNull)
        {
            bool mandatoryNull = false;
            bool isNullable = true;
            string nullPhase = "NULL, ";

            if (PropertyType.Name.Contains("Nullable"))
            {
                if (PropertyType.GenericTypeArguments != null && PropertyType.GenericTypeArguments.Length > 0)
                {
                    mandatoryNull = true;
                }
            }

            object? fkAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(ForeignKey));

            if (fkAttribute != null)
            {
                if (typeof(ForeignKey).GetProperty("Nullability")?.GetValue(fkAttribute, null) is bool Nullability)
                {
                    isNullable = Nullability;
                }

                nullPhase = isNullable ? "NULL, " : "NOT NULL, ";

                if (mandatoryNull && !isNullable)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{PropName}' of Entity '{TableName}' CAN NOT become a NOT NULL column in the Database." +
                        $"Please change the Nullability on the '[ForeignKey]' Attribute or Remove the (?) from the Property's Type.");
                }

                if (!wasNotNull && !firstTime && !isNullable)
                {
                    throw ProtectDbAndThrow("CAN NOT Add a 'NOT NULLABLE' Foreign Key on an Existing Table. Please Change the Nullability on the " +
                        $"'[ForeignKey]' Attribute on the Property '{PropName}' of the Entity '{TableName}'.");
                }

                return nullPhase;
            }

            object? nnAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));

            if (nnAttribute != null)
            {
                isNullable = false;
                nullPhase = "NOT NULL, ";

                if (mandatoryNull)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{PropName}' of Entity '{TableName}' CAN NOT become a 'NOT NULL' column in the Database." +
                        $"Please remove the (?) from the Property's Type or Remove the [NotNullable] Attribute.");
                }

                if (!wasNotNull && !firstTime)
                {
                    string defaultValCommand = GetDefaultValue(PropertyType, PropName, TableName);
                    return CheckLitePreviousColumnState(defaultValCommand, nullPhase, TableName, PropName);
                }

                return nullPhase;
            }

            return "NULL, ";
        }

        FKInfo AddForeignKey(string propName, object? tName, object? tColumn, bool nullability)
        {
            return new FKInfo
            {
                PropertyName = propName,
                ReferencedTable = $"{tName}",
                ReferencedColumn = $"{tColumn}",
                IsNullable = nullability,
                OnDelete = nullability ? OnDeleteBehavior.SetNull : OnDeleteBehavior.Cascade
            };
        }

        UniqueInfo AddUniqueConstraint(string propName, int groupId)
        {
            return new UniqueInfo
            {
                PropertyName = propName,
                GroupId = groupId
            };
        }

        string CreateForeignKeyConstraintLite(List<FKInfo> tableFKs, string TableName)
        {
            string result = string.Empty;
            foreach (string referencedTable in tableFKs.Select(x => x.ReferencedTable).Distinct())
            {
                result += $"{CreateFkConstraint(tableFKs.Where(x => x.ReferencedTable == referencedTable).ToList(), TableName, referencedTable)}, ";
            }
            return result;
        }

        string CreateUniqueConstraintLite(List<UniqueInfo> tableUQs, string TableName)
        {
            string result = string.Empty;
            foreach (int columnsGroup in tableUQs.Select(x => x.GroupId).Distinct())
            {
                result += CreateUQConstraint(tableUQs.Where(x => x.GroupId == columnsGroup).ToList(), TableName);
            }

            if (result.Length > 1)
            {
                return result.Substring(0, result.Length - 1);
            }

            return string.Empty;
        }

        string CreateIndicesLite(List<IndexInfo> tableUQs, string TableName)
        {
            string result = string.Empty;

            foreach (var idx in tableUQs)
            {
                result += CreateIndexConstraint(idx, TableName);
            }

            if (result.Length > 1)
            {
                return result.Substring(0, result.Length - 1);
            }

            return string.Empty;
        }

        string CreateUQConstraint(List<UniqueInfo> groupUQ, string TableName)
        {
            string constraintBegin = "CREATE UNIQUE INDEX";

            int groupId = 0;
            string uniqueColumns = string.Empty;

            foreach (UniqueInfo Uq in groupUQ)
            {
                uniqueColumns += $",{Uq.PropertyName}";
                groupId = Uq.GroupId;
            }

            uniqueColumns = uniqueColumns.Remove(0, 1);
            string key = $"{TableName}_{groupId}";
            string hash = HashKey(key).Substring(2, 9);
            string uniqueConstraint = $"{constraintBegin} uc_{groupId}_{hash} ON {TableName} ({uniqueColumns}); ";

            return uniqueConstraint;
        }

        string CreateIndexConstraint(IndexInfo groupUQ, string TableName)
        {
            string constraintBegin = "CREATE INDEX";

            int groupId = groupUQ.GroupId;
            string uniqueColumns = string.Empty;

            foreach (string Uq in groupUQ.PropertyNames)
            {
                uniqueColumns += $",{Uq}";
            }

            uniqueColumns = uniqueColumns.Remove(0, 1);
            string key = $"{TableName}_{groupId}";
            string hash = HashKey(key).Substring(1, 7);
            string uniqueConstraint = $"{constraintBegin} ix_{groupId}_{hash} ON {TableName} ({uniqueColumns}); ";

            return uniqueConstraint;
        }

        string CreateFkConstraint(List<FKInfo> commonFKs, string TableName, string ReferencedTable)
        {
            string constraintBegin = "CONSTRAINT";

            string fromColumn = string.Empty;
            string toColumn = string.Empty;
            bool isNullable = true;

            foreach (FKInfo fk in commonFKs)
            {
                if (!fk.IsNullable)
                {
                    isNullable = false;
                }

                fromColumn += $",{fk.PropertyName}";
                toColumn += $",{fk.ReferencedColumn}";
            }

            fromColumn = fromColumn.Remove(0, 1);
            toColumn = toColumn.Remove(0, 1);
            string key = $"{TableName}_{fromColumn}_{ReferencedTable}";
            string hash = HashKey(key);
            string onDeleteRule = isNullable ? "on delete set null" : "on delete cascade";
            string constraintCommand = $"{constraintBegin} fk_{hash} FOREIGN KEY ({fromColumn}) REFERENCES {ReferencedTable}({toColumn}) {onDeleteRule}";
            return constraintCommand;
        }

        private string HashKey(string key)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));

                return string.Join("",hash.Select(b => $"{b:X2}"));
            }
        }

        string CheckLitePreviousColumnState(string defaultVal, string nullPhase, string TableName, string ColumnName)
        {
            TableParsingInfo? oldColumn = DbConstraints.FirstOrDefault(x => x.TableName.ToLower() == TableName.ToLower() && x.ColumnName.ToLower() == ColumnName.ToLower());
            if (oldColumn != null && !oldColumn.Nullable)
            {
                return nullPhase;
            }
            return $"default {defaultVal} {nullPhase}";
        }

        string TransferOldTableData(List<string> CommonList, string newTablename, string oldTablename)
        {
            string result = "";
            if (CommonList.Any())
            {
                result = $"INSERT INTO {newTablename} (";
                string selection = ") SELECT ";
                string selectionClosing = "";

                foreach (string column in CommonList)
                {
                    result += $"{column},";
                    selectionClosing += $"{column},";
                }
                result = $"{result.Substring(0, result.Length - 1)}{selection}{selectionClosing.Substring(0, selectionClosing.Length - 1)} FROM {oldTablename};";
            }

            return result;
        }

        Exception ProtectDbAndThrow(string errorMessage)
        {
            return new Exception(errorMessage);
        }

        //string GetDatatypeCommand(Type PropertyType, int size, bool nullable, string Propertyname, string TableName, bool firstTime, bool wasNull)
        //{
        //    string propTypeName = PropertyType.Name;
        //    string dataCommand;

        //    string nulText = nullable ? "NULL " : "NOT NULL ";

        //    switch (propTypeName)
        //    {
        //        case "String":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[0]}({size}) {nulText}";
        //            break;
        //        case "Char":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[1]} {nulText}";
        //            break;
        //        case "Int16":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[2]} {nulText}";
        //            break;
        //        case "Int32":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[3]} {nulText}";
        //            break;
        //        case "Int64":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[4]} {nulText}";
        //            break;
        //        case "Decimal":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[5]} {nulText}";
        //            break;
        //        case "Single":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[6]} {nulText}";
        //            break;
        //        case "Double":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[7]} {nulText}";
        //            break;
        //        case "Guid":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[8]} {nulText}";
        //            break;
        //        case "Boolean":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[9]} {nulText}";
        //            break;
        //        case "DateTime":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[10]} {nulText}";
        //            break;
        //        case "DateTimeOffset":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[12]} {nulText}"; // e.g. "DATETIMEOFFSET" for SQL Server, "TEXT" for SQLite
        //            break;
        //        case "Byte[]":
        //            dataCommand = $"{Propertyname} {SqlDatatypes[11]} {nulText}";
        //            break;
        //        default:
        //            throw ProtectDbAndThrow($"Unsupported property type '{PropertyType.FullName}' at Property '{Propertyname}' of Entity '{TableName}'");
        //    }
        //    return dataCommand;
        //}

        string GetDatatypeCommand(ColumnInfo entityColumn, ColumnInfo? dbColumn, string tableName, bool firstTime)
        {
            bool wasNotNull = dbColumn?.IsNullable ?? false;

            string propTypeName = entityColumn.PropertyType.Name;
            string dataCommand;

            string nulText = entityColumn.IsNullable ? "NULL, " : "NOT NULL, ";

            if (!wasNotNull && !firstTime)
            {
                string defaultVal = GetDefaultValue(entityColumn.PropertyType, entityColumn.PropertyName, tableName);
                nulText = $"default {defaultVal} {nulText}";
            }

            switch (propTypeName)
            {
                case "String":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[0]}({entityColumn.Size}) {nulText}";
                    break;
                case "Char":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[1]} {nulText}";
                    break;
                case "Int16":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[2]} {nulText}";
                    break;
                case "Int32":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[3]} {nulText}";
                    break;
                case "Int64":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[4]} {nulText}";
                    break;
                case "Decimal":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[5]} {nulText}";
                    break;
                case "Single":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[6]} {nulText}";
                    break;
                case "Double":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[7]} {nulText}";
                    break;
                case "Guid":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[8]} {nulText}";
                    break;
                case "Boolean":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[9]} {nulText}";
                    break;
                case "DateTime":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[10]} {nulText}";
                    break;
                case "DateTimeOffset":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[12]} {nulText}"; // e.g. "DATETIMEOFFSET" for SQL Server, "TEXT" for SQLite
                    break;
                case "Byte[]":
                    dataCommand = $"{entityColumn.PropertyName} {SqlDatatypes[11]} {nulText}";
                    break;
                default:
                    throw ProtectDbAndThrow($"Unsupported property type '{entityColumn.PropertyType.FullName}' at Property '{entityColumn.PropertyName}' of Entity '{tableName}'");
            }
            return dataCommand;
        }

        string GetDefaultValue(Type PropertyType, string PropName, string TableName)
        {
            return PropertyType.Name switch
            {
                "String" => "'-'",
                "Char" => "'-'",
                "Int16" => "0",
                "Int32" => "0",
                "Int64" => "0",
                "Decimal" => "0",
                "Single" => "0",
                "Double" => "0",
                "Guid" => $"'{Guid.Empty}'",
                "Boolean" => "0",
                "DateTime" => $"'{new DateTime(1970, 1, 1).ToString(DatabaseStatics.DbDateFormat)}'",
                "DateTimeOffset" => $"'{new DateTimeOffset(1970, 1, 1, 0, 0, 0, new(0,0,0)).ToString(DatabaseStatics.DbDateFormat)}'",
                "Byte[]" => $"'{new byte[0]}'",
                _ => throw ProtectDbAndThrow($"Unsupported property type '{PropertyType.FullName}' at Property '{PropName}' of Entity '{TableName}'"),
            };
        }

        internal void ClearCommands()
        {
            CreateTablesTransaction.Clear();
            CustomTransaction.Clear();
            AfterMath.Clear();
        }

        internal bool ExecuteTableCreation()
        {
            List<string> AllCommands = new();
            AllCommands.AddRange(CreateTablesTransaction);
            AllCommands.AddRange(CustomTransaction);
            AllCommands.AddRange(AfterMath);

            if (AllCommands.Any())
            {
                BlackHoleTransaction transaction = new(CurrentConnectionString);
                connection.JustExecute("PRAGMA foreign_keys = off", null, transaction);
                foreach (string command in AllCommands)
                {
                    connection.JustExecute(command, null, transaction);
                }
                connection.JustExecute("PRAGMA foreign_keys = on", null, transaction);
                bool result = transaction.Commit();
                transaction.Dispose();
                AllCommands.Clear();
                return result;
            }
            return true;
        }
    }
}
