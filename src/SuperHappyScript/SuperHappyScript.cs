using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperHappyScript
{
    public class SuperHappyScript
    {
        abstract class ValueNode
        {
            public ValueNode Parent;
            public int PredenceLevel { get; set; }
            public abstract ValueNode AttachNode(ValueNode currentNode);
            public abstract void UpdateNode(ValueNode node);
            public abstract double GetValue(Dictionary<string, double> variableValues);
        }

        class Number : ValueNode
        {
            double Value;

            public Number(double value)
            {
                this.PredenceLevel = int.MaxValue;
                this.Value = value;
            }

            public override ValueNode AttachNode(ValueNode currentNode)
            {
                if (currentNode != null)
                {
                    currentNode.UpdateNode(this);
                    this.Parent = currentNode;
                }

                return this;
            }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Value;
            }

            public override void UpdateNode(ValueNode node)
            {
                throw new Exception("Parsing error");
            }
        }

        class Variable : ValueNode
        {
            string Name;

            public Variable(string name)
            {
                this.PredenceLevel = int.MaxValue;
                this.Name = name;
            }

            public override ValueNode AttachNode(ValueNode currentNode)
            {
                if (currentNode != null)
                {
                    currentNode.UpdateNode(this);
                    this.Parent = currentNode;
                }

                return this;
            }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return variableValues[Name];
            }

            public override void UpdateNode(ValueNode node)
            {
                throw new Exception("Parsing error");
            }
        }

        abstract class Operand : ValueNode
        {
            protected ValueNode Left;
            protected ValueNode Right;

            public Operand(int predenceLevel)
            {
                this.PredenceLevel = predenceLevel;
            }

            public override ValueNode AttachNode(ValueNode currentNode)
            {
                if (Left == null)
                {
                    ValueNode tempNode = currentNode;

                    while (tempNode.Parent != null && tempNode.Parent.PredenceLevel >= this.PredenceLevel)
                    {
                        tempNode = tempNode.Parent;
                    }

                    Left = tempNode;
                    var tempNodeParent = tempNode.Parent;
                    this.Parent = tempNodeParent;

                    if (tempNodeParent != null)
                        tempNodeParent.UpdateNode(this);

                    return this;

                }
                else
                {
                    throw new Exception("Parse exception");
                }
            }

            public override void UpdateNode(ValueNode node)
            {
                Right = node;
            }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                throw new NotImplementedException();
            }
        }

        abstract class UnaryOperand : ValueNode
        {
            protected ValueNode Right;

            public UnaryOperand(int predenceLevel)
            {
                this.PredenceLevel = predenceLevel;
            }

            public override ValueNode AttachNode(ValueNode node)
            {
                if (node != null)
                {
                    node.UpdateNode(this);
                    this.Parent = node;
                }

                return this;
            }

            public override void UpdateNode(ValueNode node)
            {
                Right = node;
            }
        }

        abstract class Function : ValueNode
        {
            protected List<ValueNode> Arguments;
            protected int ArgumentCount = 0;

            public Function(int predecenLevel, int argumentCount = 0)
            {
                this.PredenceLevel = predecenLevel;
                this.Arguments = new List<ValueNode>();
                this.ArgumentCount = argumentCount;
            }

            public override ValueNode AttachNode(ValueNode node)
            {
                if (node != null)
                {
                    node.UpdateNode(this);
                    this.Parent = node;
                }

                return this;
            }

            public void AddArgument(ValueNode node, bool last)
            {
                Arguments.Add(node);

                if (Arguments.Count > ArgumentCount && ArgumentCount != 0)
                {
                    throw new Exception("Too many arguments");
                }
            }

            public override void UpdateNode(ValueNode node)
            {
                //throw new Exception("Parsing error");
            }
        }

        class Equal : Operand
        {
            public Equal(int predenceLevel) : base(predenceLevel + 1)
            {
            }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Left.GetValue(variableValues) == Right.GetValue(variableValues) ? 1 : 0;
            }
        }

        class NotEqual : Operand
        {
            public NotEqual(int predenceLevel) : base(predenceLevel + 1)
            {
            }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Left.GetValue(variableValues) != Right.GetValue(variableValues) ? 1 : 0;
            }
        }

        class GreaterThan : Operand
        {
            public GreaterThan(int predenceLevel) : base(predenceLevel + 1)
            {
            }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Left.GetValue(variableValues) > Right.GetValue(variableValues) ? 1 : 0;
            }
        }

        class LessThan : Operand
        {
            public LessThan(int predenceLevel) : base(predenceLevel + 1)
            {
            }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Left.GetValue(variableValues) < Right.GetValue(variableValues) ? 1 : 0;
            }
        }

        class EqualOrGreaterThan : Operand
        {
            public EqualOrGreaterThan(int predenceLevel) : base(predenceLevel + 1)
            {
            }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Left.GetValue(variableValues) >= Right.GetValue(variableValues) ? 1 : 0;
            }
        }

        class EqualOrLessThan : Operand
        {
            public EqualOrLessThan(int predenceLevel) : base(predenceLevel + 1)
            {
            }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Left.GetValue(variableValues) <= Right.GetValue(variableValues) ? 1 : 0;
            }
        }

        class Add : Operand
        {
            public Add(int predenceLevel) : base(predenceLevel + 2) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Left.GetValue(variableValues) + Right.GetValue(variableValues);
            }
        }

        class Subtract : Operand
        {
            public Subtract(int predenceLevel) : base(predenceLevel + 2) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Left.GetValue(variableValues) - Right.GetValue(variableValues);
            }
        }

        class Multiply : Operand
        {
            public Multiply(int predenceLevel) : base(predenceLevel + 3) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Left.GetValue(variableValues) * Right.GetValue(variableValues);
            }
        }

        class Divide : Operand
        {
            public Divide(int predenceLevel) : base(predenceLevel + 3) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Left.GetValue(variableValues) / Right.GetValue(variableValues);
            }
        }

        class PowerOf : Operand
        {
            public PowerOf(int predenceLevel) : base(predenceLevel + 5) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                var temp1 = Left.GetValue(variableValues);
                var temp2 = Right.GetValue(variableValues);

                return Math.Pow(Left.GetValue(variableValues), Right.GetValue(variableValues));
            }
        }

        class Negate : UnaryOperand
        {
            public Negate(int predenceLevel) : base(predenceLevel + 4) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return -Right.GetValue(variableValues);
            }
        }

        class Absolute : Function
        {
            public Absolute(int predecenLevel) : base(predecenLevel, 1) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Math.Abs(Arguments[0].GetValue(variableValues));
            }
        }

        class RoundDown : Function
        {
            public RoundDown(int predecenLevel) : base(predecenLevel, 1) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Math.Floor(Arguments[0].GetValue(variableValues));
            }
        }

        class IsNaN : Function
        {
            public IsNaN(int predecenLevel) : base(predecenLevel, 1) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return double.IsNaN(Arguments[0].GetValue(variableValues)) ? 1 : 0;
            }
        }

        class Round : Function
        {
            public Round(int predecenLevel) : base(predecenLevel, 1) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Math.Round(Arguments[0].GetValue(variableValues));
            }
        }

        class IfStatement : Function
        {
            public IfStatement(int predecenLevel) : base(predecenLevel, 3) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                var val = Arguments[0].GetValue(variableValues);

                if (val > 0)
                {
                    return Arguments[1].GetValue(variableValues);
                }
                else
                {
                    return Arguments[2].GetValue(variableValues);
                }
            }
        }

        class Max : Function
        {
            public Max(int predecenLevel) : base(predecenLevel, 0) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Arguments.Max(p => p.GetValue(variableValues));
            }
        }

        class Min : Function
        {
            public Min(int predecenLevel) : base(predecenLevel, 0) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Arguments.Min(p => p.GetValue(variableValues));
            }
        }

        class And : Operand
        {
            public And(int predecenLevel) : base(predecenLevel) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                var temp1 = Left.GetValue(variableValues);
                var temp2 = Right.GetValue(variableValues);

                return (temp1 != 0 && temp2 != 0) ? 1 : 0;
            }
        }

        class Or : Operand
        {
            public Or(int predecenLevel) : base(predecenLevel) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                var temp1 = Left.GetValue(variableValues);
                var temp2 = Right.GetValue(variableValues);

                return (temp1 != 0 || temp2 != 0) ? 1 : 0;
            }
        }

        class Not : UnaryOperand
        {

            public Not(int predecenLevel) : base(predecenLevel) { }

            public override double GetValue(Dictionary<string, double> variableValues)
            {
                return Right.GetValue(variableValues) != 0 ? 1 : 0;
            }
        }

        const int MAX_PREDENCE = 5;

        static HashSet<char> IgnoreSet = new HashSet<char>() { ' ', '\n', '\t', '\r' };

        ValueNode _startNode;
        List<string> _variables;
        string _script;

        public SuperHappyScript(string script)
        {
            ValueNode currentNode = null;

            string currentParseValue = "";
            int predenceLevel = 0;

            _variables = new List<string>();
            int index = 0;
            char currentToken = ' ';
            char nextToken = ' ';

            _script = script;

            try
            {
                foreach (char token in script)
                {
                    index++;

                    if (script.Length > index)
                    {
                        nextToken = script[index];
                    }

                    currentToken = token;

                    if (currentParseValue.StartsWith("[") && !currentParseValue.EndsWith("]") && !(currentParseValue + token).EndsWith(">=") && !(currentParseValue + token).EndsWith("<=") && !(currentParseValue + token).EndsWith("<>"))
                    {
                        currentParseValue += token;
                        continue;
                    }

                    if (IgnoreSet.Contains(token))
                    {
                        if (currentParseValue != "")
                            currentNode = AddValue(currentNode, currentParseValue);
                    }
                    else if (token == ']')
                    {
                        currentParseValue += token;
                        currentNode = AddValue(currentNode, currentParseValue);
                    }
                    else if (token == ',')
                    {
                        if (predenceLevel == 0)
                            throw new Exception("Parsing error");

                        if (currentParseValue != "")
                            currentNode = AddValue(currentNode, currentParseValue);

                        currentNode = WrapFunction(currentNode, predenceLevel, false);
                    }
                    else if ((currentParseValue + token).ToUpper() == "ABS(")
                    {
                        currentNode = new Absolute(predenceLevel).AttachNode(currentNode);

                        predenceLevel += MAX_PREDENCE + 1;
                    }
                    else if ((currentParseValue + token).ToUpper() == "ROUND(")
                    {
                        currentNode = new Round(predenceLevel).AttachNode(currentNode);

                        predenceLevel += MAX_PREDENCE + 1;
                    }
                    else if ((currentParseValue + token).ToUpper() == "INT(")
                    {
                        currentNode = new RoundDown(predenceLevel).AttachNode(currentNode);

                        predenceLevel += MAX_PREDENCE + 1;
                    }
                    else if ((currentParseValue + token).ToUpper() == "IIF(")
                    {
                        currentNode = new IfStatement(predenceLevel).AttachNode(currentNode);

                        predenceLevel += MAX_PREDENCE + 1;
                    }
                    else if ((currentParseValue + token).ToUpper() == "MAX(")
                    {
                        currentNode = new Max(predenceLevel).AttachNode(currentNode);

                        predenceLevel += MAX_PREDENCE + 1;
                    }
                    else if ((currentParseValue + token).ToUpper() == "ISNAN(")
                    {
                        currentNode = new IsNaN(predenceLevel).AttachNode(currentNode);

                        predenceLevel += MAX_PREDENCE + 1;
                    }
                    else if ((currentParseValue + token).ToUpper() == "MIN(")
                    {
                        currentNode = new Min(predenceLevel).AttachNode(currentNode);

                        predenceLevel += MAX_PREDENCE + 1;
                    }
                    else if ((currentParseValue + token).ToUpper() == "AND")
                    {
                        if (currentParseValue.Length > 2)
                            currentNode = AddValue(currentNode, currentParseValue.Substring(0, currentParseValue.Length - 2));

                        currentNode = new And(predenceLevel).AttachNode(currentNode);
                    }
                    else if ((currentParseValue + token).ToUpper() == "OR")
                    {
                        if (currentParseValue.Length > 2)
                            currentNode = AddValue(currentNode, currentParseValue.Substring(0, currentParseValue.Length - 2));

                        currentNode = new Or(predenceLevel).AttachNode(currentNode);
                    }
                    else if ((currentParseValue + token).ToUpper() == "NOT")
                    {
                        if (currentParseValue.Length > 2)
                            currentNode = AddValue(currentNode, currentParseValue.Substring(0, currentParseValue.Length - 2));

                        currentNode = new Not(predenceLevel).AttachNode(currentNode);
                    }
                    else if (token == '(')
                    {
                        if (currentParseValue != "")
                            throw new Exception("Parsing error");

                        predenceLevel += MAX_PREDENCE + 1;
                    }
                    else if (token == ')')
                    {
                        if (predenceLevel == 0)
                            throw new Exception("Parsing error");

                        if (currentParseValue != "")
                            currentNode = AddValue(currentNode, currentParseValue);

                        currentNode = WrapFunction(currentNode, predenceLevel, true);

                        predenceLevel -= MAX_PREDENCE + 1;
                    }
                    else if ((currentParseValue + token).EndsWith("="))
                    {
                        if (currentParseValue.Length > 1)
                            currentNode = AddValue(currentNode, currentParseValue.Substring(0, currentParseValue.Length));

                        currentNode = new Equal(predenceLevel).AttachNode(currentNode);
                    }
                    else if ((currentParseValue + token).EndsWith(">="))
                    {
                        if (currentParseValue.Length > 2)
                            currentNode = AddValue(currentNode, currentParseValue.Substring(0, currentParseValue.Length - 1));

                        currentNode = new EqualOrGreaterThan(predenceLevel).AttachNode(currentNode);
                    }
                    else if ((currentParseValue + token).EndsWith("<="))
                    {
                        if (currentParseValue.Length > 2)
                            currentNode = AddValue(currentNode, currentParseValue.Substring(0, currentParseValue.Length - 1));

                        currentNode = new EqualOrLessThan(predenceLevel).AttachNode(currentNode);
                    }
                    else if ((currentParseValue + token).EndsWith("<>"))
                    {
                        if (currentParseValue.Length > 2)
                            currentNode = AddValue(currentNode, currentParseValue.Substring(0, currentParseValue.Length - 1));

                        currentNode = new NotEqual(predenceLevel).AttachNode(currentNode);
                    }
                    else if (token == '>' && nextToken != '=')
                    {
                        if (currentParseValue != "")
                            currentNode = AddValue(currentNode, currentParseValue);

                        currentNode = new GreaterThan(predenceLevel).AttachNode(currentNode);
                    }
                    else if (token == '<' && nextToken != '>' && nextToken != '=')
                    {
                        if (currentParseValue != "")
                            currentNode = AddValue(currentNode, currentParseValue);

                        currentNode = new LessThan(predenceLevel).AttachNode(currentNode);
                    }
                    else if (token == '+')
                    {
                        if (currentParseValue != "")
                            currentNode = AddValue(currentNode, currentParseValue);

                        currentNode = new Add(predenceLevel).AttachNode(currentNode);

                    }
                    else if (token == '-')
                    {
                        if (currentParseValue != "")
                            currentNode = AddValue(currentNode, currentParseValue);

                        if (currentNode == null || currentNode is Operand || currentNode is Function)
                        {
                            currentNode = new Negate(predenceLevel).AttachNode(currentNode);
                        }
                        else
                        {
                            currentNode = new Subtract(predenceLevel).AttachNode(currentNode);
                        }
                    }
                    else if (token == '*')
                    {
                        if (currentParseValue != "")
                            currentNode = AddValue(currentNode, currentParseValue);

                        currentNode = new Multiply(predenceLevel).AttachNode(currentNode);
                    }
                    else if (token == '/')
                    {
                        if (currentParseValue != "")
                            currentNode = AddValue(currentNode, currentParseValue);

                        currentNode = new Divide(predenceLevel).AttachNode(currentNode);
                    }
                    else if (token == '^')
                    {
                        if (currentParseValue != "")
                            currentNode = AddValue(currentNode, currentParseValue);

                        currentNode = new PowerOf(predenceLevel).AttachNode(currentNode);
                    }
                    else
                    {
                        if (currentParseValue.StartsWith("[") && currentParseValue.EndsWith("]") && token == '[')
                            throw new Exception("Two variables declared in a row");

                        currentParseValue += token;
                        continue;
                    }

                    currentParseValue = "";
                }
            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message + " at token: " + currentToken + " in row: " + index);
            }

            if (currentParseValue != "")
            {
                currentNode = AddValue(currentNode, currentParseValue);
            }

            if (predenceLevel != 0)
                throw new Exception("Parsing error");

            while (currentNode.Parent != null)
                currentNode = currentNode.Parent;

            _startNode = currentNode;
            _variables = new List<string>(_variables.Distinct());

            Test();
        }

        private void Test()
        {
            Dictionary<string, double> variables = new Dictionary<string, double>();

            foreach (var variable in GetVariables())
            {
                variables.Add(variable, 1);
            }

            Eval(variables);
        }

        private ValueNode WrapFunction(ValueNode currentNode, int predenceLevel, bool last)
        {
            ValueNode node = currentNode.Parent;
            ValueNode prevNode = currentNode;

            while (node != null && !(node is Function))
            {
                prevNode = node;
                node = node.Parent;
            }

            if (node is Function && node.PredenceLevel == predenceLevel - MAX_PREDENCE - 1)
            {
                var function = (Function)node;

                function.AddArgument(prevNode, last);

                return function;
            }
            else if (!last)
            {
                throw new Exception("Parsing Error");
            }

            return currentNode;
        }

        private ValueNode AddValue(ValueNode currentNode, string currentParseValue)
        {
            ValueNode vn;

            if (currentParseValue.StartsWith("["))
            {
                vn = new Variable(currentParseValue);
                _variables.Add(currentParseValue);
            }
            else
            {
                if (currentParseValue.ToUpper() == "NULL" || currentParseValue.ToUpper() == "NAN")
                {
                    vn = new Number(double.NaN);
                }
                else
                    vn = new Number(double.Parse(currentParseValue, System.Globalization.CultureInfo.InvariantCulture));
            }

            if (currentNode == null)
                return vn;
            else
                return vn.AttachNode(currentNode);
        }

        public IList<string> GetVariables()
        {
            return _variables.AsReadOnly();
        }

        public string GetOriginalScript()
        {
            return _script;
        }

        public double Eval(Dictionary<string, double> values)
        {
            return _startNode.GetValue(values);
        }
    }
}
