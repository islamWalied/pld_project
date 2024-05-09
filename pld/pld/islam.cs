
using System;
using System.IO;
using System.Runtime.Serialization;
using com.calitha.goldparser.lalr;
using com.calitha.commons;
using System.Windows.Forms;

namespace com.calitha.goldparser
{

    [Serializable()]
    public class SymbolException : System.Exception
    {
        public SymbolException(string message) : base(message)
        {
        }

        public SymbolException(string message,
            Exception inner) : base(message, inner)
        {
        }

        protected SymbolException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

    }

    [Serializable()]
    public class RuleException : System.Exception
    {

        public RuleException(string message) : base(message)
        {
        }

        public RuleException(string message,
                             Exception inner) : base(message, inner)
        {
        }

        protected RuleException(SerializationInfo info,
                                StreamingContext context) : base(info, context)
        {
        }

    }

    enum SymbolConstants : int
    {
        SYMBOL_EOF               =  0, // (EOF)
        SYMBOL_ERROR             =  1, // (Error)
        SYMBOL_WHITESPACE        =  2, // Whitespace
        SYMBOL_QUOTE             =  3, // '"'
        SYMBOL_LPAREN            =  4, // '('
        SYMBOL_RPAREN            =  5, // ')'
        SYMBOL_COMMA             =  6, // ','
        SYMBOL_DOT               =  7, // '.'
        SYMBOL_COLON             =  8, // ':'
        SYMBOL_LBRACE            =  9, // '{'
        SYMBOL_RBRACE            = 10, // '}'
        SYMBOL_EQ                = 11, // '='
        SYMBOL_BREAK             = 12, // break
        SYMBOL_CLOSE             = 13, // Close
        SYMBOL_ELSEIF            = 14, // 'else if'
        SYMBOL_ELSECOLON         = 15, // 'else:'
        SYMBOL_ENDIF             = 16, // endif
        SYMBOL_EXIT              = 17, // Exit
        SYMBOL_GO                = 18, // Go
        SYMBOL_IF                = 19, // if
        SYMBOL_LOOP              = 20, // Loop
        SYMBOL_NAME              = 21, // name
        SYMBOL_NUMBER            = 22, // Number
        SYMBOL_VAR               = 23, // var
        SYMBOL_ASSIGNMENT        = 24, // <Assignment>
        SYMBOL_DECLARATION       = 25, // <Declaration>
        SYMBOL_ELSEIF2           = 26, // <ElseIf>
        SYMBOL_ELSEIFS           = 27, // <ElseIfs>
        SYMBOL_EXPRESSION        = 28, // <Expression>
        SYMBOL_GO2               = 29, // <Go>
        SYMBOL_IF2               = 30, // <If>
        SYMBOL_LOOP2             = 31, // <Loop>
        SYMBOL_METHODCALLING     = 32, // <MethodCalling>
        SYMBOL_METHODDECLARATION = 33, // <MethodDeclaration>
        SYMBOL_NAME2             = 34, // <Name>
        SYMBOL_NUMBER2           = 35, // <Number>
        SYMBOL_PARAMS            = 36, // <Params>
        SYMBOL_SENTENCE          = 37, // <Sentence>
        SYMBOL_SENTENCES         = 38, // <Sentences>
        SYMBOL_STRING            = 39  // <String>
    };

