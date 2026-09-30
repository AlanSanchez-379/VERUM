(function () {
  const app = document.getElementById("porcobrar-app");
  if (!app) return;

  const registerUrl = app.dataset.registerUrl;
  const errorBox = document.getElementById("porcobrar-error");
  const submitBtn = document.getElementById("porcobrar-submit");

  submitBtn.addEventListener("click", async () => {
    const clientName = document.getElementById("porcobrar-client").value.trim();
    const description = document.getElementById("porcobrar-description").value.trim();
    const amount = parseFloat(document.getElementById("porcobrar-amount").value);
    const dueDate = document.getElementById("porcobrar-duedate").value;

    errorBox.classList.add("hidden");

    if (!amount || amount <= 0) {
      errorBox.textContent = "Ingresá un monto válido.";
      errorBox.classList.remove("hidden");
      return;
    }
    if (!dueDate) {
      errorBox.textContent = "Ingresá una fecha de vencimiento.";
      errorBox.classList.remove("hidden");
      return;
    }

    submitBtn.disabled = true;

    try {
      const res = await fetch(registerUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ clientName, description, amount, dueDate }),
      });

      if (!res.ok) {
        const data = await res.json();
        errorBox.textContent = data.error || "No se pudo registrar.";
        errorBox.classList.remove("hidden");
        submitBtn.disabled = false;
        return;
      }

      location.reload();
    } catch (err) {
      errorBox.textContent = "Error de conexión. Intenta de nuevo.";
      errorBox.classList.remove("hidden");
      submitBtn.disabled = false;
    }
  });

  app.querySelectorAll("[data-receivable-row]").forEach((row) => {
    const btn = row.querySelector("[data-mark-collected]");
    if (!btn) return;

    const accountSelect = row.querySelector("[data-receivable-account]");
    const rowError = row.querySelector("[data-receivable-error]");

    btn.addEventListener("click", async () => {
      rowError.classList.add("hidden");
      btn.disabled = true;
      try {
        const res = await fetch(`/negocio/porcobrar/cobrar/${row.dataset.id}`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ accountId: accountSelect.value }),
        });
        if (res.ok) {
          location.reload();
        } else {
          const data = await res.json().catch(() => ({}));
          rowError.textContent = data.error || "No se pudo marcar como cobrado.";
          rowError.classList.remove("hidden");
          btn.disabled = false;
        }
      } catch (err) {
        rowError.textContent = "Error de conexión. Intenta de nuevo.";
        rowError.classList.remove("hidden");
        btn.disabled = false;
      }
    });
  });
})();
