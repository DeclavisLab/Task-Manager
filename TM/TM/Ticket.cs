using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TM
{
    enum state { Todo, InProgress, Closed }

    class Ticket
    {
        string name;
        string description;
        state mystate;
        string assign;
        DateTime created;
        DateTime changed;

        public Ticket(string _name, string _des, state _my, string _ass)
        {
            created = changed = DateTime.Now;
            name = _name;
            description = _des;
            mystate = _my;
            assign = _ass;
        }

    }
}
