// dashboard.js

// Function to initialize the members chart
window.initializeMembersChart = function (memberCountOverTime) {
    console.log("Initializing the members chart.");

    const ctx = document.getElementById('membersPerMonthChart')?.getContext('2d');
    if (!ctx) {
        console.warn("Canvas element 'membersPerMonthChart' not found.");
        return;
    }

    if (!memberCountOverTime || memberCountOverTime.length === 0) {
        console.warn('No data available for the chart.');
        return;
    }

    // Parse dates into JavaScript Date objects
    const labels = memberCountOverTime.map(item => new Date(item.Date));
    const data = memberCountOverTime.map(item => item.Count);

    // Check if chart instance already exists to avoid "Canvas is already in use" error
    if (window.membersChart) {
        window.membersChart.destroy();
    }

    window.membersChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Total Members Over Time',
                data: data,
                borderColor: 'rgba(75, 192, 192, 1)',
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                fill: true,
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            scales: {
                x: {
                    type: 'time',
                    time: {
                        unit: 'month',
                        tooltipFormat: 'MMM yyyy',
                        displayFormats: {
                            month: 'MMM yyyy'
                        }
                    },
                    title: {
                        display: true,
                        text: 'Date'
                    }
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Member Count'
                    }
                }
            },
            plugins: {
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `Members: ${context.parsed.y}`;
                        }
                    }
                }
            }
        }
    });
};

// Function to handle tab functionality
function initializeTabs() {
    const tabs = document.querySelectorAll('.tab-btn');
    const tabContents = document.querySelectorAll('.tab-content');

    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            // Remove active class from all tabs and contents
            tabs.forEach(t => t.classList.remove('active'));
            tabContents.forEach(c => c.classList.remove('active'));

            // Add active class to clicked tab and corresponding content
            tab.classList.add('active');
            document.getElementById(tab.dataset.tab).classList.add('active');
        });
    });
}

// Function to handle card hover effect
function initializeCardHoverEffect() {
    const cards = document.querySelectorAll('.metric-card');
    cards.forEach(card => {
        card.addEventListener('mouseenter', () => {
            card.style.boxShadow = '0 8px 16px rgba(0, 0, 0, 0.1)';
        });

        card.addEventListener('mouseleave', () => {
            card.style.boxShadow = '0 4px 6px rgba(0, 0, 0, 0.05)';
        });
    });
}

// Initialize all functionalities when the DOM is fully loaded
document.addEventListener("DOMContentLoaded", () => {
    console.log("DOM fully loaded and parsed");

    // Initialize tabs
    initializeTabs();

    // Initialize card hover effect
    initializeCardHoverEffect();

    // Initialize the members chart
    window.initializeMembersChart(window.memberCountOverTime);
});
