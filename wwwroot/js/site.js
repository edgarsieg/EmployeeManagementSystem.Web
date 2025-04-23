// Slėpti pranešimus po 3s
document.addEventListener("DOMContentLoaded", function () {
    const msg = document.getElementById("flash-message");
    if (msg) {
        setTimeout(() => {
            msg.style.opacity = '0';
            setTimeout(() => msg.remove(), 500);
        }, 3000);
    }

    // Slaptažodžio parodymas/slėpimas
    const toggleBtns = document.querySelectorAll(".toggle-password");
    toggleBtns.forEach(btn => {
        btn.addEventListener("click", () => {
            const input = document.getElementById(btn.dataset.target);
            if (input.type === "password") {
                input.type = "text";
                btn.textContent = "Slėpti";
            } else {
                input.type = "password";
                btn.textContent = "Rodyti";
            }
        });
    });
});
