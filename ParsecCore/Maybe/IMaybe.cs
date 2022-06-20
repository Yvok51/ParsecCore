using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsecCore.Maybe
{
    interface IMaybe<T>
    {
        bool IsEmpty { get; }

        T Value { get; }

        IMaybe<TNew> Bind<TNew>(Func<T, TNew> map);

        TNew Match<TNew>(Func<T, TNew> just, Func<TNew> nothing);
    }
}
