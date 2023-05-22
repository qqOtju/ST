using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST_1
{
    class Symbols
    {
        private Dictionary<string, IdInfo> _idPair;
        private Dictionary<string, IdInfo> _idMap; 

        public bool AddIdent(Dictionary<string, IdInfo> Map, string S)
        {
            var id = new IdInfo();
            if (Map == null || string.IsNullOrEmpty(S) || S[0] == ';' || !id.GetInfo(S)) return false;
            Map.Add(id.IdName, id);
            return true;
        }

        public Dictionary<string, IdInfo> CreateIdMap(IEnumerable<string> S)
        {
            if (S == null) return null;
            var Map = new Dictionary<string, IdInfo>();
            foreach (var item in S) AddIdent(Map, item);
            return Map;
        }
    }
}
