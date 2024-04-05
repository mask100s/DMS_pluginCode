/*using DamManagementSystem.Plugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dam_Management_System
{
    public class AgreementOnAdditionOfFreightItemsCalculateAdvance : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            // Check if the plugin is triggered by the desired event and entity.
            if (context.MessageName.ToLower() == "create" && context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // Get the target entity (assuming it's the primary entity).
                Entity targetEntity = (Entity)context.InputParameters["Target"];

                // Check if the target entity contains the column you're filtering with.
                if (targetEntity.Contains(FreightItems.freightItem))
                {
                    // Retrieve the filtering value.
                    var filterValue = targetEntity.GetAttributeValue<string>(FreightItems.agreement);

                    // Retrieve related records from subgrid using QueryExpression.
                    QueryExpression query = new QueryExpression
                    {
                        EntityName = FreightItems.totalTransportCost,
                        ColumnSet = new ColumnSet(FreightItems.totalTransportCost),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                            {
                                // Apply filtering based on the value from another column.
                                new ConditionExpression("your_filtering_column_in_subgrid_entity", ConditionOperator.Equal, filterValue)
                            }
                        }
                    };

                    // Retrieve records from D365.
                    EntityCollection results = context.RetrieveMultiple(query);

                    // Iterate through the retrieved records.
                    foreach (Entity record in results.Entities)
                    {
                        // Fetch the values from the desired column.
                        var columnValue = record.GetAttributeValue<string>("your_desired_column_name");

                        // Do something with the fetched value.
                    }
                }
            }




        }
    }
}
*/