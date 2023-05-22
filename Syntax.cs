using System;
using System.Collections.Generic;

namespace ST_1
{
    class Syntax
    {
        private List<Token> _tokens;
        private int _index;

        private SyntaxNode _rootNode;

        public SyntaxNode RootNode => _rootNode;

        public Syntax(List<Token> tokens)
        {
            _tokens = tokens;
            _index = 0;
            if (!CheckSyntax()) throw new Exception("");
        }

        /// <summary>
        /// Обновляет счетчик в зависимости от выражения
        /// </summary>
        /// <param name="exp"> выражение </param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool UpdateIndex(bool exp, out int index)
        {
            if (exp)
            {
                index = _index;
                _index++;
            }
            else
            {
                index = -1;
            }
            return exp;
        }

        private bool CheckSyntax() => Logic(out _rootNode) && _tokens[_index].type == Tokenizer.TOK_EOF;

        private bool Logic(out SyntaxNode node)
        {
            node = null;
            return Logic2(out SyntaxNode left) && LogicTail(left, out node);
        }

        private bool Logic2(out SyntaxNode node)
        {
            node = null;
            return LogicTerm(out SyntaxNode left) && LogicTail2(left, out node);
        }

        /// <summary>
        /// Проверяет является ли токен опеартором !
        /// </summary>
        /// <param name="index"> индекс токена</param>
        /// <returns>true - если токен равен оператору ! </returns>
        private bool IsNot(out int index) => UpdateIndex(_tokens[_index].subType == Tokenizer.OP_CMP_NOT, out index);

        /// <summary>
        /// ( x < y ) || ! ( x < y )
        /// Является ли выражение обычным или с оператором !
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool LogicTerm(out SyntaxNode node)
        {
            if (IsNot(out int index) && Comp(out node))
            {
                node = SyntaxNode.NewOp(index, node, null);
                return true;
            }
            return Comp(out node);
        }

        /// <summary>
        /// Проверяет является ли токен разделителем
        /// </summary>
        /// <param name="brType"> индекс токена </param>
        /// <returns> true - если токен равен соответствующему разделителю '(' или ')' </returns>
        private bool IsBr(int brType) => UpdateIndex(_tokens[_index].subType == brType, out int index);

        /// <summary>
        /// Проверяет является ли токен Const или Id
        /// </summary>
        /// <returns> true - если токен равен Const или Id </returns>
        private bool IsTerm() => _tokens[_index].type == Tokenizer.TOK_ID || _tokens[_index].type == Tokenizer.TOK_CONST;

        private bool CompTerm(out int index) => UpdateIndex(IsTerm(), out index);

        /// <summary>
        /// Проверяет является ли токен оператором сравнения '<', '>', '=='.
        /// </summary>
        /// <returns> true - если токен является оператором сравнения '<', '>', '=='. </returns>
        private bool IsOp() => _tokens[_index].type == Tokenizer.TOK_OP && _tokens[_index].subType <= Tokenizer.TOK_OP + 6;

        private bool CompOp(out int index) => UpdateIndex(IsOp(), out index);


        private bool Comp(out SyntaxNode node)
        {
            int prevIndex = _index;
            node = null;
            //если левая скобка проверяет логику
            if (IsBr(Tokenizer.DIV_LPR))
                return Logic(out node) && IsBr(Tokenizer.DIV_RPR);
            //( x < y )  ( left operator right )  ( arg op arg )
            if (CompTerm(out int left) && CompOp(out int op) && CompTerm(out int right))
            {
                node = SyntaxNode.NewOp(op, SyntaxNode.NewArg(left), SyntaxNode.NewArg(right));
                return true;
            }
            _index = prevIndex;
            if (CompTerm(out left))
            {
                node = SyntaxNode.NewArg(left);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Проверяет является ли токен оператором "&&".
        /// </summary>
        /// <param name="index"></param>
        /// <returns> true - если токен является оператором "&&". </returns>
        private bool IsAnd(out int index) => UpdateIndex(_tokens[_index].subType == Tokenizer.OP_CMP_AN, out index);

        /// <summary>
        /// Проверяет является ли токен оператором "||".
        /// </summary>
        /// <param name="index"></param>
        /// <returns> true - если токен является оператором "||". </returns>
        private bool IsOr(out int index) => UpdateIndex(_tokens[_index].subType == Tokenizer.OP_CMP_OR, out index); 


        /// <summary>
        /// Проверяет закончилась ли логика или имеет оператор ||
        /// </summary>
        /// <param name="left"></param>
        /// <param name="node"></param>
        /// <returns> true - если выражение закончилось </returns>
        private bool LogicTail(SyntaxNode left, out SyntaxNode node)
        {
            node = left;
            if (IsOr(out int index))
                return Logic2(out SyntaxNode right) && LogicTail(SyntaxNode.NewOp(index, left, right), out node);
            return true;
        }

        /// <summary>
        /// Проверяет закончилась ли логика или имеет оператор &&
        /// </summary>
        /// <param name="left"></param>
        /// <param name="node"></param>
        /// <returns> true - если выражение закончилось </returns>
        private bool LogicTail2(SyntaxNode left, out SyntaxNode node)
        {
            node = left;
            if (IsAnd(out int index))
                return LogicTerm(out SyntaxNode right) && LogicTail2(SyntaxNode.NewOp(index, left, right), out node);
            return true;
        }
    }
}
