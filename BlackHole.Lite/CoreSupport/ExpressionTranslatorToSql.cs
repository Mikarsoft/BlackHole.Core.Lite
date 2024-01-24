using System.Linq.Expressions;
using System.Reflection;

namespace BlackHole.CoreSupport
{
    internal static class ExpressionTranslatorToSql
    {
        internal static ColumnsAndParameters SplitMembers<T>(this Expression<Func<T, bool>> fullexpression, string? letter, List<BlackHoleParameter>? DynamicParams, int index)
        {
            List<ExpressionsData> expressionTree = new();

            BinaryExpression? currentOperation = null;
            MethodCallExpression? methodCallOperation = null;

            if (fullexpression.Body is BinaryExpression bExp)
            {
                currentOperation = bExp;
            }

            if (fullexpression.Body is MethodCallExpression mcExp)
            {
                methodCallOperation = mcExp;
            }

            int currentIndx = 0;
            bool startTranslate = false;

            if (currentOperation != null || methodCallOperation != null)
            {
                startTranslate = true;

                expressionTree.Add(new ExpressionsData()
                {
                    operation = currentOperation,
                    leftMethodMember = methodCallOperation,
                    leftMember = currentOperation?.Left as MemberExpression,
                    rightMember = currentOperation?.Right as MemberExpression,
                    expressionType = currentOperation != null ? currentOperation.NodeType : ExpressionType.Default,
                    rightChecked = false,
                    leftChecked = false,
                    memberValue = null
                });
            }
            else if(fullexpression.Body is UnaryExpression unExpr)
            {
                if (unExpr.Operand is MemberExpression mExpr)
                {
                    MemberExpression Exp = mExpr;
                    ExpressionType ExpType = ExpressionType.NotEqual;

                    expressionTree.Add(new ExpressionsData()
                    {
                        operation = null,
                        leftMethodMember = null,
                        expressionType = ExpType,
                        rightChecked = false,
                        leftChecked = false,
                        memberValue = true,
                        parentIndex = currentIndx,
                        leftMember = Exp,
                    });
                }
            }
            else if(fullexpression.Body is MemberExpression memberExpr)
            {
                ExpressionType ExpType = ExpressionType.Equal;

                expressionTree.Add(new ExpressionsData()
                {
                    operation = null,
                    leftMethodMember = null,
                    expressionType = ExpType,
                    rightChecked = false,
                    leftChecked = false,
                    memberValue = true,
                    parentIndex = currentIndx,
                    leftMember = memberExpr
                });
            }

            while (startTranslate)
            {
                bool addTotree = false;

                if (expressionTree[currentIndx].operation != null)
                {
                    if (expressionTree[currentIndx].expressionType == ExpressionType.AndAlso || expressionTree[currentIndx].expressionType == ExpressionType.OrElse)
                    {

                        BinaryExpression? leftOperation = expressionTree[currentIndx].operation?.Left as BinaryExpression;
                        BinaryExpression? rightOperation = expressionTree[currentIndx].operation?.Right as BinaryExpression;
                        MethodCallExpression? leftCallOperation = expressionTree[currentIndx].operation?.Left as MethodCallExpression;
                        MethodCallExpression? rightCallOperation = expressionTree[currentIndx].operation?.Right as MethodCallExpression;

                        if (!expressionTree[currentIndx].leftChecked)
                        {
                            if(leftOperation != null || leftCallOperation != null)
                            {
                                expressionTree.Add(new ExpressionsData()
                                {
                                    operation = leftOperation,
                                    leftMethodMember = leftCallOperation,
                                    expressionType = leftOperation != null ? leftOperation.NodeType : ExpressionType.Default,
                                    rightChecked = false,
                                    leftChecked = false,
                                    memberValue = null,
                                    parentIndex = currentIndx
                                });
                                expressionTree[currentIndx].leftChecked = true;
                                addTotree = true;
                            }
                            else if(expressionTree[currentIndx].operation?.Left is UnaryExpression uExp)
                            {
                                if (uExp.Operand is MemberExpression mExp)
                                {
                                    MemberExpression leftExp = mExp;
                                    ExpressionType leftExpType = ExpressionType.Equal;

                                    expressionTree.Add(new ExpressionsData()
                                    {
                                        operation = leftOperation,
                                        leftMethodMember = leftCallOperation,
                                        expressionType = leftExpType,
                                        rightChecked = false,
                                        leftChecked = false,
                                        memberValue = true,
                                        parentIndex = currentIndx,
                                        leftMember = leftExp,
                                    });
                                    expressionTree[currentIndx].leftChecked = true;
                                    addTotree = true;
                                }
                            }
                            else if (expressionTree[currentIndx].leftMember != null)
                            {
                                MemberExpression? leftExp = expressionTree[currentIndx].leftMember;
                                ExpressionType leftExpType = ExpressionType.Equal;

                                expressionTree.Add(new ExpressionsData()
                                {
                                    operation = leftOperation,
                                    leftMethodMember = leftCallOperation,
                                    expressionType = leftExpType,
                                    rightChecked = false,
                                    leftChecked = false,
                                    memberValue = true,
                                    parentIndex = currentIndx,
                                    leftMember = leftExp,
                                });
                                expressionTree[currentIndx].leftChecked = true;
                                addTotree = true;
                            }
                        }

                        if (!expressionTree[currentIndx].rightChecked)
                        {
                            if(rightOperation != null || rightCallOperation != null)
                            {
                                expressionTree.Add(new ExpressionsData()
                                {
                                    operation = rightOperation,
                                    rightMethodMember = rightCallOperation,
                                    expressionType = rightOperation != null ? rightOperation.NodeType : ExpressionType.Default,
                                    rightChecked = false,
                                    leftChecked = false,
                                    memberValue = null,
                                    parentIndex = currentIndx
                                });
                                expressionTree[currentIndx].rightChecked = true;
                                addTotree = true;
                            }
                            else if (expressionTree[currentIndx].operation?.Right is UnaryExpression uExp)
                            {
                                if (uExp.Operand is MemberExpression mExp)
                                {
                                    MemberExpression rightExp = mExp;
                                    ExpressionType rightExpType = ExpressionType.NotEqual;

                                    expressionTree.Add(new ExpressionsData()
                                    {
                                        operation = rightOperation,
                                        leftMethodMember = rightCallOperation,
                                        expressionType = rightExpType,
                                        rightChecked = false,
                                        leftChecked = false,
                                        memberValue = true,
                                        parentIndex = currentIndx,
                                        leftMember = rightExp
                                    });
                                    expressionTree[currentIndx].rightChecked = true;
                                    addTotree = true;
                                }
                            }
                            else if (expressionTree[currentIndx].rightMember != null)
                            {
                                MemberExpression? rightExp = expressionTree[currentIndx].rightMember;
                                ExpressionType rightExpType = ExpressionType.Equal;

                                expressionTree.Add(new ExpressionsData()
                                {
                                    operation = rightOperation,
                                    leftMethodMember = rightCallOperation,
                                    expressionType = rightExpType,
                                    rightChecked = false,
                                    leftChecked = false,
                                    memberValue = true,
                                    parentIndex = currentIndx,
                                    leftMember = rightExp
                                });
                                expressionTree[currentIndx].rightChecked = true;
                                addTotree = true;
                            }
                        }

                        if (addTotree)
                        {
                            currentIndx = expressionTree.Count - 1;
                        }
                    }
                    else
                    {
                        if (!expressionTree[currentIndx].rightChecked)
                        {
                            if (expressionTree[currentIndx].operation?.Right is MemberExpression rightMember)
                            {
                                expressionTree[currentIndx].InvokeOrTake<T>(rightMember, true);
                            }

                            if (expressionTree[currentIndx].operation?.Right is ConstantExpression rightConstant)
                            {
                                expressionTree[currentIndx].memberValue = rightConstant?.Value;
                                expressionTree[currentIndx].IsNullValue = rightConstant?.Value == null;
                            }

                            if (expressionTree[currentIndx].operation?.Right is BinaryExpression rightBinary)
                            {
                                expressionTree[currentIndx].memberValue = Expression.Lambda(rightBinary).Compile().DynamicInvoke();
                                expressionTree[currentIndx].IsNullValue = expressionTree[currentIndx].memberValue == null;
                            }

                            if (expressionTree[currentIndx].operation?.Right is MethodCallExpression rightmethodMember)
                            {
                                expressionTree[currentIndx].rightMethodMember = rightmethodMember;
                            }

                            expressionTree[currentIndx].rightChecked = true;
                        }

                        if (!expressionTree[currentIndx].leftChecked)
                        {
                            if (expressionTree[currentIndx].operation?.Left is MemberExpression leftMember)
                            {
                                expressionTree[currentIndx].InvokeOrTake<T>(leftMember, false);
                            }

                            if (expressionTree[currentIndx].operation?.Left is ConstantExpression leftConstant)
                            {
                                expressionTree[currentIndx].memberValue = leftConstant?.Value;
                                expressionTree[currentIndx].IsNullValue = leftConstant?.Value == null;
                            }

                            if (expressionTree[currentIndx].operation?.Left is BinaryExpression leftBinary)
                            {
                                expressionTree[currentIndx].memberValue = Expression.Lambda(leftBinary).Compile().DynamicInvoke();
                                expressionTree[currentIndx].IsNullValue = expressionTree[currentIndx].memberValue == null;
                            }

                            if (expressionTree[currentIndx].operation?.Left is MethodCallExpression leftmethodMember)
                            {
                                expressionTree[currentIndx].leftMethodMember = leftmethodMember;
                            }

                            expressionTree[currentIndx].leftChecked = true;
                        }
                    }
                }

                if (expressionTree[currentIndx].methodData.Count == 0)
                {
                    MethodCallExpression? leftMethodMember = expressionTree[currentIndx].leftMethodMember;
                    MethodCallExpression? rightMethodMember = expressionTree[currentIndx].rightMethodMember;

                    if (leftMethodMember != null)
                    {
                        var func = leftMethodMember.Method;
                        var arguments = leftMethodMember.Arguments;
                        var obj = leftMethodMember.Object;
                        bool cleanOfMembers = true;

                        if (obj?.NodeType == ExpressionType.MemberAccess)
                        {
                            cleanOfMembers = false;
                        }

                        if (!expressionTree[currentIndx].methodChecked)
                        {
                            List<object?> MethodArguments = new();
                            object?[] parameters = new object[arguments.Count];

                            for (int i = 0; i < arguments.Count; i++)
                            {
                                if (arguments[i] is MemberExpression argMemmber)
                                {
                                    string? typeName = argMemmber.Member.ReflectedType?.FullName;

                                    if (typeName != null && (typeName == typeof(T).BaseType?.FullName || typeName == typeof(T).FullName))
                                    {
                                        cleanOfMembers = false;
                                        obj = argMemmber;
                                        MethodArguments.Add(argMemmber.Member);
                                    }
                                    else
                                    {
                                        parameters[i] = Expression.Lambda(argMemmber).Compile().DynamicInvoke();
                                        MethodArguments.Add(parameters[i]);
                                    }
                                }

                                if (arguments[i] is ConstantExpression argConstant)
                                {
                                    parameters[i] = argConstant.Value;
                                    MethodArguments.Add(argConstant.Value);
                                }

                                if (arguments[i] is BinaryExpression argBinary)
                                {
                                    parameters[i] = Expression.Lambda(argBinary).Compile().DynamicInvoke();
                                    MethodArguments.Add(parameters[i]);
                                }

                                if (arguments[i] is MethodCallExpression argMethod)
                                {
                                    foreach (var arg in argMethod.Arguments)
                                    {
                                        MemberExpression? SubargMemmber = arg as MemberExpression;

                                        if (SubargMemmber?.Member.ReflectedType?.FullName == typeof(T).FullName)
                                        {
                                            cleanOfMembers = false;
                                        }
                                    }

                                    if (cleanOfMembers)
                                    {
                                        parameters[i] = Expression.Lambda(argMethod).Compile().DynamicInvoke();
                                        MethodArguments.Add(parameters[i]);
                                    }
                                }

                                if (arguments[i] is LambdaExpression argLambda)
                                {
                                    expressionTree[currentIndx].rightMember = argLambda.Body as MemberExpression;
                                }
                            }

                            if (cleanOfMembers)
                            {
                                if (obj != null)
                                {
                                    object? skata = obj.Type != typeof(string) ? Activator.CreateInstance(obj.Type, null) : string.Empty;
                                    expressionTree[currentIndx].memberValue = func.Invoke(skata, parameters);
                                }
                            }
                            else
                            {
                                expressionTree[currentIndx].methodData.Add(new MethodExpressionData
                                {
                                    MethodName = func.Name,
                                    MethodArguments = MethodArguments,
                                    CastedOn = obj,
                                    ComparedValue = expressionTree[currentIndx].memberValue,
                                    CompareProperty = expressionTree[currentIndx].rightMember,
                                    OperatorType = expressionTree[currentIndx].expressionType,
                                    ReverseOperator = true,
                                    TableName = typeof(T).Name
                                });
                            }
                        }
                    }

                    if (rightMethodMember != null)
                    {
                        var func = rightMethodMember.Method;
                        var arguments = rightMethodMember.Arguments;
                        var obj = rightMethodMember.Object;
                        bool cleanOfMembers = true;

                        if (obj?.NodeType == ExpressionType.MemberAccess)
                        {
                            cleanOfMembers = false;
                        }

                        if (!expressionTree[currentIndx].methodChecked)
                        {
                            List<object?> MethodArguments = new();
                            object?[] parameters = new object[arguments.Count];

                            for (int i = 0; i < arguments.Count; i++)
                            {
                                if (arguments[i] is MemberExpression argMemmber)
                                {
                                    string? typeName = argMemmber.Member.ReflectedType?.FullName;

                                    if (typeName != null && (typeName == typeof(T).BaseType?.FullName || typeName == typeof(T).FullName))
                                    {
                                        cleanOfMembers = false;
                                        obj = argMemmber;
                                        MethodArguments.Add(argMemmber.Member);
                                    }
                                    else
                                    {
                                        parameters[i] = Expression.Lambda(argMemmber).Compile().DynamicInvoke();
                                        MethodArguments.Add(parameters[i]);
                                    }
                                }

                                if (arguments[i] is ConstantExpression argConstant)
                                {
                                    parameters[i] = argConstant.Value;
                                    MethodArguments.Add(argConstant.Value);
                                }

                                if (arguments[i] is BinaryExpression argBinary)
                                {
                                    parameters[i] = Expression.Lambda(argBinary).Compile().DynamicInvoke();
                                    MethodArguments.Add(parameters[i]);
                                }

                                if (arguments[i] is MethodCallExpression argMethod)
                                {
                                    foreach (var arg in argMethod.Arguments)
                                    {
                                        MemberExpression? SubargMemmber = arg as MemberExpression;

                                        if (SubargMemmber?.Member.ReflectedType?.FullName == typeof(T).FullName)
                                        {
                                            cleanOfMembers = false;
                                        }
                                    }

                                    if (cleanOfMembers)
                                    {
                                        parameters[i] = Expression.Lambda(argMethod).Compile().DynamicInvoke();
                                        MethodArguments.Add(parameters[i]);
                                    }
                                }

                                if (arguments[i] is LambdaExpression argLambda)
                                {
                                    expressionTree[currentIndx].leftMember = argLambda.Body as MemberExpression;
                                }
                            }

                            if (cleanOfMembers)
                            {
                                if (obj != null)
                                {
                                    object? skata = obj.Type != typeof(string) ? Activator.CreateInstance(obj.Type, null) : string.Empty;
                                    expressionTree[currentIndx].memberValue = func.Invoke(skata, parameters);
                                }
                            }
                            else
                            {
                                expressionTree[currentIndx].methodData.Add(new MethodExpressionData
                                {
                                    MethodName = func.Name,
                                    MethodArguments = MethodArguments,
                                    CastedOn = obj,
                                    ComparedValue = expressionTree[currentIndx].memberValue,
                                    CompareProperty = expressionTree[currentIndx].leftMember,
                                    OperatorType = expressionTree[currentIndx].expressionType,
                                    ReverseOperator = false,
                                    TableName = typeof(T).Name
                                });
                            }
                        }
                    }
                }

                if (!addTotree)
                {
                    currentIndx -= 1;
                }

                if (currentIndx < 0)
                {
                    startTranslate = false;
                }
            }

            return expressionTree.ExpressionTreeToSql(letter, DynamicParams, index);
        }

