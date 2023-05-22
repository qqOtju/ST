using System;
using System.Collections.Generic;

namespace ST_1
{
    class Tokenizer
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

        private List<Token> tokens;

        public List<Token> Tokens => tokens;

        public Tokenizer(string src)
        {
            var elems = src.Split(' ');
            tokens = new List<Token>();
            foreach (var str in elems)
            {
                Token? tkn;
                if (str != "" && (IsDiv(str, out tkn) || IsConst(str, out tkn) || IsOp(str, out tkn)
                    || IsId(str, out tkn)))
                {
                    if (tkn != null)
                        tokens.Add((Token)tkn);
                }
                else throw new Exception("Invalid Token!");
            }
            tokens.Add(GetToken(TOK_EOF, 0, ""));
        }

        public bool IsDiv(string src, out Token? token)
        {
            switch (src[0])
            {
                case '(':
                    token = GetToken(TOK_DIV, DIV_LPR, src);
                    return true;
                case ')':
                    token = GetToken(TOK_DIV, DIV_RPR, src);
                    return true;
                default:
                    token = null;
                    return false;
            }
        }

        public bool IsOp(string src, out Token? token)
        {
            token = null;
            if (src.Equals("==")) token = GetToken(TOK_OP, OP_CMP_EQ, src);
            else if (src.Equals("!=")) token = GetToken(TOK_OP, OP_CMP_NE, src);
            else if (src.Equals("<")) token = GetToken(TOK_OP, OP_CMP_LS, src);
            else if (src.Equals(">")) token = GetToken(TOK_OP, OP_CMP_GR, src);
            else if (src.Equals("<=")) token = GetToken(TOK_OP, OP_CMP_LE, src);
            else if (src.Equals(">=")) token = GetToken(TOK_OP, OP_CMP_GE, src);
            else if (src.Equals("&&")) token = GetToken(TOK_OP, OP_CMP_AN, src);
            else if (src.Equals("||")) token = GetToken(TOK_OP, OP_CMP_OR, src);
            else if (src.Equals("!")) token = GetToken(TOK_OP, OP_CMP_NOT, src);
            else return false;
            return true;
        }

        public bool IsId(string src, out Token? token)
        {
            token = null;
            if (src == "" || !Char.IsLetter(src[0])) return false;
            foreach (var ch in src)
                if (!Char.IsLetter(ch) && !Char.IsDigit(ch)) return false;
            token = GetToken(TOK_ID, 0, src);
            return true;
        }
        
        public bool IsConst(string src, out Token? token) 
            => IsInt(src, out token) || IsFloat(src, out token) || IsString(src, out token);


        public bool IsInt(string src, out Token? token)
        {
            token = null;
            int _int = 0;
            if (Int32.TryParse(src, out _int)) token = GetToken(TOK_CONST, CONST_INT, src);
            else return false;
            return true;
        }

        public bool IsFloat(string src, out Token? token)
        {
            token = null;
            if (src[src.Length - 1] != 'f')
                return false;
            float _float = 0f;
            if (float.TryParse(src.Trim('f'), out _float)) token = GetToken(TOK_CONST, CONST_FLOAT, src);
            else return false;
            return true;
        }

        public bool IsString(string src, out Token? token)
        {
            token = null;
            if(src[0] == '"' && src[src.Length-1] == '"') token = GetToken(TOK_CONST, CONST_STR, src);
            else return false;
            return true;
        }

        public Token GetToken(int type, int subType, string text) => new Token(type, subType, text);

        public static string GetTypeName(int type)
        {
            switch (type)
            {
                case TOK_DIV:
                    return "Div";
                case TOK_OP:
                    return "Op";
                case TOK_CONST:
                    return "Const";
                case TOK_ID:
                    return "Id";
                case TOK_EOF:
                    return "EOF";
                default:
                    return "---";
            }
        }

        public static string GetSubTypeName(int type)
        {
            switch (type)
            {
                case DIV_LPR:
                    return "DIV_LPR";
                case DIV_RPR:
                    return "DIV_RPR";
                case OP_CMP_EQ:
                    return "OP_CMP_EQ";
                case OP_CMP_NE:
                    return "OP_CMP_NE";
                case OP_CMP_LS:
                    return "OP_CMP_LS";
                case OP_CMP_GR:
                    return "OP_CMP_GR";
                case OP_CMP_LE:
                    return "OP_CMP_LE";
                case OP_CMP_GE:
                    return "OP_CMP_GE";
                case OP_CMP_AN:
                    return "OP_CMP_AN";
                case OP_CMP_OR:
                    return "OP_CMP_OR";
                case OP_CMP_NOT:
                    return "OP_CMP_NT";
                case CONST_INT:
                    return "CONST_INT";
                case CONST_FLOAT:
                    return "CONST_FLT";
                case CONST_STR:
                    return "CONST_STR";
                default:
                    return "---";
            }
        }
    }
}
