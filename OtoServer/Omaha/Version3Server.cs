using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace OtoServer.Omaha
{
    namespace V3
    {
        [Serializable]
        public class Result
        {
            [XmlAttribute]
            public string protocol;
            [XmlAttribute]
            public string server;

            public DayStart daystart;
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
            public string hash;
            public string name;
            public string required;
            public UInt32 size;
        }

        public class UpdateAction
        {
            [XmlAttribute("event")]
            public string on_event;
            public string arguments;
            public string run;
            public string onsuccess;
            public string version;

        }
    }
}