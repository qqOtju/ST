namespace ST_1
{
    class StackItem
    {
        public string Str { get; private set; }
        public IdType Type { get; private set; }

        public StackItem(string str, IdType type)
        {
            Str = str;
            Type = type;
        }
    }
}
