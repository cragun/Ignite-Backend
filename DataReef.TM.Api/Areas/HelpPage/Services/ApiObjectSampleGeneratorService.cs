using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http.Description;
using DataReef.Core.Attributes;
using DataReef.TM.Api.Areas.HelpPage.Services.SampleGeneration;

namespace DataReef.TM.Api.Areas.HelpPage.Services
{
    [Service(typeof(IApiObjectSampleGeneratorService))]
    internal class ApiObjectSampleGeneratorService : IApiObjectSampleGeneratorService
    {
        public HelpPageSampleGenerator SampleGenerator { get; set; }

        public ApiObjectSampleGeneratorService(HelpPageSampleGenerator sampleGenerator)
        {
            this.SampleGenerator = sampleGenerator;
        }

        public object GenerateResponseSampleForApi(ApiDescription apiDescription, ICollection<string> sampleGenerationErrors, string mediaType = "application/json")
        {
            try
            {
                foreach (var item in this.SampleGenerator.GetSampleResponses(apiDescription))
                {
                    if (!item.Key.MediaType.Equals(mediaType))
                        continue;

                    var initializedObject = item.Value;

                    InvalidSample invalidSample = item.Value as InvalidSample;
                    if (invalidSample != null)
                        sampleGenerationErrors.Add(invalidSample.ErrorMessage);

                    return initializedObject;
                }
            }
            catch (Exception e)
            {
                sampleGenerationErrors.Add(String.Format(CultureInfo.CurrentCulture,
                    "An exception has occurred while generating the sample. Exception message: {0}",
                    HelpPageSampleGenerator.UnwrapException(e).Message));
            }

            return null;
        }

        public object GenerateRequestSampleForApi(ApiDescription apiDescription, ICollection<string> sampleGenerationErrors, string mediaType = "application/json")
        {
            try
            {
                foreach (var item in this.SampleGenerator.GetSampleRequests(apiDescription))
                {
                    if (!item.Key.MediaType.Equals(mediaType))
                        continue;

                    var initializedObject = item.Value;

                    InvalidSample invalidSample = item.Value as InvalidSample;
                    if (invalidSample != null)
                        sampleGenerationErrors.Add(invalidSample.ErrorMessage);

                    return initializedObject;
                }
            }
            catch (Exception e)
            {
                sampleGenerationErrors.Add(String.Format(CultureInfo.CurrentCulture,
                    "An exception has occurred while generating the sample. Exception message: {0}",
                    HelpPageSampleGenerator.UnwrapException(e).Message));
            }

            return null;
        }
    }
}