using System.Collections.Generic;

namespace JPL.classLibrary.Nodes
{
    public class AssignmentNode : StatementNode
    {
        public string AssignmentTarget { get; set; }
        
        public ExpressionNode Expression { get; set; }

        public override string ToString()
        {
            var output = "Assignment Node:\n";
            output += $"AssignmentTarget: {AssignmentTarget}\n";
            output += $"Expression: {Expression}\n";
            return output;
        }
    }
}
