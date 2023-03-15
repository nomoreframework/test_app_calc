using System.Collections.Generic;

namespace CalcLib
{
    public interface ICalculator
    {
        public Queue<String> GetPolishNotation(string pattern);
        public double GetResult(string pattern);
    }
}
