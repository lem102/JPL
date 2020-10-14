using System;
using System.Linq;
using System.Collections.Generic;
using JPL.classLibrary;
using JPL.classLibrary.Nodes;

namespace JPL.compilerComponent
{
    public class Parser
    {
        public Parser(List<Token> tokenList)
        {
            Parse(tokenList);
        }

        private void Parse(List<Token> tokenList)
        {
            var nestingStatus = new List<ContainingNode>();
            var symbolTable = new List<(int, string, object)>();
            var rootNode = new RootNode();

            for (int i = 0; i < tokenList.Count; i++)
            {
                if (tokenList[i].TokenType == TokenType.Define)
                {
                    i = ParseDefinition(tokenList, nestingStatus, rootNode, i);
                }
                else
                {
                    ThrowParserException(rootNode, "all code must be contained within definitions.");
                }
            }

            Console.WriteLine(rootNode);
        }

        private int ParseDefinition(List<Token> tokenList,
                                    List<ContainingNode> nestingStatus,
                                    RootNode rootNode,
                                    int i)
        {
            if (nestingStatus.Find(x => x.GetType() == typeof(DefinitionNode)) != null)
            {
                ThrowParserException(rootNode,
                                     "cannot define function inside of function.");
            }
            else
            {
                // TODO: try to isolate the definition chunk into its own method and return the definition node.
                i = ParseDefinitionDeclaration(tokenList, nestingStatus, rootNode, i);
                // statement parsing goes here
                i = ParseStatements(tokenList, nestingStatus, rootNode, i);
            }

            return i;
        }

        private int ParseDefinitionDeclaration(List<Token> tokenList,
                                               List<ContainingNode> nestingStatus,
                                               RootNode rootNode,
                                               int i)
        {
            var definitionNode = new DefinitionNode();
            rootNode.Definitions.Add(definitionNode);
            nestingStatus.Add(definitionNode);
            i++;
            if (tokenList[i].TokenType != TokenType.Identifier)
            {
                Console.WriteLine(tokenList[i].TokenValue);
                Console.WriteLine(tokenList[i].TokenType);
                ThrowParserException(rootNode, "Definition must have a name.");
            }
            definitionNode.DefinitionName = tokenList[i].TokenValue;
            i++;
            if (tokenList[i].TokenType != TokenType.OpeningParenthesis)
            {
                ThrowParserException(rootNode, "definition must have an opening parenthesis after name.");
            }
            i++;
            if (tokenList[i].TokenType == TokenType.ClosingParenthesis)
            {
                i++;
            }
            else
            {
                // argument parsing
                i = ParseDefinitionArguments(tokenList, rootNode, i, definitionNode);
            }
            if (tokenList[i].TokenType != TokenType.OpeningBrace)
            {
                ThrowParserException(rootNode,
                                     "Opening brace required after function arguments");
            }
            i++;
            return i;
        }

