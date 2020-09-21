using System.Collections.Generic;

namespace JPL.classLibrary.Nodes
{
    public class ExpressionNode
    {
        public List<Token> ExpressionTokens { get; set; }

        public ExpressionNode()
        {
            ExpressionTokens = new List<Token>();
        }

        public override string ToString()
        {
            return "\n" + string.Join("\n", ExpressionTokens) + "\n";
        }
    }
}
