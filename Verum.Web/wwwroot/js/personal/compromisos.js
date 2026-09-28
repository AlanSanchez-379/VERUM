(function () {
  const app = document.getElementById("compromisos-app");
  if (!app) return;

  app.querySelectorAll("[data-commitment-row]").forEach((row) => {
    const btn = row.querySelector("[data-mark-paid]");
    if (!btn) return;

    const accountSelect = row.querySelector("[data-commitment-account]");
    const errorBox = row.querySelector("[data-commitment-error]");

    btn.addEventListener("click", async () => {
      errorBox.classList.add("hidden");
      btn.disabled = true;

      try {
        const res = await fetch(`/personal/compromisos/${row.dataset.id}/pagar`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ accountId: accountSelect.value }),
        });

        if (!res.ok) {
          const data = await res.json().catch(() => ({}));
          errorBox.textContent = data.error || "No se pudo marcar como pagado.";
          errorBox.classList.remove("hidden");
          btn.disabled = false;
          return;
        }

        location.reload();
      } catch (err) {
        errorBox.textContent = "Error de conexión. Intenta de nuevo.";
        errorBox.classList.remove("hidden");
        btn.disabled = false;
      }
    });
  });
})();
