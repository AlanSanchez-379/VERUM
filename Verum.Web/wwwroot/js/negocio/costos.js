(function () {
  const app = document.getElementById("costos-app");
  if (!app) return;

  const registerUrl = app.dataset.registerUrl;
  const errorBox = document.getElementById("costos-error");
  const submitBtn = document.getElementById("costos-submit");

  submitBtn.addEventListener("click", async () => {
    const name = document.getElementById("costos-name").value.trim();
    const amount = parseFloat(document.getElementById("costos-amount").value);
    const dueDate = document.getElementById("costos-duedate").value;

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
        body: JSON.stringify({ name, amount, dueDate }),
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

  app.querySelectorAll("[data-cost-row]").forEach((row) => {
    const btn = row.querySelector("[data-mark-paid]");
    if (!btn) return;
    btn.addEventListener("click", async () => {
      errorBox.classList.add("hidden");
      btn.disabled = true;
      try {
        const res = await fetch(`/negocio/costos/pagar/${row.dataset.id}`, { method: "POST" });
        if (res.ok) {
          location.reload();
        } else {
          const data = await res.json().catch(() => ({}));
          errorBox.textContent = data.error || "No se pudo marcar como pagado.";
          errorBox.classList.remove("hidden");
          btn.disabled = false;
        }
      } catch (err) {
        errorBox.textContent = "Error de conexión. Intenta de nuevo.";
        errorBox.classList.remove("hidden");
        btn.disabled = false;
      }
    });
  });
})();