        private static void InvokeOrTake<T>(this ExpressionsData thisBranch, MemberExpression memberExp, bool isRight)
        {
            string? typeName = memberExp.Member.ReflectedType?.FullName;

            if (typeName != null && (typeName == typeof(T).BaseType?.FullName || typeName == typeof(T).FullName))
            {
                try
                {
                    var lExp = Expression.Lambda(memberExp);

                    if(lExp.Parameters.Count > 0)
                    {
                        thisBranch.memberValue = lExp.Compile().DynamicInvoke();
                        thisBranch.IsNullValue = thisBranch.memberValue == null;
                    }
                    else
                    {
                        if (isRight)
                        {
                            thisBranch.rightMember = memberExp;
                        }
                        else
                        {
                            thisBranch.leftMember = memberExp;
                        }
                    }
                }
                catch
                {
                    if (isRight)
                    {
                        thisBranch.rightMember = memberExp;
                    }
                    else
                    {
                        thisBranch.leftMember = memberExp;
                    }
                }
            }
            else
            {
                thisBranch.memberValue = Expression.Lambda(memberExp).Compile().DynamicInvoke();
                thisBranch.IsNullValue = thisBranch.memberValue == null;
            }
        }

        internal static ColumnsAndParameters ExpressionTreeToSql(this List<ExpressionsData> data, string? letter, List<BlackHoleParameter>? parameters, int index)
        {
            if (parameters == null)
            {
                parameters = new List<BlackHoleParameter>();
            }

            List<ExpressionsData> children = data.Where(x => x.memberValue != null || x.methodData.Count > 0  || (x.memberValue == null && x.IsNullValue)).ToList();
            string[] translations = new string[children.Count];

            foreach (ExpressionsData child in children)
            {
                ExpressionsData parent = data[child.parentIndex];

                if (child.methodData.Count > 0)
                {
                    if (child.memberValue != null && child.methodData[0].ComparedValue == null && child.methodData[0].CompareProperty == null)
                    {
                        child.methodData[0].ComparedValue = child.memberValue;
                    }

                    SqlFunctionsReader sqlFunctionResult = new(child.methodData[0], index, letter);

                    if (sqlFunctionResult.ParamName != string.Empty)
                    {
                        parameters.Add(new BlackHoleParameter { Name = sqlFunctionResult.ParamName, Value = sqlFunctionResult.Value });
                        index++;
                    }

                    if (parent.sqlCommand == string.Empty)
                    {
                        parent.sqlCommand = $"{sqlFunctionResult.SqlCommand}";
                    }
                    else
                    {
                        parent.sqlCommand += $" and {sqlFunctionResult.SqlCommand}";
                    }

                    parent.leftChecked = false;
                }
                else
                {
                    if (parent.leftChecked)
                    {
                        ColumnAndParameter childParams = child.TranslateExpression(index, letter);

                        if (childParams.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = childParams.ParamName, Value = childParams.Value });
                        }

                        parent.sqlCommand = $"{childParams.Column}";

                        parent.leftChecked = false;
                        index++;
                    }
                    else
                    {
                        ColumnAndParameter parentCols = parent.TranslateExpression(index, letter);

                        if (parentCols.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = parentCols.ParamName, Value = parentCols.Value });
                        }

