using LLVMSharp;
using System.Collections.Generic;
using System;

namespace ST_1
{
    public class Compiler
    {
        private List<Token> tokens;
        private Dictionary<string, IdInfo> map;

        public Compiler(List<Token> tokens)
        {
            map = new Dictionary<string, IdInfo>();
            this.tokens = tokens;
            foreach(var tkn in tokens)
                if(tkn.type == Tokenizer.TOK_ID)
                    AddIdent(tkn.text);
        }

        private bool AddIdent(string s)
        {
            IdInfo id = new IdInfo();
            if (s == "")
                return false;
            if (!id.GetInfo(s))
                return false;
            map[id.IdName] = id;
            return true;
        }

        private StackItem SI(string n, IdType t) => new StackItem(n, t);

        private string NewReg(ref int idx) => $"%var.{idx++}";

        private string Command_Not(Stack<StackItem> St, ref int idx)
        {
            var LArg = St.Pop();
            var res = NewReg(ref idx);
            var resultStr = $"{res} = xor i1 {LArg.Str}, true\n";
            St.Push(SI(res, IdType.IdBool));
            return resultStr;
        }

        private string Command_Binary(Stack<StackItem> St, ref int idx, string OpCode)
        {
            var RArg = St.Pop();
            var LArg = St.Pop();
            var res = NewReg(ref idx);
            var resultString = $"{res} = {OpCode} i1 {LArg.Str}, {RArg.Str}\n";
            St.Push(SI(res, IdType.IdBool));
            return resultString;
        }

        private string Command_cmp_int(Stack<StackItem> St, ref int idx, 
            int CmpCode, string L, string R, string IntType = "i32")
        {
            string CmpName,
            res = NewReg(ref idx);
            switch (CmpCode)
            {
                case Tokenizer.OP_CMP_EQ:
                    CmpName = "eq";
                    break;
                case Tokenizer.OP_CMP_NE:
                    CmpName = "ne";
                    break;
                case Tokenizer.OP_CMP_LS:
                    CmpName = "slt";
                    break;
                case Tokenizer.OP_CMP_GR:
                    CmpName = "sgt";
                    break;
                case Tokenizer.OP_CMP_LE:
                    CmpName = "sle";
                    break;
                case Tokenizer.OP_CMP_GE:
                    CmpName = "sge";
                    break;
                default:
                    throw new Exception("Invalid CmpCode");
            }
            St.Push(SI(res, IdType.IdBool));
            return $"{res} = icmp {CmpName} {IntType} {L}, {R}\n";
        }

        string Command_cmp_flt(Stack<StackItem> St, ref int idx, int CmpCode, string L, string R, string FloatType = "float")
        {
            string CmpName,
            res = NewReg(ref idx);
            switch (CmpCode)
            {
                case Tokenizer.OP_CMP_EQ:
                    CmpName = "oeq";
                    break;
                case Tokenizer.OP_CMP_NE:
                    CmpName = "one";
                    break;
                case Tokenizer.OP_CMP_LS:
                    CmpName = "olt";
                    break;
                case Tokenizer.OP_CMP_GR:
                    CmpName = "ogt";
                    break;
                case Tokenizer.OP_CMP_LE:
                    CmpName = "ole";
                    break;
                case Tokenizer.OP_CMP_GE:
                    CmpName = "oge";
                    break;
                default:
                    throw new System.Exception("Invalid CmpCode");
            }
            var result = $"{res} = fcmp {CmpName} {FloatType} {L}, {R}\n";
            St.Push(SI(res, IdType.IdBool));
            return result;
        }

        private string Command_cmp_str(Stack<StackItem> St, ref int idx, int CmpCode, string L, string R)
        {
            string tmp = NewReg(ref idx);
            string CmpName, 
            result = $"{tmp} = call i32 @strcmp(ptr {L}, ptr {R})\n",
            res = NewReg(ref idx);
            switch (CmpCode)
            {
                case Tokenizer.OP_CMP_EQ:
                    CmpName = "eq";
                    break;
                case Tokenizer.OP_CMP_NE:
                    CmpName = "ne";
                    break;
                case Tokenizer.OP_CMP_LS:
                    CmpName = "slt";
                    break;
                case Tokenizer.OP_CMP_GR:
                    CmpName = "sgt";
                    break;
                case Tokenizer.OP_CMP_LE:
                    CmpName = "sle";
                    break;
                case Tokenizer.OP_CMP_GE:
                    CmpName = "sge";
                    break;
                default:
                    throw new Exception("Invalid CmpCode");
            }

            St.Push(SI(res, IdType.IdBool));
            return $"{result}{res} = icmp {CmpName} i32 {tmp}, 0\n";
        }


