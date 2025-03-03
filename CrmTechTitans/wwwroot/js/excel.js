document.addEventListener('DOMContentLoaded', function () {
    const exportExcelBtn = document.getElementById('exportExcelBtn');
    const excelExportModal = new bootstrap.Modal(document.getElementById('excelExportModal'));
    const selectAllCheckbox = document.getElementById('selectAllMembers');
    const memberCheckboxes = document.querySelectorAll('.member-checkbox');
    const downloadForm = document.getElementById('downloadForm');
    const downloadExcelBtn = document.getElementById('downloadExcelBtn');

    // Show modal when "Export to Excel" button is clicked
    exportExcelBtn.addEventListener('click', function () {
        excelExportModal.show();
    });

    // Handle "Select All" checkbox
    selectAllCheckbox.addEventListener('change', function () {
        memberCheckboxes.forEach(checkbox => {
            checkbox.checked = this.checked;
        });
    });

    // Update "Select All" checkbox when individual checkboxes change
    memberCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', updateSelectAllCheckbox);
    });

    function updateSelectAllCheckbox() {
        selectAllCheckbox.checked = Array.from(memberCheckboxes).every(checkbox => checkbox.checked);
    }

    // Handle form submission
    downloadExcelBtn.addEventListener('click', function () {
        const selectedMembers = document.querySelectorAll('.member-checkbox:checked');
        const selectedFields = document.querySelectorAll('input[name="SelectedFields"]:checked');

        if (selectedMembers.length === 0) {
            alert('Please select at least one member to download.');
            return;
        }

        if (selectedFields.length === 0) {
            alert('Please select at least one field to include in the download.');
            return;
        }

        // If validation passes, submit the form
        downloadForm.submit();
        excelExportModal.hide();
    });
});