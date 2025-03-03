document.addEventListener('DOMContentLoaded', function () {
    const cardsView = document.getElementById('cards-view');
    const tableView = document.getElementById('table-view');
    const viewButtons = document.querySelectorAll('.view-btn');
    const memberNameFilter = document.getElementById('memberNameFilter');
    const membershipStatusFilter = document.getElementById('membershipStatusFilter');
    const archiveButtons = document.querySelectorAll('.archive-btn');

    function setView(view) {
        cardsView.style.display = view === 'cards' ? 'grid' : 'none';
        tableView.style.display = view === 'table' ? 'block' : 'none';
        viewButtons.forEach(btn => btn.classList.toggle('active', btn.dataset.view === view));
        localStorage.setItem('preferredView', view);
    }

    function filterMembers() {
        const nameFilter = memberNameFilter.value.toLowerCase();
        const statusFilter = membershipStatusFilter.value.toLowerCase();
        const members = document.querySelectorAll('.member-card, #membersTable tbody tr');

        members.forEach(member => {
            const name = (member.dataset.name || '').toLowerCase();
            const status = (member.dataset.status || '').toLowerCase();
            const nameMatch = name.includes(nameFilter);
            const statusMatch = statusFilter === '' || status === statusFilter;

            member.style.display = nameMatch && statusMatch ? '' : 'none';
        });
    }

    function initializeViewButtons() {
        viewButtons.forEach(button => {
            button.addEventListener('click', function () {
                setView(this.dataset.view);
            });
        });
    }

    function initializeFilters() {
        membershipStatusFilter.value = "GoodStanding";
        memberNameFilter.addEventListener('input', filterMembers);
        membershipStatusFilter.addEventListener('change', filterMembers);
    }

    function initializeTooltips() {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    function initializeArchiveButtons() {
        archiveButtons.forEach(button => {
            button.addEventListener('click', handleArchiveButtonClick);
        });
    }

    function handleArchiveButtonClick() {
        const memberId = this.dataset.id;
        let currentStatus = this.dataset.status.toLowerCase();
        const newStatus = (currentStatus === 'cancelled') ? 'GoodStanding' : 'Cancelled';
        const confirmMessage = (currentStatus === 'cancelled')
            ? "Are you sure you want to unarchive this member?"
            : "Are you sure you want to archive this member?";

        if (!confirm(confirmMessage)) {
            return;
        }

        toggleArchiveMember(memberId, newStatus);
    }

    function toggleArchiveMember(memberId, newStatus) {
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
    }

    function initialize() {
        const preferredView = localStorage.getItem('preferredView') || 'cards';
        setView(preferredView);
        initializeViewButtons();
        initializeFilters();
        initializeTooltips();
        initializeArchiveButtons();
        filterMembers();
    }

    initialize();
});

function sortTable(columnIndex, initialDirection = "asc") {
    var table = document.getElementById("membersTable");
    var switching = true;
    var dir = initialDirection;
    var switchcount = 0;

    while (switching) {
        switching = false;
        var rows = table.rows;

        for (var i = 1; i < (rows.length - 1); i++) {
            var shouldSwitch = false;
            var x = rows[i].getElementsByTagName("td")[columnIndex];
            var y = rows[i + 1].getElementsByTagName("td")[columnIndex];

            if (dir === "asc" ? x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase() : x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
                shouldSwitch = true;
                break;
            }
        }

        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            switchcount++;
        } else if (switchcount === 0 && dir === "asc") {
            dir = "desc";
            switching = true;
        }
    }

    updateSortingClasses(table, columnIndex, dir);
}

function updateSortingClasses(table, columnIndex, direction) {
    var headers = table.rows[0].getElementsByTagName("th");
    for (var i = 0; i < headers.length; i++) {
        headers[i].classList.remove("sorted-asc", "sorted-desc");
    }
    headers[columnIndex].classList.add(direction === "asc" ? "sorted-asc" : "sorted-desc");
}