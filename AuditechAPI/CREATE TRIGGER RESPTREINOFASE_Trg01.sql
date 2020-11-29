CREATE TRIGGER RESPTREINOFASE_Trg01 ON TREINAMENTOFASE
FOR INSERT
AS
BEGIN
    -- Declaracao de Variaveis:
    DECLARE @respostaPadrao VARCHAR(255);
    DECLARE @respostaTreinamento VARCHAR(255);
    DECLARE @tamRespPadrao INT, @tamRespTreinamento INT;
    DECLARE @count1 INT, @count2 INT, @count3 INT, @count4 INT;
    DECLARE @letra1 CHAR(1), @letra2 CHAR(1);
    DECLARE @resultado FLOAT, @count5 FLOAT, @count6 FLOAT;
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
    SET NOCOUNT ON;

    IF EXISTS(SELECT 1 FROM inserted as INS) AND NOT EXISTS(SELECT 1 FROM deleted)
    BEGIN
        --Selecionando as Respostas
        SELECT @respostaPadrao = padraoResposta FROM EXERCICIO WHERE idExercicio = 1;
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
            SELECT @count5 = @count5 +1;
        END
        --Calculando o Percentual de acertos
        SELECT @resultado = (@count6 * 100) / (@count5 - 2);
        --Grava resultado do treinamento no TreinamentoFase
        UPDATE TREINAMENTOFASE
        SET resultadoTreino = (SELECT @resultado)
        WHERE idTreinamentoFase = (SELECT INS.idTreinamentoFase FROM inserted as INS);
    END
END
