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

    // Create the chart
    window.membersChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Total Members Over Time',
                data: data,
                // Slightly thicker, teal line
                borderColor: '#00bcd4',
                borderWidth: 2,
                // A gradient fill from teal to transparent
                backgroundColor: function (context) {
                    const chart = context.chart;
                    const { ctx, chartArea } = chart;
                    if (!chartArea) {
                        // Chart not yet rendered, fallback color
                        return 'rgba(75, 192, 192, 0.2)';
                    }
                    const gradient = ctx.createLinearGradient(0, chartArea.top, 0, chartArea.bottom);
                    gradient.addColorStop(0, 'rgba(0, 188, 212, 0.4)'); // top
                    gradient.addColorStop(1, 'rgba(0, 188, 212, 0)');   // bottom
                    return gradient;
                },
                fill: true,
                // Make the line slightly curved
                tension: 0.3,
                // Bigger points for a modern look
                pointRadius: 5,
                pointHoverRadius: 7
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false, // fill the container's height

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
                    },
                    // Dashed & lighter grid lines
                    grid: {
                        color: 'rgba(0, 0, 0, 0.05)',
                        borderDash: [2, 2]
                    }
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Member Count'
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.05)',
                        borderDash: [2, 2]
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
                },
                // Optionally tweak the legend text color / size
                legend: {
                    labels: {
                        color: '#444',
                        font: {
                            size: 14,
                            family: 'sans-serif'
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
