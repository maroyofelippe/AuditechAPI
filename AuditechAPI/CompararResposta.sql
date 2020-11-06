DECLARE @resposta VARCHAR(255);
DECLARE @tamanho INT, @count1 INT, @count2 INT, @count3 INT, @count4 INT;
DECLARE @letra CHAR(1);
DECLARE @ar_resposta TABLE (ID INT, letra CHAR(1));
DECLARE @resultado FLOAT;

SELECT @resposta = padraoResposta FROM EXERCICIO WHERE idExercicio = 1;

SELECT @tamanho = LEN(@resposta);
SELECT @count1 = 1;
SELECT @count2 = 1;

WHILE (@count1 <= @tamanho)
BEGIN
    SELECT @letra = SUBSTRING(@resposta, @count1, 1);
    IF(@letra = '-' OR @letra = '|')
        SELECT @count1 = @count1 + 1;
    ELSE
    BEGIN
        INSERT INTO @ar_resposta VALUES (@count2, @letra);
        SELECT @count1 = @count1 + 1;
        SELECT @count2 = @count2 + 1;
    END
END

SELECT @count3 = 1;
SELECT @count4 = 0;

WHILE (@count3 <= @count2)
BEGIN
    SELECT @letra = letra FROM @ar_resposta WHERE ID = @count3;

    IF(@letra = 'C')
    BEGIN
        SELECT @count4 = @count4 + 1
        --SELECT CONCAT(@count3, '-', @count4, '-', @letra)
    END

    SELECT @count3 = @count3 + 1;
END

SELECT @resultado = (@count4/@count2);

SELECT @resultado;
SELECT @count4;

SELECT * FROM @ar_resposta



Select padraoResposta from EXERCICIO where idExercicio = 1
