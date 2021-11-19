using System.Collections;
using System.Collections.Generic;
using System;

namespace ES3Internal
{
    internal enum ES3SpecialByte : byte
    {
        Null        = 0,
        Bool        = 1,
        Byte        = 2,
        Sbyte       = 3,
        Char        = 4,
        Decimal     = 5,
        Double      = 6,
        Float       = 7,
        Int         = 8,
        Uint        = 9,
        Long        = 10,
        Ulong       = 11,
        Short       = 12,
        Ushort      = 13,
        String      = 14,
        ByteArray   = 15,
        Collection  = 128,
        Dictionary  = 129,
        CollectionItem = 130,
        Object      = 254,
        Terminator  = 255
    }

    internal static class ES3Binary
    {
        internal const string ObjectTerminator = ".";

        internal static readonly Dictionary<ES3SpecialByte, Type> IdToType = new Dictionary<ES3SpecialByte, Type>()
        {
            { ES3SpecialByte.Null,       null },
            { ES3SpecialByte.Bool,       typeof(bool)},
            { ES3SpecialByte.Byte,       typeof(byte)},
            { ES3SpecialByte.Sbyte,      typeof(sbyte)},
            { ES3SpecialByte.Char,       typeof(char)},
            { ES3SpecialByte.Decimal,    typeof(decimal)},
            { ES3SpecialByte.Double,     typeof(double)},
            { ES3SpecialByte.Float,      typeof(float)},
            { ES3SpecialByte.Int,        typeof(int)},
            { ES3SpecialByte.Uint,       typeof(uint)},
            { ES3SpecialByte.Long,       typeof(long)},
            { ES3SpecialByte.Ulong,      typeof(ulong)},
            { ES3SpecialByte.Short,      typeof(short)},
            { ES3SpecialByte.Ushort,     typeof(ushort)},
            { ES3SpecialByte.String,     typeof(string)},
            { ES3SpecialByte.ByteArray,  typeof(byte[])}
        };

        internal static readonly Dictionary<Type, ES3SpecialByte> TypeToId = new Dictionary<Type, ES3SpecialByte>()
        {
            { typeof(bool),     ES3SpecialByte.Bool},
            { typeof(byte),     ES3SpecialByte.Byte},
            { typeof(sbyte),    ES3SpecialByte.Sbyte},
            { typeof(char),     ES3SpecialByte.Char},
            { typeof(decimal),  ES3SpecialByte.Decimal},
            { typeof(double),   ES3SpecialByte.Double},
            { typeof(float),    ES3SpecialByte.Float},
            { typeof(int),      ES3SpecialByte.Int},
            { typeof(uint),     ES3SpecialByte.Uint},
            { typeof(long),     ES3SpecialByte.Long},
            { typeof(ulong),    ES3SpecialByte.Ulong},
            { typeof(short),    ES3SpecialByte.Short},
            { typeof(ushort),   ES3SpecialByte.Ushort},
            { typeof(string),   ES3SpecialByte.String},
            { typeof(byte[]),   ES3SpecialByte.ByteArray}
        };

        internal static ES3SpecialByte TypeToByte(Type type)
        {
            ES3SpecialByte b;
            if (TypeToId.TryGetValue(type, out b))
                return b;
            return ES3SpecialByte.Object;
        }

        internal static Type ByteToType(ES3SpecialByte b)
        {
            return ByteToType((byte)b);
        }

        internal static Type ByteToType(byte b)
        {
            Type type;
            if (IdToType.TryGetValue((ES3SpecialByte)b, out type))
                return type;
            return typeof(object);
        }

        internal static bool IsPrimitive(ES3SpecialByte b)
        {
            switch(b)
            {
                case ES3SpecialByte.Bool:
                case ES3SpecialByte.Byte:
                case ES3SpecialByte.Sbyte:
                case ES3SpecialByte.Char:
                case ES3SpecialByte.Decimal:
                case ES3SpecialByte.Double:
                case ES3SpecialByte.Float:
                case ES3SpecialByte.Int:
                case ES3SpecialByte.Uint:
                case ES3SpecialByte.Long:
                case ES3SpecialByte.Ulong:
                case ES3SpecialByte.Short:
                case ES3SpecialByte.Ushort:
                case ES3SpecialByte.String:
                case ES3SpecialByte.ByteArray:
                    return true;
                default:
                    return false;
            }
        }
    }
}
