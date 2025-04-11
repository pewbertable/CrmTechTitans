using System.Collections.Generic;

namespace CrmTechTitans.ViewModels
{
    public class AddressExportOptions
    {
        public List<int>? SelectedAddressIds { get; set; } // IDs of selected addresses
        public List<string>? SelectedFields { get; set; } // Fields user wants in the export
        public bool DownloadAll { get; set; } = false; // Whether to download all addresses
    }
} 