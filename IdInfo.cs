using System;

namespace ST_1
{
    public enum IdType { IdUnknown, IdBool, IdInt, IdFloat, IdStr, IdDate };

    public class IdInfo
    {
        public string IdName { get; private set; }
        public IdType IdType { get; private set; }
        public string LLName { get; private set; }
        public int LLField { get; private set; }
        
        public bool GetInfo(string str)
        {
            if (string.IsNullOrEmpty(str) || str[0] == ';')
                return false;

            string[] sa = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (sa.Length < 2)
                return false;

            IdName = sa[0].ToUpper();
            sa[1] = sa[1].ToUpper();

            switch (sa[1])
            {
                case "BOOL":
                    IdType = IdType.IdBool;
                    break;
                case "INT":
                    IdType = IdType.IdInt;
                    break;
                case "FLT":
                    IdType = IdType.IdFloat;
                    break;
                case "STR":
                    IdType = IdType.IdStr;
                    break;
                case "DATE":
                    IdType = IdType.IdDate;
                    break;
                default:
                    return false;
            }

            LLName = "%" + IdName;
            LLField--;  

            if (sa.Length >= 4)
            {
                LLName = "%" + sa[2];
                LLField = int.Parse(sa[3]);
            }

            return true;
        }
    }
}
