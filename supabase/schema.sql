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
    account_id      uuid not null references accounts(id) on delete cascade,
    source          text not null,
    amount          numeric(14,2) not null check (amount > 0),
    expected_date   date not null,
    is_received     boolean not null default false,
    created_at      timestamptz not null default now()
);

-- recurring_incomes (Verum.Domain.Entities.RecurringIncome)
-- Patron de ingreso esperado (ej. "Sueldo, dia 5"). Nunca aumenta el dinero
-- disponible por si solo: hace falta confirmar cuanto llego cada periodo.
create table recurring_incomes (
    id              uuid primary key default gen_random_uuid(),
    user_id         uuid not null references auth.users(id) on delete cascade,
    source          text not null,
    expected_amount numeric(14,2) not null check (expected_amount > 0),
    day_of_month    int not null check (day_of_month between 1 and 28),
    is_active       boolean not null default true,
    created_at      timestamptz not null default now()
);

-- recurring_income_confirmations (Verum.Domain.Entities.RecurringIncomeConfirmation)
-- Una fila por patron por periodo (mes). confirmed_amount = 0 significa que
-- no llego nada ese mes; income_id queda null si no se creo un ingreso real.
create table recurring_income_confirmations (
    id                  uuid primary key default gen_random_uuid(),
    user_id             uuid not null references auth.users(id) on delete cascade,
    recurring_income_id uuid not null references recurring_incomes(id) on delete cascade,
    period              date not null,
    confirmed_amount    numeric(14,2) not null default 0,
    income_id           uuid references incomes(id) on delete set null,
    confirmed_at        timestamptz not null default now(),
    unique (recurring_income_id, period)
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
    image_path      text,
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

-- business_accounts (Verum.Domain.Entities.BusinessAccount)
-- Donde vive de verdad la plata del negocio (bancos, efectivo, billeteras).
create table business_accounts (
    id          uuid primary key default gen_random_uuid(),
    user_id     uuid not null references auth.users(id) on delete cascade,
    business_id uuid not null references businesses(id) on delete cascade,
    name        text not null,
    subtitle    text not null default '',
    balance     numeric(14,2) not null default 0,
    created_at  timestamptz not null default now()
);

-- sales (Verum.Domain.Entities.Sale)
create table sales (
    id           uuid primary key default gen_random_uuid(),
    user_id      uuid not null references auth.users(id) on delete cascade,
    business_id  uuid not null references businesses(id) on delete cascade,
    account_id   uuid not null references business_accounts(id) on delete cascade,
    description  text not null,
    amount       numeric(14,2) not null check (amount > 0),
    date         timestamptz not null default now()
);

-- business_expenses (Verum.Domain.Entities.BusinessExpense)
create table business_expenses (
    id           uuid primary key default gen_random_uuid(),
    user_id      uuid not null references auth.users(id) on delete cascade,
    business_id  uuid not null references businesses(id) on delete cascade,
    account_id   uuid not null references business_accounts(id) on delete cascade,
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
    description  text not null default '',
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

-- transfers (Verum.Domain.Entities.Transfer)
-- Movimiento real de plata entre una cuenta personal y una cuenta del
-- negocio. Nunca se mezclan solas: todo transfer queda registrado.
create table transfers (
    id                   uuid primary key default gen_random_uuid(),
    user_id              uuid not null references auth.users(id) on delete cascade,
    business_id          uuid not null references businesses(id) on delete cascade,
    personal_account_id  uuid not null references accounts(id) on delete cascade,
    business_account_id  uuid not null references business_accounts(id) on delete cascade,
    direction            text not null check (direction in ('to_business', 'to_personal')),
    amount               numeric(14,2) not null check (amount > 0),
    note                 text not null default '',
    date                 timestamptz not null default now()
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
        'accounts','expenses','incomes','recurring_incomes','recurring_income_confirmations',
        'commitments','goals','debts','credit_accounts',
        'businesses','business_accounts','sales','business_expenses','receivables','payables','business_goals',
        'costs','taxes','investments','transfers'
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
create index idx_incomes_account         on incomes(account_id);
create index idx_recurring_incomes_user  on recurring_incomes(user_id);
create index idx_recurring_confirmations_user on recurring_income_confirmations(user_id);
create index idx_recurring_confirmations_pattern_period on recurring_income_confirmations(recurring_income_id, period);
create index idx_commitments_user        on commitments(user_id);
create index idx_goals_user              on goals(user_id);
create index idx_debts_user              on debts(user_id);
create index idx_credit_accounts_user    on credit_accounts(user_id);

create index idx_businesses_user             on businesses(user_id);
create index idx_business_accounts_business  on business_accounts(business_id);
create index idx_business_accounts_user      on business_accounts(user_id);
create index idx_sales_account                on sales(account_id);
create index idx_business_expenses_account    on business_expenses(account_id);
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
create index idx_transfers_user              on transfers(user_id);
create index idx_transfers_business          on transfers(business_id);
create index idx_transfers_date              on transfers(date);

-- ============================================================
-- Storage: fotos de metas
-- Bucket privado, cada usuario solo puede leer/escribir su propia carpeta
-- (goal-images/{user_id}/...). La imagen nunca se guarda en una columna.
-- ============================================================
insert into storage.buckets (id, name, public)
values ('goal-images', 'goal-images', false)
on conflict (id) do nothing;

create policy "select_own_goal_images" on storage.objects
    for select using (
        bucket_id = 'goal-images'
        and (storage.foldername(name))[1] = auth.uid()::text
    );

create policy "insert_own_goal_images" on storage.objects
    for insert with check (
        bucket_id = 'goal-images'
        and (storage.foldername(name))[1] = auth.uid()::text
    );

create policy "update_own_goal_images" on storage.objects
    for update using (
        bucket_id = 'goal-images'
        and (storage.foldername(name))[1] = auth.uid()::text
    );

create policy "delete_own_goal_images" on storage.objects
    for delete using (
        bucket_id = 'goal-images'
        and (storage.foldername(name))[1] = auth.uid()::text
    );
