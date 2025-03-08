﻿document.addEventListener('DOMContentLoaded', function () {
    const tableView = document.getElementById('table-view');
    const viewButtons = document.querySelectorAll('.view-btn');
    const memberNameFilter = document.getElementById('memberNameFilter');
    const membershipStatusFilter = document.getElementById('membershipStatusFilter');
    const recordsPerPageDropdown = document.getElementById('recordsPerPage');
    const prevPageButton = document.getElementById('prevPage');
    const nextPageButton = document.getElementById('nextPage');
    const pageInfo = document.getElementById('pageInfo');

    let currentPage = 1;
    let recordsPerPage = parseInt(recordsPerPageDropdown.value);
    let totalRecords = 0;
    let totalPages = 0;

    // Set default filter to "Good Standing"
    membershipStatusFilter.value = "GoodStanding";

    function setView(view) {
        tableView.style.display = 'block';
        viewButtons.forEach(btn => btn.classList.toggle('active', btn.dataset.view === view));
        localStorage.setItem('preferredView', view);
        updatePagination(); // Update pagination when switching views
    }

    viewButtons.forEach(button => {
        button.addEventListener('click', function () {
            setView(this.dataset.view);
        });
    });

    function getVisibleRows() {
        const nameFilter = memberNameFilter.value.toLowerCase();
        const statusFilter = membershipStatusFilter.value.toLowerCase();

        return Array.from(document.querySelectorAll('#membersTable tbody tr')).filter(row => {
            const name = (row.dataset.name || '').toLowerCase();
            const status = (row.dataset.status || '').toLowerCase();
            return name.includes(nameFilter) && (statusFilter === '' || status === statusFilter);
        });
    }

    function showPage(page) {
        const visibleRows = getVisibleRows();
        const startIndex = (page - 1) * recordsPerPage;
        const endIndex = startIndex + recordsPerPage;

        document.querySelectorAll('#membersTable tbody tr').forEach(row => row.style.display = 'none');
        visibleRows.slice(startIndex, endIndex).forEach(row => row.style.display = '');

        pageInfo.textContent = `Page ${page} of ${Math.ceil(visibleRows.length / recordsPerPage)}`;
        prevPageButton.disabled = page === 1;
        nextPageButton.disabled = page === Math.ceil(visibleRows.length / recordsPerPage);
    }

    function updatePagination() {
        const visibleRows = getVisibleRows();
        totalRecords = visibleRows.length;
        totalPages = Math.ceil(totalRecords / recordsPerPage);

        if (currentPage > totalPages) {
            currentPage = totalPages > 0 ? totalPages : 1;
        }

        showPage(currentPage);
    }

    recordsPerPageDropdown.addEventListener('change', function () {
        recordsPerPage = parseInt(this.value);
        currentPage = 1;
        updatePagination();
    });

    prevPageButton.addEventListener('click', function () {
        if (currentPage > 1) {
            currentPage--;
            showPage(currentPage);
        }
    });

    nextPageButton.addEventListener('click', function () {
        const visibleRows = getVisibleRows();
        const totalPages = Math.ceil(visibleRows.length / recordsPerPage);

        if (currentPage < totalPages) {
            currentPage++;
            showPage(currentPage);
        }
    });

    function filterMembers() {
        currentPage = 1;
        updatePagination();
    }

    memberNameFilter.addEventListener('input', filterMembers);
    membershipStatusFilter.addEventListener('change', filterMembers);

    filterMembers();

    const preferredView = 'table';
    setView(preferredView);

    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    const archiveButtons = document.querySelectorAll('.archive-btn');

    archiveButtons.forEach(button => {
        button.addEventListener('click', function () {
            const memberId = this.dataset.id;
            let currentStatus = this.dataset.status.toLowerCase();
            const newStatus = (currentStatus === 'cancelled') ? 'GoodStanding' : 'Cancelled';
            const confirmMessage = (currentStatus === 'cancelled')
                ? "Are you sure you want to unarchive this member?"
                : "Are you sure you want to archive this member?";

            if (!confirm(confirmMessage)) {
                return;
            }

            fetch(`/Member/ToggleArchive`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    memberId: parseInt(memberId),
                    newStatus: newStatus
                })
            })
                .then(response => {
                    if (!response.ok) {
                        return response.json().then(error => { throw new Error(error.message); });
                    }
                    return response.json();
                })
                .then(data => {
                    alert(data.message);
                    location.reload();
                })
                .catch(error => {
                    console.error("Error:", error);
                    alert("Failed to update member status.");
                });
        });
    });
});

// Sort the table by member name in ascending order on page load
sortTable(1, "asc"); // 0 is the index for the member name column

function sortTable(columnIndex, initialDirection = "asc") {
    var table, rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
    table = document.getElementById("membersTable");
    switching = true;
    dir = initialDirection;

    if (table.rows[0].getElementsByTagName("th")[columnIndex].classList.contains("sorted-asc")) {
        dir = "desc";
    } else if (table.rows[0].getElementsByTagName("th")[columnIndex].classList.contains("sorted-desc")) {
        dir = "asc";
    }

    for (i = 0; i < table.rows[0].getElementsByTagName("th").length; i++) {
        table.rows[0].getElementsByTagName("th")[i].classList.remove("sorted-asc", "sorted-desc");
    }

    table.rows[0].getElementsByTagName("th")[columnIndex].classList.add(dir === "asc" ? "sorted-asc" : "sorted-desc");

    while (switching) {
        switching = false;
        rows = table.rows;

        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;
            x = rows[i].getElementsByTagName("td")[columnIndex];
            y = rows[i + 1].getElementsByTagName("td")[columnIndex];

            if (dir === "asc") {
                if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                    shouldSwitch = true;
                    break;
                }
            } else if (dir === "desc") {
                if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
                    shouldSwitch = true;
                    break;
                }
            }
        }

        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            switchcount++;
        } else {
            if (switchcount === 0 && dir === "asc") {
                dir = "desc";
                switching = true;
            }
        }
    }
}