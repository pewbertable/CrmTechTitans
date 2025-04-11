using System.Collections.Generic;

namespace CrmTechTitans.ViewModels
{
    public class OpportunityExportOptions
    {
        public List<int>? SelectedOpportunityIds { get; set; } // IDs of selected opportunities
        public List<string>? SelectedFields { get; set; } // Fields user wants in the export
        public bool DownloadAll { get; set; } = false; // Whether to download all opportunities
    }
} 