using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JPL.classLibrary;

namespace JPL.compilerComponent
{
    public class Lexer
    {
        public List<Token> Output {get;}
        
        public Lexer(string sourceCode)
        {
            List<string> tokens = SplitSourceIntoTokens(sourceCode);
            Output = ConvertStringsToTokens(tokens);
        }

        private List<Token> ConvertStringsToTokens(List<string> tokens)
        {
            var tokenList = new List<Token>();

            foreach (var tokenString in tokens)
            {
                tokenList.Add(CreateToken(tokenString));
            }

            return tokenList;
        }

        private Token CreateToken(string tokenString)
        {
            TokenType tokenType;
            
            if (tokenString == "while")
            {
                tokenType = TokenType.While;
            }
            else if (tokenString == "define")
            {
                tokenType = TokenType.Define;
            }
            else if (tokenString == "if")
            {
                tokenType = TokenType.If;
            }
            else if (tokenString == "else")
            {
                tokenType = TokenType.Else;
            }
            else if (tokenString == "int")
            {
                tokenType = TokenType.IntegerDeclaration;
            }
            else if (tokenString.All(x => Char.IsNumber(x)))
            {
                tokenType = TokenType.Integer;
            }
            else if (tokenString.All(x => Char.IsLetter(x)))
            {
                tokenType = TokenType.Identifier;
            }
            else if (tokenString == "(")
            {
                tokenType = TokenType.OpeningParenthesis;
            }
            else if (tokenString == ")")
            {
                tokenType = TokenType.ClosingParenthesis;
            }
            else if (tokenString == ",")
            {
                tokenType = TokenType.Comma;
            }
            else if (tokenString == "{")
            {
                tokenType = TokenType.OpeningBrace;
            }
            else if (tokenString == "}")
            {
                tokenType = TokenType.ClosingBrace;
            }
            else if (tokenString == "=")
            {
                tokenType = TokenType.Assignment;
            }
            else if (tokenString == ";")
            {
                tokenType = TokenType.Semicolon;
            }
            else if (tokenString == "==")
            {
                tokenType = TokenType.Equal;
            }
            else if (tokenString == "||")
            {
                tokenType = TokenType.Or;
            }
            else if (tokenString == "&&")
            {
                tokenType = TokenType.And;
            }
            else if (tokenString == "!=")
            {
                tokenType = TokenType.NotEqual;
            }
            else
            {
                throw new Exception($"unrecognised token: {tokenString}");
            }

            return new Token(tokenType, tokenString);
        }

        private List<string> SplitSourceIntoTokens(string sourceCode)
        {
            var tokens = new List<string>();
            sourceCode = Regex.Replace(sourceCode, @"\s+", " ");

            string currentToken = "";

            for (int i = 0; i < sourceCode.Length; i++)
            {
                char currentChar = sourceCode[i];
                
                if (Char.IsLetterOrDigit(currentChar))
                {
                    currentToken += currentChar;
                }
                else if (Char.IsSymbol(currentChar)
                         ||
                         Char.IsPunctuation(currentChar))
                {
                    char previousChar = sourceCode[i - 1];
                    currentToken = HandleSymbolOrPunctuation(tokens,
                                                             currentToken,
                                                             currentChar,
                                                             previousChar);
                }
                else if (Char.IsSeparator(currentChar))
                {
                    if (currentToken != string.Empty)
                    {
                        tokens.Add(currentToken);
                        currentToken = "";                        
                    }
                }
            }

            return tokens;
        }

        private string HandleSymbolOrPunctuation(List<string> tokens,
                                                 string currentToken,
                                                 char currentChar,
                                                 char previousChar)
        {
            if (currentToken != string.Empty)
            {
                tokens.Add(currentToken);
                currentToken = "";
            }

            if (currentChar == '=')
            {
                tokens = HandleSymbol('=',
                                      new List<char>() { '=', '!' },
                                      tokens,
                                      previousChar);
            }
            else if (currentChar == '|')
            {
                tokens = HandleSymbol('|',
                                      new List<char>() { '|' },
                                      tokens,
                                      previousChar);
            }
            else if (currentChar == '&')
            {
                tokens = HandleSymbol('&',
                                      new List<char>() { '&' },
                                      tokens,
                                      previousChar);
            }
            else
            {
                tokens.Add(currentChar.ToString());
            }

            return currentToken;
        }

        private List<string> HandleSymbol(char currentChar,
                                          List<char> combineChars,
                                          List<string> tokens,
                                          char previousChar)
        {
            bool changed = false;
            foreach (var character in combineChars)
            {
                if (previousChar == character)
                {
                    tokens.RemoveAt(tokens.Count - 1);
                    tokens.Add(character.ToString() + currentChar.ToString());
                    changed = true;
                }
            }

            if (!changed)
            {
                tokens.Add(currentChar.ToString());
            }

            return tokens;
        }

        private List<string> RemoveBlank(List<string> tokens)
        {
            return tokens.Where(x => x != string.Empty).ToList();
        }
    }
}
