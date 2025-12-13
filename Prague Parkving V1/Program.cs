using System;
using System.Collections.Generic;

/*
 * Välkommen till Prague Parking V1
 * Detta är en enkel konsolapplikation för att hantera parkeringen vid slottet i Prag.
 * Vi använder en vanlig array av strängar för att representera de 100 parkeringsplatserna,
 * vilket är kravet för den här inlämningsuppgiften.
 * * Programmet hanterar de grundläggande funktionerna: parkering, flytt, borttagning och sökning.
 * * Parkeringsplatsen är dimensionerad från index 1 till 100.
 * * Datastrukturen på varje plats är en sträng:
 * - Vi använder '#' för att separera fordonstyp och regnummer (t.ex. BIL#ABC987).
 * - Vi använder '|' för att separera två fordon på samma plats (t.ex. MC#DEF654|MC#GHJ321).
 */

namespace Prague_Parking_V1;
class Program
{
    // Själva parkeringen. Arrayen har storleken 101 (index 0-100) men vi använder endast index 1 till 100, vilket ger oss de 100 platserna kunden efterfrågar.
    static string[] Parkeringshus = new string[101];

    // Separatorer som används för att bygga kvittosträngarna i arrayen.
    const char SEP_MULTI = '|';    // Används mellan två MC på samma plats
    const char SEP_TYPREG = '#';   // Används mellan fordonstyp och regnummer

    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("  Prague Parking  V1  ");
            Console.WriteLine("----------------------");
            Console.WriteLine("1) Parkera fordon");
            Console.WriteLine("2) Flytta fordon");
            Console.WriteLine("3) Ta bort fordon (utcheckning)");
            Console.WriteLine("4) Sök fordon");
            Console.WriteLine("5) Visa alla platser");
            Console.WriteLine("6) Avsluta");
            Console.Write("Välj: ");
            string val = (Console.ReadLine() ?? "").Trim();

