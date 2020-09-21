using System.Collections.Generic;

namespace JPL.classLibrary.Nodes
{
    public class WhileNode : ContainingNode
    {
        public ExpressionNode TestExpression { get; set; }

        public WhileNode()
        {
            Statements = new List<StatementNode>();
        }

        public override string ToString()
        {
            return $"While Loop:\n(\nTestExpression: {TestExpression})\nLoopStatements:\n{{\n{string.Join("\n", Statements)}\n}}\n";
        }
    }
}
