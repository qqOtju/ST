using System;
using System.Collections.Generic;

namespace ST_1
{
    class NewTockenizer
    {
        public const int TOK_DIV = 100;
        public const int TOK_OP = 200;
        public const int TOK_CONST = 300;
        public const int TOK_ID = 1000;
        public const int TOK_EOF = Int32.MaxValue;
        public const int DIV_LPR = TOK_DIV + 1;  // (
        public const int DIV_RPR = TOK_DIV + 2;  // )
        public const int OP_CMP_EQ = TOK_OP + 1; // ==
        public const int OP_CMP_NE = TOK_OP + 2; // !=
        public const int OP_CMP_LS = TOK_OP + 3; // <
        public const int OP_CMP_GR = TOK_OP + 4; // >
        public const int OP_CMP_LE = TOK_OP + 5; // <=
        public const int OP_CMP_GE = TOK_OP + 6; // >=
        public const int OP_CMP_AN = TOK_OP + 7; // &&
        public const int OP_CMP_OR = TOK_OP + 8; // ||
        public const int OP_CMP_NOT = TOK_OP + 9;// !
        public const int CONST_INT = TOK_CONST + 1;
        public const int CONST_FLOAT = TOK_CONST + 2;
        public const int CONST_STR = TOK_CONST + 3;
        private int _index = 0;
        private List<Token> _tokens;
        private string _codeText;

        public static readonly char[] Dividers = new char[2] { '(', ')' };
        public static readonly string[] Operators = new string[] { "==", "!=", "<", ">", "<=", ">=", "&&", "||", "!" };
        public List<Token> Tokens => _tokens;


        public NewTockenizer(string codeText)
        {
            _codeText = codeText;
            _tokens = new List<Token>();
            while (_index < codeText.Length)
            {
                //White space check
                if (IsWhiteSpace(codeText[_index], ref _index))
                    continue;
                else if (IsDivider(codeText[_index], ref _index) || IsOperator(codeText[_index], ref _index) 
                    || IsString(codeText[_index], ref _index))
                {

                }
                else
                    _index++;
            }

        }

        private bool IsWhiteSpace(char symbol, ref int ind)
        {
            if (symbol == ' ')
            {
                ind++;
                return true;
            }
            return false;
        }

        private bool IsDivider(char symbol, ref int ind)
        {
            for (int i = 0; i < Dividers.Length; i++)
                if (symbol == Dividers[i])
                {
                    ind++;
                    _tokens.Add(new Token(TOK_DIV, i, symbol.ToString()));
                    return true;
                }
            return false;
        }

        //{ "==", "!=", "<", ">", "<=", ">=", "&&", "||", "!" };
        private bool IsOperator(char symbol, ref int ind)
        {
            for (int i = 0; i < Operators.Length; i++)
            {
                if (Operators[i].Length == 1)
                {
                    if (symbol == Operators[i][0])
                    {
                        ind++;
                        _tokens.Add(new Token(TOK_OP, i, symbol.ToString()));
                        return true;
                    }
                }
                else
                {
                    if (symbol == Operators[i][0] && _codeText[ind + 1] == Operators[i][1])
                    {
                        ind += 2;
                        _tokens.Add(new Token(TOK_OP, i, $"{Operators[i][0]}{Operators[i][1]}"));
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsString(char symbol, ref int ind)
        {   
            if(symbol == '"')
            {
                string result = $"{'"'}";
                int start = ind, end;
                while (_codeText[++ind] != '"')
                {
                    result += _codeText[ind];
                }
                ind++;
                result += $"{'"'}";
                _tokens.Add(new Token(TOK_CONST, 0, result));
                return true;
            }
            return false;
        }
    }
}
