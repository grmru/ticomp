using System;
using System.Collections.Generic;

namespace ticomp
{
    public class RefData
    {
        public Dictionary<string, int> RefPictureNumbers { get; set; } = new Dictionary<string, int>();
        public int LastRefPictureNumber { get; set; } = 1;
        public Dictionary<string, int> RefTableNumbers { get; set; } = new Dictionary<string, int>();
        public int LastRefTableNumber { get; set; } = 1;
        public Dictionary<string, int> RefFormulaNumbers = new Dictionary<string, int>();
        public int LastRefFormulaNumber { get; set; } = 1;
        public Dictionary<string, int> RefListNumbers = new Dictionary<string, int>();
        public int LastRefListNumber { get; set; } = 1;
    }

}