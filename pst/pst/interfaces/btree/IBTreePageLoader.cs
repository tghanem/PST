using pst.encodables;

namespace pst.interfaces
{
    interface IBTreePageLoader
    {
        BTPage LoadPage(BREF pageBlockReference);
    }
}