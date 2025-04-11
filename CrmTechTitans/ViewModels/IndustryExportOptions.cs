using System.Collections.Generic;

namespace CrmTechTitans.ViewModels
{
    public class IndustryExportOptions
    {
        public List<int>? SelectedIndustryIds { get; set; } // IDs of selected industries
        public List<string>? SelectedFields { get; set; } // Fields user wants in the export
        public bool DownloadAll { get; set; } = false; // Whether to download all industries
    }
} 