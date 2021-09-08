using System;
using System.Threading.Tasks;
using System.Web.Http;
using Distancify.LitiumAddOns.MediaMapper.Jobs;
using Hangfire;
using Litium.Web.Administration.WebApi;

namespace Didriksons.Web.Site.Administration.Api.Controllers
{
    [RoutePrefix("site/administration/api/mediamapping")]
    public class CategoryDataCopyController : BackofficeApiController
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public CategoryDataCopyController(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> PostAsync(MediaMappingRequest mediaMappingRequest)
        {
            _backgroundJobClient.Enqueue<MediaMappingJob>(job => job.Execute(mediaMappingRequest.FolderSystemId));

            return Ok();
        }

        public class MediaMappingRequest
        {
            public Guid FolderSystemId { get; set; }
        }
    }
}