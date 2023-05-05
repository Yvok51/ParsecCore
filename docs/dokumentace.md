# Dokumentace

ParsecCore je knihovna v jazyce C#, pomocí které lze vytvářet nejrůznější parsery. Knihovna je inspirována Haskellovou knihovnou [*Parsec*](https://hackage.haskell.org/package/parsec). Jedná se tedy o knihovnu, která obsahuje několik jednoduchých základních parserů a funkce, pomocí kterých lze parsery kombinovat dohromady. S knihovnou, stejně jako s původním *Parsecem*, jsou vytvářeny *top-down* parsery, avšak knihovna obsahuje kombinátory *ChainL* a *ChainL1*, pomocí kterých se lze vypořádat s levou rekurzí.

## Parser

Parser je obvykle kus programu, který vezme vstup a pokusí se najít sekvenci znaků na začátku vstupu, která splňuje jistá kritéria. Parser si obecně můžeme představit jako funkci, která vezme vstup (řetězec znaků) a vrátí nezkonzumovaný vstup a výsledek parsování, přičemž samotný výsledek parsování může být buď úspěšný výsledek, či chyba vzniklá při parsování. Tohoto designu jsme se pokusili držet při vytváření knihovny.

Parser, který je konkrétně v naší knihovně, je delegát:

```csharp
public delegate IEither<ParseError, T> Parser<out T>(IParserInput input);
```

Jak můžeme vidět, tak `Parser` je generický delegát, který je kovariantní dle typového parametru `T`. Samotný parametr `T` je typ výsledku úspěšného parsování. Typ `ParseError` reprezentuje chybu, která nastala během parsování. Typ `T` a `ParseError` jsou zabaleny dohromady do `IEither` což je objeckt, který reprezentuje buď `ParseError`, nebo `T`, avšak ne oboje najednou. Nakonec `IParserInput` reprezentuje vstup parseru. Máme tedy fukci ze vstupu do výsledku parsování. Poslední nezmíněná část - kolik vstupu nebylo zkonzumováno - si hlídá samotný `IParserInput` jako vnitřní stav.

## Používání knihovny

### IParserInput

Nejdříve si řekneme jak vytvářet vstup pro `Parser`. Vstup vytvoříme pomocí statické metody `Create` na třídě `ParserInput`.

Příklad:

```csharp
using ParsecCore.Input;

.
.
.

IParserInput stringParserInput = ParserInput.Create("Some string input");
using (StreamReader input = new StreamReader(inputFilename))
{
    IParserInput streamParserInput = ParserInput.Create(input);
}
```

Můžeme vytvořit vstup jak z řetězců, tak ze streamů. V případě streamů objekt `IParserInput` nepřebírá vlastnictví streamu a je tedy povinností volajícího se zbavit zdrojů. Dalším omezením na streamovém vstupu je fakt, že je nutné, aby stream byl schopen seekovat. Pokud stream není schopen seekovat, tak je vyhozena vyjímka při volání `ParserInput.Create`

### Delegate Parser a IEither

Delegát `Parser` je volán jako každý delegát. Při volání může zkonzumovat nějakou část vstupu z objektu `IParserInput`, který mu byl předán. `Parser` vrací jako návratovou hodnotu `IEither<ParseError, T>`, který reprezentuje buď chybu, či výsledek parsování.

Běžná práce s parsováním:

```csharp
using ParsecCore;
using ParsecCore.Input;
using ParsecCore.EitherNS;

.
.
.

IParserInput input = ParserInput.Create("-12");
Parser<int> parser = Parsers.DecimalInteger;

IEither<ParseError, int> result = parser(input);

if (result.IsError)
{
    Console.WriteLine(result.Error);
    return;
}

int parserInteger = result.Result;
```

Kde `Parsers.DecimalInteger` je parser, který je součástí knihovny a parsuje celá čísla v desítkové soustavě.

### LINQ syntax

Velmi často potřebujeme, aby parsery byly použity v sekvenci za sebou. V takovém případě bychom mohli napsat použití parserů manuálně, avšak nesmíme zapomenout, že parsery mohou končit chybou a takové chyby bychom měli odchytávat. Výsledkem by tedy bylo mnoho if příkazů.

Místo toho "zneužijeme" LINQ syntaxi, která se nám bude automaticky starat o zachycování chyb.

Příklad:

```csharp
var someParser = 
    from prefix in Parsers.Char('x')
    from integer in Parsers.DecimalInteger
    from suffix in Parsers.Char('L')
    select 10 * integer;
```

