using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class PlayerHand
    {
        private List<Card> _cards = new List<Card>();
        public List<Card> Cards
        {
            get { return _cards; }
            set { _cards = value; }
        }
    }
}
