using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;

namespace DamManagementSystem.Plugins
{
    public class FreightItemOnSelectOfProductAutopopulateFixedAndAddOnCost : IPlugin
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
                    Entity freightitem = (Entity)context.InputParameters["Target"];

                    if (freightitem.LogicalName.Equals(FreightItems.freightItem, StringComparison.OrdinalIgnoreCase))
                    {
                        EntityReference mediumOfTransportRef = freightitem.GetAttributeValue<EntityReference>(FreightItems.mediumOfTransport);
                        decimal distance = freightitem.GetAttributeValue<decimal>(FreightItems.distance);

                        if (mediumOfTransportRef != null && distance != null)
                        {

                            Entity fixedCost = service.Retrieve(mediumOfTransportRef.LogicalName, mediumOfTransportRef.Id, new ColumnSet(FreightRate.fixedPrice));
                            Entity addedCost = service.Retrieve(mediumOfTransportRef.LogicalName, mediumOfTransportRef.Id, new ColumnSet(FreightRate.addOnCost));
                            Entity fixedLength = service.Retrieve(mediumOfTransportRef.LogicalName, mediumOfTransportRef.Id, new ColumnSet(FreightRate.fixedPriceLength));


                            if ((fixedCost.Contains(FreightRate.fixedPrice) && fixedCost.GetAttributeValue<Money>(FreightRate.fixedPrice) != null) &&
                                (addedCost.Contains(FreightRate.addOnCost) && addedCost.GetAttributeValue<Money>(FreightRate.addOnCost) != null) &&
                                (fixedLength.Contains(FreightRate.fixedPriceLength) && fixedLength.GetAttributeValue<decimal>(FreightRate.fixedPriceLength) != null))
                            {
                                freightitem[FreightItems.fixedPrice] = fixedCost.GetAttributeValue<Money>(FreightRate.fixedPrice);
                                Money fcost = fixedCost.GetAttributeValue<Money>(FreightRate.fixedPrice);

                               

                                decimal fixedDistance = fixedLength.GetAttributeValue<decimal>(FreightRate.fixedPriceLength);

                                if (distance>fixedDistance) 
                                {
                                    Money acost = addedCost.GetAttributeValue<Money>(FreightRate.addOnCost);
                                    freightitem[FreightItems.addOnCost] = acost.Value * (distance - fixedDistance);

                                    freightitem[FreightItems.totalTransportCost] = fcost.Value + (acost.Value * (distance - fixedDistance));
                                }
                                else
                                {
                                    freightitem[FreightItems.totalTransportCost] = fcost.Value;
                                }
                                service.Update(freightitem);
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



