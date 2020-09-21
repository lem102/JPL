using System.Collections.Generic;

namespace JPL.classLibrary.Nodes
{
    public class RootNode
    {
        public List<DefinitionNode> Definitions { get; set; }

        public RootNode()
        {
            Definitions = new List<DefinitionNode>();
        }

        public override string ToString()
        {
            string output = "";
            foreach (var definitionNode in Definitions)
            {
                output += definitionNode;
            }
            return output;
        }
    }
}
