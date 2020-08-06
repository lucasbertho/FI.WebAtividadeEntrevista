﻿ALTER PROC FI_SP_AltBeneficiario
	@CPF		   VARCHAR (14),
    @NOME          VARCHAR (50),
	@Id           BIGINT
AS
BEGIN
	UPDATE BENEFICIARIOS 
	SET 
		CPF				= @CPF,
		NOME			= @NOME		
	WHERE Id = @Id
END