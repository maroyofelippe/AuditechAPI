CREATE TRIGGER RESULTADOFASE_ADD_Trg02 ON FASE
FOR INSERT
AS
BEGIN
    --Declaracao de variaveis
    DECLARE @datafinal DATETIME;
    DECLARE @idFase INT;
    DECLARE @idPaciente INT;

    --incializacao de variaveis
    SELECT @datafinal = INS.dataFinal FROM inserted AS INS;
    SELECT @idFase = INS.idFase FROM inserted AS INS;
    SELECT @idPaciente = t.pacienteIDPaciente FROM TRATAMENTO AS t
                                                WHERE t.idTratamento = (SELECT tratamentoIDtratamento FROM FASE AS f 
                                                WHERE f.idFase = (SELECT INS.idFase FROM inserted AS INS));
    SET NOCOUNT ON;

    IF EXISTS(SELECT 1 FROM inserted as INS) AND NOT EXISTS(SELECT 1 FROM deleted)
    BEGIN
        INSERT INTO RESULTADOFASE (dataTermino, faseIDfase, pacienteIDpaciente, qtdeResultadoFase)
        VALUES                     (@datafinal, @idFase, @idPaciente, 0 )
    END
END
