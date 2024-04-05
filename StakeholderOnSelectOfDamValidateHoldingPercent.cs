using DamManagementSystem.Plugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dam_Management_System
{
    public class StakeholderOnSelectOfDamValidateHoldingPercent : IPlugin
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
                /*if (context.MessageName.Equals("Update", StringComparison.OrdinalIgnoreCase))
                  {

                    if (holdingPercent > availableShare)
                    {
                         throw new InvalidPluginExecutionException("Holding % entered is more than available share.");
                    }
                    else
                    {
                         dam[Dams.availableShares] = availableShare - holdingPercent;
                    }
                }*/
              
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                {
                    if (context.MessageName.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                    {


                        Entity stackHolder = (Entity)context.PreEntityImages["Image"];
                        decimal holdingPercent = stackHolder.GetAttributeValue<decimal>(Stakeholders.holdingsPercent);
                        EntityReference damRef = stackHolder.GetAttributeValue<EntityReference>(Dams.logicalName);
                        Entity damRecord = service.Retrieve(Dams.logicalName, damRef.Id, new ColumnSet(Dams.availableShares));
                        decimal availableShare = damRecord.GetAttributeValue<decimal>(Dams.availableShares);
                        availableShare = availableShare + holdingPercent;
                        service.Update(damRecord);



                        //EntityReference targetEntityReference = (EntityReference)context.InputParameters["Target"];
                        //  Entity stackRecord = service.Retrieve(Stakeholders.logicalName, targetEntityReference.Id, new ColumnSet(Stakeholders.holdingsPercent));




                    }
                        }
                    }
            catch (Exception ex)
            {
                tracingService.Trace($"Error: {ex.Message}");
                throw new InvalidPluginExecutionException($"Unexpected error: {ex.Message}"); ;
            }

        }














        /*public void Execute(IServiceProvider serviceProvider)
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
                    Entity stake = (Entity)context.InputParameters["Target"];

                    if (stake.LogicalName.Equals(Stakeholders.stakeholder, StringComparison.OrdinalIgnoreCase))
                    {
                        EntityReference stakeRef = stake.GetAttributeValue<EntityReference>(Stakeholders.dam);
                        decimal holdingPercent = stake.GetAttributeValue<decimal>(Stakeholders.holdingsPercent);

                        if (stakeRef != null && holdingPercent != null)
                        {

                            Entity dam = service.Retrieve(stakeRef.LogicalName, stakeRef.Id, new ColumnSet(Dams.availableShares));


                            if (dam.Contains(Dams.availableShares) && dam.GetAttributeValue<decimal>(Dams.availableShares) != null)
                            {
                                decimal availableShare = dam.GetAttributeValue<decimal>(Dams.availableShares);

                                if (holdingPercent>availableShare)
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
            catch (Exception ex)
            {
                tracingService.Trace($"Error: {ex.Message}");
                throw new InvalidPluginExecutionException($"Unexpected error: {ex.Message}"); ;
            }

        }*/
    }
}
