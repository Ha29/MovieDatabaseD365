using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDH.MoviePlugins
{
    public class AssociateCreateProductionCompany : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //Extract the tracing service for use in debugging sandboxed plug-ins.
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            tracingService.Trace("Started the Association of Movie and Prod Company.");

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            var movieReference = (EntityReference)context.InputParameters["Movie"];
            var json = context.InputParameters["JSON"].ToString();

            var controller = new MovieCreateGetOmdbApiController();
            var omdbResponse = controller.GetOmdbApiResponse(json);

            if (!omdbResponse.Production.Equals("N/A"))
            {
                var productionArray = omdbResponse.Production.Split(',');
                var productionList = new List<string>();
                var relatedEntities = new EntityReferenceCollection();

                foreach (var prod in productionArray)
                {
                    productionList.Add(prod.Trim());
                }

                foreach (var prod in productionList)
                {
                    var accounts = service.RetrieveMultiple(new FetchExpression(controller.GetAccountByNameFetch(prod)));

                    if (accounts.Entities.Count > 0)
                    {
                        relatedEntities.Add(new EntityReference("account", accounts[0].Id));
                    }
                    else
                    {
                        var newAccount = new Entity("account");
                        newAccount["name"] = prod;
                        newAccount["sdh_isproductioncompany"] = true;
                        newAccount.Id = service.Create(newAccount);
                        relatedEntities.Add(new EntityReference("account", newAccount.Id));
                    }
                }

                var relationship = new Relationship("sdh_ProductionCompany");

                service.Associate("sdh_movie", movieReference.Id, relationship,
                        relatedEntities);
            }
        }
    }
}
