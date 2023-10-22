using System.Text.RegularExpressions;

namespace InsaneStrings;

    /**
     * InsaneStrings
     *
     * Implementation of numerical operations which may only use
     *  Finite enumerable indexing (e.g. accessing nth el of array)
     *  Strings: String concatenation and interpolation, constants
     *  RegEx (results may use multiple groups, matches, and indexes thereof as above)
     *  Cast to string for input / .ToString()
     *  Cast to integer types for return / Int32 Parse.
     *  Cast to boolean types for return
     * It may not use:
     *  Boolean logic (aside from above)
     *  Arithmetic operators
     *  Direct comparison (RegEx exempted)
     *  Numeric constants (dude, use string constants? - exempt from indexes)
     *
     * Assume that numeric types are locale invariant
     * Numerals are 0-9, decimal places are "." and negatives are "-" preceding.
     * No commas separate place values above 10^0.
     * Assume booleans cast to strings are either "True" or "False" (for true, false respectively)
     */
public static class Extensions {
    private static readonly Regex ABS_REGEX =
        new("-?(.*)", RegexOptions.Compiled);

    private static readonly Regex EQ_REGEX =
        new("^(.+) \\1$", RegexOptions.Compiled);

    private static readonly Regex OR_REGEX =
        new("True", RegexOptions.Compiled);

    private static readonly Regex NAND_REGEX =
        new("False", RegexOptions.Compiled);

    private static readonly Regex DECIMAL_REGEX =
        new(@"^(.*?)(?:(?:\.)(.*?))?$", RegexOptions.Compiled);
    
    // --- Boolean operations ---
    public static bool AndStr(this bool a, params bool[] other) {
        // AND(a,b) = NOT(NAND(a, b))
        return NAND_REGEX.IsMatch(a + String.Join("", other)).NotStr();
    }

    public static bool OrStr(this bool x, params bool[] other) {
        // Matches any true.
        return OR_REGEX.IsMatch(x + String.Join("", other));
    }

    public static bool NotStr(this bool x) {
        // NAND(x) = NOT(x)
        return NAND_REGEX.IsMatch($"{x}");
    }

    // --- Numeric operations ---

    public static T AbsStr<T>(this T x) where T : notnull {
        // Absolute value in numerics has the effect of omitting the negative sign
        // since negative sign <-> negative number <-> x < 0
        var m = ABS_REGEX.Match(x.ToString() ?? "");

        return (T)Convert.ChangeType(m.Groups[1].Value, typeof(T));
    }

    public static bool EqStr<T, S>(this T a, S b)
        where T : notnull
        where S : notnull {
        // Numerics, booleans, will never have spaces.
        // Concat and match that the string is repeated 
        return EQ_REGEX.IsMatch($"{a} {b}");
    }

    public static bool GeqZeroStr<T>(this T x) where T : notnull {
        // |x| >= 0 <-> |x| = x
        return x.EqStr(x.AbsStr());
    }

    public static bool GeZeroStr<T>(this T x) where T : notnull {
        // x > 0 <-> x >= 0 && x != 0
        return x.GeqZeroStr().AndStr(x.EqStr(0).NotStr());
    }

    public static T IfStr<T>(this bool x, T then, T @else) where T : notnull {
        // Restriction is output types mustn't differ - dynamic is such a dick type
        // to fuck with.
        // build a string with mutually exclusive substrings for which any predicate
        // will only match one.
        // Then build a predicate that does exactly that, matching half the substring
        // for the result value and return that.
        var str = $"(True:{then})(False:{@else})";
        var regex = new Regex($@"\({x}:(.*?)\)");

        return (T)Convert.ChangeType(regex.Match(str).Groups[1].Value,
            typeof(T));
    }

    public static int SignStr<T>(this T x) where T : notnull {
        // The world's worst implementation of x > 0 ? 1 : (x == 0 ? 0 : -1).
        return Int32.Parse(x.GeZeroStr()
            .IfStr("1", x.EqStr("0").IfStr("0", "-1")));
    }

    public static int TruncStr<T>(this T x) where T : notnull {
        // Everything before the decimal point is integer part
        // Truncation means that we round to zero
        return Int32.Parse(DECIMAL_REGEX.Match($"{x}").Groups[1].Value);
    }
}