(function () {
  const app = document.getElementById("prioridades-app");
  if (!app) return;

  const updateUrl = app.dataset.updateUrl;

  app.querySelectorAll("[data-goal-row]").forEach((row) => {
    const goalId = row.dataset.goalId;

    row.querySelectorAll("[data-priority]").forEach((btn) => {
      btn.addEventListener("click", async () => {
        const priority = btn.dataset.priority;

        try {
          const res = await fetch(updateUrl, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ goalId, priority }),
          });
          if (!res.ok) return;

          row.querySelectorAll("[data-priority]").forEach((b) => {
            const active = b === btn;
            b.classList.toggle("bg-paper", active);
            b.classList.toggle("text-obsidian", active);
            b.classList.toggle("bg-graphite-2", !active);
            b.classList.toggle("text-muted", !active);
            b.classList.toggle("border", !active);
            b.classList.toggle("border-ash", !active);
          });
        } catch (err) {
          // silencioso: el usuario ve que el boton no cambio de estado
        }
      });
    });
  });
})();
