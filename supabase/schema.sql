-- VERUM · Schema completo (Personal + Negocio)
-- Correr completo en Supabase → SQL Editor → New query → Run.
-- Multiusuario: cada tabla lleva user_id -> auth.users, con RLS para que
-- cada usuario solo vea/edite sus propias filas. Las tablas de Negocio
-- ademas llevan business_id -> businesses, para separar los datos de
-- cada negocio del mismo usuario.

-- ============================================================
-- Extensiones
-- ============================================================
create extension if not exists "pgcrypto"; -- gen_random_uuid()

-- ============================================================
-- Enums (equivalentes a Verum.Domain.Enums)
-- ============================================================
create type account_type as enum ('Banco', 'Efectivo', 'Billetera');
create type goal_priority as enum ('Baja', 'Media', 'Alta');

-- ============================================================
-- PERSONAL
-- ============================================================

-- accounts (Verum.Domain.Entities.Account)
create table accounts (
    id          uuid primary key default gen_random_uuid(),
    user_id     uuid not null references auth.users(id) on delete cascade,
    name        text not null,
    type        account_type not null default 'Banco',
    subtitle    text not null default '',
    balance     numeric(14,2) not null default 0,
    created_at  timestamptz not null default now(),
    updated_at  timestamptz not null default now()
);

-- expenses (Verum.Domain.Entities.Expense)
create table expenses (
    id          uuid primary key default gen_random_uuid(),
    user_id     uuid not null references auth.users(id) on delete cascade,
    account_id  uuid not null references accounts(id) on delete cascade,
    category    text not null,
    amount      numeric(14,2) not null check (amount > 0),
    date        timestamptz not null default now(),
    created_at  timestamptz not null default now()
);

-- incomes (Verum.Domain.Entities.Income)
create table incomes (
    id              uuid primary key default gen_random_uuid(),
    user_id         uuid not null references auth.users(id) on delete cascade,
    source          text not null,
    amount          numeric(14,2) not null check (amount > 0),
    expected_date   date not null,
    is_received     boolean not null default false,
    created_at      timestamptz not null default now()
);

-- commitments (Verum.Domain.Entities.Commitment)
create table commitments (
    id          uuid primary key default gen_random_uuid(),
    user_id     uuid not null references auth.users(id) on delete cascade,
    name        text not null,
    amount      numeric(14,2) not null check (amount > 0),
    due_date    date not null,
    is_paid     boolean not null default false,
    created_at  timestamptz not null default now()
);

-- goals (Verum.Domain.Entities.Goal)
create table goals (
    id              uuid primary key default gen_random_uuid(),
    user_id         uuid not null references auth.users(id) on delete cascade,
    name            text not null,
    target_amount   numeric(14,2) not null check (target_amount > 0),
    current_amount  numeric(14,2) not null default 0,
    target_date     date not null,
    priority        goal_priority not null default 'Media',
    created_at      timestamptz not null default now(),
    updated_at      timestamptz not null default now()
);

-- debts (Verum.Domain.Entities.Debt)
create table debts (
    id                  uuid primary key default gen_random_uuid(),
    user_id             uuid not null references auth.users(id) on delete cascade,
    name                text not null,
    total_amount        numeric(14,2) not null check (total_amount > 0),
    remaining_amount    numeric(14,2) not null default 0,
    monthly_payment     numeric(14,2) not null default 0,
    next_due_date       date not null,
    created_at          timestamptz not null default now()
);

-- credit_accounts (Verum.Domain.Entities.CreditAccount)
create table credit_accounts (
    id              uuid primary key default gen_random_uuid(),
    user_id         uuid not null references auth.users(id) on delete cascade,
    name            text not null,
    credit_limit    numeric(14,2) not null check (credit_limit > 0),
    used_amount     numeric(14,2) not null default 0,
    cutoff_date     date not null,
    due_date        date not null,
    created_at      timestamptz not null default now()
);

-- ============================================================
-- NEGOCIO
-- ============================================================

-- businesses (Verum.Domain.Entities.Business)
create table businesses (
    id          uuid primary key default gen_random_uuid(),
    user_id     uuid not null references auth.users(id) on delete cascade,
    name        text not null,
    industry    text not null default '',
    created_at  timestamptz not null default now()
);

-- sales (Verum.Domain.Entities.Sale)
create table sales (
    id           uuid primary key default gen_random_uuid(),
    user_id      uuid not null references auth.users(id) on delete cascade,
    business_id  uuid not null references businesses(id) on delete cascade,
    description  text not null,
    amount       numeric(14,2) not null check (amount > 0),
    date         timestamptz not null default now()
);

-- business_expenses (Verum.Domain.Entities.BusinessExpense)
create table business_expenses (
    id           uuid primary key default gen_random_uuid(),
    user_id      uuid not null references auth.users(id) on delete cascade,
    business_id  uuid not null references businesses(id) on delete cascade,
    category     text not null,
    amount       numeric(14,2) not null check (amount > 0),
    date         timestamptz not null default now()
);

