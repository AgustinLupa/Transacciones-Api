SELECT * FROM cartera;

ALTER TABLE HistorialSaldo
ADD (saldo_anterior number);

select * from Historialsaldo

update HistorialSaldo set Saldo_anterior = 18000

ALTER TABLE HistorialSaldo
MODIFY fecha TIMESTAMP;

DROP PROCEDURE CalcularVariacionSaldoPorFecha

CREATE OR REPLACE PROCEDURE CalcularVariacionSaldoPorFecha (
  p_id_cartera IN NUMBER,
  p_fecha_inicio IN DATE,
  p_fecha_fin IN DATE,
  p_porcentaje_variacion OUT NUMBER)
AS
  saldo_actual NUMBER;
  saldo_subsecuente NUMBER;
  variacion NUMBER;
BEGIN
  -- Obtener el saldo actual del portafolio en la fecha de inicio.
  SELECT saldo_anterior
  INTO saldo_actual
  FROM historialsaldo
  WHERE id_cartera = p_id_cartera  
  AND fecha >= TO_DATE(p_fecha_inicio, 'DD/MM/YYYY')
  ORDER BY fecha ASC
  FETCH FIRST 1 ROWS ONLY;

  -- Obtener el saldo subsecuente del portafolio en la fecha de fin.
  SELECT saldo_posterior
  INTO saldo_subsecuente
  FROM historialsaldo
  WHERE id_cartera = p_id_cartera -- Corregir aquí
  AND fecha <= TO_DATE(p_fecha_fin, 'DD/MM/YYYY')
  ORDER BY fecha DESC
  FETCH FIRST 1 ROWS ONLY;

  -- Calcular la variación porcentual del portafolio.
  variacion := (saldo_subsecuente - saldo_actual) / saldo_actual * 100;

  -- Asignar la variación porcentual al parámetro de salida.
  p_porcentaje_variacion := variacion;
END;

DECLARE
    resultado NUMBER;
BEGIN
    -- Llamar al procedimiento con la cláusula INTO
    CalcularVariacionSaldoPorFecha(1, TO_DATE('01-01-2024', 'DD-MM-YYYY'), TO_DATE('03-01-2024', 'DD-MM-YYYY'), resultado);

    -- Mostrar el resultado
    DBMS_OUTPUT.PUT_LINE('Resultado: ' || resultado);
END;

SELECT saldo_anterior
FROM historialsaldo
WHERE id_cartera = 1  
AND fecha >= TO_DATE('01-01-2024', 'DD-MM-YYYY')
ORDER BY fecha ASC
FETCH FIRST 1 ROWS ONLY;

SELECT saldo_posterior  
  FROM historialsaldo
  WHERE id_cartera = 1
  AND fecha <= TO_DATE('03-01-2024', 'DD/MM/YYYY')
  ORDER BY fecha DESC
  FETCH FIRST 1 ROWS ONLY;
  
  DROP PROCEDURE ObtenerHistorialesSaldo
  DROP PROCEDURE RealizarTransaccion
  
  create or replace PROCEDURE RealizarTransaccion (
    p_id_cartera IN NUMBER,
    p_monto_transaccion IN NUMBER,
    p_descripcion IN VARCHAR2,
    p_tipo_transaccion IN VARCHAR2
) AS
    v_saldo_anterior NUMBER;
BEGIN
    -- Obtén el saldo actual antes de la transacción
    SELECT saldo_actual INTO v_saldo_anterior
    FROM Cartera
    WHERE id_cartera = p_id_cartera;

    -- Realiza la transacción actualizando el saldo en la tabla "Cartera"
    UPDATE Cartera
    SET saldo_actual = CASE
                          WHEN p_tipo_transaccion = 'Ganancia' THEN v_saldo_anterior + p_monto_transaccion
                          WHEN p_tipo_transaccion = 'Gasto' THEN v_saldo_anterior - p_monto_transaccion
                          ELSE v_saldo_anterior
                       END
    WHERE id_cartera = p_id_cartera;

    -- Registra el historial de saldo en la tabla "HistorialSaldo"
    INSERT INTO HistorialSaldo (id_cartera, fecha, descripcion, monto_transaccion, saldo_posterior, tipo_transaccion)
    VALUES (p_id_cartera, SYSDATE, p_descripcion, p_tipo_transaccion,
            CASE WHEN p_tipo_transaccion = 'Ganancia' THEN p_monto_transaccion
                 WHEN p_tipo_transaccion = 'Gasto' THEN -p_monto_transaccion
                 ELSE 0
            END,
            CASE WHEN p_tipo_transaccion = 'Ganancia' THEN v_saldo_anterior + p_monto_transaccion
                 WHEN p_tipo_transaccion = 'Gasto' THEN v_saldo_anterior - p_monto_transaccion
                 ELSE v_saldo_anterior
            END);

    COMMIT; -- Se realiza la transacción de manera permanente.
END RealizarTransaccion;