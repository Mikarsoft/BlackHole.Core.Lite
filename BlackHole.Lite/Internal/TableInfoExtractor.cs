using BlackHole.CoreSupport;
using BlackHole.Entities;
using System.Reflection;

namespace BlackHole.Internal
{
    internal class TableInfoExtractor
    {
        internal readonly List<ColumnInfo> Columns = new();

        internal readonly List<FKInfo> ForeignKeys = new();

        internal readonly List<UniqueInfo> UniqueIndices = new();

        internal readonly List<IndexInfo> Indices  = new();

        Type TableType { get; set; }

        Type FkType = typeof(ForeignKey<>);
        Type UQType = typeof(Unique);
        Type NNType = typeof(NotNullable);
        Type VCSize = typeof(VarCharSize);

        internal TableInfoExtractor(Type tableType)
        {
            TableType = tableType;
        }

        public TableCompleteInfo ExtractData()
        {
            string TableName = TableType.Name;

            var relationType = GetRelationEntityType(TableType);

            if (relationType != null)
            {
                var builderType = typeof(RelationBuilder<>).MakeGenericType(relationType);
                var builder = Activator.CreateInstance(builderType);

                var instance = Activator.CreateInstance(TableType);
                var configureMethod = TableType.GetMethod("Congifure");
                configureMethod!.Invoke(instance, new[] { builder });

                var flags = BindingFlags.NonPublic | BindingFlags.Instance;
                var indices = (List<BHEntityIndex>)builderType.GetField("Indices", flags)!.GetValue(builder)!;
                var relations = (List<IBHRelation>)builderType.GetField("Relations", flags)!.GetValue(builder)!;

                ushort uniqueId = 256;

                foreach (var index in indices)
                {
                    if (!index.IsIndexUnique)
                    {
                        Indices.Add(new IndexInfo(index.IndexColumns, uniqueId));
                    }
                    else
                    {
                        foreach (var prop in index.IndexColumns)
                        {
                            UniqueIndices.Add(new UniqueInfo
                            {
                                PropertyName = prop,
                                GroupId = uniqueId
                            });
                        }
                    }

                    uniqueId++;
                }

                foreach (var relation in relations)
                {
                    ForeignKeys.Add(new FKInfo()
                    {
                        ReferencedColumn = "Id",
                        ReferencedTable = relation.IncludedType.Name,
                        OnDelete = relation.OnDelete.DeleteAction,
                        IsNullable = relation.OnDelete.IsNullable,
                        PropertyName = relation.ForeignKeyPropertyName,
                        MapPropertyName = relation.PropertyName,
                        BackwardsIncludeProp = relation.BackwardInclude?.PropertyName
                    });
                }
            }

            PropertyInfo[] Properties = TableType.GetProperties()
                .Where(p => p.PropertyType.IsAllowedType())
                .ToArray();

            foreach (var property in Properties)
            {
                object[] attributes = property.GetCustomAttributes(true);

                bool mandatoryNull = false;

                if (property.PropertyType.Name.Contains("Nullable"))
                {
                    mandatoryNull = true;
                }

                bool nullability = mandatoryNull;
                int size = 255;

                if (attributes.Length > 0)
                {
                    object? FK_attribute = attributes.FirstOrDefault(x => x.GetType() == FkType);

                    if (FK_attribute != null)
                    {
                        var tName = FkType.GetProperty("TableName")?.GetValue(FK_attribute, null);
                        var tColumn = FkType.GetProperty("Column")?.GetValue(FK_attribute, null);
                        var tNullable = FkType.GetProperty("Nullability")?.GetValue(FK_attribute, null) ?? true;

                        if (FkType.GetProperty("OnDelete")?.GetValue(FK_attribute, null) is OnDeleteBehavior deleteBehavior)
                        {
                            nullability = (bool?)tNullable == true;

                            ForeignKeys.Add(new FKInfo()
                            {
                                PropertyName = property.Name,
                                ReferencedTable = $"{tName}",
                                ReferencedColumn = $"{tColumn}",
                                IsNullable = nullability,
                                OnDelete = deleteBehavior
                            });
                        }
                    }
                    else
                    {
                        object? NN_attribute = attributes.FirstOrDefault(x => x.GetType() == NNType);

                        if (NN_attribute != null)
                        {
                            nullability = false;
                        }
                    }

                    object? textSize = attributes.FirstOrDefault(x => x.GetType() == VCSize);

                    if (textSize != null)
                    {
                        if (VCSize.GetProperty("Charlength")?.GetValue(textSize, null) is int chSize)
                        {
                            size = chSize;
                        }
                    }

                    if (nullability != mandatoryNull)
                    {
                        throw new Exception($"Inconsistent Nullablility between Attributes and Property at Property '{property.Name}' of  Table '{TableName}'");
                    }

                    object? UQ_attribute = attributes.FirstOrDefault(x => x.GetType() == UQType);

                    if (UQ_attribute != null)
                    {
                        if (UQType.GetProperty("UniqueGroupId")?.GetValue(UQ_attribute, null) is int groupId)
                        {
                            if (!nullability)
                            {
                                UniqueIndices.Add(new UniqueInfo
                                {
                                    PropertyName = property.Name,
                                    GroupId = groupId
                                });
                            }
                            else
                            {
                                throw new Exception($"Property '{property.Name}' of Entity '{TableType.Name}' is marked with UNIQUE CONSTRAINT and it Requires to be 'NOT NULL'.");
                            }
                        }
                    }
                }

                Type propBaseType;

                if (property.PropertyType.Name.Contains("Nullable"))
                {
                    if (property.PropertyType.GenericTypeArguments != null && property.PropertyType.GenericTypeArguments.Length > 0)
                    {
                        propBaseType = property.PropertyType.GenericTypeArguments[0];
                    }
                    else
                    {
                        propBaseType = property.PropertyType;
                    }
                }
                else
                {
                    propBaseType = property.PropertyType;
                }

                Columns.Add(new ColumnInfo()
                {
                    PropertyType = property.PropertyType,
                    PropertyBaseType = propBaseType,
                    PropertyName= property.Name,
                    IsNullable = nullability,
                    IsPrimaryKey = property.Name == "Id",
                    Size = size
                });
            }

            return new TableCompleteInfo(TableType)
            {
                Columns = Columns,
                ForeignKeys = ForeignKeys,
                Indices = Indices,
                UniqueIndices = UniqueIndices
            };
        }

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
    }
}
