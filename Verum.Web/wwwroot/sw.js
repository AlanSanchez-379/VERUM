// Service worker minimo: solo cachea assets estaticos (css/js/iconos) para que
// carguen mas rapido. Nunca cachea paginas ni respuestas de la API: los datos
// financieros y la sesion siempre tienen que venir frescos del servidor.
const CACHE_NAME = "verum-static-v1";
const STATIC_ASSETS = [
  "/css/tailwind.css",
  "/css/app.css",
  "/js/app.js",
  "/icons/icon-192.png",
  "/icons/icon-512.png",
];

self.addEventListener("install", (event) => {
  event.waitUntil(
    caches.open(CACHE_NAME).then((cache) => cache.addAll(STATIC_ASSETS)).catch(() => {})
  );
  self.skipWaiting();
});

self.addEventListener("activate", (event) => {
  event.waitUntil(
    caches.keys().then((keys) =>
      Promise.all(keys.filter((k) => k !== CACHE_NAME).map((k) => caches.delete(k)))
    )
  );
  self.clients.claim();
});

self.addEventListener("fetch", (event) => {
  const req = event.request;

  if (req.method !== "GET") return;

  const url = new URL(req.url);
  const isStaticAsset =
    url.origin === self.location.origin &&
    (url.pathname.startsWith("/css/") ||
      url.pathname.startsWith("/js/") ||
      url.pathname.startsWith("/icons/"));

  if (!isStaticAsset) return; // paginas, API, auth: siempre a la red

  event.respondWith(
    caches.match(req).then((cached) => {
      if (cached) return cached;
      return fetch(req).then((res) => {
        const copy = res.clone();
        caches.open(CACHE_NAME).then((cache) => cache.put(req, copy));
        return res;
      });
    })
  );
});
