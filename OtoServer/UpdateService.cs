using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceHost;

using OtoServer.Omaha.V3;
using System.Text.RegularExpressions;
using ServiceStack.Logging;
using ServiceStack.Logging.Log4Net;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;

namespace OtoServer
{
    public class OmahaClientService : Service
    {
        public object Get(OmahaClient request)
        {
            OmahaClientResponse resp = new OmahaClientResponse();
            DateTime beginning_of_day = DateTime.Now.Date;
            resp.daystart = new DayStart { elapsed_seconds = (uint)(DateTime.Now - beginning_of_day).TotalSeconds };
            return resp;
        }
        private byte[] rdata = new byte[4096];
        private static DataContractSerializer client_serializer = new DataContractSerializer(typeof(OmahaClient));
        public object Post(OmahaClient request)
        {
            ILog log = LogManager.GetLogger(GetType());
            log.Info("Client is : " + Request.UserAgent);
            //log.Debug("Query is : " + Request.GetRawBody());
            //log.Debug("Query is : " + Request.InputStream.Length + " is at " + Request.InputStream.Position);
            //log.Debug("?? " + Request.QueryString);

            if (Request.ContentType == null || Request.ContentType == "")
            {
                string rb = Request.GetRawBody();
                XmlTextReader sxtr = new XmlTextReader(new StringReader(rb));
                //log.Warn("Thus, have " + request.protocol + " and " + rb);
                log.Warn("No ContentType supplied. Reparsing data as xml.");
                request = client_serializer.ReadObject(sxtr) as OmahaClient;

            }
            //else
            //    log.Info("Client Content Type was " + Request.ContentType);
            if (Request.AcceptTypes == null)
                log.Warn("No Accept Types - forcing application/xml out.");
            else
                foreach (string accepttype in base.Request.AcceptTypes)
                    log.Info("Accepts: " + accepttype);

            if( Request.AcceptTypes == null )
                Request.ResponseContentType = "application/xml";
            //log.Warn("Response Content Type: " + Request.ResponseContentType);
            OmahaClientResponse resp = new OmahaClientResponse();
            resp.protocol = "3.0";
            resp.server = "oto-test";
            DateTime beginning_of_day = DateTime.Now.Date;
            resp.daystart = new DayStart { elapsed_seconds = (uint)(DateTime.Now - beginning_of_day).TotalSeconds };
            resp.app_results = new List<AppInfoResult>();
            if (request.apps != null)
            {
                foreach (AppInfoRequest app_req in request.apps)
                {
                    log.Info("Checking " + app_req.appid);
                    AppInfoResult app_res = new AppInfoResult();
                    app_res.appid = app_req.appid;
                    app_res.status = "ok";

                    if (app_req.ping != null)
                        app_res.ping = new PingResult { status = "ok" };
                    if (app_req.events != null && app_req.events.Count > 0)
                    {
                        app_res.event_responses = new List<EventResponse>();
                        foreach (EventReport event_rep in app_req.events)
                        {
                            app_res.event_responses.Add(new EventResponse { status = "ok" });
                        }
                    }

                    if (app_req.data != null && app_req.data.Count > 0)
                    {
                        app_res.data_results = new List<DataResult>();
                        foreach (DataRequest data_req in app_req.data)
                        {
                            DataResult dtd_res = new DataResult();
                            if (data_req.index != null)
                                dtd_res.index = data_req.index;
                            if (data_req.name != null)
                                dtd_res.name = data_req.name;
                            dtd_res.status = "ok";
                            app_res.data_results.Add(dtd_res);
                        }

                    }


                    if (app_req.updatecheck != null)
                    {
                        if (app_req.version != null)
                            app_req.version = app_req.version.ToUpper();
                        UpdateResult update_result = null;
                        DataStore.App matched = DataStore.DataStore.Instance().KnownApps.SingleOrDefault(k_app => k_app.guid == app_req.appid);
                        if (matched == null || matched.current == null || !VersionShouldUpdate(app_req.version, matched.current.version))
                        {
                            if (matched == null)
                                log.Info("Requested unknown application " + app_req.appid);
                            else if( matched.current == null )
                                log.Warn("Requested application " + app_req.appid + " has no active version!" );
                            else
                                log.Info( app_req.appid + " version \"" + app_req.version + "\" had no update");
                            update_result = new UpdateResult { status = "noupdate" };
                        }
                        else
                        {
                            log.Info( app_req.appid +" had an update");
                            UpdateManifest update_manifest = new UpdateManifest
                            {
                                version = matched.current.version,
                                updated_packages = matched.current.package.Select<DataStore.AppPackage, UpdatePackage>(
                                    ap => new UpdatePackage { hash = ap.hash, name = ap.name, required = ap.required ? "true" : "false", size = ap.size }).ToList(),
                                update_actions = matched.current.actions == null ? null : matched.current.actions.Select<DataStore.AppAction, UpdateAction>(
                                    aa => new UpdateAction { on_event = aa.on_event, arguments = aa.arguments, run = aa.run, onsuccess = aa.on_success, version = aa.version }).ToList()
                            };
                            List<UpdateUrl> url_list = matched.current.url_locations.Select<string, UpdateUrl>(url_string => new UpdateUrl { codebase = url_string }).ToList();

                            update_result = new UpdateResult { status = "ok", urls = url_list, manifest = update_manifest };

                        }

                        app_res.updatecheck = update_result;
                    }

                    resp.app_results.Add(app_res);
                }
            }
            else
                log.Info("No Apps Requested");

            return resp;

        }

        private bool VersionShouldUpdate(string supplied_version, string repo_version)
        {
            if ( supplied_version == null ||  supplied_version == "") return true;
            DataStore.VersionTuple supplied = new DataStore.VersionTuple(supplied_version);
            DataStore.VersionTuple repo = new DataStore.VersionTuple(repo_version);
            return supplied < repo;
        }

    }
}