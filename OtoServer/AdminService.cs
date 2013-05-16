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
                    Files = FakeFileResult( request.Guid )
                };
                       

            }
            else
                return new OtoFilesResponse {
                    Files = FakeFileResult( request.Guid )
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
        private List<OtoFile> FakeFileResult( string guid )
        {
            List<OtoFile> results = new List<OtoFile>();
            bool guid_set = ( guid != null && guid != "");
            if( ! guid_set || guid == "{abcde}" )
            {
            results.Add(
                new OtoFile{
                    Name = "Station A Installer 1.0.1",
                    Extension = ".msi",
                    FileSizeBytes = 1024,
                    Version = "1.0.1.0",
                    ModifiedDate = new DateTime( 2013, 05,01,08,30,00)
                }
                );
            results.Add(
                new OtoFile{
                    Name = "Station A Installer 1.0.2",
                    Extension = ".msi",
                    FileSizeBytes = 1024,
                    Version = "1.0.2.1",
                    ModifiedDate = new DateTime( 2013, 05,04,15,29,04)
                }
                );
            }

            if( ! guid_set || guid ==  "{fghij}" )
            {
                results.Add(
                    new OtoFile
                    {
                        Name = "Station B Installer 2.8.32",
                        Extension = ".msi",
                        FileSizeBytes = 1024,
                        Version = "2.8.32.1",
                        ModifiedDate = new DateTime(2013, 05, 01, 08, 30, 00)
                    }
                    );
                results.Add(
                    new OtoFile
                    {
                        Name = "Station B Installer 2.8",
                        Extension = ".msi",
                        FileSizeBytes = 1024,
                        Version = "2.8.0.0",
                        ModifiedDate = new DateTime(2013, 01, 09, 10, 14, 37)
                    }
                );
            }

            return results;
        }
    }

}