                        index++;

                        if (child != parent)
                        {
                            ColumnAndParameter childCols = child.TranslateExpression(index, letter);

                            if (childCols.ParamName != string.Empty)
                            {
                                parameters.Add(new BlackHoleParameter { Name = childCols.ParamName, Value = childCols.Value });
                            }

                            parent.sqlCommand = $"({parent.sqlCommand} {parentCols.Column} {childCols.Column})";
                            index++;
                        }
                        else
                        {
                            parent.sqlCommand = $"({parentCols.Column})";
                        }
                    }
                }
            }

            List<ExpressionsData> selfCompair = data.Where(x => x.memberValue == null && x.leftMember != null && x.rightMember != null && x.methodData.Count == 0).ToList();

            foreach(ExpressionsData self in selfCompair)
            {
                ExpressionsData parent = data[self.parentIndex];
                ColumnAndParameter selfCompairCols = self.TranslateSelfCompairExpression(letter);

                parent.sqlCommand = $"({selfCompairCols.Column})";
                parent.leftChecked = false;
            }

            List<ExpressionsData> parents = data.Where(x => x.memberValue == null && !x.IsNullValue && x.methodData.Count == 0 && x.expressionType.IsParentExpressionType()).ToList();

            if (parents.Count > 1)
            {
                parents.RemoveAt(0);
                int parentsCount = parents.Count;

                for (int i = 0; i < parentsCount; i++)
                {
                    ExpressionsData parent = data[parents[parentsCount - 1 - i].parentIndex];

                    if (parent.leftChecked)
                    {
                        parent.sqlCommand = parents[parentsCount - 1 - i].sqlCommand;
                        parent.leftChecked = false;
                    }
                    else
                    {
                        ColumnAndParameter parentParams = parent.TranslateExpression(index, letter);

                        if (parentParams.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = parentParams.ParamName, Value = parentParams.Value });
                        }

                        parent.sqlCommand = $"({parent.sqlCommand} {parentParams.Column} {parents[parentsCount - 1 - i].sqlCommand})";

                        index++;
                    }
                }
            }

            return new ColumnsAndParameters { Columns = data[0].sqlCommand, Parameters = parameters, Count = index };
        }

        private static bool IsParentExpressionType(this ExpressionType expType)
        {
            return expType switch
            {
                ExpressionType.AndAlso or ExpressionType.OrElse => true,
                _ => false
            };
        }

        private static ColumnAndParameter TranslateSelfCompairExpression(this ExpressionsData expression, string? letter)
        {
            string? leftPart = expression.leftMember?.ToString().Split('.')[1];
            string? rightPart = expression.rightMember?.ToString().Split(".")[1];
            string subLetter = letter != string.Empty ? $"{letter}." : string.Empty;

            string column = expression.expressionType switch
            {
                ExpressionType.Equal => $"{subLetter}{leftPart} = {subLetter}{rightPart}",
                ExpressionType.GreaterThanOrEqual => $"{subLetter}{leftPart} >= {subLetter}{rightPart}",
                ExpressionType.LessThanOrEqual => $"{leftPart} <= {rightPart}",
                ExpressionType.LessThan => $"{subLetter} {leftPart} < {subLetter}{rightPart}",
                ExpressionType.GreaterThan => $"{subLetter} {leftPart} > {subLetter}{rightPart}",
                ExpressionType.NotEqual => $"{subLetter} {leftPart} != {subLetter}{rightPart}",
                _ => string.Empty
            };

            return new ColumnAndParameter { Column = column};
        }


        private static ColumnAndParameter TranslateExpression(this ExpressionsData expression, int index, string? letter)
        {
            string column = string.Empty;
            string parameter = string.Empty;
            object? value = new();
            string[]? variable = new string[2];
            string subLetter = letter != string.Empty ? $"{letter}." : string.Empty;
            string sqlOperator = "=";

            switch (expression.expressionType)
            {
                case ExpressionType.AndAlso:
                    column = " and ";
                    break;
                case ExpressionType.OrElse:
                    column = " or ";
                    break;
                case ExpressionType.Equal:
                    value = expression?.memberValue;
                    variable = expression?.leftMember != null ? expression?.leftMember.ToString().Split('.') : expression?.rightMember?.ToString().Split('.');
                    column = $"{subLetter}{variable?[1]} = @{variable?[1]}{index}";
                    if (value == null)
                    {
                        column = $"{subLetter}{variable?[1]} is @{variable?[1]}{index}";
                    }
                    parameter = $"{variable?[1]}{index}";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    if (expression?.leftMember != null)
                    {
                        variable = expression?.leftMember.ToString().Split('.');
                        sqlOperator = ">=";
                    }
                    else
                    {
                        variable = expression?.rightMember?.ToString().Split('.');
                        sqlOperator = "<=";
                    }
                    column = $"{subLetter}{variable?[1]} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.LessThanOrEqual:
                    if (expression?.leftMember != null)
                    {
                        variable = expression?.leftMember.ToString().Split('.');
                        sqlOperator = "<=";
                    }
                    else
                    {
                        variable = expression?.rightMember?.ToString().Split('.');
                        sqlOperator = ">=";
                    }
                    column = $"{subLetter}{variable?[1]} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.LessThan:
                    if (expression?.leftMember != null)
                    {
                        variable = expression?.leftMember.ToString().Split('.');
                        sqlOperator = "<";
                    }
                    else
                    {
                        variable = expression?.rightMember?.ToString().Split('.');
                        sqlOperator = ">";
                    }
                    column = $"{subLetter}{variable?[1]} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.GreaterThan:
                    if (expression?.leftMember != null)
                    {
                        variable = expression?.leftMember.ToString().Split('.');
                        sqlOperator = ">";
                    }
                    else
                    {
                        variable = expression?.rightMember?.ToString().Split('.');
                        sqlOperator = "<";
                    }
                    column = $"{subLetter}{variable?[1]} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.NotEqual:
                    value = expression?.memberValue;
                    variable = expression?.leftMember != null ? expression?.leftMember.ToString().Split('.') : expression?.rightMember?.ToString().Split('.');
                    column = $"{subLetter}{variable?[1]} != @{variable?[1]}{index}";
                    if (value == null)
                    {
                        column = $"{subLetter}{variable?[1]} is not @{variable?[1]}{index}";
                    }
                    parameter = $"{variable?[1]}{index}";
                    break;
                case ExpressionType.Default:
                    column = "1 != 1";
                    break;
            }

            return new ColumnAndParameter { Column = column, ParamName = parameter, Value = value };
        }

        internal static void AdditionalParameters(this ColumnsAndParameters colsAndParams, object item)
        {
            Type type = item.GetType();
            PropertyInfo[] props = type.GetProperties();

            foreach (var prop in props)
            {
                colsAndParams.Parameters.Add(new BlackHoleParameter { Name = prop.Name, Value = prop.GetValue(item) });
            }
        }

        internal static JoinsData<T, TOther> CreateJoin<T, TOther>(this PreJoinsData<T, TOther> prejoin, LambdaExpression key, LambdaExpression otherKey)
        {
            if (prejoin.IsFirstJoin)
            {
                return prejoin.CreateFirstJoin(key, otherKey);
            }

            return prejoin.CreateOtherJoin(key, otherKey);
        }

        private static JoinsData<T, TOther> CreateFirstJoin<T, TOther>(this PreJoinsData<T, TOther> data, LambdaExpression key, LambdaExpression otherKey)
        {
            string? parameter = key.Parameters[0].Name;
            MemberExpression? member = key.Body as MemberExpression;
            string? propName = member?.Member.Name;
            string? parameterOther = otherKey.Parameters[0].Name;
            MemberExpression? memberOther = otherKey.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;

            JoinsData<T, TOther> firstJoin = new()
            {
                BaseTable = typeof(T),
                Ignore = false,
                TablesToLetters = new(),
                Letters = new(),
                WherePredicates = string.Empty,
                DynamicParams = new(),
            };

            firstJoin.TablesToLetters.Add(new TableLetters { Table = typeof(T), Letter = parameter });
            firstJoin.Letters.Add(parameter);

            if (parameterOther == parameter)
            {
                parameterOther += firstJoin.HelperIndex.ToString();
                firstJoin.HelperIndex++;
            }

            firstJoin.TablesToLetters.Add(new TableLetters { Table = typeof(TOther), Letter = parameterOther });
            firstJoin.Letters.Add(parameterOther);

            firstJoin.Joins = $" {data.JoinType} join {typeof(TOther).Name} {parameterOther} on {parameterOther}.{propNameOther} = {parameter}.{propName}";
            firstJoin.AllProps = CollectJoinProperties<T, TOther>(parameter, parameterOther, true);
            return firstJoin;
        }

        private static JoinsData<T, TOther> CreateOtherJoin<T, TOther>(this PreJoinsData<T, TOther> previousData, LambdaExpression key, LambdaExpression otherKey)
        {
            JoinsData<T, TOther> data = new()
            {
                BaseTable = previousData.BaseTable,
                TablesToLetters = previousData.TablesToLetters,
                AllProps = previousData.AllProps,
                Joins = previousData.Joins,
                Letters = previousData.Letters,
                WherePredicates = previousData.WherePredicates,
                DynamicParams = previousData.DynamicParams,
                HelperIndex = previousData.HelperIndex,
                ParamsCount = previousData.ParamsCount,
                Ignore = previousData.Ignore
            };

            string? parameter = string.Empty;

            TableLetters? firstType = data.TablesToLetters.FirstOrDefault(x => x.Table == typeof(T));

            if (firstType == null)
            {
                data.Ignore = true;
            }
            else
            {
                parameter = firstType.Letter;
            }

            if (data.Ignore)
            {
                return data;
            }

            MemberExpression? member = key.Body as MemberExpression;
            string? propName = member?.Member.Name;
            string? parameterOther = otherKey.Parameters[0].Name;
            MemberExpression? memberOther = otherKey.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;

            TableLetters? secondTable = data.TablesToLetters.FirstOrDefault(x => x.Table == typeof(TOther));

            if (secondTable == null)
            {
                bool letterExists = data.Letters.Contains(parameterOther);

                if (letterExists)
                {
                    parameterOther += data.HelperIndex.ToString();
                    data.Letters.Add(parameterOther);
                    data.HelperIndex++;
                }
                data.TablesToLetters.Add(new TableLetters { Table = typeof(TOther), Letter = parameterOther });
            }
            else
            {
                parameterOther = secondTable.Letter;
            }

            data.Joins += $" inner join {typeof(TOther).Name} {parameterOther} on {parameterOther}.{propNameOther} = {parameter}.{propName}";
            data.AllProps.AddRange(CollectJoinProperties<T, TOther>(parameter, parameterOther, false));
            return data;
        }

        private static List<TableProperties> CollectJoinProperties<T, TOther>(string? letter, string? otherLetter, bool firstJoin)
        {
            List<TableProperties> AllProps = new();

            if (firstJoin)
            {
                Type firstType = typeof(T);

                foreach (PropertyInfo propA in firstType.GetProperties())
                {
                    AllProps.Add(new TableProperties
                    {
                        PropName = propA.Name,
                        PropType = propA.PropertyType,
                        TableLetter = letter,
                        TableType = firstType
                    });
                }
            }

            Type otherType = typeof(TOther);

            foreach (PropertyInfo propB in otherType.GetProperties())
            {
                AllProps.Add(new TableProperties
                {
                    PropName = propB.Name,
                    PropType = propB.PropertyType,
                    TableLetter = otherLetter,
                    TableType = otherType
                });
            }

            return AllProps;
        }

        internal static JoinsData<T, TOther> AdditionalJoint<T, TOther, TKey>(this JoinsData<T, TOther> data, Expression<Func<T, TKey>> key, Expression<Func<TOther, TKey>> otherkey)
        {
            if (data.Ignore)
            {
                return data;
            }

            string? firstLetter = data.TablesToLetters.First(t => t.Table == typeof(T)).Letter;
            string? secondLetter = data.TablesToLetters.First(t => t.Table == typeof(TOther)).Letter;

            MemberExpression? member = key.Body as MemberExpression;
            string? propName = member?.Member.Name;
            MemberExpression? memberOther = otherkey.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;

            data.Joins += $" and {secondLetter}.{propNameOther} = {firstLetter}.{propName}";
            return data;
        }

        internal static JoinsData<T, TOther> OptionalJoint<T, TOther, TKey>(this JoinsData<T, TOther> data, Expression<Func<T, TKey>> key, Expression<Func<TOther, TKey>> otherkey)
        {
            if (data.Ignore)
            {
                return data;
            }

            string? firstLetter = data.TablesToLetters.First(t => t.Table == typeof(T)).Letter;
            string? secondLetter = data.TablesToLetters.First(t => t.Table == typeof(TOther)).Letter;

            MemberExpression? member = key.Body as MemberExpression;
            string? propName = member?.Member.Name;
            MemberExpression? memberOther = otherkey.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;

            data.Joins += $" or {secondLetter}.{propNameOther} = {firstLetter}.{propName}";
            return data;
        }

        internal static JoinsData NextAction<T, TOther>(this JoinsData<T, TOther> data)
        {
            return new JoinsData
            {
                DatabaseName = data.DatabaseName,
                BaseTable = data.BaseTable,
                TablesToLetters = data.TablesToLetters,
                AllProps = data.AllProps,
                Joins = data.Joins,
                Letters = data.Letters,
                WherePredicates = data.WherePredicates,
                DynamicParams = data.DynamicParams,
                HelperIndex = data.HelperIndex,
                ParamsCount = data.ParamsCount,
                Ignore = false
            };
        }

        internal static PreJoinsData<T, TOther> NextJoin<T, TOther>(this JoinsData data, string joinType)
        {
            return new PreJoinsData<T, TOther>
            {
                DatabaseName = data.DatabaseName,
                BaseTable = data.BaseTable,
                TablesToLetters = data.TablesToLetters,
                AllProps = data.AllProps,
                Joins = data.Joins,
                Letters = data.Letters,
                WherePredicates = data.WherePredicates,
                DynamicParams = data.DynamicParams,
                HelperIndex = data.HelperIndex,
                ParamsCount = data.ParamsCount,
                Ignore = false,
                IsFirstJoin = false,
                JoinType = joinType
            };
        }

        internal static JoinsData<T, TOther> AddWhereStatementOne<T, TOther>(this JoinsData<T, TOther> data, Expression<Func<T, bool>> predicate)
        {
            if (data.Ignore)
            {
                return data;
            }

            string? letter = data.TablesToLetters.First(x => x.Table == typeof(T)).Letter;
            ColumnsAndParameters colsAndParams = predicate.SplitMembers<T>(letter, data.DynamicParams, data.ParamsCount);
            data.DynamicParams = colsAndParams.Parameters;
            data.ParamsCount = colsAndParams.Count;

            if (data.WherePredicates == string.Empty)
            {
                data.WherePredicates = $" where {colsAndParams.Columns}";
            }
            else
            {
                data.WherePredicates += $" and {colsAndParams.Columns}";
            }

            return data;
        }

        internal static JoinsData<T, TOther> AddWhereStatementTwo<T, TOther>(this JoinsData<T, TOther> data, Expression<Func<TOther, bool>> predicate)
        {
            if (data.Ignore)
            {
                return data;
            }

            string? letter = data.TablesToLetters.First(x => x.Table == typeof(TOther)).Letter;
            ColumnsAndParameters colsAndParams = predicate.SplitMembers<TOther>(letter, data.DynamicParams, data.ParamsCount);
            data.DynamicParams = colsAndParams.Parameters;
            data.ParamsCount = colsAndParams.Count;

            if (data.WherePredicates == string.Empty)
            {
                data.WherePredicates = $" where {colsAndParams.Columns}";
            }
            else
            {
                data.WherePredicates += $" and {colsAndParams.Columns}";
            }

            return data;
        }

        internal static void GivePriority<T, TKey>(this List<TableProperties> existingProps, Expression<Func<T, TKey>> key)
        {
            Type propertyType = key.Body.Type;
            MemberExpression? member = key.Body as MemberExpression;
            string? propName = member?.Member.Name;
            existingProps.First(x => x.PropName == propName && x.TableType == typeof(T)).HasPriority = true;
        }

        internal static List<PropertyOccupation> MapPropertiesToDto<Dto>(this List<TableProperties> existingProps)
        {
            List<PropertyOccupation> occupiedProps = new();

            foreach (PropertyInfo prop in typeof(Dto).GetProperties())
            {
                occupiedProps.Add(new PropertyOccupation
                {
                    PropName = prop.Name,
                    PropType = prop.PropertyType,
                });
            }

            foreach (TableProperties existingProp in existingProps.Where(x => !x.HasPriority))
            {
                PropertyOccupation? occupied = occupiedProps.FirstOrDefault(x => x.PropName == existingProp.PropName);
                if (occupied != null && !occupied.Occupied && occupied.PropType == existingProp.PropType)
                {
                    occupied.TableLetter = existingProp.TableLetter;
                    occupied.Occupied = true;
                }
            }

            foreach (TableProperties existingProp in existingProps.Where(x => x.HasPriority))
            {
                PropertyOccupation? occupied = occupiedProps.FirstOrDefault(x => x.PropName == existingProp.PropName);
                if (occupied != null && occupied.PropType == existingProp.PropType)
                {
                    occupied.TableLetter = existingProp.TableLetter;
                }
            }

            return occupiedProps;
        }
    }
}
