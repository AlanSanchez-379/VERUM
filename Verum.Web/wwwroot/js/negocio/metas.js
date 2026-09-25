(function () {
  const app = document.getElementById("metas-app");
  if (!app) return;

  const registerUrl = app.dataset.registerUrl;
  const errorBox = document.getElementById("metas-error");
  const submitBtn = document.getElementById("metas-submit");

  submitBtn.addEventListener("click", async () => {
    const name = document.getElementById("metas-name").value.trim();
    const targetAmount = parseFloat(document.getElementById("metas-target").value);
    const targetDate = document.getElementById("metas-date").value;

    errorBox.classList.add("hidden");

    if (!targetAmount || targetAmount <= 0) {
      errorBox.textContent = "Ingresá un monto objetivo válido.";
      errorBox.classList.remove("hidden");
      return;
    }
    if (!targetDate) {
      errorBox.textContent = "Ingresá una fecha objetivo.";
      errorBox.classList.remove("hidden");
      return;
    }

    submitBtn.disabled = true;

    try {
      const res = await fetch(registerUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name, targetAmount, targetDate }),
      });

      if (!res.ok) {
        const data = await res.json();
        errorBox.textContent = data.error || "No se pudo crear la meta.";
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

  app.querySelectorAll("[data-goal-card]").forEach((card) => {
    const btn = card.querySelector("[data-contribute-submit]");
    const input = card.querySelector("[data-contribute-amount]");
    if (!btn || !input) return;

    btn.addEventListener("click", async () => {
      const amount = parseFloat(input.value);
      if (!amount || amount <= 0) return;

      btn.disabled = true;
      try {
        const res = await fetch(`/negocio/metas/aportar/${card.dataset.id}`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ amount }),
        });
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
