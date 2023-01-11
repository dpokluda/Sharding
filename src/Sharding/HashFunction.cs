// -------------------------------------------------------------------------
// <copyright file="HashFunction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

namespace Sharding;

/// <summary>
///     An implemented to provide custom hash functions.
/// </summary>
public abstract class HashFunction
{
    /// <summary>
    ///     Hashes the specified value.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>
    ///     Hash value
    /// </returns>
    public abstract int ComputeHash(byte[] data);

    public static uint RotateLeft(uint original, int bits)
    {
        return (original << bits) | (original >> (32 - bits));
    }

    public static int RightMove(int value, int pos)
    {
        if (pos != 0)
        {
            var mask = 0x7fffffff;
            value >>= 1;
            value &= mask;
            value >>= pos - 1;
        }

        return value;
    }
    
    public static long RightMove(long value, int pos)
    {
        if (pos != 0)
        {
            var mask = 0x7fffffff;
            value >>= 1;
            value &= mask;
            value >>= pos - 1;
        }

        return value;
    }
}