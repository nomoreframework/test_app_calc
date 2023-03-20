using System.Text;

namespace CalcLib
{
    public class MyCalculator : ICalculator
    {
        private  struct Operator
        {
            internal readonly char op;
            internal readonly int priority;
            internal readonly bool unary = false;
            internal Operator(char op)
            {
                this.op = op;
                priority = get_priotity(ref unary);
            }
            int get_priotity(ref bool unary)
            {
                switch (op)
                {
                    case '+': return 1;
                    case '-': return 1;
                    case '*': return 2;
                    case '/': return 2;
                    case '^':
                        {
                            unary = true;
                            return 3;
                        }
                    case 'v':
                        {
                            unary = false; 
                            return 3;
                        }
                }
                return priority;
            }
        }
        /// <summary>
        /// Returns the string pattern in Polish (postfix) Lukasiewicz notation format. 5*6 -> 56*
        /// </summary>
        public Queue<String> GetPolishNotation(string pattern) => getPolishNotation(pattern);
        /// <summary>
        /// Returns the result, converting the string pattern to postfix notation before evaluating the result
        /// </summary>
        public double GetResult(string pattern) => getResult(GetPolishNotation(pattern));

        private Queue<string> getPolishNotation(string pattern)
        {
            var stack = new Stack<Operator>();
            var queue = new Queue<string>();
            for(int i = 0; i < pattern.Length; i++)
            {
                if (!isNumber(pattern[i]) && !isOperator(pattern[i]) && !isBracket(pattern[i])) 
                    throw new CalculatorException($"{"\n"}Operand or operator '{pattern[i]}' does not match the list of valid values{"\n"}");
                else if (isNumber(pattern[i]))
                {
                    queue.Enqueue(getNumber(pattern,ref i,false));
                }
                else if (isOperator(pattern[i]))
                {
                    var _operator = new Operator(pattern[i]);
                    if ((i == 0 && pattern[i] == '-') || (i > 0
                        && pattern[i] == '-' 
                        && !isUnary(pattern[i - 1]))
                        && !isNumber(pattern[i - 1]))
                    {
                        queue.Enqueue(getNumber(pattern, ref i, true));
                    }
                    else if (stack.Count == 0) stack.Push(_operator);
                    else if (stack.Count != 0)
                    {
                        if (stack.Peek().op == '(') stack.Push(_operator);
                        else if (_operator.priority > stack.Peek().priority) stack.Push(_operator);
                        else if (_operator.priority <= stack.Peek().priority)
                        {
                            while (stack.Count > 0 && (stack.Peek().priority >= _operator.priority || stack.Peek().op != '('))
                                queue.Enqueue(stack.Pop().op.ToString());
                            stack.Push(_operator);
                        }
                    }
                }
                else if(pattern[i] == '(') stack.Push(new Operator(pattern[i]));
                else if (pattern[i] == ')')
                {
                    while (stack.Count > 0 && stack.Peek().op != '(') queue.Enqueue(stack.Pop().op.ToString());
                    if(stack.Count > 0) stack.Pop();
                    else throw new CalculatorException($"Couldn't parse expression: {pattern}");
                }
            }
            while(stack.Count > 0) queue.Enqueue(stack.Pop().op.ToString());
            return queue;
        }
        double getResult(Queue<string> queue)
        {
            var stack = new Stack<double>();
            while(queue.Count > 0)
            {
                if (Double.TryParse(queue.Peek(), out double parse_result))
                {
                    stack.Push(parse_result); queue.Dequeue();
                }
                else
                {
                    if(stack.Count == 0) throw new CalculatorException("Operand was missed!"); 
                    switch (queue.Dequeue())
                    {
                        case "+":
                            stack.Push(stack.Pop() + stack.Pop());
                            break;
                        case "-":
                            stack.Push(stack.Peek() - stack.Pop() * 2 + stack.Pop());
                            break;
                        case "*":
                            stack.Push(stack.Pop() * stack.Pop());
                            break;
                        case "/":
                            {
                                var top = stack.Pop();
                                stack.Push(stack.Pop() / (top == 0 ? throw new ArithmeticException("Divide-by-zero error"): top));
                            }
                            break;
                        case "^":
                            stack.Push(Math.Pow(stack.Pop(), 2));
                            break;
                        case "v":
                            stack.Push(Math.Sqrt(stack.Pop()));
                            break;
                            default: throw new CalculatorException("Unknown operand");
                    }
                }
            }
            return stack.Count > 0 ? stack.Pop() : throw new CalculatorException("Invalid expression");
        }
        private char[] operators = new char[6] { '+', '-', '*', '/', '^', 'v' };
        public virtual char[] OPERATORS { get { return operators;}}
        bool isOperator(char op)
        {
            bool result = false;
            foreach(char c in OPERATORS) if(op == c) result = true;
            return result;
        }
        bool isNumber(char c) => (byte)c >= 48 && (byte)c <= 57;
        bool isBracket(char c) => c == ')' || c == '(';
        bool isUnary(char c) => c == '^' || c == 'v';
        bool isUnary(string c) => c == "^" || c == "v";
        string getNumber(string pattern, ref int counter,bool negative)
        {
            var operand = negative ? new StringBuilder('-') : new StringBuilder();
            do
            {
                operand.Append(pattern[counter]);
                counter++;
            }
            while (counter < pattern.Length && (isNumber(pattern[counter]) || pattern[counter] == '.'));
            counter--;
            return operand.ToString();
        }
    }
}
