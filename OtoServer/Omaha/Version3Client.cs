using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using ServiceStack.ServiceHost;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml;
using System.Xml.Schema;

namespace OtoServer.Omaha
{
    namespace V3
    {
        [Route("/service/update2")]
        public class OmahaClient : IXmlSerializable
        {
            public string protocol;
            public string version;
            public string ismachine;
            public string sessionid;
            public string userid;
            public string installsource;
            public string testsource;
            public string requestid;



            public OSInfo os;
            public List<AppInfoRequest> apps;

            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }
            public void ReadXml(XmlReader r)
            {
                while (r.MoveToNextAttribute())
                {
                            if (r.Name == "protocol") protocol = r.Value;
                            else if (r.Name == "version") version = r.Value;
                            else if (r.Name == "ismachine") ismachine = r.Value;
                            else if (r.Name == "sessionid") sessionid = r.Value;
                            else if (r.Name == "userid") userid = r.Value;
                            else if (r.Name == "installsource") installsource = r.Value;
                            else if (r.Name == "testsource") testsource = r.Value;
                            else if (r.Name == "requestid") requestid = r.Value;
                }
                while (r.Read())
                {
                    switch (r.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (r.Name == "os")
                            {
                                XmlReader s = r.ReadSubtree();
                                s.Read();
                                os = new OSInfo();
                                os.ReadXml(s);
                                s.Close();
                            }
                            else if (r.Name == "app")
                            {
                                if (apps == null)
                                    apps = new List<AppInfoRequest>();
                                XmlReader s = r.ReadSubtree();
                                s.Read();
                                AppInfoRequest ai_r = new AppInfoRequest();
                                ai_r.ReadXml(s);
                                s.Close();
                                apps.Add(ai_r);
                            }
                            break;
                    }
                }

                r.Read();
                r.ReadAttributeValue();
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("protocol", protocol);
                w.WriteAttributeString("version", version);
                w.WriteAttributeString("ismachine", version);
                w.WriteAttributeString("sessionid", sessionid);
                w.WriteAttributeString("userid", userid);
                w.WriteAttributeString("installsource", installsource);
                w.WriteAttributeString("testsource", testsource);
                w.WriteAttributeString("requestid", requestid);

                w.WriteStartElement("os");
                os.WriteXml(w);
                w.WriteEndElement();
                foreach (AppInfoRequest app in apps)
                {
                    w.WriteStartElement("app");
                    app.WriteXml(w);
                    w.WriteEndElement();
                }
            }
            #endregion IXmlSerializable
        }

        [Serializable]
        public class OSInfo : IXmlSerializable
        {
            public string platform;
            public string version;
            public string sp;
            public string arch;

