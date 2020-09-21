using System;

namespace JPL.classLibrary
{
    public class Token
    {
        public TokenType TokenType { get; }
        
        public string TokenValue { get; }

        public Token(TokenType type, string actualValue)
        {
            TokenType = type;
            TokenValue = actualValue;
        }

        public override string ToString()
        {
            return String.Format("{0,-20} {1,-5}", TokenType, TokenValue);
        }
    }
}
