(function () {
  const list = document.getElementById("goals-list");
  if (!list) return;

  const MAX_DIMENSION = 1000;
  const JPEG_QUALITY = 0.72;

  // Achica y comprime la imagen en el navegador antes de subirla, para que
  // nunca llegue al servidor (ni a la base) un archivo pesado sin procesar.
  function compressImage(file) {
    return new Promise((resolve, reject) => {
      const img = new Image();
      const url = URL.createObjectURL(file);
      img.onload = () => {
        URL.revokeObjectURL(url);
        let { width, height } = img;
        if (width > height && width > MAX_DIMENSION) {
          height = Math.round((height * MAX_DIMENSION) / width);
          width = MAX_DIMENSION;
        } else if (height > MAX_DIMENSION) {
          width = Math.round((width * MAX_DIMENSION) / height);
          height = MAX_DIMENSION;
        }

        const canvas = document.createElement("canvas");
        canvas.width = width;
        canvas.height = height;
        const ctx = canvas.getContext("2d");
        ctx.drawImage(img, 0, 0, width, height);
        canvas.toBlob((blob) => (blob ? resolve(blob) : reject(new Error("No se pudo procesar la imagen."))), "image/jpeg", JPEG_QUALITY);
      };
      img.onerror = () => {
        URL.revokeObjectURL(url);
        reject(new Error("No se pudo leer la imagen."));
      };
      img.src = url;
    });
  }

  list.querySelectorAll("[data-goal-toggle]").forEach((btn) => {
    btn.addEventListener("click", () => {
      const card = btn.closest(".goal-card");
      const detail = card.querySelector("[data-goal-detail]");
      detail.classList.toggle("hidden");
    });
  });

  list.querySelectorAll(".goal-card").forEach((card) => {
    const goalId = card.dataset.goalId;
    const fileInput = card.querySelector("[data-goal-file]");
    const uploadBtn = card.querySelector("[data-goal-upload]");
    const removeBtn = card.querySelector("[data-goal-remove-image]");
    const status = card.querySelector("[data-goal-image-status]");
    const imageBox = card.querySelector(".goal-detail > div");

    uploadBtn.addEventListener("click", (e) => {
      e.stopPropagation();
      fileInput.click();
    });

    fileInput.addEventListener("click", (e) => e.stopPropagation());

    fileInput.addEventListener("change", async () => {
      const file = fileInput.files[0];
      if (!file) return;

      status.textContent = "Comprimiendo...";
      uploadBtn.disabled = true;

      try {
        const compressed = await compressImage(file);
        status.textContent = `Subiendo (${Math.round(compressed.size / 1024)} KB)...`;

        const formData = new FormData();
        formData.append("imagen", compressed, "meta.jpg");

        const res = await fetch(`/personal/metas/${goalId}/imagen`, {
          method: "POST",
          body: formData,
        });

        if (!res.ok) {
          const data = await res.json().catch(() => ({}));
          status.textContent = data.error || "No se pudo subir la imagen.";
          uploadBtn.disabled = false;
          return;
        }

        const data = await res.json();
        imageBox.innerHTML = `<img src="${data.imageUrl}" alt="" class="goal-image w-full h-full object-cover" data-goal-image />`;
        uploadBtn.textContent = "Cambiar foto";
        status.textContent = "";
        uploadBtn.disabled = false;
        location.reload();
      } catch (err) {
        status.textContent = "Error al procesar la imagen.";
        uploadBtn.disabled = false;
      }
    });

    if (removeBtn) {
      removeBtn.addEventListener("click", async (e) => {
        e.stopPropagation();
        removeBtn.disabled = true;
        try {
          await fetch(`/personal/metas/${goalId}/imagen/eliminar`, { method: "POST" });
          location.reload();
        } catch (err) {
          status.textContent = "Error de conexión.";
          removeBtn.disabled = false;
        }
      });
    }
  });
})();
