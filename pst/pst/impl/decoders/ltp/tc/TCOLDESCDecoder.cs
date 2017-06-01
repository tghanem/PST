using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.tc;

namespace pst.impl.decoders.ltp.tc
{
    class TCOLDESCDecoder : IDecoder<TCOLDESC>
    {
        public TCOLDESC Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new TCOLDESC(
                    parser.TakeAndSkip(4).ToInt32(),
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32());
        }
    }
}
