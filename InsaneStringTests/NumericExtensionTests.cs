namespace InsaneStringTests;

using InsaneStrings;

public class NumericExtensionTests {
    private static object[] _andCases = {
        new object[] { false, false, false },
        new object[] { false, false, true },
        new object[] { false, true, false },
        new object[] { true, true, true },
    };

    [Test, TestCaseSource(nameof(_andCases))]
    public void AndStrIsValidAndOperator(bool expectedAnd, bool a, bool b) {
        Assert.That(a.AndStr(b), Is.EqualTo(expectedAnd));
    }

    private static object[] _orCases = {
        new object[] { false, false, false },
        new object[] { true, false, true },
        new object[] { true, true, false },
        new object[] { true, true, true },
    };

    [Test, TestCaseSource(nameof(_orCases))]
    public void OrStrIsValidOrOperator(bool expectedOr, bool a, bool b) {
        Assert.That(a.OrStr(b), Is.EqualTo(expectedOr));
    }

    private static object[] _notCases = {
        new object[] { true, false },
        new object[] { false, true }
    };

    [Test, TestCaseSource(nameof(_notCases))]
    public void NotStrIsValidNotOperator(bool expectedNot, bool a) {
        Assert.That(a.NotStr(), Is.EqualTo(expectedNot));
    }

    private static object[] _absDoubleCases = {
        new object[] { 5.0d, 5.0d },
        new object[] { 0.0d, 0.0d },
        new object[] { 5.0d, -5.0d },
        new object[] { 5.17d, 5.17d }, // preserves decimals
    };

    [Test, TestCaseSource(nameof(_absDoubleCases))]
    public void
        AbsStrProducesAbsoluteValue(double expectedValue, double value) {
        Assert.That(value.AbsStr(), Is.EqualTo(expectedValue));
    }
}