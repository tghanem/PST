using pst.interfaces.ltp.hn;
using pst.utilities;

namespace pst.interfaces.ltp
{
    interface IPropertyValueLoader
    {
        PropertyValue Load(
            PropertyType type,
            BinaryData encodedValue,
            HeapOnNode heapOnNode);
    }
}
