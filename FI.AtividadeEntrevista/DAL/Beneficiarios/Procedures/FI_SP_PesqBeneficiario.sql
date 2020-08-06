ALTER PROC FI_SP_PesqBeneficiario
	@iniciarEm int,
	@quantidade int,
	@campoOrdenacao varchar(200),
	@idCliente bigint,
	@crescente bit	
AS
BEGIN
	DECLARE @SCRIPT NVARCHAR(MAX)
	DECLARE @CAMPOS NVARCHAR(MAX)
	DECLARE @ORDER VARCHAR(50)
	
	IF(@campoOrdenacao = 'CPF')
		SET @ORDER =  ' CPF '
	ELSE
		SET @ORDER = ' NOME '

	IF(@crescente = 0)
		SET @ORDER = @ORDER + ' DESC'
	ELSE
		SET @ORDER = @ORDER + ' ASC'

	SET @CAMPOS = '@iniciarEm int, @quantidade int'
	SET @SCRIPT = 
	'SELECT	 
			  ID
			, NOME
			, CPF
			, IDCLIENTE
	FROM
			(	SELECT    ROW_NUMBER() OVER (ORDER BY ' + @ORDER + ') AS Row
						, ID
						, NOME
						, CPF
						, IDCLIENTE
				FROM BENEFICIARIOS WITH(NOLOCK)
				WHERE IDCLIENTE = @idCliente) AS ClientesWithRowNumbers

	WHERE
		AND Row > @iniciarEm 
		AND Row <= (@iniciarEm+@quantidade)
	ORDER BY'
	
	SET @SCRIPT = @SCRIPT + @ORDER
			
	EXECUTE SP_EXECUTESQL @SCRIPT, @CAMPOS, @iniciarEm, @quantidade

	SELECT COUNT(1) FROM CLIENTES WITH(NOLOCK)
END