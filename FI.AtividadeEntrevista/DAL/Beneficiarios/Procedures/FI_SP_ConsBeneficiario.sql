﻿ALTER PROC FI_SP_ConsBeneficiario
	@ID			BIGINT,
	@CPF		VARCHAR(14),
	@IDCLIENTE	BIGINT
AS
BEGIN
		SELECT
			  CPF
			, NOME
			, IDCLIENTE			
			, ID
		FROM BENEFICIARIOS WITH(NOLOCK)
		WHERE
			((ISNULL(@ID,0) = 0) Or ID = @ID)
		AND ((ISNULL(@CPF,'') = '') Or CPF = @CPF)
		AND ((ISNULL(@IDCLIENTE,0) = 0) Or IDCLIENTE = @IDCLIENTE)
END