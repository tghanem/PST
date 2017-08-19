using pst.interfaces;
using System;

namespace pst.impl
{
    class FuncBasedExtractor<TInput, TValue> : IExtractor<TInput, TValue>
    {
        private readonly Func<TInput, TValue> extract;

        public FuncBasedExtractor(Func<TInput, TValue> extract)
        {
            this.extract = extract;
        }

        public TValue Extract(TInput parameter)
        {
            return extract(parameter);
        }
    }
}
