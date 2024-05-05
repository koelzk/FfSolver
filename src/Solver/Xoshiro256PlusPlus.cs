// The following code has been translated from the original code written
// by David Blackman and Sebastiano Vigna (see http://xoshiro.di.unimi.it/xoshiro256plusplus.c).

namespace FfSolver;
using System.Runtime.CompilerServices;

/// <summary>
/// This is xoshiro256++ 1.0, one of our all-purpose, rock-solid generators.
/// It has excellent (sub-ns) speed, a state (256 bits) that is large
/// enough for any parallel application, and it passes all tests we are
/// aware of.
///
/// For generating just floating-point numbers, xoshiro256+ is even faster.
///
/// The state must be seeded so that it is not everywhere zero. If you have
/// a 64-bit seed, we suggest to seed a splitmix64 generator and use its
/// output to fill s.
/// </summary>
public class Xoshiro256PlusPlus
{
    private static readonly ulong[] JUMP = [0x180ec6d33cfd0aba, 0xd5a61266f0c9392c, 0xa9582618e03fc9aa, 0x39abdc4529b1661c];
    private static readonly ulong[] LONG_JUMP = [0x76e15d3efefdcbbf, 0xc5004e441c522fb3, 0x77710069854ee241, 0x39109bb02acbe635];

    private ulong[] state = new ulong[4];

    public Xoshiro256PlusPlus(ulong seed)
    {
        var rng = new SplitMix64(seed);
        for (var i = 0; i < 4; i++)
        {
            state[i] = rng.Next();
        }
    }

    public ulong Next()
    {
        ulong result = Rotl(state[0] + state[3], 23) + state[0];

        ulong t = state[1] << 17;

        state[2] ^= state[0];
        state[3] ^= state[1];
        state[1] ^= state[2];
        state[0] ^= state[3];

        state[2] ^= t;

        state[3] = Rotl(state[3], 45);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong Rotl(ulong x, int k)
    {
        unchecked
        {
            return (x << k) | (x >> (64 - k));
        }
    }

    /// <summary>
    /// This is the jump function for the generator. It is equivalent
    /// to 2^128 calls to next(); it can be used to generate 2^128
    /// non-overlapping subsequences for parallel computations.
    /// </summary>
    public void Jump()
    {
        ulong s0 = 0;
        ulong s1 = 0;
        ulong s2 = 0;
        ulong s3 = 0;
        for (int i = 0; i < JUMP.Length; i++)
            for (int b = 0; b < 64; b++)
            {
                if ((JUMP[i] & (1ul << b)) != 0)
                {
                    s0 ^= state[0];
                    s1 ^= state[1];
                    s2 ^= state[2];
                    s3 ^= state[3];
                }
                Next();
            }

        state[0] = s0;
        state[1] = s1;
        state[2] = s2;
        state[3] = s3;
    }



    /// <summary>
    /// This is the long-jump function for the generator. It is equivalent to
    /// 2^192 calls to next(); it can be used to generate 2^64 starting points,
    /// from each of which jump() will generate 2^64 non-overlapping
    /// subsequences for parallel distributed computations.
    /// </summary>
    public void LongJump()
    {
        ulong s0 = 0;
        ulong s1 = 0;
        ulong s2 = 0;
        ulong s3 = 0;
        for (int i = 0; i < LONG_JUMP.Length; i++)
            for (int b = 0; b < 64; b++)
            {
                if ((LONG_JUMP[i] & 1ul << b) != 0)
                {
                    s0 ^= state[0];
                    s1 ^= state[1];
                    s2 ^= state[2];
                    s3 ^= state[3];
                }
                Next();
            }

        state[0] = s0;
        state[1] = s1;
        state[2] = s2;
        state[3] = s3;
    }
}