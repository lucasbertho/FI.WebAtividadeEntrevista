If Not Exists (Select Top 1 1 From sys.Columns Where Name = 'CPF' And Object_Id = Object_Id('Clientes'))
Begin
	Alter Table Clientes Add CPF Varchar(14) Not Null
	Alter Table Clientes Add Constraint UQ_Clientes_CPF Unique (CPF)

	-- Alter Table Clientes Drop Column CPF 
	-- Alter Table Clientes Drop Constraint UQ_Clientes_CPF
End

If Not Exists (Select Top 1 1 From sys.objects Where Name = 'UQ_Beneficiarios_CPF')
Begin
	Alter Table Beneficiarios Alter Column CPF Varchar(14) Not Null
	Alter Table Beneficiarios Add Constraint UQ_Beneficiarios_CPF Unique (CPF)
	Alter Table Beneficiarios Add Constraint FK_Beneficiarios_Cliente Foreign Key (IdCliente) References Clientes (Id)
End