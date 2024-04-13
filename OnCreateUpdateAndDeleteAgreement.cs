using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamManagementSystemPlugins
{
    public class OnCreateUpdateAndDeleteAgreement : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            try
            {
                // Check if the plugin is triggered by a create or update message of the Agreement Item entity
                if (context.MessageName.ToLower() == "create" || context.MessageName.ToLower() == "update")
                {
                    // Check if the target entity is Agreement Item
                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        // Obtain the target entity
                        Entity agreementItemEntity = (Entity)context.InputParameters["Target"];

                        if (agreementItemEntity.LogicalName.Equals(Items.logicalName, StringComparison.OrdinalIgnoreCase)) 
                        {
                            // Check if the agreement item has a reference to an agreement
                            if (agreementItemEntity.Contains(Items.agreementId) && agreementItemEntity[Items.agreementId] is EntityReference)
                            {
                                // Obtain the Agreement Id
                                EntityReference agreementRef = (EntityReference)agreementItemEntity[Items.agreementId];

                                // Retrieve all items associated with the agreement
                                QueryExpression query = new QueryExpression(Items.logicalName);
                                query.ColumnSet.AddColumns(Items.totalCost);
                                query.Criteria.AddCondition(Items.agreementId, ConditionOperator.Equal, agreementRef.Id);

                                EntityCollection items = service.RetrieveMultiple(query);

                                Money totalCost = new Money(0);

                                // Calculate the total cost of all items
                                foreach (Entity item in items.Entities)
                                {
                                    totalCost.Value += ((Money)item[Items.totalCost]).Value;
                                }

                                // Update the total cost field on the Agreement
                                Entity agreementEntity = new Entity(Agreements.logicalName);
                                agreementEntity.Id = agreementRef.Id;
                                agreementEntity[Agreements.productTotalAmount] = totalCost;

                                service.Update(agreementEntity);
                            }
                        }

                        
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLog.CreateExceptionLog(service, ex, String.Empty, "Plugin OnCreateUpdateAndDeleteAgreement");
                tracingService.Trace($"Error: {ex.Message}");
                throw new InvalidPluginExecutionException($" {ex.Message}"); ;
            }

        }
    }

}
