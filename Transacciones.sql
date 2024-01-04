CREATE SEQUENCE seq_autoi START WITH 1 INCREMENT BY 1;

CREATE TABLE Cartera (
    id_cartera NUMBER DEFAULT seq_autoi.NEXTVAL PRIMARY KEY,
    nombre_cuenta VARCHAR2(50),
    saldo_actual NUMBER default 0
);

CREATE TABLE HistorialSaldo (
    id_historial NUMBER DEFAULT seq_autoi.NEXTVAL PRIMARY KEY,
    id_cartera NUMBER,
    fecha DATE DEFAULT SYSDATE,
    descripcion VARCHAR2(255),
    tipo_transaccion varchar2(50),
    monto_transaccion NUMBER,
    saldo_posterior NUMBER,
    FOREIGN KEY (id_cartera) REFERENCES Cartera(id_cartera)
);

create or replace NONEDITIONABLE PROCEDURE CalcularVariacionSaldoPorFecha (
    p_id_cartera IN NUMBER,
    p_fecha_inicio IN DATE,
    p_fecha_fin IN DATE,
    p_porcentaje_variacion OUT NUMBER
) AS
    v_saldo_inicial NUMBER;
    v_saldo_final NUMBER;
BEGIN
    -- Obtén el saldo inicial
    SELECT saldo_posterior INTO v_saldo_inicial
    FROM HistorialSaldo
    WHERE id_cartera = p_id_cartera
      AND fecha = (
          SELECT MAX(fecha)
          FROM HistorialSaldo
          WHERE id_cartera = p_id_cartera
            AND fecha < p_fecha_inicio
      );

    -- Obtén el saldo final
    SELECT saldo_posterior INTO v_saldo_final
    FROM HistorialSaldo
    WHERE id_cartera = p_id_cartera
      AND fecha = (
          SELECT MAX(fecha)
          FROM HistorialSaldo
          WHERE id_cartera = p_id_cartera
            AND fecha <= p_fecha_fin
      );


    IF v_saldo_inicial IS NOT NULL AND v_saldo_final IS NOT NULL AND v_saldo_inicial <> 0 THEN
        p_porcentaje_variacion := ((v_saldo_final - v_saldo_inicial) / ABS(v_saldo_inicial)) * 100;
    ELSE
        p_porcentaje_variacion := NULL;
    END IF;

    COMMIT; 
END CalcularVariacionSaldoPorFecha;


