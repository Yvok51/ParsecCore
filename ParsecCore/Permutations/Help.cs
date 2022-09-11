using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsecCore.Permutations
{
    static class Help
    {
        public static Func<TA, TC> Compose<TA, TB, TC>(this Func<TB, TC> leftFunc, Func<TA, TB> rightFunc)
        {
            return (a) => leftFunc(rightFunc(a));
        }

        /// <summary>
        /// Creates a partial applied function composition where the "left" function -
        /// the one that is applied second - is already supplied
        /// </summary>
        /// <typeparam name="TA"> The starting type </typeparam>
        /// <typeparam name="TB"> The intermediary type </typeparam>
        /// <typeparam name="TC"> The final type </typeparam>
        /// <param name="leftFunc"> The left function that is partially applied to function composition </param>
        /// <returns> Partially applied function composition </returns>
        public static Func<Func<TA, TB>, Func<TA, TC>> PartialLeftCompose<TA, TB, TC>(this Func<TB, TC> leftFunc)
        {
            return (rightFunc) => { return (a) => leftFunc(rightFunc(a)); };
        }

        /// <summary>
        /// Creates a partial applied function composition where the "right" function -
        /// the one that is applied first - is already supplied
        /// </summary>
        /// <typeparam name="TA"> The starting type </typeparam>
        /// <typeparam name="TB"> The intermediary type </typeparam>
        /// <typeparam name="TC"> The final type </typeparam>
        /// <param name="rightFunc"> The right function that is partially applied to function composition </param>
        /// <returns> Partially applied function composition </returns>
        public static Func<Func<TB, TC>, Func<TA, TC>> PartialRightCompose<TA, TB, TC>(this Func<TA, TB> rightFunc)
        {
            return (leftFunc) => { return (a) => leftFunc(rightFunc(a)); };
        }
    }
}
