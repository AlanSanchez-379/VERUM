-- VERUM · Migración: detalle en cuentas por cobrar
-- Correr en Supabase -> SQL Editor -> New query -> Run.
-- Guarda lo que se vendió (no solo el cliente), para que al cobrar la venta
-- a crédito se registre con su descripción real en vez de un texto genérico.

alter table receivables add column if not exists description text not null default '';
