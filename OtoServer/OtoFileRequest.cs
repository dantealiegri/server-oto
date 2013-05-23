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
    [Route("/files/{Guid}/{Version}/{File}")]
    public class OtoFiles
    {
        public string Version { get; set; }
        public string Guid { get; set; }
        public string File { get; set; }
        public string AppName { get; set; }
    }

    public class OtoFilesResponse : IHasResponseStatus
    {
        public List<string> breadcrumbs { get; set; }
        public List<string> breadcrumbnames { get; set; }

        public List<OtoFile> Files { get; set; }
        public List<OtoContainer> Containers { get; set; }
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

    public class OtoContainer
    {
        public string DisplayName { get; set; }
        public string LinkName { get; set; }
    }
}