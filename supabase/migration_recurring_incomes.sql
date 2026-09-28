-- VERUM · Migración: ingresos recurrentes con confirmación real
-- Correr en Supabase -> SQL Editor -> New query -> Run.
-- Regla: un patrón recurrente (ej. "Sueldo, día 5") nunca aumenta el dinero
-- disponible por sí solo. Solo cuando el usuario confirma cuánto le llegó
-- realmente (que puede ser $0) se crea un Income real y/o una confirmación
-- para ese período, para no volver a preguntar dos veces el mismo mes.

-- ============================================================
-- recurring_incomes (Verum.Domain.Entities.RecurringIncome)
-- ============================================================
create table recurring_incomes (
    id              uuid primary key default gen_random_uuid(),
    user_id         uuid not null references auth.users(id) on delete cascade,
    source          text not null,
    expected_amount numeric(14,2) not null check (expected_amount > 0),
    day_of_month    int not null check (day_of_month between 1 and 28),
    is_active       boolean not null default true,
    created_at      timestamptz not null default now()
);

-- ============================================================
-- recurring_income_confirmations
-- Una fila por patrón por período (mes). Si confirmed_amount = 0, no llegó
-- nada ese mes. income_id queda null si no se creó un ingreso real.
-- ============================================================
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

-- ============================================================
-- Row Level Security
-- ============================================================
do $$
declare
    t text;
begin
    foreach t in array array['recurring_incomes', 'recurring_income_confirmations']
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
create index idx_recurring_incomes_user on recurring_incomes(user_id);
create index idx_recurring_confirmations_user on recurring_income_confirmations(user_id);
create index idx_recurring_confirmations_pattern_period on recurring_income_confirmations(recurring_income_id, period);
