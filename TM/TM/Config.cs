using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TM
{
    [XmlRoot("Config")]
    public class Config
    {
        public Config() { }

        [XmlElement]
        public string gitpath;

        [XmlElement]
        public bool git_e;
    }
}
