﻿
using BlackHole.Entities;

namespace BlackHole.Core
{
    /// <summary>
    /// A Set of Sql Functions to Use in 'Where' Statements
    /// </summary>
    public static class SqlFunctions
    {

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlEqualTo<TOther>(this Guid property, Func<TOther, Guid> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }
        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlEqualTo<TOther>(this string property, Func<TOther, string> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlEqualTo<TOther>(this int property, Func<TOther, int> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlEqualTo<TOther>(this decimal property, Func<TOther, decimal> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlEqualTo<TOther>(this short property, Func<TOther, short> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlEqualTo<TOther>(this long property, Func<TOther, long> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlEqualTo<TOther>(this double property, Func<TOther, double> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlEqualTo<TOther>(this float property, Func<TOther, float> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>

        public static bool SqlEqualTo<TOther>(this DateTime property, Func<TOther, DateTime> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlEqualTo<TOther>(this byte[] property, Func<TOther, byte[]> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlGreaterThan<TOther>(this int property, Func<TOther, int> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlGreaterThan<TOther>(this decimal property, Func<TOther, decimal> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlGreaterThan<TOther>(this short property, Func<TOther, short> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlGreaterThan<TOther>(this long property, Func<TOther, long> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlGreaterThan<TOther>(this double property, Func<TOther, double> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlGreaterThan<TOther>(this float property, Func<TOther, float> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>

        public static bool SqlGreaterThan<TOther>(this DateTime property, Func<TOther, DateTime> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlLessThan<TOther>(this int property, Func<TOther, int> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlLessThan<TOther>(this decimal property, Func<TOther, decimal> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlLessThan<TOther>(this short property, Func<TOther, short> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlLessThan<TOther>(this long property, Func<TOther, long> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlLessThan<TOther>(this double property, Func<TOther, double> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>
        public static bool SqlLessThan<TOther>(this float property, Func<TOther, float> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Compare a column of the current table to a column of another table in
        /// the 'where' statement of the Data provider.
        /// </summary>
        /// <typeparam name="TOther">Other Table</typeparam>
        /// <param name="property">Current Column</param>
        /// <param name="otherTypesProperty">Other Table's Column</param>
        /// <param name="Id">Id of the other Table's Line</param>

        public static bool SqlLessThan<TOther>(this DateTime property, Func<TOther, DateTime> otherTypesProperty, int Id) where TOther : BlackHoleEntity
        {
            return true;
        }

        /// <summary>
        /// Chaeck is the Current column's Date is more recent than the
        /// selected Date
        /// </summary>
        /// <param name="value">Current column's Date</param>
        /// <param name="afterDate">Selected Date</param>
        public static bool SqlDateAfter(this DateTime value, DateTime afterDate)
        {
            return value > afterDate;
        }

        /// <summary>
        /// Chaeck is the Current column's Date is older than the
        /// selected Date
        /// </summary>
        /// <param name="value">Current column's Date</param>
        /// <param name="beforeDate">Selected Date</param>
        public static bool SqlDateBefore(this DateTime value, DateTime beforeDate)
        {
            return value < beforeDate;
        }

        /// <summary>
        /// Gets The minimum value of the Current Column to compare it with
        /// other values
        /// </summary>
        /// <param name="value">Current Column's value</param>
        public static bool SqlMin(this DateTime value)
        {
            return true;
        }

        /// <summary>
        /// Gets The minimum value of the Current Column to compare it with
        /// other values
        /// </summary>
        /// <param name="value">Current Column's value</param>
        public static bool SqlMin(this int value)
        {
            return true;
        }

        /// <summary>
        /// Gets The minimum value of the Current Column to compare it with
        /// other values
        /// </summary>
        /// <param name="value">Current Column's value</param>
        public static bool SqlMin(this short value)
        {
            return true;
        }

        /// <summary>
        /// Gets The minimum value of the Current Column to compare it with
        /// other values
        /// </summary>
        /// <param name="value">Current Column's value</param>
        public static bool SqlMin(this decimal value)
        {
            return true;
        }

        /// <summary>
        /// Gets The minimum value of the Current Column to compare it with
        /// other values
        /// </summary>
        /// <param name="value">Current Column's value</param>
        public static bool SqlMin(this double value)
        {
            return true;
        }


        /// <summary>
        /// Gets The minimum value of the Current Column to compare it with
        /// other values
        /// </summary>
        /// <param name="value">Current Column's value</param>
        public static bool SqlMin(this long value)
        {
            return true;
        }

        /// <summary>
        /// Gets The maximum value of the Current Column to compare it with
        /// other values
        /// </summary>
        /// <param name="value">Current Column's value</param>
        public static bool SqlMax(this DateTime value)
        {
            return true;
        }

        /// <summary>
        /// Gets The maximum value of the Current Column to compare it with
        /// other values
        /// </summary>
        /// <param name="value">Current Column's value</param>
        public static bool SqlMax(this int value)
        {
            return true;
        }

        /// <summary>
        /// Gets The maximum value of the Current Column to compare it with
        /// other values
        /// </summary>
        /// <param name="value">Current Column's value</param>
        public static bool SqlMax(this short value)
        {
            return true;
        }

        /// <summary>
        /// Gets The maximum value of the Current Column to compare it with
        /// other values
        /// </summary>
        /// <param name="value">Current Column's value</param>
        public static bool SqlMax(this decimal value)
        {
            return true;
        }

        /// <summary>
        /// Gets The maximum value of the Current Column to compare it with
        /// other values
        /// </summary>
        /// <param name="value">Current Column's value</param>
        public static bool SqlMax(this double value)
        {
            return true;
        }

        /// <summary>
        /// Gets The maximum value of the Current Column to compare it with
        /// other values
        /// </summary>
        /// <param name="value">Current Column's value</param>
        public static bool SqlMax(this long value)
        {
            return true;
        }


        /// <summary>
        /// Adds a value to the Current Column's value, to compare the result
        /// with another value.
        /// </summary>
        /// <param name="value">CUrrent Column's Value</param>
        /// <param name="otherValue">Value to Add</param>
        public static int SqlPlus(this int value, int otherValue)
        {
            return value + otherValue;
        }

        /// <summary>
        /// Adds a value to the Current Column's value, to compare the result
        /// with another value.
        /// </summary>
        /// <param name="value">CUrrent Column's Value</param>
        /// <param name="otherValue">Value to Add</param>
        public static int SqlPlus(this short value, short otherValue)
        {
            return value + otherValue;
        }

        /// <summary>
        /// Adds a value to the Current Column's value, to compare the result
        /// with another value.
        /// </summary>
        /// <param name="value">CUrrent Column's Value</param>
        /// <param name="otherValue">Value to Add</param>
        public static decimal SqlPlus(this decimal value, decimal otherValue)
        {
            return value + otherValue;
        }

        /// <summary>
        /// Adds a value to the Current Column's value, to compare the result
        /// with another value.
        /// </summary>
        /// <param name="value">CUrrent Column's Value</param>
        /// <param name="otherValue">Value to Add</param>
        public static double SqlPlus(this double value, double otherValue)
        {
            return value + otherValue;
        }

        /// <summary>
        /// Adds a value to the Current Column's value, to compare the result
        /// with another value.
        /// </summary>
        /// <param name="value">CUrrent Column's Value</param>
        /// <param name="otherValue">Value to Add</param>
        public static long SqlPlus(this long value, long otherValue)
        {
            return value + otherValue;
        }

        /// <summary>
        /// Subtracts a value to the Current Column's value, to compare the result
        /// with another value.
        /// </summary>
        /// <param name="value">CUrrent Column's Value</param>
        /// <param name="otherValue">Value to Subtract</param>
        public static int SqlMinus(this int value, int otherValue)
        {
            return value - otherValue;
        }

        /// <summary>
        /// Subtracts a value to the Current Column's value, to compare the result
        /// with another value.
        /// </summary>
        /// <param name="value">CUrrent Column's Value</param>
        /// <param name="otherValue">Value to Subtract</param>
        public static int SqlMinus(this short value, short otherValue)
        {
            return value - otherValue;
        }

        /// <summary>
        /// Subtracts a value to the Current Column's value, to compare the result
        /// with another value.
        /// </summary>
        /// <param name="value">CUrrent Column's Value</param>
        /// <param name="otherValue">Value to Subtract</param>
        public static decimal SqlMinus(this decimal value, decimal otherValue)
        {
            return value - otherValue;
        }

        /// <summary>
        /// Subtracts a value to the Current Column's value, to compare the result
        /// with another value.
        /// </summary>
        /// <param name="value">CUrrent Column's Value</param>
        /// <param name="otherValue">Value to Subtract</param>
        public static double SqlMinus(this double value, double otherValue)
        {
            return value - otherValue;
        }

        /// <summary>
        /// Subtracts a value to the Current Column's value, to compare the result
        /// with another value.
        /// </summary>
        /// <param name="value">CUrrent Column's Value</param>
        /// <param name="otherValue">Value to Subtract</param>
        public static long SqlMinus(this long value, long otherValue)
        {
            return value - otherValue;
        }

        /// <summary>
        /// Gets the average value of the Current Column and allows to compare it
        /// with another value or Column.
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        public static int SqlAverage(this int value)
        {
            return value;
        }

        /// <summary>
        /// Gets the average value of the Current Column and allows to compare it
        /// with another value or Column.
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        public static short SqlAverage(this short value)
        {
            return value;
        }

        /// <summary>
        /// Gets the average value of the Current Column and allows to compare it
        /// with another value or Column.
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        public static decimal SqlAverage(this decimal value)
        {
            return value;
        }

        /// <summary>
        /// Gets the average value of the Current Column and allows to compare it
        /// with another value or Column.
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        public static double SqlAverage(this double value)
        {
            return value;
        }

        /// <summary>
        /// Gets the average value of the Current Column and allows to compare it
        /// with another value or Column.
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        public static long SqlAverage(this long value)
        {
            return value;
        }

        /// <summary>
        /// Gets the absolut value of the Current Column and allows to compare it
        /// with another value..
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        public static int SqlAbsolut(this int value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Gets the absolut value of the Current Column and allows to compare it
        /// with another value..
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        public static short SqlAbsolut(this short value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Gets the absolut value of the Current Column and allows to compare it
        /// with another value..
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        public static long SqlAbsolut(this long value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Gets the absolut value of the Current Column and allows to compare it
        /// with another value..
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        public static double SqlAbsolut(this double value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Gets the absolut value of the Current Column and allows to compare it
        /// with another value..
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        public static decimal SqlAbsolut(this decimal value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Gets the round value of the Current Numeric Column and allows to compare it
        /// with another value..
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        public static decimal SqlRound(this decimal value)
        {
            return Math.Round(value);
        }

        /// <summary>
        /// Gets the round value of the Current Numeric Column and allows to compare it
        /// with another value..
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        public static double SqlRound(this double value)
        {
            return Math.Round(value);
        }

        /// <summary>
        /// Gets the round value with (n) decimals of the Current Numeric Column and allows to compare it
        /// with another value..
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        /// <param name="decimalDigits">Number of decimals</param>
        public static decimal SqlRound(this decimal value, int decimalDigits)
        {
            return Math.Round(value, decimalDigits);
        }

        /// <summary>
        /// Gets the round value with (n) decimals of the Current Numeric Column and allows to compare it
        /// with another value..
        /// </summary>
        /// <param name="value">Average Value of the Column</param>
        /// <param name="decimalDigits">Number of decimals</param>
        public static double SqlRound(this double value, int decimalDigits)
        {
            return Math.Round(value, decimalDigits);
        }


        /// <summary>
        /// Uses the 'Like' operator of the sql and allows user
        /// to add all the supported arguments of this operator
        /// </summary>
        /// <param name="value">Column value</param>
        /// <param name="similarValue">Value that 'Like' operator will be applied to</param>
        public static bool SqlLike(this string value, string similarValue)
        {
            return value.Contains(similarValue);
        }

        /// <summary>
        /// Gets the length of a text value of a column for
        /// each line of the table , to compare it with a number.
        /// </summary>
        /// <param name="value">Value of the Column</param>
        public static int SqlLength(this string value)
        {
            return value.Length;
        }
    }
}
