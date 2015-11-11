using System.Collections.Generic;

namespace ProcCore.Web
{
    public class GridInfo2<T>
    {
        public int total  {get;set;}
        public int page { get; set; }
        public int records { get; set; }
        public int startcount { get; set; }
        public int endcount { get; set; }
        public IEnumerable<T> rows { get; set; }
    }
}