        private int ParseStatements(List<Token> tokenList,
                                    List<ContainingNode> nestingStatus,
                                    RootNode rootNode,
                                    int i)
        {
            while (true)
            {
                if (tokenList[i].TokenType == TokenType.ClosingBrace)
                {
                    Console.WriteLine("denest");
                    Console.WriteLine(nestingStatus.Count - 1);
                    nestingStatus.RemoveAt(nestingStatus.Count - 1);
                    if (nestingStatus.Count <= 0)
                    {
                        break;
                    }
                }
                else if (tokenList[i].TokenType == TokenType.While)
                {
                    // while loop
                    i = ParseWhileDeclaration(tokenList, nestingStatus, rootNode, i);
                }
                else if (tokenList[i].TokenType == TokenType.IntegerDeclaration)
                {
                    // this section needs to be expanded in future to handle other data types. for now we will just have integers.
                    // in the nestingstatus variable, include a reference to the node with it. then you can add statements to that reference.
                    // declaration
                    i = ParseDeclarationStatement(tokenList, nestingStatus, rootNode, i);
                }
                else if (tokenList[i].TokenType == TokenType.Identifier)
                {
                    i++;
                    // handle non-declarative assignments and standalone method calls
                    if (tokenList[i].TokenType == TokenType.Assignment)
                    {
                        i = ParseAssignmentStatement(tokenList, nestingStatus, i);
                    }
                    else if (tokenList[i].TokenType == TokenType.OpeningParenthesis)
                    {
                        i = ParseStandaloneMethodCallStatement(tokenList, nestingStatus, rootNode, i);
                    }
                    else
                    {
                        Console.WriteLine("banana");
                        Console.WriteLine(tokenList[i]);
                        ThrowParserException(rootNode,
                                             "Invalid token, was expecting either an opening brace token " +
                                             "in case of a standalone method call or an assignment " +
                                             "token in case of variable assignment.");
                    }
                }
                else if (tokenList[i].TokenType == TokenType.If)
                {
                    var ifNode = new IfNode();
                    nestingStatus[nestingStatus.Count - 1].Statements.Add(ifNode);
                    nestingStatus.Add(ifNode);
                    i++;
                    i = ParseIfStatement(tokenList,
                                         nestingStatus,
                                         rootNode,
                                         ifNode,
                                         i);
                }
                else if (tokenList[i].TokenType == TokenType.Else)
                {
                    // i should implement a sort of chain, starting with the initial if node.
                    // a property of the if node should be an if or an else node,
                    // which can continue to chain until an else is reached.

                    // if node should have a property for a following if node,
                    // this property would be populated if there is an else
                    // statement directly after the if statement.

                    StatementNode previousStatementNode = nestingStatus.Last().Statements.Last();
                    if (previousStatementNode.GetType() != typeof(IfNode))
                    {
                        ThrowParserException(rootNode, "else statement can only occur after an if or else if statement.");
                    }
                    var parentIfNode = (IfNode)previousStatementNode;
                    i++;
                    while (parentIfNode.Else != null)
                    {
                        // TODO: Add a check here to check for rouge ElseNodes.
                        parentIfNode = (IfNode)parentIfNode.Else;
                    }
                    if (tokenList[i].TokenType == TokenType.If)
                    {
                        // do else if stuff here
                        var elseIfNode = new IfNode();
                        parentIfNode.Else = elseIfNode;
                        i++;
                        i = ParseIfStatement(tokenList,
                                             nestingStatus,
                                             rootNode,
                                             elseIfNode,
                                             i);
                    }
                    else if (tokenList[i].TokenType == TokenType.OpeningBrace)
                    {
                        // do else stuff here
                        var elseNode = new ElseNode();
                        parentIfNode.Else
                    }
                    else
                    {
                        ThrowParserException(rootNode, "else token must be followed by if " +
                                             "token incase of else if statement or opening paren " +
                                             "in the case of a straight else statement.");
                    }

                    ThrowParserException(rootNode, "not implemented");
                }
                i++;
            }
            
            return i;
        }

        private int ParseIfStatement(List<Token> tokenList,
                                     List<ContainingNode> nestingStatus,
                                     RootNode rootNode,
                                     IfNode ifNode,
                                     int i)
        {
            if (tokenList[i].TokenType != TokenType.OpeningParenthesis)
            {
                ThrowParserException(rootNode, "Opening parenthesis expected after if token.");
            }
            i++;
            ifNode.TestExpression = new ExpressionNode();
            var expressionTokens = new List<Token>();

            while (tokenList[i].TokenType != TokenType.ClosingParenthesis)
            {
                expressionTokens.Add(tokenList[i]);
                i++;
            }
            ifNode.TestExpression.ExpressionTokens = expressionTokens;
            i++;
            if (tokenList[i].TokenType != TokenType.OpeningBrace)
            {
                ThrowParserException(rootNode, "Opening brace expected after expression of if statement.");
            }

            return i;
        }

        private int ParseStandaloneMethodCallStatement(List<Token> tokenList,
                                                       List<ContainingNode> nestingStatus,
                                                       RootNode rootNode,
                                                       int i)
        {
            var methodCallNode = new MethodCallNode();
            nestingStatus[nestingStatus.Count - 1].Statements.Add(new MethodCallNode());
            methodCallNode.Identifier = tokenList[i - 1].TokenValue;
            i++;
            if (tokenList[i].TokenType != TokenType.ClosingParenthesis)
            {
                // start reading args.
                // each arg will take the form of an expressionNode.
                while (true)
                {
                    // create new argument
                    var expressionNode = new ExpressionNode();
                    methodCallNode.Arguments.Add(expressionNode);
                    var expressionTokens = new List<Token>();

                    while (tokenList[i].TokenType != TokenType.ClosingParenthesis
                           ||
                           tokenList[i].TokenType != TokenType.Comma)
                    {
                        // populate argument
                        expressionTokens.Add(tokenList[i]);
                        i++;
                    }
                    expressionNode.ExpressionTokens = expressionTokens;
                    if (tokenList[i].TokenType == TokenType.ClosingParenthesis)
                    {
                        i++;
                        break;
                    }
                    else if (tokenList[i].TokenType == TokenType.Comma)
                    {
                        i++;
                        continue;
                    }
                }
                if (tokenList[i].TokenType != TokenType.Semicolon)
                {
                    ThrowParserException(rootNode, "semi colon expected after standalone method call.");
                }
                i++;
            }

            return i;
        }

