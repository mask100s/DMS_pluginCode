using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;

namespace DamManagementSystem.Plugins
{
    public class ItemOnSelectOfProductAutopopulatePerUnitCost : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Use the organization service from the context
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {

                // Check if the plugin is triggered on the creation or update of the Policy entity.
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    // Retrieve the target entity from the input parameters.
                    Entity item = (Entity)context.InputParameters["Target"];

                    if (item.LogicalName.Equals(Items.items, StringComparison.OrdinalIgnoreCase))
                    {
                        EntityReference productRef = item.GetAttributeValue<EntityReference>(Items.product);
                        decimal quantity = item.GetAttributeValue<decimal>(Items.quantity);
                        DateTime startDate = item.GetAttributeValue<DateTime>(Items.startDate);
                        DateTime endDate = item.GetAttributeValue<DateTime>(Items.endDate);

                        if (productRef != null && quantity != null && startDate != null && endDate != null)
                        {

                            Entity perUnitCost = service.Retrieve(productRef.LogicalName, productRef.Id, new ColumnSet(Catalogs.perUnitCost));

                            if (perUnitCost.Contains(Catalogs.perUnitCost) && perUnitCost.GetAttributeValue<Money>(Catalogs.perUnitCost) != null)
                            {
                                item[Items.perUnitCost] = perUnitCost.GetAttributeValue<Money>(Catalogs.perUnitCost);
                                Money cost = perUnitCost.GetAttributeValue<Money>(Catalogs.perUnitCost);

                                item[Items.totalcost] =  cost.Value * quantity * (endDate.Date-startDate.Date).Days;

                                service.Update(item);
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                tracingService.Trace($"Error: {ex.Message}");
                throw new InvalidPluginExecutionException($"Unexpected error: {ex.Message}"); ;
            }

        }
    }
}



