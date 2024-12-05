CREATE TRIGGER trg_AuditBankAccount
ON BankAccount
FOR UPDATE
AS
BEGIN
    DECLARE @Id INT, @OldBalance DECIMAL(18, 2), @NewBalance DECIMAL(18, 2), 
            @OldLockedBalance DECIMAL(18, 2), @NewLockedBalance DECIMAL(18, 2);

    SELECT @Id = INSERTED.Id, 
           @OldBalance = DELETED.Balance, @NewBalance = INSERTED.Balance,
           @OldLockedBalance = DELETED.LockedBalance, @NewLockedBalance = INSERTED.LockedBalance
    FROM INSERTED
    INNER JOIN DELETED ON INSERTED.Id = DELETED.Id;

    IF @OldBalance <> @NewBalance
    BEGIN
        INSERT INTO AuditLog (TableName, ColumnName, OldValue, NewValue)
        VALUES ('BankAccount', 'Balance', CAST(@OldBalance AS NVARCHAR(256)), CAST(@NewBalance AS NVARCHAR(256)));
    END

    IF @OldLockedBalance <> @NewLockedBalance
    BEGIN
        INSERT INTO AuditLog (TableName, ColumnName, OldValue, NewValue)
        VALUES ('BankAccount', 'LockedBalance', CAST(@OldLockedBalance AS NVARCHAR(256)), CAST(@NewLockedBalance AS NVARCHAR(256)));
    END
END;