create or replace NONEDITIONABLE PROCEDURE ObtenerHistorialesConTipo (
    p_id_cartera IN NUMBER,
    p_tipo_transaccion IN VARCHAR2,
    p_resultados OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_resultados FOR
    SELECT hs.id_historial,
           c.nombre_cuenta,
           hs.fecha,
           hs.descripcion,
           hs.monto_transaccion,
           hs.saldo_posterior
    FROM HistorialSaldo hs
    JOIN Cartera c ON hs.id_cartera = c.id_cartera
    WHERE hs.id_cartera = p_id_cartera
      AND hs.tipo_transaccion = p_tipo_transaccion
    ORDER BY hs.fecha;

    COMMIT; -- Asegurarse de que los cambios sean permanentes
END ObtenerHistorialesConTipo;


create or replace NONEDITIONABLE PROCEDURE ObtenerHistorialesSaldo (
    p_id_cartera IN NUMBER,
    p_resultados OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_resultados FOR
    SELECT hs.id_historial,
           c.nombre_cuenta,
           hs.fecha,
           hs.descripcion,
           hs.monto_transaccion,
           hs.saldo_posterior
    FROM HistorialSaldo hs
    JOIN Cartera c ON hs.id_cartera = c.id_cartera
    WHERE hs.id_cartera = p_id_cartera
    ORDER BY hs.fecha;

    COMMIT; -- Asegurarse de que los cambios sean permanentes
END ObtenerHistorialesSaldo;

DROP PROCEDURE RealizarTransaccion;

CREATE OR REPLACE PROCEDURE RealizarTransaccion (
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
    INSERT INTO HistorialSaldo (id_cartera, fecha, descripcion, monto_transaccion, saldo_posterior)
    VALUES (p_id_cartera, SYSDATE, p_descripcion, 
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


INSERT INTO Cartera (nombre_cuenta, saldo_actual)
VALUES ('Banco Santander', 0);


CREATE OR REPLACE PROCEDURE ObtenerCarteras (
    p_nombre_cuenta IN VARCHAR2,
    p_id_cartera IN NUMBER DEFAULT NULL,
    p_resultado OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_resultado FOR
        SELECT id_cartera, nombre_cuenta, saldo_actual
        FROM Cartera
        WHERE (p_nombre_cuenta IS NULL OR nombre_cuenta = p_nombre_cuenta)
          AND (p_id_cartera IS NULL OR id_cartera = p_id_cartera);
END ObtenerCarteras;

DROP PROCEDURE OBTENERHISTORIALESSALDO;

SELECT * FROM Cartera

CREATE OR REPLACE PROCEDURE ObtenerHistorialesSaldo (
    p_id_cartera IN NUMBER,
    p_resultados OUT SYS_REFCURSOR
) AS
BEGIN   
    OPEN p_resultados FOR
    SELECT hs.id_historial,
           hs.id_cartera,
           hs.fecha,
           hs.descripcion,
           hs.monto_transaccion,
           hs.saldo_posterior,
           hs.tipo_transaccion
    FROM HistorialSaldo hs    
    WHERE hs.id_cartera = p_id_cartera
    ORDER BY hs.fecha;

    -- No hay necesidad de COMMIT aquí, ya que esta Stored Procedure no realiza cambios en los datos.
END ObtenerHistorialesSaldo;

select * from cartera

update cartera SET sALDO_ACTUAL = 18000 WHERE id_cartera = 1


BEGIN
    RealizarTransaccion(1, 8000, 'Pago de asado fin de anio', 'Gasto');
END;

select * from HistorialSaldo

DECLARE
    resultado NUMBER;
BEGIN
    CalcularVariacionSaldoPorFecha(p_id_cartera => 1, p_fecha_inicio => TO_DATE('01-1-2024', 'DD-MM-YYYY'), p_fecha_fin => TO_DATE('01-3-2024', 'DD-MM-YYYY'), p_porcentaje_variacion => :resultado);
    -- Puedes imprimir o utilizar el resultado según tus necesidades.
    DBMS_OUTPUT.PUT_LINE('Porcentaje de Variación: ' || resultado);
END;

DROP PROCEDURE CalcularVariacionSaldoPorFecha


CREATE OR REPLACE NONEDITIONABLE PROCEDURE CalcularVariacionSaldoPorFecha (
  p_id_cartera IN NUMBER,
  p_fecha_inicio IN DATE,
  p_fecha_fin IN DATE,
  p_porcentaje_variacion OUT NUMBER
) AS
  v_saldo_inicial NUMBER;
  v_saldo_final NUMBER;
BEGIN
  -- Obtén el saldo inicial
  SELECT saldo_posterior
  INTO v_saldo_inicial
  FROM HistorialSaldo
  WHERE id_cartera = p_id_cartera
    AND fecha BETWEEN p_fecha_inicio AND p_fecha_fin
  ORDER BY fecha ASC
  FETCH FIRST 1 ROWS ONLY;

  -- Obtén el saldo final
  SELECT saldo_posterior
  INTO v_saldo_final
  FROM HistorialSaldo
  WHERE id_cartera = p_id_cartera
    AND fecha BETWEEN p_fecha_inicio AND p_fecha_fin
  ORDER BY fecha DESC
  FETCH FIRST 1 ROWS ONLY;

  -- Calcula el porcentaje de variación
  IF v_saldo_inicial IS NOT NULL AND v_saldo_final IS NOT NULL AND v_saldo_inicial <> 0 THEN
    p_porcentaje_variacion := ((v_saldo_final - v_saldo_inicial) / ABS(v_saldo_inicial)) * 100;
  ELSE
    p_porcentaje_variacion := NULL;
  END IF;

  COMMIT; 
END CalcularVariacionSaldoPorFecha;