(function () {
  const app = document.getElementById("gastos-app");
  if (!app) return;

  const registerUrl = app.dataset.registerUrl;
  const errorBox = document.getElementById("gastos-error");
  const submitBtn = document.getElementById("gastos-submit");

  submitBtn.addEventListener("click", async () => {
    const category = document.getElementById("gastos-category").value.trim();
    const amount = parseFloat(document.getElementById("gastos-amount").value);
    const accountId = document.getElementById("gastos-account").value;

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
        body: JSON.stringify({ category, amount, accountId }),
      });

      const data = await res.json();

      if (!res.ok) {
        errorBox.textContent = data.error || "No se pudo registrar el gasto.";
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
