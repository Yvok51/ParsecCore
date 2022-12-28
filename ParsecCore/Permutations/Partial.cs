﻿using System;

namespace ParsecCore.Permutations
{
    internal static partial class PartialExt
    {
        internal static Func<TA, TR> Partial<TA, TB, TR>(this Func<TA, TB, TR> f, TB b) => (TA a) => f(a, b);
        internal static Func<TB, TR> Partial<TA, TB, TR>(this Func<TA, TB, TR> f, TA a) => (TB b) => f(a, b);

        internal static Func<TA, TB, TR> Partial<TA, TB, TC, TR>(this Func<TA, TB, TC, TR> f, TC c) => (a, b) => f(a, b, c);
        internal static Func<TA, TC, TR> Partial<TA, TB, TC, TR>(this Func<TA, TB, TC, TR> f, TB b) => (a, c) => f(a, b, c);
        internal static Func<TB, TC, TR> Partial<TA, TB, TC, TR>(this Func<TA, TB, TC, TR> f, TA a) => (b, c) => f(a, b, c);

        internal static Func<TA, TB, TC, TR> Partial<TA, TB, TC, TD, TR>(this Func<TA, TB, TC, TD, TR> f, TD d) => (a, b, c) => f(a, b, c, d);
        internal static Func<TA, TB, TD, TR> Partial<TA, TB, TC, TD, TR>(this Func<TA, TB, TC, TD, TR> f, TC c) => (a, b, d) => f(a, b, c, d);
        internal static Func<TA, TC, TD, TR> Partial<TA, TB, TC, TD, TR>(this Func<TA, TB, TC, TD, TR> f, TB b) => (a, c, d) => f(a, b, c, d);
        internal static Func<TB, TC, TD, TR> Partial<TA, TB, TC, TD, TR>(this Func<TA, TB, TC, TD, TR> f, TA a) => (b, c, d) => f(a, b, c, d);

    }
}