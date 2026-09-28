(function () {
  const submitBtn = document.getElementById("patron-submit");
  if (!submitBtn) return;

  const errorBox = document.getElementById("patron-error");

  submitBtn.addEventListener("click", async () => {
    const source = document.getElementById("patron-source").value.trim();
    const amount = parseFloat(document.getElementById("patron-amount").value);
    const day = parseInt(document.getElementById("patron-day").value, 10);

    errorBox.classList.add("hidden");

    if (!amount || amount <= 0) {
      errorBox.textContent = "Ingresá un monto esperado válido.";
      errorBox.classList.remove("hidden");
      return;
    }
    if (!day || day < 1 || day > 28) {
      errorBox.textContent = "Ingresá un día del mes entre 1 y 28.";
      errorBox.classList.remove("hidden");
      return;
    }

    submitBtn.disabled = true;

    try {
      const res = await fetch("/personal/ingresos-recurrentes/registrar", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ source, expectedAmount: amount, dayOfMonth: day }),
      });

      if (!res.ok) {
        const data = await res.json();
        errorBox.textContent = data.error || "No se pudo crear el patrón.";
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
