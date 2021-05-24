using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace SDH.MoviePlugins
{
    public class CreateMovieRatingsAction : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //Extract the tracing service for use in debugging sandboxed plug-ins.
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            tracingService.Trace("Started the Create movie Ratings Action.");

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            var json = context.InputParameters["JSON"].ToString();
            var movie = (EntityReference)context.InputParameters["Movie"];

            var controller = new MovieCreateGetOmdbApiController();
            var response = controller.GetOmdbApiResponse(json);

            if (response.Ratings == null || response.Ratings.Count == 0)
            {
                return;
            }

            foreach (var rating in response.Ratings)
            {
                tracingService.Trace($"Source {rating.Source}; Value {rating.Value}");

                var accounts = service.RetrieveMultiple(new FetchExpression(controller.GetAccountByNameFetch(rating.Source)));
                var accountEntityReference = new EntityReference("account");

                if (accounts.Entities.Count > 0)
                {
                    accountEntityReference.Id = accounts.Entities[0].Id;
                }
                else
                {
                    var newAccount = new Entity("account");
                    newAccount["name"] = rating.Source;
                    newAccount.Id = service.Create(newAccount);
                    accountEntityReference.Id = newAccount.Id;
                }

                var ratingsEntity = new Entity("sdh_rating");
                ratingsEntity["sdh_organization"] = accountEntityReference;
                ratingsEntity["sdh_movie"] = movie;
                ratingsEntity["sdh_name"] = rating.Value;

                service.Create(ratingsEntity);

            }
        }
    }
}
