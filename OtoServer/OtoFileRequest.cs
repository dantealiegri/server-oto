using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;

namespace OtoServer
{
    [Route("/files")]
    [Route("/files/{Guid}")]
    [Route("/files/{Guid}/{Version}")]
    public class OtoFiles
    {
        public string Version { get; set; }
        public string Guid { get; set; }
    }

    public class OtoFilesResponse : IHasResponseStatus
    {
        public List<string> breadcrumbs { get; set; }
        public List<string> breadcrumbnames { get; set; }

        public List<OtoFile> Files { get; set; }
        public List<OtoApplication> Applications { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class OtoFile
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Version { get; set; }
        public long FileSizeBytes { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class OtoApplication
    {
        public string DisplayName { get; set; }
        public string LinkName { get; set; }
    }
}