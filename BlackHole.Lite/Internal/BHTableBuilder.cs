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
        private List<string> CreateTablesTransaction { get; set; } = new();
        private List<string> CustomTransaction { get; set; } = new();
        private EntityContextBuilder entityRegistrator { get; } = new();
        private string CurrentConnectionString { get; set; } 

        public bool IsDeveloperMode { get; set; }

        internal BHTableBuilder()
        {
            CurrentConnectionString = DatabaseStatics.DefaultConnectionString;
            connection = _multiDatabaseSelector.GetExecutionProvider(CurrentConnectionString);
            SqlDatatypes = _multiDatabaseSelector.SqlDatatypesTranslation();
        }

        internal void SwitchConnectionString(string connectionString)
        {
            CurrentConnectionString = connectionString;
            connection.SwitchConnectionString(CurrentConnectionString);
        }

        internal void BuildMultipleTables(List<Type> TableTypes)
        {
            DatabaseStatics.InitializeData = true;
            TableCompleteInfo[] Built = new TableCompleteInfo[TableTypes.Count];

            for (int i = 0; i < Built.Length; i++)
            {
                Built[i] = CreateTable(TableTypes[i]);
                entityRegistrator.InsertEntityContext(Built[i]);
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
            //bool isNullable = true;
            OnDeleteBehavior bhv = OnDeleteBehavior.Restrict;

            foreach (FKInfo fk in commonFKs)
            {
                //if (!fk.IsNullable)
                //{
                //    isNullable = false;
                //}

                fromColumn += $",{fk.PropertyName}";
                toColumn += $",{fk.ReferencedColumn}";
                bhv = fk.OnDelete;
            }

            fromColumn = fromColumn.Remove(0, 1);
            toColumn = toColumn.Remove(0, 1);
            string key = $"{TableName}_{fromColumn}_{ReferencedTable}";
            string hash = HashKey(key);
            string onDeleteRule = OnDeleteCommand(bhv);
            string constraintCommand = $"{constraintBegin} fk_{hash} FOREIGN KEY ({fromColumn}) REFERENCES {ReferencedTable}({toColumn}) {onDeleteRule}";
            return constraintCommand;
        }

        string OnDeleteCommand(OnDeleteBehavior beh)
        {
            return beh switch
            {
                OnDeleteBehavior.Cascade => "on delete cascade",
                OnDeleteBehavior.SetNull => "on delete set null",
                _=> "on delete restrict"
            };
        }

        private string HashKey(string key)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));

                return string.Join("",hash.Select(b => $"{b:X2}"));
            }
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
        }

        internal bool ExecuteTableCreation()
        {
            List<string> AllCommands = [.. CreateTablesTransaction, .. CustomTransaction];

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
