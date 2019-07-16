using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class CenterStackRow
    {
        public CenterStack CenterStack1 { get; set; }
        public CenterStack CenterStack2 { get; set; }
        public CenterStack CenterStack3 { get; set; }
        public CenterStack CenterStack4 { get; set; }

        public CenterStackRow()
        {
            CenterStack1 = new CenterStack();
            CenterStack2 = new CenterStack();
            CenterStack3 = new CenterStack();
            CenterStack4 = new CenterStack();
        }
    }
}
