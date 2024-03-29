﻿using BlackHole.Core;
using System.Reflection;

namespace BlackHole.Internal
{
    internal class BHInitialDataBuilder
    {
        internal void InsertDefaultData(List<Type> initialDataClasses, string databaseName)
        {
            foreach (Type initialData in initialDataClasses)
            {
                object? instance = Activator.CreateInstance(initialData);
                MethodInfo? method = initialData.GetMethod("DefaultData");
                if (instance != null && method != null)
                {
                    object[] Argumnet = new object[1];
                    BHDataInitializer initializer = new(databaseName);
                    Argumnet[0] = initializer;
                    method.Invoke(instance, Argumnet);
                }
            }
        }

        internal void StoreDefaultViews(List<Type> initialViewClasses, string databaseName)
        {
            foreach (Type initialView in initialViewClasses)
            {
                object? instance = Activator.CreateInstance(initialView);
                MethodInfo? method = initialView.GetMethod("DefaultViews");
                if (instance != null && method != null)
                {
                    object[] Argumnet = new object[1];
                    BHViewInitializer initializer = new(databaseName);
                    Argumnet[0] = initializer;
                    method.Invoke(instance, Argumnet);
                }
            }
        }
    }
}