        private static int ParseAssignmentStatement(List<Token> tokenList,
                                                    List<ContainingNode> nestingStatus,
                                                    int i)
        {
            var assignmentNode = new AssignmentNode();
            nestingStatus[nestingStatus.Count - 1].Statements.Add(assignmentNode);
            assignmentNode.AssignmentTarget = tokenList[i - 1].TokenValue;
            i++;
            assignmentNode.Expression = new ExpressionNode();
            var expressionTokens = new List<Token>();
            while (tokenList[i].TokenType != TokenType.Semicolon)
            {
                expressionTokens.Add(tokenList[i]);
                i++;
            }
            assignmentNode.Expression.ExpressionTokens = expressionTokens;
            return i;
        }

        private int ParseDeclarationStatement(List<Token> tokenList,
                                              List<ContainingNode> nestingStatus,
                                              RootNode rootNode,
                                              int i)
        {
            var declarationNode = new DeclarationNode();
            nestingStatus[nestingStatus.Count - 1].Statements.Add(declarationNode);
            declarationNode.Type = JPLType.Integer;
            i++;
            if (tokenList[i].TokenType != TokenType.Identifier)
            {
                ThrowParserException(rootNode, "Missing variable name.");
            }
            Console.WriteLine(tokenList[i].TokenValue);
            declarationNode.Identifier = tokenList[i].TokenValue;
            i++;
            if (tokenList[i].TokenType == TokenType.Assignment)
            {
                i++;
                declarationNode.Expression = new ExpressionNode();
                var expressionTokens = new List<Token>();

                while (tokenList[i].TokenType != TokenType.Semicolon)
                {
                    expressionTokens.Add(tokenList[i]);
                    i++;
                }
                declarationNode.Expression.ExpressionTokens = expressionTokens;
            }
            else if (tokenList[i].TokenType == TokenType.Semicolon)
            {
                // i++;
            }
            else
            {
                ThrowParserException(rootNode, "if something is being assigned, assignment token is required. If declaring then a semi colon token is required.");
            }

            return i;
        }

        private int ParseWhileDeclaration(List<Token> tokenList,
                                          List<ContainingNode> nestingStatus,
                                          RootNode rootNode,
                                          int i)
        {
            Console.WriteLine("while loop entered");
            var whileNode = new WhileNode();
            nestingStatus[nestingStatus.Count - 1].Statements.Add(whileNode);
            nestingStatus.Add(whileNode);
            i++;
            if (tokenList[i].TokenType != TokenType.OpeningParenthesis)
            {
                ThrowParserException(rootNode, "Parenthesis at start of expression expected.");
            }
            i++;
            whileNode.TestExpression = new ExpressionNode();
            var expressionTokens = new List<Token>();
            while (tokenList[i].TokenType != TokenType.OpeningBrace)
            {
                expressionTokens.Add(tokenList[i]);
                i++;
            }
            if (expressionTokens[expressionTokens.Count - 1].TokenType != TokenType.ClosingParenthesis)
            {
                ThrowParserException(rootNode, "while test expression must have closing bracket.");
            }
            expressionTokens.RemoveAt(expressionTokens.Count - 1);
            whileNode.TestExpression.ExpressionTokens = expressionTokens;
            return i;
        }

        private int ParseDefinitionArguments(List<Token> tokenList,
                                             RootNode rootNode,
                                             int i,
                                             DefinitionNode definitionNode)
        {
            while (true)
            {
                definitionNode.Arguments.Add(new ArgumentNode());
                var argumentNode = definitionNode.Arguments[definitionNode.Arguments.Count - 1];
                // this if needs to include all other declaration tokens if/when they are added.
                if (tokenList[i].TokenType != TokenType.IntegerDeclaration)
                {
                    ThrowParserException(rootNode, "arguments must have a type.");
                }
                argumentNode.Type = JPLType.Integer;
                i++;
                if (tokenList[i].TokenType != TokenType.Identifier)
                {
                    ThrowParserException(rootNode,
                                         "arguments must have a identifier after their type.");
                }

                if (tokenList[i].TokenType != TokenType.IntegerDeclaration)
                {
                    ThrowParserException(rootNode,
                                         "arguments must have a identifier after their type.");
                }
                
                argumentNode.Identifier = tokenList[i].TokenValue;
                i++;
                if (tokenList[i].TokenType == TokenType.ClosingParenthesis)
                {
                    i++;
                    break;
                }
                else if (tokenList[i].TokenType == TokenType.Comma)
                {
                    i++;
                    continue;
                }
                else
                {
                    ThrowParserException(rootNode,
                                         "Invalid extra token after argument");
                }
            }

            return i;
        }

        private void ThrowParserException(RootNode rootNode, string message)
        {
            Console.WriteLine(rootNode);
            throw new JPLException(message);
        }
    }
}
