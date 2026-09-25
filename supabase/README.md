# Base de datos de VERUM en Supabase

Este proyecto usa un único proyecto de Supabase para los módulos Personal y
Negocio. Todo el acceso a datos pasa por Row Level Security (RLS): cada fila
lleva `user_id` y solo su dueño puede verla o editarla.

## Recrear la base desde cero (nuevo entorno)

1. **Crear el proyecto** en [supabase.com](https://supabase.com) → *New
   project*. Guardá la contraseña de la base en un gestor de contraseñas —
   nunca la compartas ni la subas al repo.
2. **Correr el schema**: SQL Editor → New query → pegá todo
   `supabase/schema.sql` → Run. Esto crea las 16 tablas (7 de Personal, 9 de
   Negocio), activa RLS y crea las policies e índices.
3. **Autenticación por email**: Authentication → Providers → confirmá que
   *Email* esté habilitado (viene activado por defecto).
4. **(Opcional) Datos de ejemplo**: abrí `supabase/seed.sql`, reemplazá
   `'TU_USER_ID'` por el UUID de un usuario real (Authentication → Users →
   copiá el UID), y corré el script.
5. **Configurar la app**: en `Verum.Web/appsettings.json`, cargá `Supabase:Url`
   y `Supabase:AnonKey` (Project Settings → API — la `anon`/`public` key es
   segura de compartir; la `service_role` key **nunca** debe usarse acá, salta
   todas las reglas de RLS) y dejá `Supabase:UseDummyData` en `false`.

## Estructura del schema

- **Personal**: `accounts`, `expenses`, `incomes`, `commitments`, `goals`,
  `debts`, `credit_accounts`.
- **Negocio**: `businesses` (un usuario puede tener varios), y por cada
  negocio: `sales`, `business_expenses`, `receivables`, `payables`,
  `business_goals`, `costs`, `taxes`, `investments` — todas con `business_id`
  además de `user_id`, para separar los datos de cada negocio.

## Modo dummy vs. real

`Supabase:UseDummyData` en `appsettings.json` controla si la app usa
repositorios en memoria (sin base de datos, sin login) o los repositorios
reales contra Supabase (`Verum.Infrastructure/Supabase/Supabase*Repository.cs`).
En real, `Verum.Web` requiere sesión iniciada (`/auth/login` o
`/auth/register`) para cualquier página que no sea de autenticación.
