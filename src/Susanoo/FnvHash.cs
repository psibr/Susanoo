// ***********************************************************************
// Authors          : Jasmin Muharemovic
// Created          : 07-17-2014
//
// Last Modified By : Donovan Crone
// Last Modified On : 07-17-2014
// Original Implementation By: Jasmin Muharemovic
// Adjusted to C# 4.0 BigInteger by Donovan Crone
// ***********************************************************************
// ***********************************************************************

#region

using System;
using System.Globalization;
using System.Numerics;

#endregion

/// <summary>
///     Implementation of the FNV variable bit hash alogrithm.
/// </summary>
internal static class FnvHash
{
    #region Constants

    private static readonly BigInteger fnv_prime_32 =
        new BigInteger(16777619);

    private static readonly BigInteger fnv_prime_64 =
        new BigInteger(1099511628211);

    private static readonly BigInteger fnv_prime_128 =
        BigInteger.Parse("309485009821345068724781371");

    private static readonly BigInteger fnv_prime_256 =
        BigInteger.Parse("374144419156711147060143317175368453031918731002211");

    private static readonly BigInteger fnv_prime_512 =
        BigInteger.Parse("3583591587484486736891907648909510844994632795575439"
                         + "2558399825615420669938882575126094039892345713852759");

    private static readonly BigInteger fnv_prime_1024 =
        BigInteger.Parse("5016456510113118655434598811035278955030765345404790"
                         + "74430301752383111205510814745150915769222029538271616265187852689"
                         + "52493852922918165243750837466913718040942718731604847379667202603"
                         + "89217684476157468082573");

    private static readonly BigInteger fnv_offset_32 =
        new BigInteger(2166136261);

    private static readonly BigInteger fnv_offset_64 =
        new BigInteger(14695981039346656037);

    private static readonly BigInteger fnv_offset_128 =
        BigInteger.Parse("275519064689413815358837431229664493455");

    private static readonly BigInteger fnv_offset_256 =
        BigInteger.Parse("10002925795805258090707096862062570483709279601424119"
                         + "3945225284501741471925557");

    private static readonly BigInteger fnv_offset_512 =
        BigInteger.Parse("96593031294966694980094354007163104660904187456726378"
                         + "961083743294344626579945829321977164384498130518922065398057844953"
                         + "28239340083876191928701583869517785");

    private static readonly BigInteger fnv_offset_1024 =
        BigInteger.Parse("14197795064947621068722070641403218320880622795441933"
                         + "960878474914617582723252296732303717722150864096521202355549365628"
                         + "174669108571814760471015076148029755969804077320157692458563003215"
                         + "304957150157403644460363550505412711285966361610267868082893823963"
                         + "790439336411086884584107735010676915");

    private static readonly BigInteger fnv_mod_32 =
        BigInteger.Pow(new BigInteger(2), 32);

    private static readonly BigInteger fnv_mod_64 =
        BigInteger.Pow(new BigInteger(2), 64);

    private static readonly BigInteger fnv_mod_128 =
        BigInteger.Pow(new BigInteger(2), 128);

    private static readonly BigInteger fnv_mod_256 =
        BigInteger.Pow(new BigInteger(2), 256);

    private static readonly BigInteger fnv_mod_512 =
        BigInteger.Pow(new BigInteger(2), 512);

    private static readonly BigInteger fnv_mod_1024 =
        BigInteger.Pow(new BigInteger(2), 1024);

    #endregion Constants

    /// <summary>
    ///     Gets the hash.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="hashBitSize">Size of the hash bit.</param>
    /// <returns>BigInteger.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">hashBitSize</exception>
    public static BigInteger GetHash(string value, int hashBitSize)
    {
        BigInteger fnv_prime = default(BigInteger);
        BigInteger fnv_offset = default(BigInteger);
        BigInteger fnv_mod = default(BigInteger);
        if (hashBitSize <= 32)
        {
            fnv_prime = fnv_prime_32;
            fnv_offset = fnv_offset_32;
            fnv_mod = fnv_mod_32;
        }
        else if (hashBitSize <= 64)
        {
            fnv_prime = fnv_prime_64;
            fnv_offset = fnv_offset_64;
            fnv_mod = fnv_mod_64;
        }
        else if (hashBitSize <= 128)
        {
            fnv_prime = fnv_prime_128;
            fnv_offset = fnv_offset_128;
            fnv_mod = fnv_mod_128;
        }
        else if (hashBitSize <= 256)
        {
            fnv_prime = fnv_prime_256;
            fnv_offset = fnv_offset_256;
            fnv_mod = fnv_mod_256;
        }
        else if (hashBitSize <= 512)
        {
            fnv_prime = fnv_prime_512;
            fnv_offset = fnv_offset_512;
            fnv_mod = fnv_mod_512;
        }
        else if (hashBitSize <= 1024)
        {
            fnv_prime = fnv_prime_1024;
            fnv_offset = fnv_offset_1024;
            fnv_mod = fnv_mod_1024;
        }
        else
        {
            throw new ArgumentOutOfRangeException("hashBitSize");
        }
        BigInteger hash = fnv_offset;
        for (int i = 0; i < value.Length; i++)
        {
            hash ^= (uint) value[i];
            hash %= fnv_mod;
            hash *= fnv_prime;
        }
        if (!IsPowerOfTwo(hashBitSize))
        {
            BigInteger mask = BigInteger.Parse(new string('f', (hashBitSize/4) + (hashBitSize%4 != 0 ? 1 : 0)),
                NumberStyles.HexNumber);
            hash = (hash >> hashBitSize) ^ (mask & hash);
        }

        return hash;
    }

    private static bool IsPowerOfTwo(int number)
    {
        return (number & (number - 1)) == 0;
    }
}