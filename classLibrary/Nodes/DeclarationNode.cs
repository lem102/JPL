namespace JPL.classLibrary.Nodes
{
    public class DeclarationNode : StatementNode
    {
        public JPLType Type { get; set; }
        
        public string Identifier { get; set; }
        
        public ExpressionNode Expression { get; set; }

        public override string ToString()
        {
            return $"Declaration:\nType: {Type}\nIdentifier: {Identifier}\nExpression: {Expression}";
        }
    }
}
