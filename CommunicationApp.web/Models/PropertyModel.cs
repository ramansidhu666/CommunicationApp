using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CommunicationApp.Web.Models
{
    public class PropertyModel
    {
        public int PropertyId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTrebId { get; set; }
        public string CustomerPhoto { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNo { get; set; }
        public string Kitchen { get; set; }
        public string SideDoorEntrance { get; set; }
        public int? PropertyStatusId { get; set; }
        public string MLS { get; set; }
        public string Price { get; set; }
        public string MininumPrice { get; set; }
        public string MaximumPrice { get; set; }
        public string LocationPrefered { get; set; }
        public string Style { get; set; }
        public string Age { get; set; }
        public string Garage { get; set; }
        public string Bedrooms { get; set; }
        public string Bathrooms { get; set; }
        public string PropertyType { get; set; }
        public string Basement { get; set; }
        public string BasementValue { get; set; }
        public string Community { get; set; }
        public string Size { get; set; }
        public string Remark { get; set; }
        public string WebsiteUrl { get; set; }
        public string Type { get; set; }
        public string Balcony { get; set; }
        public string Alivator { get; set; }
        public string ParkingSpace { get; set; }
        public string GarageType { get; set; }
        public string PropertyTypeStatus { get; set; }
        public string TypeOfProperty { get; set; }//This variable for check of exlisive,Newhotlistiong,lookingfor
        public string PropertyFor { get; set; }
        public string SaleOfBusinessType { get; set; }
        public string PropertyImage { get; set; }
        public string Loundry { get; set; }
        public string Level { get; set; }
        public string ListPriceCode { get; set; }
        public string TypeTaxes { get; set; }
        public string TypeCommercial { get; set; }
        public string CategoryCommercial { get; set; }
        public string Use { get; set; }
        public string Zoning { get; set; }
        public string AdminPhoto { get; set; }
        public string AdminCompanyName { get; set; }
        public string AdminCompanyAddress { get; set; }
        public string AdminCompanyLogo { get; set; }
        public string AdminName { get; set; }
        public string AdminPhoneNo { get; set; }
        public string AdminWebSiteUrl { get; set; }
        public string AdminEmail{ get; set; }
        public int? propertyViewsCount  { get; set; }
        public int? TotalPropertyCount { get; set; }
        public List<string> Imagelist { get; set; }
        public List<PropertyImages> PropertyPhotolist { get; set; }
        public PropertyPhotosAndCustomerModel PropertyPhotosAndCustomerModelList { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsPropertyUpdated { get; set; }
        public int? ParentId{ get; set; }
    }

    public class PropertyImages
    {
    public string imagelist { get; set; }
    public int PropertyImageId { get; set; }
    }
    


    public class PropertyPhotosAndCustomerModel
    {
        public string CustomerName { get; set; }
        public string CustomerPhoto { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNo { get; set; }
        public string PhotoPath { get; set; }
        public string WebsiteUrl { get; set; }
        public List<PropertyImages> PropertyPhotolist { get; set; }
    }
    public class PropertyListModel
    {
        public string UserName { get; set; }
        public string StartDate{ get; set; }
        public string EndDate { get; set; }
        public List<PropertyModel> PropertyModelList { get; set; }
    }
}