    enum RuleConstants : int
    {
        RULE_GO_GO_EXIT                                                 =  0, // <Go> ::= Go <Sentences> Exit
        RULE_SENTENCES                                                  =  1, // <Sentences> ::= <Sentence> <Sentences>
        RULE_SENTENCES2                                                 =  2, // <Sentences> ::= <Sentence>
        RULE_SENTENCE                                                   =  3, // <Sentence> ::= <Declaration>
        RULE_SENTENCE2                                                  =  4, // <Sentence> ::= <Loop>
        RULE_SENTENCE3                                                  =  5, // <Sentence> ::= <If>
        RULE_SENTENCE4                                                  =  6, // <Sentence> ::= <MethodDeclaration>
        RULE_SENTENCE5                                                  =  7, // <Sentence> ::= <MethodCalling>
        RULE_SENTENCE6                                                  =  8, // <Sentence> ::= <Assignment>
        RULE_DECLARATION_VAR_EQ_DOT                                     =  9, // <Declaration> ::= var <Name> '=' <Expression> '.'
        RULE_LOOP_LOOP_COLON_CLOSE                                      = 10, // <Loop> ::= Loop <Number> ':' <Sentences> Close
        RULE_LOOP_LOOP_COLON_BREAK                                      = 11, // <Loop> ::= Loop <Number> ':' <If> break
        RULE_NUMBER_NUMBER                                              = 12, // <Number> ::= Number
        RULE_IF_IF_COLON_ENDIF_ELSECOLON                                = 13, // <If> ::= if <Expression> ':' <Sentences> endif <ElseIfs> 'else:' <Sentences>
        RULE_ELSEIFS                                                    = 14, // <ElseIfs> ::= <ElseIf>
        RULE_ELSEIFS2                                                   = 15, // <ElseIfs> ::= <ElseIf> <ElseIfs>
        RULE_ELSEIF_ELSEIF_COLON                                        = 16, // <ElseIf> ::= 'else if' <If> ':' <Sentences>
        RULE_STRING_QUOTE_QUOTE                                         = 17, // <String> ::= '"' <Name> '"'
        RULE_NAME_NAME                                                  = 18, // <Name> ::= name
        RULE_EXPRESSION                                                 = 19, // <Expression> ::= <String>
        RULE_EXPRESSION2                                                = 20, // <Expression> ::= <Number>
        RULE_EXPRESSION3                                                = 21, // <Expression> ::= <MethodCalling>
        RULE_ASSIGNMENT_NAME_EQ                                         = 22, // <Assignment> ::= name '=' <Expression>
        RULE_METHODDECLARATION_VAR_NAME_LPAREN_RPAREN_LBRACE_RBRACE_DOT = 23, // <MethodDeclaration> ::= var name '(' <Params> ')' '{' <Sentences> '}' '.'
        RULE_PARAMS_NAME_COMMA                                          = 24, // <Params> ::= name ',' <Params>
        RULE_PARAMS_NAME                                                = 25, // <Params> ::= name
        RULE_METHODCALLING_NAME_LPAREN_RPAREN_DOT                       = 26  // <MethodCalling> ::= name '(' <Params> ')' '.'
    };

    public class MyParser
    {
        private LALRParser parser;
        ListBox lst;
        ListBox ls;

        public MyParser(string filename, ListBox lst, ListBox ls)
        {
            FileStream stream = new FileStream(filename,
                                               FileMode.Open,
                                               FileAccess.Read,
                                               FileShare.Read);

            this.lst = lst;
            this.ls = ls;
            Init(stream);
            stream.Close();
        }
        public MyParser(string baseName, string resourceName)
        {
            byte[] buffer = ResourceUtil.GetByteArrayResource(
                System.Reflection.Assembly.GetExecutingAssembly(),
                baseName,
                resourceName);
            MemoryStream stream = new MemoryStream(buffer);
            Init(stream);
            stream.Close();
        }

        public MyParser(Stream stream)
        {
            Init(stream);
        }


        private void Init(Stream stream)
        {
            CGTReader reader = new CGTReader(stream);
            parser = reader.CreateNewParser();
            parser.TrimReductions = false;
            parser.StoreTokens = LALRParser.StoreTokensMode.NoUserObject;

            parser.OnTokenError += new LALRParser.TokenErrorHandler(TokenErrorEvent);
            parser.OnParseError += new LALRParser.ParseErrorHandler(ParseErrorEvent);
            parser.OnTokenRead += new LALRParser.TokenReadHandler(TokenReadEvent);
        }

        public void Parse(string source)
        {
            NonterminalToken token = parser.Parse(source);
            if (token != null)
            {
                Object obj = CreateObject(token);
                //todo: Use your object any way you like
            }
        }

        private Object CreateObject(Token token)
        {
            if (token is TerminalToken)
                return CreateObjectFromTerminal((TerminalToken)token);
            else
                return CreateObjectFromNonterminal((NonterminalToken)token);
        }

        private Object CreateObjectFromTerminal(TerminalToken token)
        {
            switch (token.Symbol.Id)
            {
                case (int)SymbolConstants.SYMBOL_EOF :
                //(EOF)
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ERROR :
                //(Error)
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_WHITESPACE :
                //Whitespace
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_QUOTE :
                //'"'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_LPAREN :
                //'('
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_RPAREN :
                //')'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_COMMA :
                //','
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DOT :
                //'.'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_COLON :
                //':'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_LBRACE :
                //'{'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_RBRACE :
                //'}'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_EQ :
                //'='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_BREAK :
                //break
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_CLOSE :
                //Close
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ELSEIF :
                //'else if'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ELSECOLON :
                //'else:'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ENDIF :
                //endif
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_EXIT :
                //Exit
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_GO :
                //Go
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_IF :
                //if
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_LOOP :
                //Loop
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_NAME :
                //name
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_NUMBER :
                //Number
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_VAR :
                //var
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ASSIGNMENT :
                //<Assignment>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DECLARATION :
                //<Declaration>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ELSEIF2 :
                //<ElseIf>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ELSEIFS :
                //<ElseIfs>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_EXPRESSION :
                //<Expression>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_GO2 :
                //<Go>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_IF2 :
                //<If>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_LOOP2 :
                //<Loop>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_METHODCALLING :
                //<MethodCalling>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_METHODDECLARATION :
                //<MethodDeclaration>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_NAME2 :
                //<Name>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_NUMBER2 :
                //<Number>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_PARAMS :
                //<Params>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_SENTENCE :
                //<Sentence>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_SENTENCES :
                //<Sentences>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_STRING :
                //<String>
                //todo: Create a new object that corresponds to the symbol
                return null;

            }
            throw new SymbolException("Unknown symbol");
        }

