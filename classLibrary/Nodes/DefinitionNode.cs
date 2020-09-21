using System.Collections.Generic;

namespace JPL.classLibrary.Nodes
{
    public class DefinitionNode : ContainingNode
    {
        public string DefinitionName { get; set; }
        
        public List<ArgumentNode> Arguments { get; set; }

        public DefinitionNode()
        {
            Arguments = new List<ArgumentNode>();
            Statements = new List<StatementNode>();
        }

        public override string ToString()
        {
            string output = $"function: {DefinitionName}\n(\n";

            foreach (var argument in Arguments)
            {
                output = output + argument;
            }

            output = output + ")\n{\n";

            foreach (var statement in Statements)
            {
                output = output + statement;
            }

            output = output + "}\n";
            return output;
        }
    }
}
