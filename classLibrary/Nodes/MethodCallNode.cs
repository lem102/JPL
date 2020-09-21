using System.Collections.Generic;

namespace JPL.classLibrary.Nodes
{
    public class MethodCallNode : StatementNode
    {
        public string Identifier { get; set; }
        
        public List<ExpressionNode> Arguments { get; set; }

        public MethodCallNode()
        {
            Arguments = new List<ExpressionNode>();
        }
    }
}
