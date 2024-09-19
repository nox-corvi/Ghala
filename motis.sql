-- lustige auftr#ge 20242024

/*
verbesserung ... 
suche nach packlistennr


führende nummer = packliste
abblenden nicht relevanter aufträge in bezug zur suche





*/


SELECT 
	KundenNr, AuftragMandant, AuftragNr, Status,
      Versandart, Prio, TimeStampErfassu, LieferNr

FROM ERPAuftrag 
	WHERE 
LieferNr = '20222022' Order by 7


--select * from erpauftrag where liefernr = '20024456';
select * from ERPAuftragPosition where auftragnr = '40276518';
select * from ERPAuftragPosition where auftragnr = '40276518';

select * from auftragskopf where auftragnr = 'A10511098'

select *,
	Mandant, AuftragNr, PosNr, Status, SollMenge, PlanMenge, IstMenge, ArtikelNr

from Auftragsposition where masterauftragnr = '40276518';

SELECT * FROM Journal 
WHERE Typ = 27 AND auftragnr = 'A10511098'

select mandant,artikelnr,bezeichnung2,ArtikelEinheit,Warenhauptgruppe,Warenuntergruppe from artikelstamm;

select * from auftragskopf;
/*

*/



select * from fahrauftrag where auftragnr = 'A10511098'


-- name für die pick position über die refnummer des fahrauftrages
SELECT * FROM Journal 
WHERE  refnr = '5226242'

SELECT * FROM Journal 
WHERE Typ = 27 AND auftragnr = 'A10511098'


select * from PackstueckPosition where FahrauftragNr = '5226242'
select * from PackstueckPosition where FahrauftragNr = '5226243'

-- dpd = collinr
-- gls = trackingnr
-- ups = trackingnr
select * from Packstueck where PackstueckNr  = '509148' 

-- versandliste -> provider
select * from Versandliste where VersandListenNr = 7571


select * from Packstueck where  VersandListe = 8991

select kommizone from kommizone;
select bereichnr, bem, kommizone from lagerbereich where LagerplatzTyp = 18;

select platznr, platzstatus,kommizone, lahi, sperrgrund,InventurAktiv,LetzteInventur from lagerplatz
select id,bereichnr,platznr, palnr, WarenStatus,sperrgrund,mandant,artikelnr,istmenge,verfmenge,einlagerzeit,letzteinventur,einlagermenge,einlagergewicht,einbuchungnr from lagerbestand;
select * from artikelliste

/*
Auftrag wird in Motis angelegt 
	ERPAuftrag.LieferNr = '20222022'
	ERPAuftrag.AuftragNr = '40276518' // Packlisten-Nr 
	Auftrag kann über verschiedene Packlisten freigegeben werden ... 

	Da mehrere Packlisten zu einer A-Nummer zusammengefasst werden können, ist 
	in Motis die Relation der freigegebenen Aufträge Auftragsposition -> Auftragskopf

	Auftragsposition.MasterAuftragNr = '40276518'
	AuftragNr = A-Nummer -> Auftragskopf.AuftragsNr

*/


select * from keyvalue where schlussel = 1110 and sprache=0;; -- ERPAuftragsPos Status
select * from keyvalue where schlussel = 1007 and sprache=0;; -- Auftragskopf Status
select * from keyvalue where schlussel = 1018 and sprache=0;; -- Auftragskopf Status

select * from keyvalue where kurztext like 'FERTIG'
select * from keyvalue where schlussel = 1007

/*
a.)	Ich gebe den Lagerbereich an und bekomme eine Liste der Lagerplätze zurück
b.)	Ich gebe einen Lagerplatz an und bekomme eine Liste der Artikel zurück, die auf diesem Lagerplatz liegen (so etwas brauchen Sie ja bestimmt auch)
*/





