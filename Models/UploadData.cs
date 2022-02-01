using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
using System.ComponentModel.DataAnnotations;

namespace WireMayr.Modules.DzUpload.Models
{
    [TableName("wm_UploadData")]
    //setup the primary key for table
    [PrimaryKey("UploadId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("UploadData", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    [Scope("ModuleId")]
    public class UploadData : IUploadData
    {
        public UploadData()
        {
            UploadId = -1;
        }

        public int UploadId { get; set; }

        public int ModuleId { get; set; }

        public string GUID { get; set; }

        [Required]
        [MaxLength(100)]
        public string ItemName { get; set; }

        [MaxLength(100)]
        public string ItemNumber { get; set; }

        [MaxLength(5000)]
        public string AdditionalInfo { get; set; }

        [Required]
        public int? Quantity { get; set; }

        [MaxLength(100)]
        public string PackagingUnit { get; set; }

        [MaxLength(100)]
        public string BoxSize { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string CompanyName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string VatNumber { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string Zip { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(100)]
        public string CompanyWebsite { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true")]
        public bool PrivacyConsent { get; set; }

        public int CreatedByUserId { get; set; }

        public DateTime CreatedOnDate { get; set; }

        public int LastUpdatedByUserId { get; set; }

        public DateTime LastUpdatedOnDate { get; set; }
    }


    [TableName("wm_UploadFiles")]
    //setup the primary key for table
    [PrimaryKey("FileId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("UploadFile", CacheItemPriority.Default, 20)]
    [Scope("UploadId")]
    public class UploadFile : IUploadFile
    {
        public UploadFile()
        {
            FileId = -1;
        }
        public int FileId { get; set; }

        public int UploadId { get; set; }

        public string FileName { get; set; }

        public int FileSize { get; set; }

        public string MimeType { get; set; }
    }
}