        public Object CreateObjectFromNonterminal(NonterminalToken token)
        {
            switch (token.Rule.Id)
            {
                case (int)RuleConstants.RULE_GO_GO_EXIT :
                //<Go> ::= Go <Sentences> Exit
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_SENTENCES :
                //<Sentences> ::= <Sentence> <Sentences>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_SENTENCES2 :
                //<Sentences> ::= <Sentence>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_SENTENCE :
                //<Sentence> ::= <Declaration>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_SENTENCE2 :
                //<Sentence> ::= <Loop>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_SENTENCE3 :
                //<Sentence> ::= <If>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_SENTENCE4 :
                //<Sentence> ::= <MethodDeclaration>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_SENTENCE5 :
                //<Sentence> ::= <MethodCalling>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_SENTENCE6 :
                //<Sentence> ::= <Assignment>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_DECLARATION_VAR_EQ_DOT :
                //<Declaration> ::= var <Name> '=' <Expression> '.'
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_LOOP_LOOP_COLON_CLOSE :
                //<Loop> ::= Loop <Number> ':' <Sentences> Close
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_LOOP_LOOP_COLON_BREAK :
                //<Loop> ::= Loop <Number> ':' <If> break
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_NUMBER_NUMBER :
                //<Number> ::= Number
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_IF_IF_COLON_ENDIF_ELSECOLON :
                //<If> ::= if <Expression> ':' <Sentences> endif <ElseIfs> 'else:' <Sentences>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_ELSEIFS :
                //<ElseIfs> ::= <ElseIf>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_ELSEIFS2 :
                //<ElseIfs> ::= <ElseIf> <ElseIfs>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_ELSEIF_ELSEIF_COLON :
                //<ElseIf> ::= 'else if' <If> ':' <Sentences>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_STRING_QUOTE_QUOTE :
                //<String> ::= '"' <Name> '"'
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_NAME_NAME :
                //<Name> ::= name
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_EXPRESSION :
                //<Expression> ::= <String>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_EXPRESSION2 :
                //<Expression> ::= <Number>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_EXPRESSION3 :
                //<Expression> ::= <MethodCalling>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_ASSIGNMENT_NAME_EQ :
                //<Assignment> ::= name '=' <Expression>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_METHODDECLARATION_VAR_NAME_LPAREN_RPAREN_LBRACE_RBRACE_DOT :
                //<MethodDeclaration> ::= var name '(' <Params> ')' '{' <Sentences> '}' '.'
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_PARAMS_NAME_COMMA :
                //<Params> ::= name ',' <Params>
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_PARAMS_NAME :
                //<Params> ::= name
                //todo: Create a new object using the stored tokens.
                return null;

                case (int)RuleConstants.RULE_METHODCALLING_NAME_LPAREN_RPAREN_DOT :
                //<MethodCalling> ::= name '(' <Params> ')' '.'
                //todo: Create a new object using the stored tokens.
                return null;

            }
            throw new RuleException("Unknown rule");
        }
        private void TokenErrorEvent(LALRParser parser, TokenErrorEventArgs args)
        {
            string message = "Token error with input: '" + args.Token.ToString() + "'";
            //todo: Report message to UI?
        }

        private void ParseErrorEvent(LALRParser parser, ParseErrorEventArgs args)
        {
            string message = "Parse error caused by token: '" + args.UnexpectedToken.ToString() + " In line " + args.UnexpectedToken.Location.LineNr;
            lst.Items.Add(message);
            string m2 = "Expected Token : " + args.ExpectedTokens.ToString();
            lst.Items.Add(m2);
            //todo: Report message to UI?
        }
        private void TokenReadEvent(LALRParser parser, TokenReadEventArgs args)
        {
            //lexeme                     //category
            string info = args.Token.Text + "   \t \t" + (SymbolConstants)args.Token.Symbol.Id;
            ls.Items.Add(info);
        }



    }
}
