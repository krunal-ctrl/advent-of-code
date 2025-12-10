using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adventofcode.Utilities;

// Using a static class for constants and pure helper methods.
public static class Util {
    /// <summary>Calculates the population count (Hamming weight) of an integer.</summary>
    public static int BitCount(int n) {
        // Use intrinsic/hardware support if available (e.g., in a performance-critical environment)
        // For general C#, this is a standard fast implementation:
        int res = 0;
        while (n != 0) {
            n &= (n - 1);
            res++;
        }
        return res;
    }

    /// <summary>Calculates the result of XORing a subset of buttons based on a bitmask.</summary>
    public static int Xor(int[] buttons, int mask) {
        int res = 0;
        for (int i = 0; i < buttons.Length; i++) {
            if ((mask & (1 << i)) != 0) {
                res ^= buttons[i];
            }
        }
        return res;
    }
}
