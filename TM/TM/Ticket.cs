using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace TM
{
    public enum state { Todo, InProgress, Closed }

    [XmlType("Ticket")]
    //[XmlInclude(typeof(Ticket))]
    public class Ticket
    {
        [XmlAttribute("name", DataType = "string")]
        public string name { get; set; }

        [XmlElement("description")]
        public string description { get; set; }
      
        [XmlElement("state")]
        public state mystate { get; set; }

        [XmlElement("assign")]
        public string assign { get; set; }

        [XmlElement("created")]
        public DateTime created { get; set; }

        [XmlElement("changed")]
        public DateTime changed { get; set; }

        public Ticket(string _name, string _des, state _my, string _ass, DateTime cr)
        {
            created = cr;
            changed = DateTime.Now;
            name = _name;
            description = _des;
            mystate = _my;
            assign = _ass;
        }

        public Ticket() { }

        public void changenow()
        {
            changed = DateTime.Now;
        }
    }

    [XmlRoot("TicketList")]
    public class TicketList
    {
        public TicketList() { Items = new List<Ticket>(); }
        [XmlElement("user")]
        public List<Ticket> Items { get; set; }

        [XmlElement]
        public string Version;   
    }
}
