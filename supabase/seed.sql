-- VERUM · Datos de ejemplo (opcional)
-- Corré esto DESPUES de crear tu primer usuario (paso 6 del README).
-- Reemplazá TU_USER_ID por el UUID real que copiaste de Authentication → Users.

do $$
declare
    uid uuid := 'TU_USER_ID'; -- <-- reemplazar
    acc_bbva uuid;
    acc_nu uuid;
begin
    insert into accounts (user_id, name, type, subtitle, balance) values
        (uid, 'BBVA', 'Banco', 'Banco', 8500) returning id into acc_bbva;
    insert into accounts (user_id, name, type, subtitle, balance) values
        (uid, 'Nu', 'Banco', 'Banco', 3200) returning id into acc_nu;
    insert into accounts (user_id, name, type, subtitle, balance) values
        (uid, 'Efectivo', 'Efectivo', 'En mano', 1000);
    insert into accounts (user_id, name, type, subtitle, balance) values
        (uid, 'Mercado Pago', 'Billetera', 'Saldo bajo', 500);

    insert into expenses (user_id, account_id, category, amount, date) values
        (uid, acc_bbva, 'Comida', 180, now() - interval '1 day'),
        (uid, acc_bbva, 'Transporte', 95, now() - interval '2 day'),
        (uid, acc_nu, 'Café', 68, now() - interval '3 day');

    insert into incomes (user_id, account_id, source, amount, expected_date, is_received) values
        (uid, acc_bbva, 'Sueldo', 5000, date_trunc('month', now())::date + 4, true);

    insert into commitments (user_id, name, amount, due_date, is_paid) values
        (uid, 'Tarjeta BBVA', 2000, now() + interval '2 day', true),
        (uid, 'Internet', 600, now() + interval '5 day', false),
        (uid, 'Gym', 500, now() + interval '8 day', false),
        (uid, 'Netflix', 299, now() + interval '10 day', false);

    insert into goals (user_id, name, target_amount, current_amount, target_date, priority) values
        (uid, 'Moto', 65000, 40300, '2027-06-30', 'Alta'),
        (uid, 'Fondo de emergencia', 30000, 14400, '2027-12-31', 'Media'),
        (uid, 'Viaje', 18000, 3200, '2027-03-01', 'Baja');

    insert into debts (user_id, name, total_amount, remaining_amount, monthly_payment, next_due_date) values
        (uid, 'Préstamo personal', 20000, 12500, 1100, now() + interval '12 day');

    insert into credit_accounts (user_id, name, credit_limit, used_amount, cutoff_date, due_date) values
        (uid, 'BBVA Oro', 25000, 6200, now() + interval '9 day', now() + interval '24 day');
end $$;
