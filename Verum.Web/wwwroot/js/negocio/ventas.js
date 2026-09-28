(function () {
  const app = document.getElementById("ventas-app");
  if (!app) return;

  const registerUrl = app.dataset.registerUrl;
  const errorBox = document.getElementById("ventas-error");
  const submitBtn = document.getElementById("ventas-submit");

  submitBtn.addEventListener("click", async () => {
    const description = document.getElementById("ventas-description").value.trim();
    const amount = parseFloat(document.getElementById("ventas-amount").value);
    const accountId = document.getElementById("ventas-account").value;

    errorBox.classList.add("hidden");

    if (!amount || amount <= 0) {
      errorBox.textContent = "Ingresá un monto válido.";
      errorBox.classList.remove("hidden");
      return;
    }

    submitBtn.disabled = true;

    try {
      const res = await fetch(registerUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ description, amount, accountId }),
      });

      const data = await res.json();

      if (!res.ok) {
        errorBox.textContent = data.error || "No se pudo registrar la venta.";
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
})();
