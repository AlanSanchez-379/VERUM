(function () {
  const app = document.getElementById("ingreso-app");
  if (!app) return;

  const available = Number(app.dataset.available);
  const registerUrl = app.dataset.registerUrl;

  const steps = ["amount", "category", "account", "confirm"];
  const panels = {};
  steps.forEach((s) => (panels[s] = app.querySelector(`[data-step="${s}"]`)));
  const backBtn = app.querySelector("[data-back]");
  const errorBox = document.getElementById("ingreso-error");

  const state = { amount: "0", tag: null, accountId: null, accountName: null };

  function fmt(n) {
    return "$" + Math.round(n).toLocaleString("en-US");
  }

  function amountValue() {
    return parseFloat(state.amount) || 0;
  }

  function showStep(name) {
    steps.forEach((s) => {
      panels[s].classList.toggle("hidden", s !== name);
      panels[s].classList.toggle("flex", s === name);
    });
    backBtn.classList.toggle("invisible", name === "amount");
  }

  function renderAmount() {
    app.querySelector("[data-amount-display]").textContent = state.amount;
    app.querySelector("[data-after-amount]").textContent = fmt(available + amountValue());
    app.querySelector("[data-next-from-amount]").disabled = amountValue() <= 0;
  }

  // --- Paso 1: keypad ---
  app.querySelectorAll("[data-key]").forEach((btn) => {
    btn.addEventListener("click", () => {
      const key = btn.dataset.key;
      if (key === "del") {
        state.amount = state.amount.length > 1 ? state.amount.slice(0, -1) : "0";
      } else if (key === ".") {
        if (!state.amount.includes(".")) state.amount += ".";
      } else {
        if (state.amount.replace(".", "").length >= 7) return;
        state.amount = state.amount === "0" ? key : state.amount + key;
      }
      renderAmount();
    });
  });

  app.querySelector("[data-next-from-amount]").addEventListener("click", () => {
    if (amountValue() <= 0) return;
    const full = fmt(amountValue());
    app.querySelector("[data-amount-full-display]").textContent = full;
    app.querySelector("[data-amount-neg-display]").textContent = "+" + full;
    showStep("category");
  });

  // --- Paso 2: origen ---
  app.querySelectorAll("[data-tag]").forEach((btn) => {
    btn.addEventListener("click", () => {
      state.tag = btn.dataset.tag;
      app.querySelectorAll("[data-tag]").forEach((b) => {
        const active = b === btn;
        b.classList.toggle("border-paper", active);
        b.classList.toggle("text-paper", active);
        b.classList.toggle("bg-ash", active);
        b.classList.toggle("border-ash", !active);
        b.classList.toggle("text-muted", !active);
        b.classList.toggle("bg-graphite-2", !active);
      });
      app.querySelector("[data-next-from-category]").disabled = false;
    });
  });

  app.querySelector("[data-next-from-category]").addEventListener("click", () => {
    if (!state.tag) return;
    showStep("account");
  });

  // --- Paso 3: cuenta ---
  app.querySelectorAll("[data-account-id]").forEach((btn) => {
    btn.addEventListener("click", () => {
      state.accountId = btn.dataset.accountId;
      state.accountName = btn.dataset.account;
      app.querySelectorAll("[data-account-id]").forEach((b) => {
        const active = b === btn;
        b.classList.toggle("border-paper", active);
        b.classList.toggle("bg-graphite-2", active);
        b.classList.toggle("border-ash", !active);
      });
      app.querySelector("[data-register]").disabled = false;
    });
  });

  app.querySelector("[data-register]").addEventListener("click", async () => {
    if (!state.accountId) return;
    errorBox.classList.add("hidden");
    const registerBtn = app.querySelector("[data-register]");
    registerBtn.disabled = true;

    try {
      const res = await fetch(registerUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          accountId: state.accountId,
          source: state.tag,
          amount: amountValue(),
        }),
      });

      const data = await res.json();

      if (!res.ok) {
        errorBox.textContent = data.error || "No se pudo registrar el ingreso.";
        errorBox.classList.remove("hidden");
        registerBtn.disabled = false;
        return;
      }

      const full = fmt(amountValue());
      app.querySelector("[data-amount-neg-full]").textContent = "+" + full;
      app.querySelector("[data-summary-before]").textContent = fmt(available);
      app.querySelector("[data-summary-after]").textContent = fmt(data.totalAvailable);
      app.querySelector("[data-summary-account-name]").textContent = state.accountName;
      app.querySelector("[data-summary-account-balance]").textContent = fmt(data.accountBalance);
      showStep("confirm");
    } catch (err) {
      errorBox.textContent = "Error de conexión. Intenta de nuevo.";
      errorBox.classList.remove("hidden");
      registerBtn.disabled = false;
    }
  });

  // --- Navegacion ---
  backBtn.addEventListener("click", () => {
    const current = steps.find((s) => !panels[s].classList.contains("hidden"));
    const idx = steps.indexOf(current);
    if (idx > 0) showStep(steps[idx - 1]);
  });

  app.querySelector("[data-restart]").addEventListener("click", () => {
    location.reload();
  });

  renderAmount();
  showStep("amount");
})();
