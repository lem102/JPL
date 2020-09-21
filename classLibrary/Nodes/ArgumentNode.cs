namespace JPL.classLibrary.Nodes
{
    public class ArgumentNode
    {
        public JPLType Type { get; set; }
        
        public string Identifier { get; set; }

        public override string ToString()
        {
            return $"Type: {Type}, Identifier: {Identifier}\n";
        }
    }
}
