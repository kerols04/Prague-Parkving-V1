                                                           Loggbok
Detta är min loggbok som jag använt genom uppgiftens gång för att dokumentera mitt arbete. Målet är att nå Godkänt kravet med en enkel och robust konsolapplikation för att hantera parkeringen för ett parkeringshus i Prague. Jag beskriver vad jag gjort varje dag, vilka problem som uppstod och slutligen hur jag löste dem.

3 december– Planering och Grundstruktur
Idag påbörjade inlämningsuppgiften genom att noggranna läsa igenom kritikerna för uppgiften och vad de tekniska kraven för systemet är. Jag började med att titta genom de tidigare lektioner samt mina egna anteckningar för att få en bättre förståelse för upplägget av systemet som jag ska skapa. 
Gjort: Skapade en konsolapplikation som tar in data från användaren med meny. Fastställde kvittoformat TYP#REG, där # separerar fordonstyp och | separerar två MC (t.ex. MC#ABC123|MC#DEF456).
Problem: Hur parkerar två MC på samma plats?
Lösning: Använd strängar med | som separator mellan MC.

4 december  
Gjort: Idag skapade jag flera små hjälpfunktioner. Till exempel SkapaKvitto (som sätter ihop typ och regnummer), DelaKvitton (som separerar informationen) och TolkaKvitto (som kollar om det är en MC eller BIL och validerar). Jag såg också till att DelaKvitton returnerar en tom lista om det inte fanns någon information att hämta, för att undvika problem.
Reflektion: Jag lärde mig att genom att använda sådana här små hjälpfunktioner fick jag en mycket renare kod.

5 december 
Gjort: Jag behövde se till att folk skrev in registreringsnummer på rätt sätt. Därför byggde jag in en kontroll som heter ÄrGiltigtRegnr. Den kollar så att numret inte är för långt eller kort (max 10 tecken) och att det inte finns konstiga tecken som mellanslag eller #. Dessutom såg jag till att alla bokstäver blev stora automatiskt. Det gör att sökningar och jämförelser fungerar säkert, även om någon skriver med små bokstäver.
Reflektion: Jag insåg att det är smart att kolla inmatningen direkt. Genom tidig validering sparar jag tid på felsökning senare, eftersom det är lättare att fixa felet genast.

7 december
Gjort: Idag implementerade jag Parkera(). Jag lät bilar ta första bästa plats, men gjorde så att motorcyklar (MC) först letar efter en annan MC att parkera bredvid genom HittaFörstaSingelMc . Om det inte gick, tog de nästa lediga ruta. Jag fixade även loopen så att den räknade 1-100.
Reflektion: Jag lärde mig att om jag använder 1 som startnummer, måste jag vara konsekvent och använda det överallt. Det är viktigt för att undvika buggar

8 december
Gjort: Idag skapade jag funktionen HittaFordon som söker genom alla parkeringsplatser. Jag byggde in ett dubblettskydd för att vara säker att den kollar att HittaFordon inte hittar något innan jag tillåter en ny parkering, alltså returnerar -1. Jag fick också hantera rutor som hade flera fordon genom att använda funktionerna DelaKvitton och TolkaKvitto.
Reflektion: Jag lärde mig att genom att dela upp sökningen i små, tydliga steg, blev hela jobbet mycket enklare för mig

9 december
Gjort: Idag implementerade jag FlyttaFordon. Jag använde först funktionen TaBortReg för att få bort fordonet från den gamla platsen. Sedan lät jag funktionen KanPlaceraPå kontrollera om den nya målplatsen faktiskt var lämplig. Jag lade också till en while loop med int.TryParse för att se till att användaren alltid skriver in ett giltigt platsnummer (1-100).
Reflektion: Jag lärde mig att genom att samla all kontroll logik i en enda funktion, KanPlaceraPå, blir det mycket enklare att hålla ordning på koden.

10 december
Gjort: Idag implementerade jag funktionen TaBort. Den använder TaBortReg för att ta bort fordonet och sedan se till att platsen blir uppdaterad. Efter det testade jag ALLT systematiskt. Jag kollade även vad som hände när garaget var fullt, när jag skrev in ett ogiltigt platsnummer och slutligen när jag försökte parkera ett fordon med ett registreringsnummer som redan fanns. Det ledde till att jag fick justera felmeddelandena så att de blev tydligare.
Reflektion: Jag lärde mig att systematisk testning att kolla varje funktion noggrant är superviktigt. Det hjälpte mig att hitta flera småfel som jag annars hade missat. 
Detta är en kompletteringsuppgift som jag arbetat med parallellt med andra moment, vilket gör att loggboken är mer kompakt och fokuserad på de viktigaste delarna av arbetet.
