using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamManagementSystemPlugins
{
    public class OnCreateUpdateAndDeleteStakeholder : IPlugin
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
                if (context.MessageName.Equals("Create", StringComparison.OrdinalIgnoreCase))
                {
                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        // Retrieve the target entity from the input parameters.
                        Entity stake = (Entity)context.InputParameters["Target"];

                        if (stake.LogicalName.Equals(Stakeholders.logicalName, StringComparison.OrdinalIgnoreCase))
                        {
                            EntityReference stakeRef = stake.GetAttributeValue<EntityReference>(Stakeholders.dam);
                            decimal holdingPercent = stake.GetAttributeValue<decimal>(Stakeholders.holdingsPercent);

                            if (stakeRef != null && holdingPercent != null)
                            {

                                Entity dam = service.Retrieve(stakeRef.LogicalName, stakeRef.Id, new ColumnSet(Dams.availableShares));


                                if (dam.Contains(Dams.availableShares) && dam.GetAttributeValue<decimal>(Dams.availableShares) != null)
                                {
                                    decimal availableShare = dam.GetAttributeValue<decimal>(Dams.availableShares);

                                    if (holdingPercent > availableShare)
                                    {
                                        throw new InvalidPluginExecutionException("Holding % entered is more than available share.");
                                    }
                                    else
                                        {
                                        dam[Dams.availableShares] = availableShare - holdingPercent;
                                    }

                                    service.Update(dam);
                                }

                            }
                        }
                    }
                }



                else if (context.MessageName.Equals("Update", StringComparison.OrdinalIgnoreCase))
                {
                    if (context.PreEntityImages.Contains("Image") && context.PreEntityImages["Image"] is Entity &&
                        context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        Entity preImage = (Entity)context.PreEntityImages["Image"];
                        Entity postImage = (Entity)context.InputParameters["Target"];

                        decimal oldHoldingPercent = preImage.GetAttributeValue<decimal>(Stakeholders.holdingsPercent);
                        decimal newHoldingPercent = postImage.GetAttributeValue<decimal>(Stakeholders.holdingsPercent);

                        if (oldHoldingPercent != newHoldingPercent)
                        {
                            EntityReference damRef = preImage.GetAttributeValue<EntityReference>(Dams.logicalName);
                            Entity damRecord = service.Retrieve(damRef.LogicalName, damRef.Id, new ColumnSet(Dams.availableShares));
                            decimal availableShare = damRecord.GetAttributeValue<decimal>(Dams.availableShares);

                            // Update available share based on the change in holding percent
                            decimal holdingPercentDifference = newHoldingPercent - oldHoldingPercent;
                            if (holdingPercentDifference < availableShare)
                            {
                                if (newHoldingPercent > oldHoldingPercent)
                                {
                                    availableShare -= Math.Abs(holdingPercentDifference);
                                    damRecord[Dams.availableShares] = availableShare;
                                }
                                else
                                {
                                    availableShare += Math.Abs(holdingPercentDifference);
                                    damRecord[Dams.availableShares] = availableShare;
                                }
                            }
                            else
                            {
                                throw new InvalidPluginExecutionException("Holding % entered is more than available share.");
                            }
                            service.Update(damRecord);
                        }
                    }
                }




            else if (context.MessageName.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    if (context.PreEntityImages.Contains("Image") && context.PreEntityImages["Image"] is Entity)
                    {
                        Entity stackHolder = (Entity)context.PreEntityImages["Image"];
                        decimal holdingPercent = stackHolder.GetAttributeValue<decimal>(Stakeholders.holdingsPercent);
                        EntityReference damRef = stackHolder.GetAttributeValue<EntityReference>(Dams.logicalName);
                        Entity damRecord = service.Retrieve(damRef.LogicalName, damRef.Id, new ColumnSet(Dams.availableShares));
                        decimal availableShare = damRecord.GetAttributeValue<decimal>(Dams.availableShares);
                        availableShare += holdingPercent;
                        damRecord[Dams.availableShares] = availableShare;
                        service.Update(damRecord);
                    }
                 }

            }
            catch (Exception ex)
            {
                ErrorLog.CreateExceptionLog(service, ex, String.Empty, "Plugin OnCreateUpdateAndDeleteStakeholder");
                tracingService.Trace($"Error: {ex.Message}");
                throw new InvalidPluginExecutionException($" {ex.Message}"); ;
            }

        }

    }
}
