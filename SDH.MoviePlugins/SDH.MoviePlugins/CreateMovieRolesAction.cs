using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDH.MoviePlugins
{
    public class CreateMovieRolesAction : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            // get the execution context
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            tracingService.Trace("Started Movie Roles Create Action");

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            tracingService.Trace("Get Service Successful");

            var directorsValue = context.InputParameters["Directors"].ToString();
            tracingService.Trace(directorsValue);

            var writersValue = context.InputParameters["Writers"].ToString();
            tracingService.Trace(writersValue);

            var actorsValue = context.InputParameters["Actors"].ToString();
            tracingService.Trace(actorsValue);

            var movieEntity = (EntityReference)context.InputParameters["Movie"];
            tracingService.Trace($"Movie ID is {movieEntity.Id}");

            var controller = new CreateMovieRolesController(actorsValue, writersValue, directorsValue, service, tracingService);
            controller.Run(movieEntity, movieEntity.Name);
        }

        private static List<string> GetValues(string rawValues)
        {
            var valuesUntrimmed = rawValues.Split(',');
            var values = new List<string>();
            foreach (var val in valuesUntrimmed)
            {
                values.Add(val.Trim());
            }
            return values;
        }

        public static string GetPeopleByNameFetchXml(string name)
        {
            return $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                < entity name = 'sdh_person' >
                    < attribute name = 'sdh_personid' />
                        < filter type = 'and' >
                            < condition attribute = 'sdh_name' operator= 'eq' value = '{name}' />
                        </ filter >
                </ entity >
             </ fetch > ";
        }

        public static string GetRoleByMoviePositionFullName(Guid movieId, string person_name, string position)
        {
            // get xml that grabs roles by movieid {just grab all of them for that movie then sort it out by role
            return string.Empty;
        }
    }
}
