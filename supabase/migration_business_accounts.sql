-- VERUM · Migración: cuentas reales del negocio
-- Correr en Supabase -> SQL Editor -> New query -> Run.
-- Crea las cuentas bancarias propias de cada negocio y liga las ventas y
-- gastos ya existentes a una cuenta real, para que "Utilidad" deje de ser
-- el único número de dinero del negocio y exista un saldo real por cuenta.

-- ============================================================
-- business_accounts (Verum.Domain.Entities.BusinessAccount)
-- ============================================================
create table business_accounts (
    id          uuid primary key default gen_random_uuid(),
    user_id     uuid not null references auth.users(id) on delete cascade,
    business_id uuid not null references businesses(id) on delete cascade,
    name        text not null,
    subtitle    text not null default '',
    balance     numeric(14,2) not null default 0,
    created_at  timestamptz not null default now()
);

alter table business_accounts enable row level security;

create policy "select_own_business_accounts" on business_accounts
    for select using (auth.uid() = user_id);
create policy "insert_own_business_accounts" on business_accounts
    for insert with check (auth.uid() = user_id);
create policy "update_own_business_accounts" on business_accounts
    for update using (auth.uid() = user_id) with check (auth.uid() = user_id);
create policy "delete_own_business_accounts" on business_accounts
    for delete using (auth.uid() = user_id);

-- ============================================================
-- Crear una cuenta "Caja" para cada negocio que todavia no tenga ninguna
-- ============================================================
insert into business_accounts (user_id, business_id, name, subtitle, balance)
select b.user_id, b.id, 'Caja', 'Cuenta principal', 0
from businesses b
where not exists (select 1 from business_accounts ba where ba.business_id = b.id);

-- ============================================================
-- Ligar ventas y gastos existentes a la cuenta del negocio
-- ============================================================
alter table sales add column if not exists account_id uuid references business_accounts(id) on delete set null;
alter table business_expenses add column if not exists account_id uuid references business_accounts(id) on delete set null;

update sales s
set account_id = (select id from business_accounts ba where ba.business_id = s.business_id limit 1)
where s.account_id is null;

update business_expenses e
set account_id = (select id from business_accounts ba where ba.business_id = e.business_id limit 1)
where e.account_id is null;

-- ============================================================
-- Recalcular el saldo real de cada cuenta segun lo que ya existia
-- ============================================================
update business_accounts ba
set balance = coalesce((select sum(s.amount) from sales s where s.account_id = ba.id), 0)
            - coalesce((select sum(e.amount) from business_expenses e where e.account_id = ba.id), 0);

alter table sales alter column account_id set not null;
alter table business_expenses alter column account_id set not null;

-- ============================================================
-- Indices utiles
-- ============================================================
create index idx_business_accounts_business on business_accounts(business_id);
create index idx_business_accounts_user on business_accounts(user_id);
create index idx_sales_account on sales(account_id);
create index idx_business_expenses_account on business_expenses(account_id);
