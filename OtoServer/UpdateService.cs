using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceHost;

using OtoServer.Omaha.V3;

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
            OmahaClientResponse resp = new OmahaClientResponse();
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
                    app_res.updatecheck = new UpdateResult { status = "noupdate" };
                }

                resp.app_results.Add(app_res);
            }

            return resp;

        }

    }
}