using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamManagementSystem.Plugins
{
    public static class Catalogs
    {
        public static string catalogs = "wms_cateloge";
        public static string product = "wms_name";
        public static string perUnitCost = "wms_perunitcost";
    }
    public static class Items
    {
        public static string items = "wms_item";
        public static string product = "wms_itemname";
        public static string perUnitCost = "wms_perunitcost";
        public static string quantity = "wms_quantity";
        public static string totalcost = "wms_totalcost";
        public static string startDate = "wms_durationstartdate";
        public static string endDate = "wms_durationenddate";        
    }
    public static class FreightRate
    {
        public static string freightRate = "wms_freightrate";
        public static string mediumOfTransport = "wms_name";
        public static string fixedPriceLength = "wms_ffxedpricelength";
        public static string fixedPrice = "wms_fixedprice";
        public static string addOnCost = "wms_addoncost";
    }
    public static class FreightItems
    {
        public static string freightItem = "wms_feightitem";
        public static string mediumOfTransport = "wms_mediumoftransport";
        public static string fixedPrice = "wms_fixedprice";
        public static string addOnCost = "wms_addoncost";
        public static string distance = "wms_distance";
        public static string totalTransportCost = "wms_totaltransportcost";
        public static string agreement = "wms_agreementfreightitem";
    }
    public static class Dams
    {
        public static string logicalName = "wms_dam";
        public static string damName = "wms_name";
        public static string availableShares = "wms_availableshares";
    }
    public static class Stakeholders
    {
        public static string logicalName = "wms_stakeholder";
        public static string dam = "wms_dam";
        public static string holdingsPercent = "wms_holdingspercent";
    }
}
