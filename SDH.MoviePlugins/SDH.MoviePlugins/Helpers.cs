using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDH.MoviePlugins
{
    public class Helpers
    {
        public static Dictionary<string, int?> GetOptionSetCollection(string entity, string attName, IOrganizationService service, ITracingService traceService)
        {
            try
            {
                var attributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entity,
                    LogicalName = attName,
                    RetrieveAsIfPublished = true
                };
                var attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);
                var attributeMetadata = (EnumAttributeMetadata)attributeResponse.AttributeMetadata;

                var optionList = (from o in attributeMetadata.OptionSet.Options
                                  select new { Value = o.Value, Text = o.Label.UserLocalizedLabel.Label }).ToList();
                var result = new Dictionary<string, int?>();
                foreach (var option in optionList)
                {
                    result.Add(option.Text, option.Value);
                }
                return result;
            }
            catch (Exception e)
            {
                traceService.Trace(e.Message);
                traceService.Trace(e.StackTrace);
                throw new InvalidPluginExecutionException($"{e}");
            }
        }
    }
}
