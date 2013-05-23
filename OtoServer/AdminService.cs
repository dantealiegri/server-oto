using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceHost;

namespace OtoServer
{
    public class OtoFilesService : Service
    {
        private OtoFilesResponse FilterStoreToResponse( List<DataStore.App> apps, string guid, string version )
        {
            OtoFilesResponse resp = new OtoFilesResponse();
            resp.breadcrumbs = new List<string>(new string[] { "files" });
            resp.breadcrumbnames = new List<string>(new string[] { "Applications" });
            bool want_version = version != null && version != "";
            bool found_version = false;

            foreach (DataStore.App app in apps)
            {
                string display_name = String.Format("{0} ({1})", app.name, app.guid);
                if ( guid == app.guid)
                {
                    resp.breadcrumbnames.Add( display_name );
                    resp.breadcrumbs.Add(app.guid);
                    if (app.versions != null)
                    {
                        foreach (DataStore.AppVersion version_of_app in app.versions)
                        {
                            if (version != null && version == version_of_app.version)
                            {
                                found_version = true;
                                resp.breadcrumbnames.Add(version);
                                resp.breadcrumbs.Add(version);
                                if( version_of_app.package != null )

                                    foreach (DataStore.AppPackage pkg in version_of_app.package)
                                    {

                                        OtoFile of = new OtoFile();
                                        of.Extension = "msi";
                                        of.FileSizeBytes = pkg.size;
                                        of.ModifiedDate = DateTime.Now;
                                        of.Name = pkg.name;
                                        if (resp.Files == null)
                                            resp.Files = new List<OtoFile>();
                                        resp.Files.Add(of);
                                    }
                            }
                            else
                            {
                                if (resp.Containers == null)
                                    resp.Containers = new List<OtoContainer>();
                                OtoContainer over = new OtoContainer { DisplayName = version_of_app.version, LinkName = version_of_app.version,
                                IsCurrentVersion = app.current != null && app.current.version == version_of_app.version ? "true" : "false"
                                };
                                resp.Containers.Add(over);
                            }
                        }
                    }
                }
                else if (guid == null || guid == "")
                {
                    if( resp.Containers == null )
                        resp.Containers = new List<OtoContainer>();
                    OtoContainer oapp = new OtoContainer { DisplayName = display_name , LinkName = app.guid };
                    resp.Containers.Add( oapp );


                }
            }

            if (want_version && !found_version)
            {
                throw HttpError.NotFound( String.Format("Application {0}, version {1} not found", guid, version));
            }
            return resp;
        }

        private object RetrieveFileFromStore(string guid, string version, string file)
        {
            return DataStore.DataStore.Instance().GetAppVersionFile(guid, version, file);
        }
        public object Get( OtoFiles request )
        {
            if( request.File != null && request.File != "" )
                return RetrieveFileFromStore(request.Guid, request.Version, request.File );
            else
            return FilterStoreToResponse(DataStore.DataStore.Instance().KnownApps, request.Guid, request.Version);
        }
        public object Put(OtoFiles request)
        {
            if (request.Guid != "" && request.Version != "" && request.VersionDefault == "true")
            {
                DataStore.DataStore.Instance().SetAppDefaultVersion(request.Guid, request.Version);
            }
            return HttpError.NotFound("Could not PUT this version request");
        }

        public void Post(OtoFiles request)
        {
            if (request.Guid != null && request.AppName != null)
            {
                DataStore.DataStore.Instance().AddApp(request.AppName, request.Guid);
            }
            
            if (request.Guid != null && request.Version != null && RequestContext.Files.Length == 0)
            {
                DataStore.DataStore.Instance().AddAppVersion(request.Guid, request.Version);
            }
            if (request.Guid != null && request.Version != null && RequestContext.Files.Length > 0)
            {
                DataStore.DataStore.Instance().AddAppVersionFile(request.Guid, request.Version, RequestContext.Files);
            }

        }
    }

}