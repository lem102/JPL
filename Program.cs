using System;
using System.Collections.Generic;
using System.IO;
using JPL.classLibrary;
using JPL.compilerComponent;

namespace JPL
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("A source file must be provided.");
            }

            string sourceCode = File.ReadAllText(args[0]);

            var program = new Program();
            program.Start(sourceCode);
        }

        private void Start(string sourceCode)
        {
            var lexer = new Lexer(sourceCode);
            List<Token> tokenList = lexer.Output;

            // PrintTokenList(tokenList);

            var parser = new Parser(tokenList);
        }

        private void PrintTokenList(List<Token> tokenList)
        {
            tokenList.ForEach(Console.WriteLine);
        }
    }
}