            public XmlSchema GetSchema() { return null; }
            public void ReadXml(XmlReader r)
            {
                while (r.MoveToNextAttribute())
                {
                    if (r.Name == "platform") platform = r.Value;
                    else if (r.Name == "version") version = r.Value;
                    else if (r.Name == "sp") sp = r.Value;
                    else if (r.Name == "arch") arch = r.Value;
                }
            }
            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("platform", platform);
                w.WriteAttributeString("version", version);
                w.WriteAttributeString("sp", sp);
                w.WriteAttributeString("arch", arch);
            }
        }

        public class AppInfoRequest : IXmlSerializable
        {
            public string appid;
            public string version;
            public string nextversion;
            public string lang;
            public string brand;
            public string client;
            public uint installage;

            public UpdateCheck updatecheck;
            public PingRequest ping;

            public List<EventReport> events;

            public List<DataRequest> data;

            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }
            public void ReadXml(XmlReader r)
            {
                while (r.MoveToNextAttribute())
                {
                    if (r.Name == "appid") appid = r.Value;
                    else if (r.Name == "version") version = r.Value;
                    else if (r.Name == "nextversion") nextversion = r.Value;
                    else if (r.Name == "lang") lang = r.Value;
                    else if (r.Name == "brand") brand = r.Value;
                    else if (r.Name == "client") client = r.Value;
                    else if (r.Name == "installage") installage = UInt32.Parse(r.Value);
                }

                while (r.Read())
                {
                    if (r.NodeType == XmlNodeType.Element)
                    {
                        if (r.Name == "updatecheck")
                        {
                            updatecheck = new UpdateCheck();
                            XmlReader s = r.ReadSubtree();
                            s.Read();
                            updatecheck.ReadXml(s);
                            s.Close();
                        }
                        else if (r.Name == "ping")
                        {
                            ping = new PingRequest();
                            XmlReader s = r.ReadSubtree();
                            s.Read();
                            ping.ReadXml(s);
                            s.Close();
                        }
                        else if (r.Name == "event")
                        {
                            if (events == null)
                                events = new List<EventReport>();
                            EventReport e_rpt = new EventReport();
                            XmlReader s = r.ReadSubtree();
                            s.Read();
                            e_rpt.ReadXml(s);
                            s.Close();
                            events.Add(e_rpt);
                        }
                        else if (r.Name == "data")
                        {
                            if (data == null)
                                data = new List<DataRequest>();
                            DataRequest d_rpt = new DataRequest();
                            XmlReader s = r.ReadSubtree();
                            s.Read();
                            d_rpt.ReadXml(s);
                            s.Close();
                            data.Add(d_rpt);
                        }
                    }
                }

            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("appid", appid);
                w.WriteAttributeString("version", version);
                w.WriteAttributeString("nextversion", nextversion);
                w.WriteAttributeString("lang", lang);
                w.WriteAttributeString("brand", brand);
                w.WriteAttributeString("client", client);
                w.WriteAttributeString("installage", installage.ToString());

                if (updatecheck != null)
                {
                        w.WriteStartElement("updatecheck");
                        updatecheck.WriteXml(w);
                        w.WriteEndElement();
                }

                if (ping != null)
                {
                        w.WriteStartElement("ping");
                        ping.WriteXml(w);
                        w.WriteEndElement();
                }

                if( events != null )
                    foreach( EventReport evrpt in events )
                    {
                        w.WriteStartElement("event");
                        evrpt.WriteXml(w);
                        w.WriteEndElement();
                    }

                if( data != null )
                    foreach( DataRequest datum in data )
                    {
                        w.WriteStartElement("data");
                        datum.WriteXml(w);
                        w.WriteEndElement();
                    }

            }
            #endregion IXmlSerializable
        }


        public class UpdateCheck
        {

            #region IXmlSerializable

            public XmlSchema GetSchema() { return null; }
            public void ReadXml(XmlReader r)
            {
            }

            public void WriteXml(XmlWriter w)
            {
            }

            #endregion IXmlSerializable
        }


        public class PingRequest
        {
            public string r;

            #region IXmlSerializable

            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                reader.MoveToNextAttribute();
                if (reader.Name == "r")
                    r = reader.Value;
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("r", r);
            }
            #endregion IXmlSerializable
        }

        [Serializable]
        public class EventReport
        {
            public Int32 eventtype;
            public Int32 eventresult;
            public Int32 errorcode;
            public Int32 extracode1;


            #region IXmlSerializable

            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader r)
            {
                while (r.MoveToNextAttribute())
                {
                    if (r.Name == "eventtype") eventtype = Int32.Parse( r.Value);
                    else if (r.Name == "eventresult")eventresult = Int32.Parse( r.Value);
                    else if (r.Name == "errorcode") errorcode = Int32.Parse( r.Value);
                    else if (r.Name == "extracode1") extracode1 = Int32.Parse( r.Value);
                }
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("eventtype", eventtype.ToString());
                w.WriteAttributeString("eventresult", eventresult.ToString());
                w.WriteAttributeString("errorcode", errorcode.ToString());
                w.WriteAttributeString("extracode1", extracode1.ToString());
            }
            #endregion IXmlSerializable

        }

        public class DataRequest
        {
            public string name;
            public string index;
            public string data;


            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader r)
            {
                while (r.MoveToNextAttribute())
                {
                    if (r.Name == "name") name = r.Value;
                    else if (r.Name == "index") index = r.Value;
                }

                while (r.Read())
                {
                    if (r.NodeType == XmlNodeType.Text)
                        data = r.Value;
                }
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("name", name);
                w.WriteAttributeString("index", index);
                w.WriteString(data);
            }
            #endregion IXmlSerializable
        }

    }
}