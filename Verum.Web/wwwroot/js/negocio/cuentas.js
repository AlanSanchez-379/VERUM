(function () {
  const submitBtn = document.getElementById("cuenta-submit");
  if (!submitBtn) return;

  const errorBox = document.getElementById("cuenta-error");

  submitBtn.addEventListener("click", async () => {
    const name = document.getElementById("cuenta-name").value.trim();
    const subtitle = document.getElementById("cuenta-subtitle").value.trim();

    errorBox.classList.add("hidden");

    if (!name) {
      errorBox.textContent = "Ingresá un nombre.";
      errorBox.classList.remove("hidden");
      return;
    }

    submitBtn.disabled = true;

    try {
      const res = await fetch("/negocio/cuentas/registrar", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name, subtitle }),
      });

      if (!res.ok) {
        const data = await res.json();
        errorBox.textContent = data.error || "No se pudo crear la cuenta.";
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