        private string Command_Cmp(int CmpCode, Stack<StackItem> St, ref int idx)
        {
            var RArg = St.Pop();
            var LArg = St.Pop();
            switch (LArg.Type)
            {
                case IdType.IdBool:
                    return Command_cmp_int(St, ref idx, CmpCode, LArg.Str, RArg.Str, "i8");
                case IdType.IdInt:
                    return Command_cmp_int(St, ref idx, CmpCode, LArg.Str, RArg.Str);
                case IdType.IdFloat:
                    return Command_cmp_flt(St, ref idx, CmpCode, LArg.Str, RArg.Str);
                case IdType.IdStr:
                    return Command_cmp_str(St, ref idx, CmpCode, LArg.Str, RArg.Str);
                default:
                    return "";
            }
        }

        private string FloatTextToHex(string T, bool IsSingle)
        {
            if (!double.TryParse(T,out var D) || !float.TryParse(T,out var S))
                return string.Empty;
            if (IsSingle)
                D = S;
            return $"0x{BitConverter.DoubleToInt64Bits(D):X16}";
        }

        private string StringTextToConst(string T, out string str)
        {
            var result = $"@\"{T.ToUpper()}\"";
            str = $"{result} = linkonce_odr dso_local unnamed_addr constant [{T.Length + 1} x i8] c\"{T}\\00\", align 1\n";
            return result;
        }

        private void PushId(Stack<StackItem> St, ref int idx, IdInfo Id, ref string code)
        {
            if (Id.LLField == -1)
            {
                St.Push(SI(Id.LLName, Id.IdType));
                return;
            }

            var ptr = NewReg(ref idx);
            code += $"{ptr} = getelementptr inbounds %struct.Customer, ptr {Id.LLName}, i64 0, i32 {Id.LLField}";

            if (Id.IdType == IdType.IdStr)
            {
                St.Push(SI(ptr, Id.IdType));
                return;
            }

            var val = NewReg(ref idx);
            var res = "";
            switch (Id.IdType)
            {
                case IdType.IdBool:
                    res = $"{val} = load i8, ptr {ptr}";
                    break;
                case IdType.IdInt:
                    res = $"{val} = load i32, ptr {ptr}";
                    break;
                case IdType.IdFloat:
                    res = $"{val} = load float, ptr {ptr}";
                    break;
                default:
                    throw new Exception("Unsupported IdType");
            }
            code += res;
            St.Push(SI(val, Id.IdType));
        }


        public string EmitLLVM(int startIdx = 0)
        {
            var res_code = "";
            var St = new Stack<StackItem>();
            var idx = startIdx;

            foreach (var token in tokens)
                if (token.type == Tokenizer.TOK_ID)
                    PushId(St, ref idx, map[token.text], ref res_code);
                else if (token.type == Tokenizer.TOK_CONST)
                    switch (token.subType)
                    {
                        case Tokenizer.CONST_INT:
                            St.Push(SI(token.text, IdType.IdInt));
                            break;
                        case Tokenizer.CONST_STR:
                            St.Push(SI(StringTextToConst(token.text, out var res_const), IdType.IdStr));
                            break;
                        case Tokenizer.CONST_FLOAT:
                            St.Push(SI(FloatTextToHex(token.text, true), IdType.IdFloat));
                            break;
                    }
                else if (token.type == Tokenizer.TOK_OP)
                    switch (token.subType)
                    {
                        case Tokenizer.OP_CMP_EQ:
                        case Tokenizer.OP_CMP_NE:
                        case Tokenizer.OP_CMP_LS:
                        case Tokenizer.OP_CMP_GR:
                        case Tokenizer.OP_CMP_LE:
                        case Tokenizer.OP_CMP_GE:
                            res_code += Command_Cmp(token.subType, St, ref idx);
                            break;
                        case Tokenizer.OP_CMP_AN:
                            res_code += Command_Binary(St, ref idx, "and");
                            break;
                        case Tokenizer.OP_CMP_OR:
                            res_code += Command_Binary(St, ref idx, "or");
                            break;
                        case Tokenizer.OP_CMP_NOT:
                            res_code += Command_Not(St, ref idx);
                            break;
                    }

            var Res = St.Pop();
            res_code += $"ret i1 {Res.Str}";
            return res_code;
        }
        public string InjectLLVM(string llvm,string template, string codeMark)
        {
            if (!template.Contains(codeMark)) return null;
            if (!string.IsNullOrEmpty(llvm)) template = template.Replace(codeMark, llvm);
            return template;
        }
    }
}



