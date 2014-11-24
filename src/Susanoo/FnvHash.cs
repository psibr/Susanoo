// ***********************************************************************
// Authors          : Jasmin Muharemovic
// Created          : 07-17-2014
//
// Last Modified By : Donovan Crone
// Last Modified On : 09-03-2014
// Original Implementation By: Jasmin Muharemovic
// Adjusted to C# 4.0 BigInteger by Donovan Crone
// ***********************************************************************
// ***********************************************************************

#region

using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using Susanoo.Annotations;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     Implementation of the FNV variable bit hash algorithm.
    /// </summary>
    internal static class FnvHash
    {
        #region Constants

        private const uint FnvPrime32 = 16777619;

        private const ulong FnvPrime64 = 1099511628211;

        private const uint FnvOffset32 = 2166136261;

        private const ulong FnvOffset64 = 14695981039346656037;

        private const uint FnvMod32 = 2 ^ 32;

        private const ulong FnvMod64 = 2 ^ 64;

        #endregion Constants

        //public static uint GetHash32(string value)
        //{
        //    const uint fnvPrime = FnvPrime32;
        //    const uint fnvOffset = FnvOffset32;
        //    const uint fnvMod = FnvMod32;
        //    var hash = fnvOffset;

        //    for (int i = 0; i < value.Length; i++)
        //    {
        //        hash ^= (uint)value[i];
        //        hash %= fnvMod;
        //        hash *= fnvPrime;
        //    }

        //    return hash;
        //}

        public static ulong GetHash64(string value)
        {
            const ulong fnvPrime = FnvPrime64;
            const ulong fnvOffset = FnvOffset64;
            const ulong fnvMod = FnvMod64;
            var hash = fnvOffset;

            for (int i = 0; i < value.Length; i++)
            {
                hash ^= (uint)value[i];
                hash %= fnvMod;
                hash *= fnvPrime;
            }

            return hash;
        }
    }
}

internal static class HashBuilder
{
    public static BigInteger Compute(string value)
    {
        return new BigInteger(new Murmur3().ComputeHash(Encoding.UTF8.GetBytes(value)));
    }

    public static BigInteger Seed {
        get { return Murmur3.Seed; }
    }
}

internal static class IntHelpers
{
    public static ulong RotateLeft(this ulong original, int bits)
    {
        return (original << bits) | (original >> (64 - bits));
    }
    public static ulong RotateRight(this ulong original, int bits)
    {
        return (original >> bits) | (original << (64 - bits));
    }
    unsafe public static ulong GetUInt64(this byte[] bb, int pos)
    {
        // we only read aligned longs, so a simple casting is enough
        fixed (byte* pbyte = &bb[pos])
        {
            return *((ulong*)pbyte);
        }
    }
}

internal class Murmur3 {

    // 128 bit output, 64 bit platform version
    public const ulong READ_SIZE = 16;
    private const ulong C1 = 0x87c37b91114253d5L;
    private const ulong C2 = 0x4cf5ad432745937fL;
    private ulong _length;
    [UsedImplicitly] internal static uint Seed = 866398230;
    // if want to start with a seed, create a constructor
    ulong h1;
    ulong h2;
    private void MixBody(ulong k1, ulong k2)
    {
        h1 ^= MixKey1(k1);
        h1 = h1.RotateLeft(27);
        h1 += h2;
        h1 = h1 * 5 + 0x52dce729;
        h2 ^= MixKey2(k2);
        h2 = h2.RotateLeft(31);
        h2 += h1;
        h2 = h2 * 5 + 0x38495ab5;
    }
    private static ulong MixKey1(ulong k1)
    {
        k1 *= C1;
        k1 = k1.RotateLeft(31);
        k1 *= C2;
        return k1;
    }
    private static ulong MixKey2(ulong k2)
    {
        k2 *= C2;
        k2 = k2.RotateLeft(33);
        k2 *= C1;
        return k2;
    }
    private static ulong MixFinal(ulong k)
    {
        // avalanche bits
        k ^= k >> 33;
        k *= 0xff51afd7ed558ccdL;
        k ^= k >> 33;
        k *= 0xc4ceb9fe1a85ec53L;
        k ^= k >> 33;
        return k;
    }
    public byte[] ComputeHash(byte[] bb)
    {
        ProcessBytes(bb);
        return Hash;
    }
    private void ProcessBytes(byte[] bb)
    {
        h1 = Seed;
        this._length = 0L;
        int pos = 0;
        ulong remaining = (ulong)bb.Length;
        // read 128 bits, 16 bytes, 2 longs in eacy cycle
        while (remaining >= READ_SIZE)
        {
            ulong k1 = bb.GetUInt64(pos);
            pos += 8;
            ulong k2 = bb.GetUInt64(pos);
            pos += 8;
            _length += READ_SIZE;
            remaining -= READ_SIZE;
            MixBody(k1, k2);
        }
        // if the input MOD 16 != 0
        if (remaining > 0)
            ProcessBytesRemaining(bb, remaining, pos);
    }
    private void ProcessBytesRemaining(byte[] bb, ulong remaining, int pos)
    {
        ulong k1 = 0;
        ulong k2 = 0;
        _length += remaining;
        // little endian (x86) processing
        switch (remaining)
        {
            case 15:
                k2 ^= (ulong)bb[pos + 14] << 48; // fall through
                goto case 14;
            case 14:
                k2 ^= (ulong)bb[pos + 13] << 40; // fall through
                goto case 13;
            case 13:
                k2 ^= (ulong)bb[pos + 12] << 32; // fall through
                goto case 12;
            case 12:
                k2 ^= (ulong)bb[pos + 11] << 24; // fall through
                goto case 11;
            case 11:
                k2 ^= (ulong)bb[pos + 10] << 16; // fall through
                goto case 10;
            case 10:
                k2 ^= (ulong)bb[pos + 9] << 8; // fall through
                goto case 9;
            case 9:
                k2 ^= (ulong)bb[pos + 8]; // fall through
                goto case 8;
            case 8:
                k1 ^= bb.GetUInt64(pos);
                break;
            case 7:
                k1 ^= (ulong)bb[pos + 6] << 48; // fall through
                goto case 6;
            case 6:
                k1 ^= (ulong)bb[pos + 5] << 40; // fall through
                goto case 5;
            case 5:
                k1 ^= (ulong)bb[pos + 4] << 32; // fall through
                goto case 4;
            case 4:
                k1 ^= (ulong)bb[pos + 3] << 24; // fall through
                goto case 3;
            case 3:
                k1 ^= (ulong)bb[pos + 2] << 16; // fall through
                goto case 2;
            case 2:
                k1 ^= (ulong)bb[pos + 1] << 8; // fall through
                goto case 1;
            case 1:
                k1 ^= (ulong)bb[pos]; // fall through
                break;
            default:
                throw new Exception("Something went wrong with remaining bytes calculation.");
        }
        h1 ^= MixKey1(k1);
        h2 ^= MixKey2(k2);
    }

    private byte[] Hash    
    {
        get
        {
            h1 ^= _length;
            h2 ^= _length;
            h1 += h2;
            h2 += h1;
            h1 = Murmur3.MixFinal(h1);
            h2 = Murmur3.MixFinal(h2);
            h1 += h2;
            h2 += h1; 
            var hash = new byte[Murmur3.READ_SIZE];
            Array.Copy(BitConverter.GetBytes(h1), 0, hash, 0, 8);
            Array.Copy(BitConverter.GetBytes(h2), 0, hash, 8, 8);
            return hash;
        }
    }
}
