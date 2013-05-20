using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace OtoServer.Omaha
{
    namespace V3
    {
        public class OmahaClientResponse
        {
            public string protocol;
            public string server;

            public DayStart daystart;
            public List<AppInfoResult> app_results;
            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == "protocol")
                        protocol = reader.Value;
                    else if (reader.Name == "server")
                        server = reader.Value;
                }
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("protocol", protocol);
                w.WriteAttributeString("server", server);
                daystart.WriteXml(w);
                if (app_results != null)
                    foreach (AppInfoResult app in app_results)
                        app.WriteXml(w);
            }
            #endregion IXmlSerializable
        }

        public class DayStart
        {
            public UInt32 elapsed_seconds;
            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                reader.MoveToNextAttribute();
                if (reader.Name == "elapsed_seconds")
                    elapsed_seconds = UInt32.Parse(reader.Value);
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("elapsed_seconds", elapsed_seconds.ToString());
            }
            #endregion IXmlSerializable
        }

        public class AppInfoResult
        {
            public string appid;
            public string status;

            public UpdateResult updatecheck;
            public PingResult ping;
            public List<EventResponse> event_responses;
            public List<DataResult> data_results;

            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == "appid")
                        appid = reader.Value;
                    else if (reader.Name == "status")
                        status = reader.Value;
                }
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "updatecheck")
                        {
                            updatecheck = new UpdateResult();
                            XmlReader s = reader.ReadSubtree();
                            s.Read();
                            updatecheck.ReadXml( s );
                            s.Close();
                        }
                        else if( reader.Name == "ping" )
                        {
                            ping = new PingResult();
                            XmlReader s = reader.ReadSubtree();
                            s.Read();
                            ping.ReadXml( s );
                            s.Close();
                        }
                        else if( reader.Name == "event" )
                        {
                            if( event_responses == null )
                                event_responses = new List<EventResponse>();
                            XmlReader s = reader.ReadSubtree();
                            EventResponse e_rsp = new EventResponse();
                            s.Read();
                            e_rsp.ReadXml( s );
                            s.Close();
                            event_responses.Add( e_rsp );
                        }
                        else if( reader.Name == "data" )
                        {
                            if( data_results == null )
                                data_results = new List<DataResult>();
                            DataResult d_rst = new DataResult();
                            XmlReader s = reader.ReadSubtree();
                            s.Read();
                            d_rst.ReadXml( s );
                            s.Close();
                            data_results.Add( d_rst );
                        }
                    }
                }
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("appid", appid);
                w.WriteAttributeString("status", status);

                if (updatecheck != null) updatecheck.WriteXml(w);
                if( ping != null ) ping.WriteXml(w);
                if (event_responses != null)
                    foreach (EventResponse rsp in event_responses)
                        rsp.WriteXml(w);

            }
            #endregion IXmlSerializable
        }

        public class PingResult
        {
            [XmlAttribute]
            public string status;
            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                reader.MoveToNextAttribute();
                if (reader.Name == "status")
                    status = reader.Value;
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("status", status);
            }
            #endregion IXmlSerializable
        }

        public class UpdateResult
        {
            public string status;

            public List<UpdateUrl> urls;
            public UpdateManifest manifest;

            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                bool in_urls = false;
                reader.MoveToNextAttribute();
                if (reader.Name == "status")
                    status = reader.Value;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (!in_urls)
                        {
                            if (reader.Name == "urls")
                            {
                                urls = new List<UpdateUrl>();
                                in_urls = true;
                            }
                            else if (reader.Name == "manifest")
                            {
                                manifest = new UpdateManifest();
                                XmlReader s = reader.ReadSubtree();
                                s.Read();
                                manifest.ReadXml(s);
                                s.Close();
                            }
                        }
                        else if (reader.Name == "url")
                        {
                            UpdateUrl uu = new UpdateUrl();
                            XmlReader s = reader.ReadSubtree();
                            s.Read();
                            uu.ReadXml(s);
                            s.Close();
                            urls.Add( uu );
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                        if (reader.Name == "urls")
                            in_urls = false;
                }
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("status", status);
                if (urls != null && urls.Count > 0)
                {
                    w.WriteStartElement("urls");
                    foreach (UpdateUrl url in urls)
                        url.WriteXml(w);
                    w.WriteEndElement();
                }
                if (manifest != null)
                    manifest.WriteXml(w);

            }
            #endregion IXmlSerializable
        }

        public class UpdateUrl
        {
            public string codebase;

            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                reader.MoveToNextAttribute();
                if (reader.Name == "codebase")
                    codebase = reader.Value;
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("codebase", codebase);
            }
            #endregion IXmlSerializable
        }

        public class UpdateManifest
        {
            public string version;

            public List<UpdatePackage> updated_packages;
            public List<UpdateAction> update_actions;

            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                bool in_package = false, in_action = false;
                reader.MoveToNextAttribute();
                if (reader.Name == "version")
                    version = reader.Value;

                while (reader.Read())
                {

                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (!in_action && !in_package)
                        {
                            if (reader.Name == "packages")
                            {
                                in_package = true;
                                updated_packages = new List<UpdatePackage>();
                            }
                            else if (reader.Name == "actions")
                            {
                                in_action = true;
                                update_actions = new List<UpdateAction>();
                            }
                        }
                        else if (in_package)
                        {
                            if (reader.Name == "package")
                            {
                                UpdatePackage pkg = new UpdatePackage();
                                XmlReader s = reader.ReadSubtree();
                                s.Read();
                                pkg.ReadXml(s);
                                s.Close();
                                updated_packages.Add(pkg);
                            }
                        }
                        else if (in_action)
                        {
                            if (reader.Name == "action")
                            {
                                UpdateAction action = new UpdateAction();
                                XmlReader s = reader.ReadSubtree();
                                s.Read();
                                action.ReadXml(s);
                                s.Close();
                                update_actions.Add(action);
                            }
                        }
                    }
                    else if( reader.NodeType == XmlNodeType.EndElement )
                    {
                        if( reader.Name == "packages" )
                            in_package = false;
                        else if( reader.Name == "actions" )
                            in_action = false;
                    }
                }
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("version", version);
                if (update_actions != null && update_actions.Count > 0)
                {
                    w.WriteStartElement("actions");
                    foreach (UpdateAction ua in update_actions)
                        ua.WriteXml(w);
                    w.WriteEndElement();
                }
                if (updated_packages != null && updated_packages.Count > 0)
                {
                    w.WriteStartElement("packages");
                    foreach (UpdatePackage up in updated_packages)
                        up.WriteXml(w);
                    w.WriteEndElement();
                }
            }
            #endregion IXmlSerializable
        }

        public class UpdatePackage
        {
            public string hash;
            public string name;
            public string required;
            public UInt32 size;

            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                reader.MoveToNextAttribute();
                if (reader.Name == "hash")
                    hash = reader.Value;
                else if (reader.Name == "name")
                    name = reader.Value;
                else if (reader.Name == "required")
                    required = reader.Value;
                else if (reader.Name == "size")
                    size = UInt32.Parse(reader.Value);
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("hash", hash);
                w.WriteAttributeString("name", name);
                w.WriteAttributeString("required", required);
                w.WriteAttributeString("size", size.ToString());
            }
            #endregion IXmlSerializable
        }

        public class UpdateAction
        {
            public string on_event;
            public string arguments;
            public string run;
            public string onsuccess;
            public string version;

            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                reader.MoveToNextAttribute();
                if (reader.Name == "event")
                    on_event = reader.Value;
                if (reader.Name == "arguments")
                    arguments = reader.Value;
                if (reader.Name == "run")
                    run = reader.Value;
                if (reader.Name == "onsuccess")
                    onsuccess = reader.Value;
                if (reader.Name == "version")
                    version = reader.Value;
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("event", on_event);
                w.WriteAttributeString("arguments", arguments);
                w.WriteAttributeString("run", run);
                w.WriteAttributeString("onsuccess", onsuccess);
                w.WriteAttributeString("version", version);
            }
            #endregion IXmlSerializable

        }

        public class EventResponse
        {
            public string status;

            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                reader.MoveToNextAttribute();
                if (reader.Name == "status")
                    status = reader.Value;
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("status", status);
            }
            #endregion IXmlSerializable
        }

        public class DataResult
        {
            public string name;
            public string index;
            public string status;
            public string data;

            #region IXmlSerializable
            public XmlSchema GetSchema() { return null; }

            public void ReadXml(XmlReader reader)
            {
                while( reader.MoveToNextAttribute())
                {

                    if (reader.Name == "name") name = reader.Value;
                    else if (reader.Name == "index") index = reader.Name;
                    else if (reader.Name == "status") status = reader.Name;
                }
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Text)
                        data = reader.Value;
                }
            }

            public void WriteXml(XmlWriter w)
            {
                w.WriteAttributeString("name", name);
                w.WriteAttributeString("index", index);
                w.WriteAttributeString("status", status);
                w.WriteString( data );
            }
            #endregion IXmlSerializable
        }
    }


}