-- receivables (Verum.Domain.Entities.Receivable) - "Por cobrar"
create table receivables (
    id           uuid primary key default gen_random_uuid(),
    user_id      uuid not null references auth.users(id) on delete cascade,
    business_id  uuid not null references businesses(id) on delete cascade,
    client_name  text not null,
    amount       numeric(14,2) not null check (amount > 0),
    due_date     timestamptz not null,
    is_collected boolean not null default false
);

-- payables (Verum.Domain.Entities.Payable) - "Proveedores"
create table payables (
    id            uuid primary key default gen_random_uuid(),
    user_id       uuid not null references auth.users(id) on delete cascade,
    business_id   uuid not null references businesses(id) on delete cascade,
    supplier_name text not null,
    amount        numeric(14,2) not null check (amount > 0),
    due_date      timestamptz not null,
    is_paid       boolean not null default false
);

-- business_goals (Verum.Domain.Entities.BusinessGoal)
create table business_goals (
    id             uuid primary key default gen_random_uuid(),
    user_id        uuid not null references auth.users(id) on delete cascade,
    business_id    uuid not null references businesses(id) on delete cascade,
    name           text not null,
    target_amount  numeric(14,2) not null check (target_amount > 0),
    current_amount numeric(14,2) not null default 0,
    target_date    timestamptz not null,
    priority       goal_priority not null default 'Media'
);

-- costs (Verum.Domain.Entities.Cost) - costos fijos del negocio
create table costs (
    id           uuid primary key default gen_random_uuid(),
    user_id      uuid not null references auth.users(id) on delete cascade,
    business_id  uuid not null references businesses(id) on delete cascade,
    name         text not null,
    amount       numeric(14,2) not null check (amount > 0),
    due_date     timestamptz not null,
    is_paid      boolean not null default false
);

-- taxes (Verum.Domain.Entities.Tax)
create table taxes (
    id           uuid primary key default gen_random_uuid(),
    user_id      uuid not null references auth.users(id) on delete cascade,
    business_id  uuid not null references businesses(id) on delete cascade,
    name         text not null,
    amount       numeric(14,2) not null check (amount > 0),
    due_date     timestamptz not null,
    is_paid      boolean not null default false
);

-- investments (Verum.Domain.Entities.Investment)
create table investments (
    id           uuid primary key default gen_random_uuid(),
    user_id      uuid not null references auth.users(id) on delete cascade,
    business_id  uuid not null references businesses(id) on delete cascade,
    description  text not null,
    amount       numeric(14,2) not null check (amount > 0),
    date         timestamptz not null default now()
);

-- ============================================================
-- Row Level Security: cada usuario solo ve/edita lo suyo
-- ============================================================
do $$
declare
    t text;
begin
    foreach t in array array[
        'accounts','expenses','incomes','commitments','goals','debts','credit_accounts',
        'businesses','sales','business_expenses','receivables','payables','business_goals',
        'costs','taxes','investments'
    ]
    loop
        execute format('alter table %1$s enable row level security;', t);
        execute format('
            create policy "select_own_%1$s" on %1$s
                for select using (auth.uid() = user_id);
            create policy "insert_own_%1$s" on %1$s
                for insert with check (auth.uid() = user_id);
            create policy "update_own_%1$s" on %1$s
                for update using (auth.uid() = user_id) with check (auth.uid() = user_id);
            create policy "delete_own_%1$s" on %1$s
                for delete using (auth.uid() = user_id);
        ', t);
    end loop;
end $$;

-- ============================================================
-- Indices utiles
-- ============================================================
create index idx_accounts_user           on accounts(user_id);
create index idx_expenses_user           on expenses(user_id);
create index idx_expenses_account        on expenses(account_id);
create index idx_incomes_user            on incomes(user_id);
create index idx_commitments_user        on commitments(user_id);
create index idx_goals_user              on goals(user_id);
create index idx_debts_user              on debts(user_id);
create index idx_credit_accounts_user    on credit_accounts(user_id);

create index idx_businesses_user             on businesses(user_id);
create index idx_sales_user                  on sales(user_id);
create index idx_sales_business              on sales(business_id);
create index idx_business_expenses_user      on business_expenses(user_id);
create index idx_business_expenses_business  on business_expenses(business_id);
create index idx_receivables_user            on receivables(user_id);
create index idx_receivables_business        on receivables(business_id);
create index idx_payables_user               on payables(user_id);
create index idx_payables_business           on payables(business_id);
create index idx_business_goals_user         on business_goals(user_id);
create index idx_business_goals_business     on business_goals(business_id);
create index idx_costs_user                  on costs(user_id);
create index idx_costs_business              on costs(business_id);
create index idx_taxes_user                  on taxes(user_id);
create index idx_taxes_business              on taxes(business_id);
create index idx_investments_user            on investments(user_id);
create index idx_investments_business        on investments(business_id);
