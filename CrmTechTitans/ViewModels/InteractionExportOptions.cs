using System.Collections.Generic;

namespace CrmTechTitans.ViewModels
{
    public class InteractionExportOptions
    {
        public List<int>? SelectedInteractionIds { get; set; } // IDs of selected interactions
        public List<string>? SelectedFields { get; set; } // Fields user wants in the export
        public bool DownloadAll { get; set; } = false; // Whether to download all interactions
    }
} 