//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ItemsEncoder
{
    using System;
    using System.Collections.Generic;
    
    public partial class Item
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> SubCategoryID { get; set; }
        public Nullable<int> BrandID { get; set; }
        public Nullable<int> ColorID { get; set; }
        public Nullable<int> SizeID { get; set; }
        public Nullable<int> UnitOfMeasurementID { get; set; }
        public Nullable<int> ReOrderLevel { get; set; }
        public Nullable<int> MinimumStock { get; set; }
        public Nullable<int> MaximumStock { get; set; }
        public string SubstituteItem { get; set; }
        public string ComplementaryField { get; set; }
        public Nullable<decimal> LastUnitCost { get; set; }
        public Nullable<decimal> WeightedAverageCost { get; set; }
        public Nullable<int> VendorCoding { get; set; }
        public string Image { get; set; }
        public Nullable<int> SubCategoriesTypesID { get; set; }
        public Nullable<int> LeadTime { get; set; }
        public string ItemID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string DescriptionPurchase { get; set; }
        public string Year { get; set; }
        public Nullable<int> DailyAverageUsage { get; set; }
    
        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public virtual UnitOfMeasurement UnitOfMeasurement { get; set; }
    }
}
