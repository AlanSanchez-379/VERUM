-- VERUM · Migración: foto de meta (Supabase Storage)
-- Correr en Supabase -> SQL Editor -> New query -> Run.
-- La imagen NUNCA se guarda en una columna de la base: solo viaja comprimida
-- desde el navegador a un bucket de Storage, y en `goals` se guarda apenas
-- la ruta (texto corto). El bucket es privado: cada quien solo puede
-- leer/escribir dentro de su propia carpeta (su user_id).

alter table goals add column if not exists image_path text;

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
