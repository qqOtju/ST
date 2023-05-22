using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace ST_1
{
    public partial class Form1 : Form
    {
        private const string FilterMark = ";%%_FILTER_%%";
        private List<Token> _postFixTokens; 
        private Tokenizer _tockenizer;
        private Syntax _syntax;

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "( 10 < 15 ) && ( 6 < 4 )";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            richTextBox3.Text = "";
            richTextBoxLLVM.Text = "";
            richTextBoxSyntaxTree.Text = "";
            Tokenize();
            CheckSyntax();
            Compile();
        }

        private void Tokenize()
        { 
            var _newTockenizer = new NewTockenizer(textBox1.Text);
            var txt = "";
            for (int i = 0; i < _newTockenizer.Tokens.Count; i++)
            {
                txt += string.Format("#{0,-2}\t| {1,-10}\t| {2, -20}\t| {3,-5}\n",
                         i, Tokenizer.GetTypeName(_newTockenizer.Tokens[i].type),
                         Tokenizer.GetSubTypeName(_newTockenizer.Tokens[i].subType), _newTockenizer.Tokens[i].text);
            }
            newTokensTextBox.Text = txt;
            try
            {
                _tockenizer = new Tokenizer(textBox1.Text);
                var text1 = "";
                var text2 = "";
                int divC = 0, opC = 0, constC = 0, idC = 0;
                int count = 0;
                foreach (var tkn in _tockenizer.Tokens)
                {
                    switch (tkn.type)
                    {
                        case 100:
                            divC++;
                            break;
                        case 200:
                            opC++;
                            break;
                        case 300:
                            constC++;
                            break;
                        case 1000:
                            idC++;
                            break;
                        default:
                            break;
                    }
                    text1 += string.Format("#{0,-2}\t| {1,-10}\t| {2, -20}\t| {3,-5}\n", ++count, Tokenizer.GetTypeName(tkn.type), Tokenizer.GetSubTypeName(tkn.subType), tkn.text);
                }
                var str = "{0,-10}\t|{1,-5}\n";
                text2 = String.Format(str, "Div", divC) + String.Format(str, "Op", opC) +
                    String.Format(str, "Const", constC) + String.Format(str, "Id", idC);
                richTextBox1.Text += text1;
                richTextBox2.Text = text2;
                label1.Text += "Tokenize complete! ";
            }
            catch
            {
                label1.Text += "Tokenize error! ";
            }
        }

        private void CheckSyntax()
        {
            if (_tockenizer == null || _tockenizer.Tokens == null)
                return;
            try
            {
                _syntax = new Syntax(_tockenizer.Tokens);
                SyntaxNode rootNode = _syntax.RootNode;
                List<string> strings = new List<string>();
                SyntaxNode.ToList(rootNode, _tockenizer.Tokens, strings, "");
                foreach (var str in strings)
                    richTextBoxSyntaxTree.Text += str + "\n";
                _postFixTokens = new List<Token>();
                SyntaxNode.ToPostfix(rootNode, _postFixTokens, _tockenizer.Tokens);
                int count = 0;
                foreach (var tkn in _postFixTokens)
                    richTextBox3.Text += string.Format("#{0,-2}\t| {1,-10}\t| {2, -20}\t| {3,-5}\n",
                        ++count, Tokenizer.GetTypeName(tkn.type), Tokenizer.GetSubTypeName(tkn.subType), tkn.text);

                richTextBox3.Text += "\n\n" + SyntaxNode.ToPostfixStr(rootNode, _tockenizer.Tokens, " ");
                label1.Text += "Syntax complete! ";
            }
            catch (Exception exc)
            {
                label1.Text += "Syntax error! ";
            }
        }

        private void Compile()
        {
            if (_postFixTokens == null)
                return;
            try
            {
                Compiler myCompiler = new Compiler(_postFixTokens);
                var llCode = richTextBoxLLVM.Text = myCompiler.EmitLLVM();
                label1.Text += "Compile complete! ";
                string stuff = "";
                var file = @"D:\languages\C#\ST№1\Template.ll";
                if (File.Exists(file)) 
                { 
                    var template = File.ReadAllText(file);
                    stuff = myCompiler.InjectLLVM(llCode, template, FilterMark);
                    stuff = myCompiler.InjectLLVM(llCode, stuff, ";%%_GREATERTHEN_%%");
                }
                else
                {
                    Console.WriteLine("file error");
                }
                File.WriteAllText(@"D:\languages\C#\ST№1\Stuff.ll", stuff);
                //var cmdText = @""C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Tools\Llvm\x64\bin\clang.exe" main.ll stuff.ll -o main.exe";
                //System.Diagnostics.Process.Start("CMD.exe", cmdText);
                //template
                //generated ll code 
                //mark to spot 
                //myCompiler.InjectLLVM();
            }
            catch
            {
                label1.Text += "Compile error! ";
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBoxLLVM_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
