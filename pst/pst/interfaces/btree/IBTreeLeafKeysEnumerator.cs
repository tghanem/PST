using pst.interfaces.io;
using System.Collections.Generic;

namespace pst.interfaces.btree
{
    interface IBTreeLeafKeysEnumerator<TKey, TNodeReference>
        where TNodeReference : class
        where TKey : class
    {
        TKey[] Enumerate(IDataBlockReader<TNodeReference> reader, TNodeReference rootNodeReference);
    }

    interface IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<TLeafKey, TIntermediateKey, TNodeReference>
        where TIntermediateKey : class
        where TNodeReference : class
        where TLeafKey : class
    {
        TLeafKey[] Enumerate(IDataBlockReader<TNodeReference> reader, IMapper<TIntermediateKey, TNodeReference> keyToNodeReferenceMapping, TNodeReference rootNodeReference);
    }
}
