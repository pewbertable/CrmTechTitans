// dashboard.js
document.addEventListener("DOMContentLoaded", () => {
    // Card hover effect
    const cards = document.querySelectorAll('.link-card');
    cards.forEach(card => {
        card.addEventListener('mouseenter', () => {
            card.style.boxShadow = '0 8px 16px rgba(0, 0, 0, 0.1)';
        });

        card.addEventListener('mouseleave', () => {
            card.style.boxShadow = '0 4px 6px rgba(0, 0, 0, 0.05)';
        });
    });
});