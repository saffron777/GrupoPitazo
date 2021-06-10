USE [GrupoPitazoDB]
GO

/****** Object:  StoredProcedure [dbo].[SP_GradeoList]    Script Date: 12/05/2021 19:15:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GradeoList] 
	-- Add the parameters for the stored procedure here	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select *
,resultado = 
	case when AceptacionID is not null  and EstatusEjemplar =1 and TipoJugadas = '1P' then IIF(LlegadaEjemplar =1, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ),IIF(aceptaciones < 0, 'GANADOR','PERDEDOR' ) )
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '2P' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=2,  IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ),IIF(aceptaciones < 0, 'GANADOR','PERDEDOR' ))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '3P' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=3,  IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ),IIF(aceptaciones < 0, 'GANADOR','PERDEDOR' ))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '4P' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=4,  IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ),IIF(aceptaciones < 0, 'GANADOR','PERDEDOR' ))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '5P' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=5,  IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ),IIF(aceptaciones < 0, 'GANADOR','PERDEDOR' ))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '1P2N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar =1, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ), IIF(LlegadaEjemplar =2,IIF(aceptaciones < 0, 'GANADORMITAD','PERDEDORMITAD' ) ,IIF(aceptaciones < 0, 'GANADOR','PERDEDOR')))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '2P2N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar =1, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ), IIF(LlegadaEjemplar =2, IIF(aceptaciones < 0, 'PERDEDORMITAD','GANADORMITAD' ), IIF(aceptaciones < 0, 'GANADOR','PERDEDOR')))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '2P3N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=2, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ), IIF(LlegadaEjemplar =3, IIF(aceptaciones < 0, 'GANADORMITAD','PERDEDORMITAD' ), IIF(aceptaciones < 0, 'GANADOR','PERDEDOR')))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '3P3N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=2, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ), IIF(LlegadaEjemplar =3, IIF(aceptaciones < 0, 'PERDEDORMITAD','GANADORMITAD' ), IIF(aceptaciones < 0, 'GANADOR','PERDEDOR')))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '3P4N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=3, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ), IIF(LlegadaEjemplar =4, IIF(aceptaciones < 0, 'GANADORMITAD','PERDEDORMITAD' ), IIF(aceptaciones < 0, 'GANADOR','PERDEDOR')))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '4P4N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=3, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ), IIF(LlegadaEjemplar =4, IIF(aceptaciones < 0, 'PERDEDORMITAD','GANADORMITAD' ), IIF(aceptaciones < 0, 'GANADOR','PERDEDOR')))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '4P5N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=4, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ), IIF(LlegadaEjemplar =5, IIF(aceptaciones < 0, 'GANADORMITAD','PERDEDORMITAD' ), IIF(aceptaciones < 0, 'GANADOR','PERDEDOR')))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '5P5N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=4, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ), IIF(LlegadaEjemplar =5, IIF(aceptaciones < 0, 'PERDEDORMITAD','GANADORMITAD' ), IIF(aceptaciones < 0, 'GANADOR','PERDEDOR')))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '2N' then IIF(LlegadaEjemplar =1, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ),IIF(LlegadaEjemplar =2, 'PUSH', IIF(aceptaciones < 0, 'GANADOR','PERDEDOR')))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '3N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=2, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ),IIF(LlegadaEjemplar =3, 'PUSH', IIF(aceptaciones < 0, 'GANADOR','PERDEDOR')))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '4N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=3, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ),IIF(LlegadaEjemplar =4, 'PUSH', IIF(aceptaciones < 0, 'GANADOR','PERDEDOR')))
		 when AceptacionID is not null and EstatusEjemplar =1 and TipoJugadas = '5N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=4, IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ),IIF(LlegadaEjemplar =5, 'PUSH', IIF(aceptaciones < 0, 'GANADOR','PERDEDOR')))
		 when AceptacionID is not null  and EstatusEjemplar =1 and (TipoJugadas = '10A2' or TipoJugadas = '10A3' or TipoJugadas = '10A4' or TipoJugadas = '10A5' or TipoJugadas = '10A6' or TipoJugadas = '10A7' or TipoJugadas = '10A8' or TipoJugadas = '10A9') then IIF(LlegadaEjemplar =1,  IIF(aceptaciones < 0, 'PERDEDOR','GANADOR' ),IIF(aceptaciones < 0, 'GANADOR','PERDEDOR' ))
		 when AceptacionID IS NULL or EstatusEjemplar = 0 then 'ANULADO'
		 else 'ANULADO'
	end

from (
select 
c.HipodromoID, 
c.NumeroCarrera,
j.JugadaID, 
a.AceptacionID,
j.CarreraID, 
TipoJugadas = (select Codigo from TipoJugadas where TipoJugadaID = j.TipoJugadaID), 
j.NumeroEjemplar, 
NombreEjemplar = (select carr.NombreEjemplar from Carreras carr  where carr.HipodromoID =c.HipodromoID and  carr.NumeroCarrera =j.NumeroCarrera and carr.NumeroEjemplar = j.NumeroEjemplar and CAST(carr.fechaCarrera as date) = CAST(getdate() as date)), 
LlegadaEjemplar = (select carr.LlegadaEjemplar from Carreras carr  where carr.HipodromoID =c.HipodromoID and  carr.NumeroCarrera =j.NumeroCarrera and carr.NumeroEjemplar = j.NumeroEjemplar and CAST(carr.fechaCarrera as date) = CAST(getdate() as date)), 
c.EstatusEjemplar,
j.Usuario, 
j.Monto,
aceptaciones = (select count(*) from Aceptaciones where JugadaID = j.JugadaID)
from carreras c 
inner join jugadas j on j.[CarreraID] = c.[CarreraID]
left outer join Aceptaciones a on j.[JugadaID] = a.[JugadaID]
where CAST(fechaCarrera as date) = CAST(getdate() as date) and CAST( FechaJugada as date) =  CAST(getdate() as date) and c.EstatusEjemplar = 1 and FechaCierreCarrera is not null and ([LlegadaEjemplar] is not null )  and j.Status not in ('GANADOR','PERDEDOR','PUSH','')
union all
select 
c.HipodromoID, 
c.NumeroCarrera,
j.JugadaID, 
a.AceptacionID,
j.CarreraID, 
TipoJugadas = (select Codigo from TipoJugadas where TipoJugadaID = j.TipoJugadaID),  
j.NumeroEjemplar, 
NombreEjemplar = (select carr.NombreEjemplar from Carreras carr  where carr.HipodromoID =c.HipodromoID and  carr.NumeroCarrera =j.NumeroCarrera and carr.NumeroEjemplar = j.NumeroEjemplar and CAST(carr.fechaCarrera as date) = CAST(getdate() as date)), 
LlegadaEjemplar = (select carr.LlegadaEjemplar from Carreras carr  where carr.HipodromoID =c.HipodromoID and  carr.NumeroCarrera =j.NumeroCarrera and carr.NumeroEjemplar = j.NumeroEjemplar and CAST(carr.fechaCarrera as date) = CAST(getdate() as date)), 
c.EstatusEjemplar, 
a.Usuario, 
a.Monto,
aceptaciones = -1
from carreras c 
inner join jugadas j on j.[CarreraID] = c.[CarreraID]
left join Aceptaciones a on j.[JugadaID] = a.[JugadaID]
where CAST(fechaCarrera as date) = CAST(getdate() as date) and CAST( FechaJugada as date) =  CAST(getdate() as date) and CAST(Fecha as date) = CAST(getdate() as date) and c.EstatusEjemplar = 1 and FechaCierreCarrera is not null  and ([LlegadaEjemplar] is not null )  and j.Status not in ('GANADOR','PERDEDOR','PUSH')

) tbl
where aceptaciones > -1
order by HipodromoID ,NumeroCarrera, JugadaID, AceptacionID
END
GO


