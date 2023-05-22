namespace ST_1
{
    public struct Token
    {
        public int type;
        public int subType;
        public string text;

        public Token(int type, int subType, string text)
        {
            this.type = type;
            this.subType = subType;
            this.text = text;
        }
    }
}
