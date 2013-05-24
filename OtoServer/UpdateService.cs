using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceHost;

using OtoServer.Omaha.V3;
using System.Text.RegularExpressions;

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
        public object Post(OmahaClient request)
        {
            ILog log = LogManager.GetLogger(GetType());
            log.Info("Client is : " + Request.UserAgent);
            log.Debug("Query is : " + Request.GetRawBody());
            //log.Warn("Response Content Type: " + Request.ResponseContentType);
            OmahaClientResponse resp = new OmahaClientResponse();
            resp.protocol = "3.0";
            resp.server = "oto-test";
            DateTime beginning_of_day = DateTime.Now.Date;
            resp.daystart = new DayStart { elapsed_seconds = (uint)(DateTime.Now - beginning_of_day).TotalSeconds };
            resp.app_results = new List<AppInfoResult>();
            foreach (AppInfoRequest app_req in request.apps)
            {
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
                    UpdateResult update_result = null;
                    DataStore.App matched = DataStore.DataStore.Instance().KnownApps.SingleOrDefault(k_app => k_app.guid == app_req.appid);
                    if (matched == null || matched.current == null || !VersionShouldUpdate( app_req.version, matched.current.version))
                    {
                        update_result = new UpdateResult { status = "noupdate" };
                    }
                    else
                    {
                        UpdateManifest update_manifest = new UpdateManifest {
                            version = matched.current.version,
                            updated_packages = matched.current.package.Select<DataStore.AppPackage,UpdatePackage>(
                                ap => new UpdatePackage { hash = ap.hash, name = ap.name, required = ap.required ? "true":"false", size = ap.size } ).ToList(),
                            update_actions = matched.current.actions == null ? null : matched.current.actions.Select<DataStore.AppAction,UpdateAction>(
                                aa => new UpdateAction { on_event = aa.on_event, arguments = aa.arguments, run = aa.run, onsuccess = aa.on_success, version = aa.version }).ToList()
                        };
                        List<UpdateUrl> url_list = matched.current.url_locations.Select<string,UpdateUrl>(url_string => new UpdateUrl { codebase = url_string }).ToList();

                        update_result = new UpdateResult { status = "ok", urls = url_list, manifest = update_manifest };

                    }

                    app_res.updatecheck = update_result;
                }

                resp.app_results.Add(app_res);
            }

            return resp;

        }

        private bool VersionShouldUpdate(string supplied_version, string repo_version)
        {
            DataStore.VersionTuple supplied = new DataStore.VersionTuple(supplied_version);
            DataStore.VersionTuple repo = new DataStore.VersionTuple(repo_version);
            return supplied < repo;
        }

    }
}