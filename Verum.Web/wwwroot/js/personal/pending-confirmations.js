(function () {
  const app = document.getElementById("pending-confirmations-app");
  if (!app) return;

  app.querySelectorAll("[data-pending-row]").forEach((row) => {
    const id = row.dataset.id;
    const errorBox = row.querySelector("[data-pending-error]");
    const confirmBtn = row.querySelector("[data-confirm]");
    const nothingBtn = row.querySelector("[data-confirm-nothing]");

    async function confirm(actualAmount, accountId) {
      errorBox.classList.add("hidden");
      confirmBtn.disabled = true;
      nothingBtn.disabled = true;

      try {
        const res = await fetch(`/personal/ingresos-recurrentes/confirmar/${id}`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ accountId, actualAmount }),
        });

        if (!res.ok) {
          const data = await res.json();
          errorBox.textContent = data.error || "No se pudo confirmar.";
          errorBox.classList.remove("hidden");
          confirmBtn.disabled = false;
          nothingBtn.disabled = false;
          return;
        }

        location.reload();
      } catch (err) {
        errorBox.textContent = "Error de conexión. Intenta de nuevo.";
        errorBox.classList.remove("hidden");
        confirmBtn.disabled = false;
        nothingBtn.disabled = false;
      }
    }

    confirmBtn.addEventListener("click", () => {
      const amount = parseFloat(row.querySelector("[data-actual-amount]").value);
      const accountId = row.querySelector("[data-account-select]").value;
      if (!amount || amount <= 0) {
        errorBox.textContent = "Ingresá un monto válido, o usá 'No llegó nada'.";
        errorBox.classList.remove("hidden");
        return;
      }
      confirm(amount, accountId);
    });

    nothingBtn.addEventListener("click", () => {
      confirm(0, null);
    });
  });
})();
