-- VERUM · Migración: ligar incomes a una cuenta real
-- Correr en Supabase -> SQL Editor -> New query -> Run.
-- Necesaria porque "incomes" ya existe sin account_id (creada antes de que
-- los ingresos se pudieran registrar contra una cuenta). Esta migración:
-- 1) agrega la columna, 2) rellena las filas existentes con la primera
-- cuenta de cada usuario, 3) la vuelve obligatoria, 4) agrega el índice.

alter table incomes add column if not exists account_id uuid references accounts(id) on delete cascade;

update incomes i
set account_id = (
    select a.id from accounts a
    where a.user_id = i.user_id
    order by a.created_at asc
    limit 1
)
where i.account_id is null;

alter table incomes alter column account_id set not null;

create index if not exists idx_incomes_account on incomes(account_id);
