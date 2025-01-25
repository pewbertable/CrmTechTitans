document.addEventListener("DOMContentLoaded", () => {
    const cards = document.querySelectorAll(".dashboard-card")
    cards.forEach((card) => {
        card.addEventListener("mouseover", function () {
            this.style.transform = "translateY(-5px)"
            this.style.boxShadow = "0 6px 10px rgba(0, 0, 0, 0.15)"
        })
        card.addEventListener("mouseout", function () {
            this.style.transform = "translateY(0)"
            this.style.boxShadow = "0 4px 6px rgba(0, 0, 0, 0.1)"
        })
    })
})

