using System;

namespace WireMayr.Modules.DzUpload.Models
{
    public interface IUploadData
    {
        int UploadId { get; set; }
        int ModuleId { get; set; }
        string GUID { get; set; }
        string ItemName { get; set; }
        string ItemNumber { get; set; }
        string AdditionalInfo { get; set; }
        int? Quantity { get; set; }
        string PackagingUnit { get; set; }
        string BoxSize { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string CompanyName { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
        string VatNumber { get; set; }
        string Address { get; set; }
        string City { get; set; }
        string Zip { get; set; }
        string Country { get; set; }
        string CompanyWebsite { get; set; }
        bool PrivacyConsent { get; set; }
        int CreatedByUserId { get; set; }
        DateTime CreatedOnDate { get; set; }
        int LastUpdatedByUserId { get; set; }
        DateTime LastUpdatedOnDate { get; set; }
    }

    public interface IUploadFile
    {
        int FileId { get; set; }

        int UploadId { get; set; }

        string FileName { get; set; }

        int FileSize { get; set; }

        string MimeType { get; set; }
    }        
}