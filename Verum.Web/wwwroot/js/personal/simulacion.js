(function () {
  const app = document.getElementById("sim-app");
  if (!app) return;

  const available = Number(app.dataset.available);
  const saving = Number(app.dataset.saving);
  const goalDate = new Date(app.dataset.goalDate);

  const steps = ["build", "impact"];
  const panels = {};
  steps.forEach((s) => (panels[s] = app.querySelector(`[data-step="${s}"]`)));
  const backBtn = app.querySelector("[data-back]");
  const eyebrow = app.querySelector("[data-eyebrow]");

  const state = { kind: "gastar", amount: 0 };

  const meses = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

  function fmt(n) {
    return "$" + Math.round(n).toLocaleString("en-US");
  }

  function days(amt) {
    if (saving <= 0) return 0;
    return Math.round((amt / saving) * 30);
  }

  function showStep(name) {
    steps.forEach((s) => {
      panels[s].classList.toggle("hidden", s !== name);
      panels[s].classList.toggle("flex", s === name);
    });
    backBtn.classList.toggle("invisible", name === "build");
    eyebrow.textContent = name === "build" ? "Escenario" : "Impacto";
  }

  function renderBuild() {
    app.querySelector("[data-sim-amount]").textContent = fmt(state.amount);
    app.querySelector("[data-sim-slider]").value = state.amount;
    const sign = state.kind === "gastar" ? -1 : 1;
    app.querySelector("[data-sim-after]").textContent = fmt(available + sign * state.amount);
  }

  // --- Selector de tipo ---
  app.querySelectorAll("[data-kind]").forEach((btn) => {
    btn.addEventListener("click", () => {
      state.kind = btn.dataset.kind;
      app.querySelectorAll("[data-kind]").forEach((b) => {
        const active = b === btn;
        b.classList.toggle("bg-ash", active);
        b.classList.toggle("border-ash-light", active);
        b.classList.toggle("text-paper", active);
        b.classList.toggle("bg-graphite", !active);
        b.classList.toggle("border-ash", !active);
        b.classList.toggle("text-muted", !active);
      });
      app.querySelector("[data-verb-label]").textContent = btn.dataset.verb;
      renderBuild();
    });
  });

  // --- Slider ---
  app.querySelector("[data-sim-slider]").addEventListener("input", (e) => {
    state.amount = Number(e.target.value);
    renderBuild();
  });

  // --- Ver impacto ---
  app.querySelector("[data-see-impact]").addEventListener("click", () => {
    const sign = state.kind === "gastar" ? -1 : 1;
    const after = available + sign * state.amount;
    const diff = sign * state.amount;
    const dly = days(state.amount);

    app.querySelector("[data-impact-after]").textContent = fmt(after);

    const diffEl = app.querySelector("[data-impact-diff]");
    diffEl.textContent = (diff >= 0 ? "+" : "-") + fmt(Math.abs(diff));
    diffEl.classList.toggle("text-harsh", diff < 0);
    diffEl.classList.toggle("text-paper", diff >= 0);

    const delayDays = state.kind === "gastar" ? dly : -dly;
    app.querySelector("[data-impact-delay]").textContent = (delayDays >= 0 ? "+" : "−") + Math.abs(delayDays) + " días";
    app.querySelector("[data-impact-delay-plain]").textContent = Math.abs(delayDays) + " días";

    const date = new Date(goalDate);
    date.setDate(date.getDate() + delayDays);
    app.querySelector("[data-impact-date]").textContent =
      date.getDate() + " " + meses[date.getMonth()] + " " + date.getFullYear();

    const barW = Math.min(18, (state.amount / 5000) * 18);
    app.querySelector("[data-impact-bar]").style.width = barW + "%";

    showStep("impact");
  });

  // --- Navegacion ---
  backBtn.addEventListener("click", () => showStep("build"));
  app.querySelector("[data-discard]").addEventListener("click", () => showStep("build"));

  renderBuild();
  showStep("build");
})();
