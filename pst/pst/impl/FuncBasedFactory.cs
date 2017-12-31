using pst.interfaces;
using System;

namespace pst.impl
{
    class FuncBasedFactory<TType> : IFactory<TType>
    {
        private readonly Func<TType> createType;

        public FuncBasedFactory(Func<TType> createType)
        {
            this.createType = createType;
        }

        public TType Create()
        {
            return createType();
        }
    }
}
