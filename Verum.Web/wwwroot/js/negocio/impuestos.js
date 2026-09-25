(function () {
  const app = document.getElementById("impuestos-app");
  if (!app) return;

  const registerUrl = app.dataset.registerUrl;
  const errorBox = document.getElementById("impuestos-error");
  const submitBtn = document.getElementById("impuestos-submit");

  submitBtn.addEventListener("click", async () => {
    const name = document.getElementById("impuestos-name").value.trim();
    const amount = parseFloat(document.getElementById("impuestos-amount").value);
    const dueDate = document.getElementById("impuestos-duedate").value;

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

  app.querySelectorAll("[data-tax-row]").forEach((row) => {
    const btn = row.querySelector("[data-mark-paid]");
    if (!btn) return;
    btn.addEventListener("click", async () => {
      btn.disabled = true;
      try {
        const res = await fetch(`/negocio/impuestos/pagar/${row.dataset.id}`, { method: "POST" });
        if (res.ok) {
          location.reload();
        } else {
          btn.disabled = false;
        }
      } catch (err) {
        btn.disabled = false;
      }
    });
  });
})();