V příkladu jsme vytvořili nový parser `someParser`, který postupně za sebou se pokusí naparsovat znak 'x', poté nějaké celě číslo a nakonec znak 'L' a vrátí desetinásobek naparsovaného čísla. Přijme tedy vstup ve formě "x123L" a jako návratovou hodnotu z parsování by předal `1230`.

Výraz `from myVariable in myParser` aplikuje parser `myParser` a pokud uspějeje, tak výsledek vloží do proměnné `myVariable`. Pokud tedy `myParser` je typu `Parser<T>`, tak `myVariable` je typu `T`.

Výraz `select myExpression` je vždy poslední výraz v naší LINQ syntaxi a určuje, co bude vytvořený parser vracet.

LINQ notace se, jak už bylo zmíněno, sama stará o chybu v parsování. Pokud by v nějakém výrazu `from myVariable in myParser` aplikace parseru `myParser` selhala, tak automaticky je chyba předána výš k vytvořenému parseru, který tedy při aplikaci vrátí tu samou chybu.

### Cyklická závislost parserů

Jedna nepříjemná situace, která může nastat, při konstruování komplexnejších parserů, je, že parsery jsou cyklicky závislé na sobě. Tedy máme dva parsery `parser1` a `parser2`, které používají sami sebe navzájem při konstrukci. Problémem samozřejmě je, že proměnná použitého parseru bude u jednoho z nich mít hodnotu null.

Příklad (ignorujte prosím, že příklad je dost uměle vytvořený):

```csharp
Parser<int> parser1 = 
    from prefix in Parsers.Char('p')
    from value in parser2
    select value;

Parser<int> parser2 =
    from prefix in Parsers.Char('u')
    from value in parser1
    select value;
```

Tento problém má řešení pomocí přidání indirekce u vytvoření takového parseru. Pro parser, který je definován jako první přidáme deklaraci lambda funkce, vytvářející nový parser, jejíž tělo bude tvořit ten samý výraz, který tvořil původní definici parseru, akorát tento parser ihned zavoláme.

```csharp
Parser<int> parser1 = (IParserInput input) =>
{
    return (from prefix in Parsers.Char('p')
           from value in parser2
           select value)(input);
}

Parser<int> parser2 =
    from prefix in Parsers.Char('u')
    from value in parser1
    select value;
```

Tímto způsobem vytvoříme closure nad lambda výrazem, který zachytí proměnnou `parser2`. Pokud budeme používat `parser1` až po definici `parser2`, tak proměnná již nebude mít hodnotu null aplikace parseru se povede.

### Základní parsery a kombinátory

Nyní představíme pár základních parserů a kombinátorů, které jsou užitečné při práci s knihovnou.

Parsery:

- `EOF` - parsuje konec souboru
- `Char` - naparsuje jeden znak
- `Satisfy` - naparsuje znak pokud projde predikátem
- `Spaces` - naparsuje libovolné množství whitespace
- `String` - naparsuje daný řetězec
- `Token` - aplikuje daný parser a zkonzumuje libovolné množství whitespace poté
- `Symbol` - naparsuje daný řetězec a zkonzumuje libovolné množství whitespace poté (kombinace `String` a `Token`)
- `Return` - parser, který vždy uspějeje
- `Fail` - parser, který vždy **ne**uspějeje

Kombinátoři:

- `Many` - modifikuje parser, aby aplikoval sám sebe libovolně-krát a vrátí list naparsovaných hodnot
- `Count` - modifikuje parser, aby aplikoval sám sebe tolikrát, kolik je dáno, a vrátí list naparsovaných hodnot
- `Option` - modifikuje parser tak, aby vrátil defaultní hodnotu v případě, že parsování neuspěje
- `Try` - modifikuje parser, aby parser nezkonzumoval žádný vstup při neúspěchu v parsování.
- `LookAhead` - modifikuje parser, aby parser nezkonzumoval žádný vstup v případě úspěchu v parsování.
- `Choose` - pokusí se aplikovat list parserů jeden po sobě a vrátí první úspěch. Pokud všechny parsery neuspějí, tak vrátí chybu posledního parseru.
- `SepBy` - pokusí se naparsovat hodnoty oddělenné jistým separátorem. Užitečné pro parsování listů
- `ChainL` - pokusí se naparsovat binární operace s levou associativitou. Užitečné pro vypořádání se s levou rekurzí

