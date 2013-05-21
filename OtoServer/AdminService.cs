﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceHost;

namespace OtoServer
{
#if false
    //public class AdminService : Service
   // {
   //     public AppConfig Config { get; set; }


//        public void Post(OtoFiles request)
 //       {


  //          foreach (IFile uploadedFile in base.RequestContext.Files)
   //         {
                System.Console.WriteLine(
                    String.Format("Uploading {0} [[1}] for {2} version {3}",
                    uploadedFile.FileName, uploadedFile.ContentLength,
                    request.Guid, request.Version));


    //        }
     //   }
   // }
#endif

    public class OtoFilesService : Service
    {
        private OtoFilesResponse FilterStoreToResponse( List<DataStore.App> apps, string guid )
        {
            OtoFilesResponse resp = new OtoFilesResponse();
            resp.breadcrumbs = new List<string>(new string[] { "/files" });
            resp.breadcrumbnames = new List<string>(new string[] { "Applications" });

            foreach (DataStore.App app in apps)
            {
                string display_name = String.Format("{0} ({1})", app.name, app.guid);
                if ( guid == app.guid)
                {
                    resp.breadcrumbnames.Add( display_name );
                    resp.breadcrumbs.Add(app.guid);
                    foreach (DataStore.AppVersion version_of_app in app.versions)
                    {
                        OtoFile of = new OtoFile();
                        of.Extension = "msi";
                        of.FileSizeBytes = version_of_app.package[0].size;
                        of.ModifiedDate = DateTime.Now;
                        of.Name = version_of_app.package[0].name;
                        if (resp.Files == null)
                            resp.Files = new List<OtoFile>();
                        resp.Files.Add(of);
                    }
                }
                else if (guid == null || guid == "")
                {
                    if( resp.Applications == null )
                        resp.Applications = new List<OtoApplication>();
                    OtoApplication oapp = new OtoApplication { DisplayName = display_name , LinkName = app.guid };
                    resp.Applications.Add( oapp );


                }
            }
            return resp;
        }
        public object Get( OtoFiles request )
        {
            Console.WriteLine("GET request");
            return FilterStoreToResponse(DataStore.DataStore.Instance().KnownApps, request.Guid);
        }

        public void Post(OtoFiles request)
        {


            foreach (IFile uploadedFile in base.RequestContext.Files)
            {
                System.Console.WriteLine(
                    String.Format("Uploading {0} [[1}] for {2} version {3}",
                    uploadedFile.FileName, uploadedFile.ContentLength,
                    request.Guid, request.Version));


            }
        }
    }

}