# Prague Parking V1

En enkel konsolapplikation för att hantera en parkeringsplats (valet parking) vid slottet i Prag.
Systemet hanterar **bilar** och **motorcyklar** och använder en **endimensionell array av strängar** som datastruktur.

## Funktioner (menyval)
När programmet startar visas en textbaserad meny:

1) Parkera fordon  
2) Flytta fordon  
3) Ta bort fordon (utcheckning)  
4) Sök fordon  
5) Visa alla platser  
6) Avsluta  

## Regler / krav som stöds
- Parkeringen har platser **1–100** (användaren jobbar alltid med platsnummer 1–100).
- En plats kan innehålla:
  - 1 bil, eller
  - 1 MC, eller
  - 2 MC, eller
  - vara tom.
- All identifiering sker via **registreringsnummer** (max 10 tecken).
- Ingen sparning används (allt finns i minnet medan programmet körs).

## Lagringsformat i arrayen
Varje plats i arrayen innehåller en sträng på formatet:

- Bil: `BIL#ABC123`
- En MC: `MC#KLM789`
- Två MC på samma plats: `MC#KLM789|MC#FTP666`

Tecken:
- `#` separerar fordonstyp och registreringsnummer
- `|` separerar två motorcyklar på samma plats

## Validering av input (registreringsnummer)
Registreringsnummer kontrolleras så att:
- längden är 1–10 tecken
- inga mellanslag används
- inte innehåller `#` eller `|` (de används internt i lagringsformatet)

## Så kör du projektet
### Alternativ A: Visual Studio (rekommenderas)
1. Klona/ladda ner repot.
2. Öppna lösningen: `Prague Parkving V1.sln`
3. Kör med **F5** (debug) eller **Ctrl+F5** (utan debug).

### Alternativ B: Terminal (dotnet CLI)
Stå i repots rotmapp och kör:

```bash
dotnet run --project "Prague Parkving V1/Prague Parkving V1.csproj"
