ALTER TRIGGER RESPTREINOFASE_Trg01 ON TREINAMENTOFASE
FOR INSERT
AS
BEGIN
    -- Declaracao de Variaveis:
    DECLARE @respostaPadrao VARCHAR(255);
    DECLARE @respostaTreinamento VARCHAR(255);
    DECLARE @tamRespPadrao INT, @tamRespTreinamento INT;
    DECLARE @count1 INT, @count2 INT, @count3 INT, @count4 INT;
    DECLARE @letra1 CHAR(1), @letra2 CHAR(1);
    DECLARE @resultado FLOAT, @count5 FLOAT, @count6 FLOAT, @count7 FLOAT, @count8 FLOAT;
    DECLARE @resultadoNR FLOAT, @resultadoErro FLOAT;
    DECLARE @qtdeResultadoFase INT;
    DECLARE @resultadoIDFase INT;
    --Declaracao de Tabelas Auxiliares
    DECLARE @TrespPadrao TABLE (ID INT, letra CHAR(1));
    DECLARE @TrespTreino TABLE (ID INT, letra CHAR(1));
    --Inicializacao de Variaveis:
    SELECT @count1 = 1;
    SELECT @count2 = 1;
    SELECT @count3 = 1;
    SELECT @count4 = 1;
    SELECT @count5 = 1.0;
    SELECT @count6 = 0.0;
    SELECT @count7 = 0.0;
    SELECT @count8 = 0.0;

    SELECT @qtdeResultadoFase = (SELECT rf.qtdeResultadoFase FROM RESULTADOFASE AS rf WHERE rf.faseIDfase = (SELECT INS.faseIDfase FROM inserted AS INS)) + 1;
    SELECT @resultadoIDFase = (SELECT rf.idResultadoFase FROM RESULTADOFASE as rf WHERE rf.faseIDfase = (SELECT INS.faseIDfase FROM inserted AS INS));
    SET NOCOUNT ON;

    IF EXISTS(SELECT 1 FROM inserted as INS) AND NOT EXISTS(SELECT 1 FROM deleted)
    BEGIN
        --Selecionando as Respostas
        SELECT @respostaPadrao = padraoResposta FROM EXERCICIO WHERE idExercicio = (SELECT INS.exercicioIdExercicio FROM inserted as INS);
        SELECT @respostaTreinamento = respostaTreino FROM TREINAMENTOFASE WHERE idTreinamentoFase = (SELECT INS.idTreinamentoFase FROM inserted as INS);
        --Determinando o tamanho das respostas
        SELECT @tamRespPadrao = LEN(@respostaPadrao)
        SELECT @tamRespTreinamento = LEN(@respostaTreinamento)
        --Populando a Tabela auxiliar com Resposta padrão
        WHILE (@count1 <= @tamRespPadrao)
        BEGIN
            SELECT @letra1 = SUBSTRING(@respostaPadrao, @count1, 1);
            IF(@letra1 = '-' OR @letra1 = '|')
                SELECT @count1 = @count1 + 1;
            ELSE
            BEGIN
                INSERT INTO @TrespPadrao VALUES (@count2, @letra1);
                SELECT @count1 = @count1 + 1;
                SELECT @count2 = @count2 + 1;
            END
        END
        --Populando a Tabela auxiliar com Resposta Treinamento
        WHILE (@count3 <= @tamRespTreinamento)
        BEGIN
            SELECT @letra2 = SUBSTRING(@respostaTreinamento, @count3, 1);
            IF(@letra2 = '-' OR @letra2 = '|')
                SELECT @count3 = @count3 + 1;
            ELSE
            BEGIN
                INSERT INTO @TrespTreino VALUES (@count4, @letra2);
                SELECT @count3 = @count3 + 1;
                SELECT @count4 = @count4 + 1;
            END
        END
        --Comparando as Respostas Padrão x Exercício
        WHILE (@count5 <= @count2)
        BEGIN
            SELECT @letra1 = letra FROM @TrespPadrao WHERE ID = @count5;
            SELECT @letra2 = Letra FROM @TrespTreino WHERE ID = @count5;

            IF(@letra1 = @letra2)
            BEGIN
                SELECT @count6 = @count6 + 1;
            END
            IF(@letra1 != @letra2 AND @letra2 != 'N')
            BEGIN
                SELECT @count7 = @count7 + 1;
            END
            IF(@letra2 = 'N')
            BEGIN
                SELECT @count8 = @count8 + 1;
            END
            SELECT @count5 = @count5 +1;
        END
        --Calculando o Percentual de acertos
        SELECT @resultado = ((@count6 -1) * 100) / (@count5 - 2);
        SELECT @resultadoErro = ((@count7) * 100) / (@count5 - 2);
        SELECT @resultadoNR = ((@count8) * 100) / (@count5 - 2);
        --Grava resultado do treinamento no TreinamentoFase
        UPDATE TREINAMENTOFASE
        SET resultadoTreino = (SELECT @resultado),
            resultadoIDresultadoFase = @resultadoIDFase,
            treinoResultadoNR = (SELECT @resultadoNR),
            treinoResultadoErros = (SELECT @resultadoErro)
        WHERE idTreinamentoFase = (SELECT INS.idTreinamentoFase FROM inserted as INS);

        IF EXISTS(SELECT 1 FROM RESULTADOFASE as ResF WHERE ResF.faseIDFase = (SELECT INS.faseIDfase FROM inserted as INS))
        BEGIN
            IF((SELECT rf.qtdeResultadoFase FROM RESULTADOFASE AS rf WHERE rf.faseIDFase = (SELECT INS.faseIDfase FROM inserted as INS)) = 0)
            BEGIN
            UPDATE RESULTADOFASE
                SET resultadoFase = (SELECT @resultado),
                    resultadoNR = (SELECT @resultadoNR),
                    resultadoErros = (SELECT @resultadoErro),
                    qtdeResultadoFase = @qtdeResultadoFase                    
                WHERE faseIDFase = (SELECT INS.faseIDfase FROM inserted as INS) ;
            END
            ELSE
            BEGIN
                UPDATE RESULTADOFASE
                SET resultadoFase = (SELECT AVG(tf.resultadoTreino) FROM TREINAMENTOFASE as tf 
                                                WHERE tf.resultadoIDresultadoFase = (SELECT @resultadoIDFase)),
                    resultadoErros = (SELECT AVG(tf.treinoResultadoErros) FROM TREINAMENTOFASE as tf 
                                                WHERE tf.resultadoIDresultadoFase = (SELECT @resultadoIDFase)),
                    resultadoNR = (SELECT AVG(tf.treinoResultadoNR) FROM TREINAMENTOFASE as tf 
                                                WHERE tf.resultadoIDresultadoFase = (SELECT @resultadoIDFase)),
                    qtdeResultadoFase = @qtdeResultadoFase
                WHERE faseIDFase = (SELECT INS.faseIDfase FROM inserted as INS);
            END
        END
    END
END
