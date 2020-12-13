using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BitUtils
{
    public static bool GetShortBit (byte value, byte bitIdx) {
        return (value & (1 << bitIdx)) != 0;
    }


    public static bool GetULongBit (ulong value, int bitIdx) {
        return (value & (1ul << bitIdx)) != 0;
    }
}
