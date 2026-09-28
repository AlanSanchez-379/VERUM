-- VERUM · Migración: transferencias Personal <-> Negocio
-- Correr en Supabase -> SQL Editor -> New query -> Run.
-- Registra cada movimiento real de plata entre una cuenta personal y una
-- cuenta del negocio, para que nunca se mezclen sin quedar registradas.

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

alter table transfers enable row level security;

create policy "select_own_transfers" on transfers
    for select using (auth.uid() = user_id);
create policy "insert_own_transfers" on transfers
    for insert with check (auth.uid() = user_id);
create policy "update_own_transfers" on transfers
    for update using (auth.uid() = user_id) with check (auth.uid() = user_id);
create policy "delete_own_transfers" on transfers
    for delete using (auth.uid() = user_id);

create index idx_transfers_user on transfers(user_id);
create index idx_transfers_business on transfers(business_id);
create index idx_transfers_date on transfers(date);
