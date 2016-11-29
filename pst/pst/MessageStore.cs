using pst.encodables.ndb;
using pst.impl.decoders.ndb;
using pst.impl.decoders.primitives;
using pst.impl.io;
using pst.impl.ltp.pc;
using pst.impl.ndb.bbt;
using pst.impl.ndb.nbt;
using System.IO;

namespace pst
{
    public class MessageStore
    {
        private readonly PropertyContext propertyContext;

        internal MessageStore(PropertyContext propertyContext)
        {
            this.propertyContext = propertyContext;
        }
    }
}