            if (val == "1") Parkera();
            else if (val == "2") FlyttaFordon();
            else if (val == "3") TaBort();
            else if (val == "4") SökFordon();
            else if (val == "5") VisaParkeradeFordon();
            else if (val == "6") break;
            else Info("Felaktigt val. Vänligen välj ett nummer mellan 1 och 6.");
        }
    }

    // ======================= MENYFUNKTIONER =======================
    // Parkera nytt fordon. Regler för detta,
    // - BIL: kräver en tom parkingsplats.
    // - MC: I första hand parkeras MC ihop med en ensam MC på en parkingsplats, annars parkeras det på en ledig parkingsplats.
    // - Samma regnr får inte finnas på flera platser (dublettskydd).


    // Hanterar inparkering av ett nytt fordon.
    static void Parkera()
    {
        Console.Write("Ange fordonstyp (BIL/MC): ");
        string typ = (Console.ReadLine() ?? "").Trim().ToUpper();

        if (!(typ == "BIL" || typ == "MC"))
        {
            Info("Här tar vi bara emot BIL eller MC. Försök igen.");
            return;
        }

        Console.Write("Ange registreringsnummer (1–10 tecken): ");
        string reg = (Console.ReadLine() ?? "").Trim().ToUpper();

        // Kontrollerar grundläggande regler för regnummer
        if (!ÄrGiltigtRegnr(reg))
        {
            Info("Ogiltigt registreringsnummer, kontrollera igen.");
            return;
        }

        // Dublettskydd: KOntrollerar ifall fordonet inte redan finns parkerat i P-huset
        if (HittaFordon(reg) != -1)
        {
            Info("Det regnumret finns redan parkerat i P-huset.");
            return;
        }

        if (typ == "BIL")
        {
            int plats = HittaLedigPlats();
            if (plats == -1) { Info("Inga lediga platser för bilar."); return; }

            Parkeringshus[plats] = SkapaKvitto("BIL", reg);
            Info($"Bilen {reg} parkerad på plats {plats}. Kom ihåg platsen!");
        }
        else // MC
        {
            // Försöker först hitta en singel MC att parkera bredvid på samma plats
            int plats = HittaFörstaSingelMc();
            if (plats != -1)
            {
                // Lägg till den andra MC på den delade parkerings platsen
                Parkeringshus[plats] = SlåIhopKvitton(Parkeringshus[plats], SkapaKvitto("MC", reg));
                Info($"Motorcykeln {reg} parkerad på plats {plats} parkeras nästintil en annan motorcyckel.");
                return;
            }

            // Hittade ingen MC att dela parkeringsplats med, letar därför efter  första lediga plats
            plats = HittaLedigPlats();
            if (plats == -1) { Info("Det finns inga lediga platser för MC just nu."); return; }

            Parkeringshus[plats] = SkapaKvitto("MC", reg);
            Info($"Motorcykeln {reg} parkerad på plats {plats}.");
        }
    }

    // Hanterar flytt av ett fordon till en annan plats 
    // Hämtar typ av fordon från kvittot.
    // Kontrollerar att parkeringsplatsen är lämplig,
    // Hindrar flytt till samma parkeringsplats, ifall användaren matar in samma info
    static void FlyttaFordon()
    {
        Console.Write("Ange registreringsnummer på fordonet som ska flyttas: ");
        string reg = (Console.ReadLine() ?? "").Trim().ToUpper();

        int från = HittaFordon(reg);
        if (från == -1)
        {
            Info("Fordonet hittades inte. Dubbelkolla regnumret."); return;
        }

        string typ = TypFörRegPåPlats(Parkeringshus[från], reg);
        if (typ == null)
        {
            Info("Kunde inte identifiera fordonstyp."); return;
        }

        int till;

        // Loopar input tills giltigt platsnummer anges 
        while (true)
        {
            Console.Write("Ange ny parkingsplats (1–100): ");
            if (int.TryParse(Console.ReadLine(), out till) && till >= 1 && till <= 100)
                break;

            Console.WriteLine("Ogiltigt platsnummer. Det måste vara ett nummer mellan 1 och 100.");
        }

        if (till == från)
        {
            Info("Fordonet står redan parkerat där!");
            return;
        }

        if (!KanPlaceraPå(till, typ, reg))
        {
            Info("Den valda platsen är upptagen eller olämplig för den här fordonstypen.");
            return;
        }

        // Steg 1: Skapa kvittot för flytten
        string kvittoAttFlytta = SkapaKvitto(typ, reg);

        // Steg 2: Placera kvittot på målplatsen
        if (string.IsNullOrEmpty(Parkeringshus[till]))
            // Parkeringsplatsen är tom
            Parkeringshus[till] = kvittoAttFlytta;
        else
            // Parkeringsplatsen har en MC, slå ihop kvitton. Vi vet att platsen är OK tack vare KanPlaceraPå.
            Parkeringshus[till] = SlåIhopKvitton(Parkeringshus[till], kvittoAttFlytta);

        // Steg 3: Ta bort kvittot från ursprungliga platsen
        Parkeringshus[från] = TaBortReg(Parkeringshus[från], reg);

        Info($"Fordon {reg} flyttat från plats {från} till plats {till}. Klart!");
    }

    // Ta bort ett fordon helt (utcheckning)
    static void TaBort()
    {
        Console.Write("Ange registreringsnummer som ska tas bort: ");
        string reg = (Console.ReadLine() ?? "").Trim().ToUpper();

        int plats = HittaFordon(reg);
        if (plats == -1) { Info("Fordonet hittades inte. Inget att ta bort."); return; }

        // Använder den smarta TaBortReg-metoden som rensar upp strängen
        Parkeringshus[plats] = TaBortReg(Parkeringshus[plats], reg);
        Info($"Fordon {reg} togs bort från plats {plats}. Välkommen åter!");
    }

    // Sök upp parkingsplats via registeringsnummer
    static void SökFordon()
    {
        Console.Write("Ange registreringsnummer att söka: ");
        string reg = (Console.ReadLine() ?? "").Trim().ToUpper();

        int plats = HittaFordon(reg);
        if (plats == -1) Info("Fordonet hittades inte i P-huset.");
        else Info($"Fordon {reg} står på parkeringsplats {plats}.");
    }

    // Skriver ut alla 100 platser och deras status
    static void VisaParkeradeFordon()
    {
        Console.WriteLine("\n--- Status för Prague Parking (Platser 1-100) ---");
        for (int i = 1; i <= 100; i++)
        {
            if (string.IsNullOrEmpty(Parkeringshus[i]))
                Console.WriteLine($"Plats {i,-3}: Ledig");
            else
                Console.WriteLine($"Plats {i,-3}: {Parkeringshus[i]}");
        }
        Paus();
    }

    // HJÄLPMETODER (array/sträng) 
    // Kontrollerar att regnumret ser okej ut (längd, inga konstiga tecken)

    static bool ÄrGiltigtRegnr(string reg)
    {
        if (reg.Length < 1 || reg.Length > 10) return false;
        // Kollar mot de tecken vi använder internt (och mellanslag)
        if (reg.IndexOf(' ') >= 0) return false;
        if (reg.IndexOf(SEP_MULTI) >= 0) return false;
        if (reg.IndexOf(SEP_TYPREG) >= 0) return false;

        return true;
    }

    // Hittar det första lediga indexet (för bil eller första MC)
    static int HittaLedigPlats()
    {
        for (int i = 1; i <= 100; i++)
            if (string.IsNullOrEmpty(Parkeringshus[i])) return i;
        return -1; // P-Huset är fullt
    }

    // Söker efter en plats som har redan en MC parkerad, perfekt för att lägga till en MC
    static int HittaFörstaSingelMc()
    {
        for (int i = 1; i <= 100; i++)
            if (ÄrSingelMc(Parkeringshus[i])) return i;
        return -1; // Ingen ledig delad plats hittades
    }

    // Hitta index där ett visst regnummer finns
    static int HittaFordon(string reg)
    {
        for (int i = 1; i <= 100; i++)
        {
            if (string.IsNullOrEmpty(Parkeringshus[i])) continue;

            // Delar upp platsens sträng i enskilda kvitton (t.ex. 2 st för MC)
            string[] kvitton = DelaKvitton(Parkeringshus[i]);
            for (int k = 0; k < kvitton.Length; k++)
            {
                string typ2, reg2;
                if (TolkaKvitto(kvitton[k], out typ2, out reg2))
                {
                    // Hittade det vi sökte?
                    if ((reg2 ?? "").ToUpper() == (reg ?? "").ToUpper())
                        return i;
                }
            }
        }
        return -1; // Hittades inte
    }

    // Kontrollerar om ett fordon av typ "typ" får placeras på "plats"
    static bool KanPlaceraPå(int plats, string typ, string reg)
    {
        string platsData = Parkeringshus[plats];
        if (InnehållerReg(platsData, reg)) return false; // Redan parkerad där.
        if (string.IsNullOrEmpty(platsData)) return true; // Parkeringsplatsen är tom

        if (ÄrBil(platsData)) return false; // En bil tar upp platsen helt
        if (typ != "MC") return false;      // En MC kan bara dela plats med en annan MC

        // Platsen har en MC, och vi flyttar en MC → OK
        if (ÄrSingelMc(platsData)) return true;

        return false;    // Antingen full MC-plats eller en bil står där
    }

    // =================== KVITTO-hjälp (Split/strängar) ===================

    // Skapar den interna strängen: "TYP#REG"
    static string SkapaKvitto(string typ, string reg)
    {
        return typ + SEP_TYPREG + reg;
    }

    // Delar upp platsdatan i enskilda kvittosträngar
    static string[] DelaKvitton(string platsData)
    {
        if (string.IsNullOrEmpty(platsData)) return new string[0];
        return platsData.Split(SEP_MULTI);
    }

    // Försöker tolka "TYP#REG" till dess delar
    static bool TolkaKvitto(string kvitto, out string typ, out string reg)
    {
        typ = null; reg = null;
        if (string.IsNullOrEmpty(kvitto)) return false;

        string[] delar = kvitto.Split(SEP_TYPREG);
        if (delar.Length != 2) return false;

        typ = (delar[0] ?? "").Trim().ToUpper();
        reg = (delar[1] ?? "").Trim().ToUpper();
        return typ == "BIL" || typ == "MC";
    }

    // Returnerar true om platsen innehåller en BIL
    static bool ÄrBil(string platsData)
    {
        string[] kvitton = DelaKvitton(platsData);
        if (kvitton.Length != 1) return false;

        string typ, reg;
        return TolkaKvitto(kvitton[0], out typ, out reg) && typ == "BIL";
    }

    // Returnerar true om platsen innehåller EN MC
    static bool ÄrSingelMc(string platsData)
    {
        string[] kvitton = DelaKvitton(platsData);
        if (kvitton.Length != 1) return false;

        string typ, reg;
        return TolkaKvitto(kvitton[0], out typ, out reg) && typ == "MC";
    }

    // Kollar om regnumret finns någonstans i platsens data
    static bool InnehållerReg(string platsData, string reg)
    {
        string[] kvitton = DelaKvitton(platsData);
        for (int i = 0; i < kvitton.Length; i++)
        {
            string typ2, reg2;
            if (TolkaKvitto(kvitton[i], out typ2, out reg2))
            {
                if ((reg2 ?? "").ToUpper() == (reg ?? "").ToUpper())
                    return true;
            }
        }
        return false;
    }

    // Lägger ihop två kvitton (t.ex. "MC#A" + "MC#B" → "MC#A|MC#B")
    static string SlåIhopKvitton(string befintligPlatsData, string nyttKvitto)
    {
        if (string.IsNullOrEmpty(befintligPlatsData)) return nyttKvitto;
        if (string.IsNullOrEmpty(nyttKvitto)) return befintligPlatsData;
        return befintligPlatsData + SEP_MULTI + nyttKvitto;
    }

    // Tar bort ett visst regnummer ur platsens sträng och bygger ihop resten.
    // T.ex. tar bort MC#B från "MC#A|MC#B" och returnerar "MC#A".
    static string TaBortReg(string platsData, string reg)
    {
        string[] kvitton = DelaKvitton(platsData); // 0, 1 eller 2 kvitton
        var kvittonAttBehålla = new List<string>();

        for (int i = 0; i < kvitton.Length; i++)
        {
            string typ2, reg2;
            if (TolkaKvitto(kvitton[i], out typ2, out reg2))
            {
                // Om detta kvitto INTE är det som ska tas bort, behåll det.
                if (!((reg2 ?? "").ToUpper() == (reg ?? "").ToUpper()))
                {
                    kvittonAttBehålla.Add(kvitton[i]);
                }
            }
        }

        // Sätter ihop de återstående kvitton till en ny sträng.
        return string.Join(SEP_MULTI.ToString(), kvittonAttBehålla);
    }

    // Kollar fordonstypen för ett specifikt regnummer på en plats (t.ex. om det är MC eller BIL)
    static string TypFörRegPåPlats(string platsData, string reg)
    {
        string[] kvitton = DelaKvitton(platsData);
        for (int i = 0; i < kvitton.Length; i++)
        {
            string typ2, reg2;
            if (TolkaKvitto(kvitton[i], out typ2, out reg2))
            {
                if ((reg2 ?? "").ToUpper() == (reg ?? "").ToUpper())
                    return typ2;
            }
        }
        return null;
    }

    // =================== UTSKRIFT/PAUS ===================

    // En enkel helper-metod för att skriva ut meddelanden och pausa.
    static void Info(string msg)
    {
        Console.WriteLine(msg);
        Paus();
    }

    // Väntar på tangenttryckning
    static void Paus()
    {
        Console.WriteLine("Tryck valfri tangent för att fortsätta...");
        Console.ReadKey(true);
    }
}
