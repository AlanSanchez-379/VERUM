(function () {
  const app = document.getElementById("misnegocios-app");
  if (!app) return;

  app.querySelectorAll("[data-business-row]").forEach((row) => {
    const toggleBtn = row.querySelector("[data-rename-toggle]");
    const cancelBtn = row.querySelector("[data-rename-cancel]");
    const form = row.querySelector("[data-rename-form]");
    const input = row.querySelector("[data-rename-input]");

    toggleBtn.addEventListener("click", () => {
      form.classList.toggle("hidden");
      if (!form.classList.contains("hidden")) {
        input.focus();
        input.select();
      }
    });

    cancelBtn.addEventListener("click", () => {
      form.classList.add("hidden");
    });
  });
})();
