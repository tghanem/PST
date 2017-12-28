using pst.encodables.ndb;
using pst.interfaces;
using System;
using System.Collections.Generic;

namespace pst.utilities
{
    static class ExtensionMethods
    {
        public static Root SetFileEOF(this Root root, long value)
        {
            return
                new Root(
                    root.Reserved,
                    value,
                    root.AMapLast,
                    root.AMapFree,
                    root.PMapFree,
                    root.NBTRootPage,
                    root.NBTRootPage,
                    root.AMapValid,
                    root.BReserved,
                    root.WReserved);
        }

        public static Root SetLastAMapOffset(this Root root, long value)
        {
            return
                new Root(
                    root.Reserved,
                    root.FileEOF,
                    value,
                    root.AMapFree,
                    root.PMapFree,
                    root.NBTRootPage,
                    root.NBTRootPage,
                    root.AMapValid,
                    root.BReserved,
                    root.WReserved);
        }

        public static Root SetFreeSpaceInAllAMaps(this Root root, long value)
        {
            return
                new Root(
                    root.Reserved,
                    root.FileEOF,
                    root.AMapLast,
                    value,
                    root.PMapFree,
                    root.NBTRootPage,
                    root.NBTRootPage,
                    root.AMapValid,
                    root.BReserved,
                    root.WReserved);
        }

        public static Header SetRoot(this Header header, Root value)
        {
            return
                new Header(
                    header.Magic,
                    header.CRCPartial,
                    header.MagicClient,
                    header.Version,
                    header.ClientVersion,
                    header.PlatformCreate,
                    header.PlatformAccess,
                    header.Reserved1,
                    header.Reserved2,
                    header.UnusedPadding,
                    header.NextPageBID,
                    header.Unique,
                    header.NIDs,
                    header.Unused,
                    value,
                    header.AlignmentBytes,
                    header.FMap,
                    header.FPMap,
                    header.Sentinel,
                    header.CryptMethod,
                    header.Reserved,
                    header.NextBID,
                    header.CRCFull,
                    header.RGBReserved2,
                    header.BReserved,
                    header.RGBReserved3);
        }

        public static T[] DecodeMultipleItems<T>(this IDecoder<T> decoder, int numberOfItems, int itemSize, BinaryData data)
        {
            var entries = new List<T>();

            for (var i = 0; i < numberOfItems; i++)
            {
                var item = data.Take(i * itemSize, itemSize);

                entries.Add(decoder.Decode(item));
            }

            return entries.ToArray();
        }

        public static int GetRemainingToNextMultipleOf(this int number, int multipleOf)
        {
            if (number % multipleOf == 0)
                return 0;

            if (number < multipleOf)
                return multipleOf - number;

            var nextMultipleOf =
                (int)
                Math.Ceiling((double)number / multipleOf) * multipleOf;

            return nextMultipleOf - number;
        }
    }
}
