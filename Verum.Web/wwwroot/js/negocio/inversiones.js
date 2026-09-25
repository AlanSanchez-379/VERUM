(function () {
  const app = document.getElementById("inversiones-app");
  if (!app) return;

  const registerUrl = app.dataset.registerUrl;
  const errorBox = document.getElementById("inversiones-error");
  const submitBtn = document.getElementById("inversiones-submit");

  submitBtn.addEventListener("click", async () => {
    const description = document.getElementById("inversiones-description").value.trim();
    const amount = parseFloat(document.getElementById("inversiones-amount").value);

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
        body: JSON.stringify({ description, amount }),
      });

      if (!res.ok) {
        const data = await res.json();
        errorBox.textContent = data.error || "No se pudo registrar la inversión.";
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