ParsecCore podporuje všechny kombinátory, které se nacházejí v původní knihovně [*Parsec*](https://hackage.haskell.org/package/parsec-3.1.15.1/docs/Text-Parsec.html#g:2) takže pokud tyto kombinátory by nestačili, tak si můžete projít funkce obsažené v souborech `Combinators.cs` a `ParserExt.cs`, zda neobsahují funkci, kterou byste zrovna potřebovali.

### Pomocné typy

Knihovna obsahuje několik pomocných typů, s kterými jako uživatel budete pracovat.

První z nich je již zmíněný `IEither<TError, TResult>`. Tento typ reprezentuje výsledek výpočtu, který mohl skončit s chybou. Pokud chyba nastala, tak `IEither` obsahuje pouze typ daný parametrem `TError`, jehož hodnotu dostaneme pomocí vlastností `Error`. Naopak pokud výpočet skončí bez chyby, tak obsahuje pouze hodnotu typu `TResult`, kterou získáme pomocí vlastnotsi `Result`. V obou případech druhá nevyužitá vlastnost vyhodí výjimku. Instance `IEither` je získáná zavoláním `Either.Result`, pokud chceme reprezentovat výsledek výpočtu, a `Either.Error`, pokud chceme reprezentovat chybu ve výpočtu.

Další z nich je `IMaybe<T>`, který také reprezentuje možnou chybu při výpočtu. Narozdíl od `IEither` nám pouze říká zda chyba nastala. Pokud chyba nastala, tak vlastnost `IsEmpty` vrátí `true` a pokus získat hodnotu skončí s vyjímkou. Instance `IMaybe` je vytvořena zavoláním `Maybe.FromValue`, pokud nenastalo k chybě, a `Maybe.Nothing`, pokud k chybě nastalo.

Následujícím pomocným typem je `ParseError`. Jedná se o jednoduchou strukturu, která nás informuje o chybě při parsování. K tomuto účelu obsahuje pozici ve vstupu, kde k chybě nastalo, a také chybovou zprávu popisující chybu.

Posledním pomocným typem, s kterým se setkáte, je `None`. Tento typ reprezentuje prázdný typ. Například u parseru `EOF` sémanticky nemá smysl vracet žádnou hodnotu a návratová hodnota takového parseru by mělo být něco jako `IEither<ParseError, void>`. Nemůžeme však použít `void` jako typový parametr, proto jsme vytvořili typ `None`, který syntakticky funguje a sémanticky má stejný význam.

## JSONtoXML

### Používání

Program JSONtoXML je použit tímto způsobem:

```powershell
JSONtoXML.exe {inputFile} {outputFile}
```

Soubor `inputFile` musí mít jako obsah validní JSON. Konkrétně se jedná o JSON specifikován [zde](https://datatracker.ietf.org/doc/html/rfc8259).

Obsah souboru `inputFile` je naparsován a následně převeden do XML formátu a napsán do souboru `outputFile`. Pokud dojde během parsování k chybě, tak je na výstup napsáno kde k chybě došlo ve formátu *řádek* : *sloupec* - *chybová hláška*.

Ukázka jak JSON bude převeden do formátu XML:

```json
{
   "firstName": "Joe",
   "lastName": "Jackson",
   "gender": null,
   "currentlyEmployed": true,
   "age": 28,
   "address": {
       "streetAddress": "101",
       "city": "San Diego",
       "state": "CA"
   },
    "phoneNumbers": [
        {
            "type": "home",
            "number": "7349282382"
        },
        {
            "type": "work",
            "number": "8329984322"
        }
    ]
}
```

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <firstName>Joe</firstName>
  <lastName>Jackson</lastName>
  <gender>null</gender>
  <currentlyEmployed>true</currentlyEmployed>
  <age>28</age>
  <address>
    <streetAddress>101</streetAddress>
    <city>San Diego</city>
    <state>CA</state>
  </address>
  <phoneNumbers>
    <Item>
      <type>home</type>
      <number>7349282382</number>
    </Item>
    <Item>
      <type>work</type>
      <number>8329984322</number>
    </Item>
  </phoneNumbers>
</root>
```

### Vnitřní reprezentace

Položky v AST JSONu jsou reprezentovány třídou `JsonValue` a jejími potomky, zatímco položky v ASAT XML jsou reprezentovány třídou `XMLNode` a jejími potomky.

Soubor `Parsers.cs` obsahuje parsery používané k parsování JSONu. Jediným používaným parserem mimo třídu je `JsonDocument`, který naparsuje celý soubor JSON a i zkontroluje zda soubor neobsahoval nic po JSONu.

Třída `ToXML` obsahuje funkce, které převedou JSON AST na XML AST a třída `PrintXML` vytiskne XML AST.
