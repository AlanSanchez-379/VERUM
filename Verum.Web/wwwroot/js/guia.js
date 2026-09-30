(function () {
  document.querySelectorAll("[data-guia-toggle]").forEach((btn) => {
    btn.addEventListener("click", () => {
      const card = btn.closest(".guia-card");
      const detail = card.querySelector("[data-guia-detail]");
      const chevron = card.querySelector("[data-guia-chevron]");
      detail.classList.toggle("hidden");
      chevron.classList.toggle("rotate-180");
    });
  });
})();
