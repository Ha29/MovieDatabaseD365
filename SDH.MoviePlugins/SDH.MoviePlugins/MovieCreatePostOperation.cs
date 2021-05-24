using Microsoft.Xrm.Sdk;
using System;

namespace SDH.MoviePlugins
{
    public class MovieCreatePostOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // useful for debugging
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            // get the execution context
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            tracingService.Trace("Started Movie Create Plugin");
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.LogicalName != "sdh_movie")
                    return;
                tracingService.Trace("Entity is Movie");
                var controller = new MovieCreateGetOmdbApiController();
                var title = entity.GetAttributeValue<string>("sdh_name");
                var year = entity.GetAttributeValue<string>("sdh_year");

                tracingService.Trace($"Title is: {title}, Year is: {year}");
                var omdbApiKey = "";

                var movieData = controller.GetMovieRecord(title, year, omdbApiKey);
                tracingService.Trace($"JSON is {movieData}");
                var response = controller.GetOmdbApiResponse(movieData);

                entity = controller.ConvertToEntity(response, entity, service, tracingService);
            }
        }
    }
}
