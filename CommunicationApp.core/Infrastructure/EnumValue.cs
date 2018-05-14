using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Core.Infrastructure
{
    public class EnumValue
    {
         

      
       public enum PropertySatus { [Description("Exclusive Residential")]ExclusiveResidential = 1,[Description("Exclusive Commercial")] ExclusiveCommercial = 2,[Description("Exclusive Condo")] ExclusiveResidentialCondo = 9,[Description("New-Hot Commercial")] NewHotListingCommercial = 3,[Description("New-Hot Residential")] NewHotListingResidential = 4,[Description("New-Hot Condo")] NewHotListingResidentialCondo = 8,[Description("Looking For Commercial")] LookingForCommercial = 5,[Description("Looking For Residential")] LookingForResidential = 6,[Description("Looking For Condo")] LookingForResidentialCondo = 7 }
       public enum PropertyForSatus { Sale = 1, Lease = 2, SubLease = 3}
       public enum SaleOfBusinessSatus { WithProperty = 1, WithoutProperty = 2, Land = 3 }
       public enum AgentSatus { [Description("Agent Required")]AgentRequired = 1,[Description("Agent Available")] AgentAvailable = 2 } 
       public enum GoogleDistanceType { Km = 1, Miles = 2, Meters = 3 }
       public enum GoogleDistanceTypeInStr { [Description("km")]Km, [Description("miles")] Miles, [Description("meters")] Meters }
       public enum DeviceType { [Description("ios")]Ios, [Description("android")] Android }
       public enum BrokerageType { Frequently = 1,  Occasionally = 2,Seldom = 3, Never = 4, NA = 5, DontKnow = 6 }
       

       #region Offered Preb Form enums
       public enum AgreementofPurchaseandSale { [Description("FREEHOLD")]FREEHOLD=1, [Description("CONDO")] CONDO=2, [Description("Town House With Fee")] TownHouseWithFee=3 }
       public enum GarbageRemovalOrCondoFee { [Description("Condo Fee")]CondoFee = 1, [Description("Garbage Removal")] GarbageRemoval = 2 }
       public enum Arewethe { [Description("Co-Operating Brokerage")]Co_OperatingBrokerage = 1, [Description("Listing Brokerage & Co-Operating Brokerage")] ListingBrokerageCoOperatingBrokerage = 2 }
       public enum Deposit { [Description("Here With")]HereWith = 1, [Description("upon acceptance")] Uponacceptance = 2 }

       public enum FinalView_Option { [Description("One")]One = 1, [Description("Two More Times")] TwoMoreTimes = 2 }
       
       
       #endregion



       public static string GetEnumDescription(Enum value)
       {
           FieldInfo fi = value.GetType().GetField(value.ToString());

           DescriptionAttribute[] attributes =
               (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

           if (attributes != null && attributes.Length > 0)
               return attributes[0].Description;
           else
               return value.ToString();
       }
    }
}
