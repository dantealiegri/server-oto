using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace OtoServer.Omaha
{
    namespace V3
    {
        [Serializable, XmlRoot( ElementName = "request")]
        public class Request
        {
            [XmlAttribute]
            public string protocol;
            [XmlAttribute]
            public string version;
            [XmlAttribute]
            public string ismachine;
            [XmlAttribute]
            public string sessionid;
            [XmlAttribute]
            public string userid;
            [XmlAttribute]
            public string installsource;
            [XmlAttribute]
            public string testsource;
            [XmlAttribute]
            public string requestid;



            public OSInfo os;
            // serialize list items in Request.
            [XmlElement("app")]
            public List<AppInfoRequest> apps;
        }

        [Serializable]
        public class OSInfo
        {
            [XmlAttribute]
            public string platform;
            [XmlAttribute]
            public string version;
            [XmlAttribute]
            public string sp;
            [XmlAttribute]
            public string arch;
        }

        [Serializable]
        public class AppInfoRequest
        {
            [XmlAttribute]
            public string appid;
            [XmlAttribute]
            public string version;
            [XmlAttribute]
            public string nextversion;
            [XmlAttribute]
            public string lang;
            [XmlAttribute]
            public string brand;
            [XmlAttribute]
            public string client;
            [XmlAttribute]
            public uint installage;

            public UpdateCheck updatecheck;
            public PingRequest ping;

            [XmlElement("event")]
            public List<EventReport> events;

            [XmlElement("data")]
            public List<DataRequest> data;

        }


        [Serializable]
        public class UpdateCheck
        {
        }


        [Serializable]
        public class PingRequest
        {
            [XmlAttribute]
            public string r;
        }

        [Serializable]
        public class EventReport
        {
            [XmlAttribute]
            public Int32 eventtype;
            [XmlAttribute]
            public Int32 eventresult;
            [XmlAttribute]
            public Int32 errorcode;
            [XmlAttribute]
            public Int32 extracode1;
        }

        public class DataRequest
        {
            [XmlAttribute]
            public string name;
            [XmlAttribute]
            public string index;
            [XmlText]
            public string data;
        }

    }
}