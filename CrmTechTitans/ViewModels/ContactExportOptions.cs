using System.Collections.Generic;

namespace CrmTechTitans.ViewModels
{
    public class ContactExportOptions
    {
        public List<int>? SelectedContactIds { get; set; } // IDs of selected contacts
        public List<string>? SelectedFields { get; set; } // Fields user wants in the export
        public bool DownloadAll { get; set; } = false; // Whether to download all contacts
    }
} 