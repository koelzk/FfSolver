// The following code has been translated from the original code written
// by Sebastiano Vigna (see https://prng.di.unimi.it/splitmix64.c).

namespace FfSolver;

/// <summary>
/// This is a fixed-increment version of Java 8's SplittableRandom generator
/// See http://dx.doi.org/10.1145/2714064.2660195 and
/// http://docs.oracle.com/javase/8/docs/api/java/util/SplittableRandom.html
///
/// It is a very fast generator passing BigCrush, and it can be useful if
/// for some reason you absolutely want 64 bits of state; otherwise, we
/// rather suggest to use a xoroshiro128+ (for moderately parallel
/// computations) or xorshift1024* (for massively parallel computations)
/// generator.
/// </summary>
public class SplitMix64
{
    private ulong state; /* The state can be seeded with any value. */

    public SplitMix64(ulong seed)
    {
        state = seed;
    }

    public ulong Next()
    {
        unchecked
        {
            ulong z = state += 0x9E3779B97F4A7C15ul;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9ul;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBul;
            return z ^ (z >> 31);
        }
    }
}
