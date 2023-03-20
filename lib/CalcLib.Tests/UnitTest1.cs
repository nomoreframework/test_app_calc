using CalcLib;
namespace CalcLib.Tests;
public class UnitTest1
{
    MyCalculator calculator = new MyCalculator();
    [Fact]
    public void CalculatorTest()
    {
        Assert.Equal(23,calculator.GetResult("5*6+(2-9)"));
        Assert.Equal(4,calculator.GetResult("2^"));
        Assert.Equal(2,calculator.GetResult("v4"));
        Assert.Equal(Math.Sqrt(2),calculator.GetResult("2v2"));
    }
    [Fact]
    public void CalculatorExceptionTest() => exceptionTest();
    private void exceptionTest()
    {
        Assert.Throws<ArithmeticException>(()=> calculator.GetResult("5*6/0"));
        Assert.Throws<CalculatorException>(()=> calculator.GetResult("5*6k0"));
        Assert.Throws<InvalidOperationException>(()=> calculator.GetResult("5*()"));
        Assert.Throws<CalculatorException>(()=> calculator.GetResult("()"));
    }
}