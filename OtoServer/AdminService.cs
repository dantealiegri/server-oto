using System;
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
        public object Get( OtoFiles request )
        {
            Console.WriteLine("GET request");
            if (request.Guid == null || request.Guid == "")
            {

                return new OtoFilesResponse {
                    Files = FakeFileResult()
                };
                       

            }
            else
                return new OtoFilesResponse {
                    Files = FakeFileResult()
                };
                       
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
        private List<OtoFile> FakeFileResult()
        {
            List<OtoFile> results = new List<OtoFile>();

            results.Add(
                new OtoFile{
                    Name = "Station A Installer 1.0.1",
                    Extension = ".msi",
                    FileSizeBytes = 1024,
                    Version = "1.0.1.0",
                    ModifiedTime = new DateTime( 2013, 05,01,08,30,00)
                }
                );
            results.Add(
                new OtoFile{
                    Name = "Station A Installer 1.0.2",
                    Extension = ".msi",
                    FileSizeBytes = 1024,
                    Version = "1.0.2.1",
                    ModifiedTime = new DateTime( 2013, 05,04,15,29,04)
                }
                );

            return results;
        }
    }

}