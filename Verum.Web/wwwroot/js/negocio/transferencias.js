(function () {
  const submitBtn = document.getElementById("transfer-submit");
  if (!submitBtn) return;

  const errorBox = document.getElementById("transfer-error");
  const dirToBusiness = document.getElementById("dir-to-business");
  const dirToPersonal = document.getElementById("dir-to-personal");
  let direction = "to_business";

  function setDirection(next) {
    direction = next;
    const active = "flex-1 py-2.5 text-[12.5px] font-medium text-paper bg-graphite-2";
    const inactive = "flex-1 py-2.5 text-[12.5px] font-medium text-muted";
    dirToBusiness.className = direction === "to_business" ? active : inactive;
    dirToPersonal.className = direction === "to_personal" ? active : inactive;
  }

  dirToBusiness.addEventListener("click", () => setDirection("to_business"));
  dirToPersonal.addEventListener("click", () => setDirection("to_personal"));

  submitBtn.addEventListener("click", async () => {
    const personalAccountId = document.getElementById("transfer-personal-account").value;
    const businessAccountId = document.getElementById("transfer-business-account").value;
    const amount = parseFloat(document.getElementById("transfer-amount").value);
    const note = document.getElementById("transfer-note").value.trim();

    errorBox.classList.add("hidden");

    if (!personalAccountId || !businessAccountId) {
      errorBox.textContent = "Elegí una cuenta personal y una cuenta del negocio.";
      errorBox.classList.remove("hidden");
      return;
    }

    if (!amount || amount <= 0) {
      errorBox.textContent = "Ingresá un monto mayor a cero.";
      errorBox.classList.remove("hidden");
      return;
    }

    submitBtn.disabled = true;

    try {
      const res = await fetch("/negocio/transferencias/transferir", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ personalAccountId, businessAccountId, direction, amount, note }),
      });

      if (!res.ok) {
        const data = await res.json();
        errorBox.textContent = data.error || "No se pudo hacer la transferencia.";
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
