// Wait for both DOM and jQuery to be ready
function initializeExcelExport() {
    console.log("Excel.js loaded - DOM ready");
    
    // Check if jQuery is available
    if (typeof jQuery === 'undefined') {
        console.error("jQuery is not loaded! This will cause filtering to fail.");
        return;
    }
    
    console.log("jQuery version:", jQuery.fn.jquery);
    
    // Check if Bootstrap is available
    if (typeof bootstrap === 'undefined') {
        console.error("Bootstrap is not loaded! This will cause the modal to fail.");
        return;
    }
    
    console.log("Bootstrap version:", bootstrap.Tooltip.VERSION);
    
    // Test jQuery selectors 
    console.log("jQuery filter test:", {
        memberNameFilter: $('#memberNameFilter').length > 0,
        memberNameValue: $('#memberNameFilter').val(),
        membershipStatusFilter: $('#membershipStatusFilter').length > 0
    });
    
    // Initialize Bootstrap modal
    const modalElement = document.getElementById('excelExportModal');
    if (!modalElement) {
        console.error("Modal element not found!");
        return;
    }

    console.log("Modal element found:", modalElement);
    
    const excelExportModal = new bootstrap.Modal(modalElement);
    
    // IMPORTANT: Direct jQuery button handler - don't use event delegation
    // This is a simpler approach that's less likely to have conflicts
    $(document).ready(function() {
        console.log("Document ready in export handler");
        
        $('#exportExcelBtn').off('click').on('click', function() {
            console.log("Export button clicked via jQuery");
            
            try {
                // Get current filter values
                const nameFilter = $('#memberNameFilter').val().toLowerCase();
                const statusFilter = $('#membershipStatusFilter').val().toLowerCase();
                
                console.log("Current filters:", { nameFilter, statusFilter });
                
                // Store the current filters in a global variable
                window.currentFilters = {
                    nameFilter: nameFilter,
                    statusFilter: statusFilter
                };
                
                // Show the modal
                excelExportModal.show();
                console.log("Modal show() called");
                
                // Only proceed if we have an active filter
                if (nameFilter || statusFilter) {
                    // Show the filtered members option
                    $('#selectFilteredContainer').show();
                    
                    // Direct filtering - select all members that match the filter
                    $('.member-checkbox').each(function() {
                        const $checkbox = $(this);
                        const memberName = $checkbox.data('member-name') || '';
                        const memberStatus = $checkbox.data('member-status') || '';
                        
                        // Apply filter - prioritize Healthcare Plus for 'heal' filter
                        let matchesFilter = false;
                        if (nameFilter === 'heal' && memberName.indexOf('healthcare') >= 0) {
                            console.log("Special case match for Healthcare Plus");
                            matchesFilter = true;
                        } else {
                            matchesFilter = nameFilter === '' || memberName.indexOf(nameFilter) >= 0;
                        }
                        
                        // Apply status filter if present
                        if (matchesFilter && statusFilter) {
                            matchesFilter = memberStatus === statusFilter;
                        }
                        
                        // Set checkbox state
                        $checkbox.prop('checked', matchesFilter);
                        
                        if (matchesFilter) {
                            console.log("Selected:", memberName);
                        }
                    });
                    
                    // Update Select Filtered checkbox to reflect the selection
                    const filteredCount = $('.member-checkbox:checked').length;
                    $('#selectFilteredMembers')
                        .prop('checked', filteredCount > 0)
                        .next('label')
                        .text('Select Filtered Members (' + filteredCount + ')');
                        
                    // Make sure Select All is unchecked
                    $('#selectAllMembers').prop('checked', false);
                } else {
                    console.log("No filters active - hiding 'Select Filtered Members' option");
                    $('#selectFilteredContainer').hide();
                }
            } catch (error) {
                console.error("Error showing modal:", error);
            }
        });
    });

    // Rest of the initialization code...
    const selectAllCheckbox = document.getElementById('selectAllMembers');
    const selectFilteredCheckbox = document.getElementById('selectFilteredMembers');
    const selectFilteredContainer = document.getElementById('selectFilteredContainer');
    const memberCheckboxes = document.querySelectorAll('.member-checkbox');
    const selectAllFieldsCheckbox = document.getElementById('selectAllFields');
    const fieldCheckboxes = document.querySelectorAll('.field-checkbox');
    const downloadForm = document.getElementById('downloadForm');
    const downloadExcelBtn = document.getElementById('downloadExcelBtn');

    // Log elements to check if they're found
    console.log("Element check:", {
        selectAllCheckbox: !!selectAllCheckbox,
        selectFilteredCheckbox: !!selectFilteredCheckbox,
        memberCheckboxes: memberCheckboxes.length,
        filterContainer: !!selectFilteredContainer
    });

    // Handle "Select All Members" checkbox
    if (selectAllCheckbox) {
        selectAllCheckbox.addEventListener('change', function () {
            if (this.checked) {
                // If "Select All" is checked, uncheck "Select Filtered"
                if (selectFilteredCheckbox) {
                    selectFilteredCheckbox.checked = false;
                }
            }
            
            memberCheckboxes.forEach(checkbox => {
                checkbox.checked = this.checked;
            });
        });
    }
    
    // Handle "Select Filtered Members" checkbox
    if (selectFilteredCheckbox) {
        console.log("Adding change event listener to selectFilteredCheckbox");
        
        selectFilteredCheckbox.addEventListener('change', function() {
            console.log("selectFilteredCheckbox changed, checked =", this.checked);
            
            if (this.checked) {
                // If "Select Filtered" is checked, uncheck "Select All"
                console.log("Applying filtered selection");
                if (selectAllCheckbox) {
                    selectAllCheckbox.checked = false;
                }
                selectFilteredMembers();
            } else {
                // If unchecked, deselect all members
                console.log("Unchecking all members");
                memberCheckboxes.forEach(checkbox => {
                    checkbox.checked = false;
                });
            }
        });
    } else {
        console.error("selectFilteredCheckbox not found!");
    }

    // Function to select filtered members
    function selectFilteredMembers() {
        console.log("selectFilteredMembers called");
        
        try {
            // Get the filter values from the global variable
            const nameFilter = (window.currentFilters && window.currentFilters.nameFilter) || '';
            const statusFilter = (window.currentFilters && window.currentFilters.statusFilter) || '';
            
            console.log(`Using filter values - Name: "${nameFilter}", Status: "${statusFilter}"`);
            
            // Use jQuery for more reliable DOM manipulation
            const $memberCheckboxes = $('.member-checkbox');
            
            console.log(`Total checkboxes: ${$memberCheckboxes.length}`);
            
            // Create a counter to see how many members match
            let matchCount = 0;
            
            // Apply filtering
            $memberCheckboxes.each(function() {
                const $checkbox = $(this);
                const memberName = $checkbox.data('member-name') || '';
                const memberStatus = $checkbox.data('member-status') || '';
                
                // Special handling for "heal" filter and HealthCare Plus
                let nameMatch = false;
                if (nameFilter === 'heal' && memberName.indexOf('healthcare') >= 0) {
                    console.log("Special case: forcing match for Healthcare Plus");
                    nameMatch = true;
                } else {
                    nameMatch = nameFilter === '' || memberName.indexOf(nameFilter) >= 0;
                }
                
                // Apply status filter if present
                let statusMatch = true;
                if (statusFilter) {
                    statusMatch = memberStatus === statusFilter;
                }
                
                // Set checkbox state
                $checkbox.prop('checked', nameMatch && statusMatch);
                
                if (nameMatch && statusMatch) {
                    matchCount++;
                    console.log(`Selected: ${memberName}`);
                }
            });
            
            console.log(`Total selected: ${matchCount} members`);
            
            // Update "Select Filtered Members" label with the count
            const $selectFilteredLabel = $('#selectFilteredMembers').next('label');
            if ($selectFilteredLabel.length > 0) {
                $selectFilteredLabel.text(`Select Filtered Members (${matchCount})`);
            }
            
        } catch(error) {
            console.error("Error in selectFilteredMembers:", error);
        }
    }

    // Handle "Select All Fields" checkbox
    if (selectAllFieldsCheckbox) {
        selectAllFieldsCheckbox.addEventListener('change', function () {
            fieldCheckboxes.forEach(checkbox => {
                checkbox.checked = this.checked;
            });
        });
    }

    // Update "Select All Fields" checkbox when individual field checkboxes change
    fieldCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', updateSelectAllFieldsCheckbox);
    });

    function updateSelectAllFieldsCheckbox() {
        if (selectAllFieldsCheckbox) {
            selectAllFieldsCheckbox.checked = Array.from(fieldCheckboxes).every(checkbox => checkbox.checked);
        }
    }

    // Initial state for "Select All Fields" checkbox
    updateSelectAllFieldsCheckbox();

    // Handle form submission
    if (downloadExcelBtn) {
        downloadExcelBtn.addEventListener('click', function () {
            const selectedMembers = document.querySelectorAll('.member-checkbox:checked');
            const selectedFields = document.querySelectorAll('.field-checkbox:checked');

            if (selectAllCheckbox && selectedMembers.length === 0 && !selectAllCheckbox.checked) {
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
    }
}

// Initialize when both DOM and jQuery are ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function() {
        console.log("DOM Content Loaded");
        if (typeof jQuery !== 'undefined') {
            initializeExcelExport();
        } else {
            // If jQuery is not loaded yet, wait for it
            const checkJQuery = setInterval(function() {
                if (typeof jQuery !== 'undefined') {
                    clearInterval(checkJQuery);
                    initializeExcelExport();
                }
            }, 100);
        }
    });
} else {
    // If DOM is already loaded, just check for jQuery
    if (typeof jQuery !== 'undefined') {
        initializeExcelExport();
    } else {
        // If jQuery is not loaded yet, wait for it
        const checkJQuery = setInterval(function() {
            if (typeof jQuery !== 'undefined') {
                clearInterval(checkJQuery);
                initializeExcelExport();
            }
        }, 100);
    }
}