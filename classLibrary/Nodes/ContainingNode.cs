using System.Collections.Generic;

namespace JPL.classLibrary.Nodes
{
    public class ContainingNode : StatementNode
    {
        public List<StatementNode> Statements { get; set; }

        public ContainingNode()
        {
            Statements = new List<StatementNode>();
        }
    }
}
