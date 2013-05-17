using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace OtoServer.Omaha
{
    namespace V3
    {
        [Serializable, XmlElement("response")]
        public class Result
        {
            [XmlAttribute]
            public string protocol;
            [XmlAttribute]
            public string server;

            public DayStart daystart;
            [XmlElement("app")]
            public List<AppInfoResult> app_results;
        }

        [Serializable]
        public class DayStart
        {
            [XmlAttribute]
            public UInt32 elapsed_seconds;
        }

        [Serializable]
        public class AppInfoResult
        {
            [XmlAttribute]
            public string appid;
            [XmlAttribute]
            public string status;

            public UpdateResult updatecheck;
            public PingResult ping;
        }

        [Serializable]
        public class PingResult
        {
            [XmlAttribute]
            public string status;
        }

        [Serializable]
        public class UpdateResult
        {
            [XmlAttribute]
            public string status;

            [XmlArrayItem(ElementName="url", Type = typeof(UpdateUrl))]
            public List<UpdateUrl> urls;
            public UpdateManifest manifest;
        }

        public class UpdateUrl
        {
            [XmlAttribute]
            public string codebase;
        }

        public class UpdateManifest
        {
            [XmlAttribute]
            public string version;

            [XmlArray("packages"), XmlArrayItem( ElementName="package")]
            public List<UpdatePackage> updated_packages;
            [XmlArray("actions"), XmlArrayItem(ElementName = "action")]
            public List<UpdateAction> update_actions;
        }

        public class UpdatePackage
        {
            [XmlAttribute]
            public string hash;
            [XmlAttribute]
            public string name;
            [XmlAttribute]
            public string required;
            [XmlAttribute]
            public UInt32 size;
        }

        public class UpdateAction
        {
            [XmlAttribute("event")]
            public string on_event;
            [XmlAttribute]
            public string arguments;
            [XmlAttribute]
            public string run;
            [XmlAttribute]
            public string onsuccess;
            [XmlAttribute]
            public string version;

        }
    }
}