using System;
using System.Collections.Generic;

namespace ST_1
{
    class SyntaxNode
    {
        public const int OP = 1;
        public const int ARG = 2;

        private int _type;
        private int _index;

        private SyntaxNode _leftNode;
        private SyntaxNode _rightNode;

        public SyntaxNode(int type, int index, SyntaxNode left, SyntaxNode right)
        {
            _type = type;
            _index = index;
            _leftNode = left;
            _rightNode = right;
        }

        public bool IsOp() => _type == OP;
        public bool IsArg() => _type == ARG;
        public static SyntaxNode NewOp(int index, SyntaxNode left, SyntaxNode right) 
            => new SyntaxNode(OP, index, left, right);
        public static SyntaxNode NewArg(int index) => new SyntaxNode(ARG, index, null, null);

        public static void ToList(SyntaxNode node, List<Token> tokens, List<String> list, String tab)
        {
            var res = tab;
            var token = tokens[node._index];
            res += node._type == ARG ? "Arg  " : "Op  ";
            res += token.text;
            list.Add(res);
            if (node._leftNode != null)  ToList(node._leftNode, tokens, list, tab + "\t");
            if (node._rightNode != null) ToList(node._rightNode, tokens, list, tab + "\t");
        }
        public static String ToPostfixStr(SyntaxNode node, List<Token> tokens, String tab)
        {
            var token = tokens[node._index];
            if (node._type == ARG)
                return token.text + tab;
            else
                if (token.subType == Tokenizer.OP_CMP_NOT)
                    return ToPostfixStr(node._leftNode, tokens, tab) + token.text + tab;
                else
                    return ToPostfixStr(node._leftNode, tokens, tab) + ToPostfixStr(node._rightNode, tokens, tab) + token.text + tab;

        }
        public static void ToPostfix(SyntaxNode node, List<Token> list, List<Token> tokens)
        {
            var token = tokens[node._index];
            if (node._type == ARG)
                list.Add(token);
            else
                if (token.subType == Tokenizer.OP_CMP_NOT) {
                    ToPostfix(node._leftNode, list, tokens);
                    list.Add(token);
                }
                else{
                    ToPostfix(node._leftNode, list,tokens);
                    ToPostfix(node._rightNode, list, tokens);
                    list.Add(token);
                }
        }
    }
}
