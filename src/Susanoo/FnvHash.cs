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

        public static uint GetHash32(string value)
        {
            const uint fnvPrime = FnvPrime32;
            const uint fnvOffset = FnvOffset32;
            const uint fnvMod = FnvMod32;
            var hash = fnvOffset;

            for (int i = 0; i < value.Length; i++)
            {
                hash ^= (uint)value[i];
                hash %= fnvMod;
                hash *= fnvPrime;
            }

            return hash;
        }

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