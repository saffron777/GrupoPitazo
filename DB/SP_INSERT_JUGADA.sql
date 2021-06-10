-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Frank Miranda
-- Create date: 2021.05.12
-- Description:	inserta una jugada nueva
-- =============================================
CREATE PROCEDURE SP_INSERT_JUGADA 
	-- Add the parameters for the stored procedure here
	@CarreraID bigint, 
	@NumeroEjemplar	int,
	@NumeroCarrera int,
	@TipoJugadaID int,
	@Monto decimal(18,2),
	@FechaJugada DateTime,
	@Usuario nvarchar(50),
	@Agente	nvarchar(50),
	@IdAgente int,
	@Moneda nvarchar(50),
	@CajaNro nvarchar(50),
	@Status  nvarchar(50),
	@id bigint output  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [dbo].[Jugadas] ([CarreraID] ,[NumeroEjemplar] ,[NumeroCarrera] ,[TipoJugadaID],[Monto] ,[FechaJugada],[Usuario] ,[Agente] ,[IdAgente] ,[Moneda], [CajaNro],[Status])
	VALUES (@CarreraID,@NumeroEjemplar,@NumeroCarrera,@TipoJugadaID,@Monto ,@FechaJugada,@Usuario ,@Agente ,@IdAgente,@Moneda,@CajaNro, @Status)
	SELECT @id = SCOPE_IDENTITY()
END
GO
