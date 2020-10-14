namespace JPL.classLibrary.Nodes
{
    public class IfNode : ContainingNode
    {
        public ExpressionNode TestExpression { get; set; }

        public ContainingNode Else { get; set; }

        public override string ToString()
        {
            return "If Statement: \n" +
                $"TestExpression: {TestExpression}\n" +
                $"Statements: {string.Join("\n", Statements)}\n" +
                $"Else: {Else}";
        }
    }
}
