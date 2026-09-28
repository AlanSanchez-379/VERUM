// Agrega el token anti-CSRF a todo fetch() del mismo origen que modifique
// datos (POST/PUT/PATCH/DELETE), para que los ~20 endpoints JSON del panel
// queden protegidos sin tener que tocar cada archivo que hace fetch().
(function () {
  const meta = document.querySelector('meta[name="csrf-token"]');
  if (!meta) return;
  const token = meta.content;

  const unsafeMethods = new Set(["POST", "PUT", "PATCH", "DELETE"]);
  const originalFetch = window.fetch;

  window.fetch = function (input, init) {
    const url = typeof input === "string" ? input : input.url;
    const isRelative = url.startsWith("/") && !url.startsWith("//");

    if (isRelative) {
      init = init || {};
      const method = (init.method || "GET").toUpperCase();
      if (unsafeMethods.has(method)) {
        const headers = new Headers(init.headers || {});
        if (!headers.has("X-CSRF-TOKEN")) {
          headers.set("X-CSRF-TOKEN", token);
        }
        init = { ...init, headers };
      }
    }

    return originalFetch.call(this, input, init);
  };
})();
