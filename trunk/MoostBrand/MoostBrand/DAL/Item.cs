namespace MoostBrand.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    public partial class Item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Item()
        {
            RequisitionDetails = new HashSet<RequisitionDetail>();
            RequisitionDetails1 = new HashSet<RequisitionDetail>();
        }

        public int ID { get; set; }

        public string ItemID { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Barcode { get; set; }

        [StringLength(50)]
        [Display(Name = "Description for Sales")]
        public string Description { get; set; }

        [StringLength(50)]
        [Display(Name = "Description for Purchase")]
        public string DescriptionPurchase { get; set; }

        public int? CategoryID { get; set; }

        public int? SubCategoryID { get; set; }

        public int? BrandID { get; set; }

        public int? ColorID { get; set; }

        public int? SizeID { get; set; }

        public int? UnitOfMeasurementID { get; set; }

        public int? ReOrderLevel { get; set; }

        public int? MinimumStock { get; set; }

        public int? MaximumStock { get; set; }

        [NotMapped]
        public bool Proceed { get; set; }

        public int? LeadTime { get; set; }

        [StringLength(50)]
        public string SubstituteItem { get; set; }

        [StringLength(50)]
        public string ComplementaryField { get; set; }

        public decimal? LastUnitCost { get; set; }

        public decimal? WeightedAverageCost { get; set; }

        public int? VendorCoding { get; set; }

        public string Image { get; set; }

        public int? SubCategoriesTypesID { get; set; }

        public virtual Brand Brand { get; set; }

        public virtual Category Category { get; set; }

        public virtual Color Color { get; set; }

        public virtual Size Size { get; set; }

        public virtual SubCategory SubCategory { get; set; }

        public virtual SubCategoriesType SubCategoriesType { get; set; }

        public virtual UnitOfMeasurement UnitOfMeasurement { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequisitionDetail> RequisitionDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequisitionDetail> RequisitionDetails1 { get; set; }

        [Display(Name = "Upload Image")]
        [ValidateImage]
        [NotMapped]
        public HttpPostedFileBase Img { get; set; }

        public class ValidateImageAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                int MaxContentLength = 1024 * 1024 * 10; //10 MB
                string[] AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".gif", ".png" };

                var file = value as HttpPostedFileBase;

                if (file == null)
                {
                    return true;
                }
                else if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower()))
                {
                    ErrorMessage = "Please upload Your Photo of type: " + string.Join(", ", AllowedFileExtensions);
                    return false;
                }
                else if (file.ContentLength > MaxContentLength)
                {
                    ErrorMessage = "Your Photo is too large, maximum allowed size is : " + (MaxContentLength / 1024).ToString() + "MB";
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}