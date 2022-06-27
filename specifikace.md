# ParsecCore

## Přehled

Mým cílem je vytvořit parsovací knihovnu, která je inspirována knihovnou Parsec z jazyka Haskell. [Parsec](https://hackage.haskell.org/package/parsec) je knihovna pro vytváření parserů pomocí "combinators". Parsery se tedy vytváří pomocí funkcí kombinovujících jednoduché parsery poskytnutých knihovnou. Mým cílem je vytvoření knihovny obsahující tyto jednoduché parsery a způsob jak je kombinovat.

## Cíle

- Vytvořit parsovací knihovnu inspirovanou knihovnou Parsec. Součástí knihovny budou tedy parsery, které jsou zobrazeny [zde](https://hackage.haskell.org/package/parsec-3.1.15.1/docs/Text-Parsec-Char.html) a kombinátory zobrazeny [zde](https://hackage.haskell.org/package/parsec-3.1.15.1/docs/Text-Parsec-Combinator.html)/[zde](https://hackage.haskell.org/package/parsec-3.1.15.1/docs/Text-Parsec.html#g:2)
- Vytvoření ukázkové CLI aplikace, která bude umět parsovat deserializovat JSON.

## Předběžné řešení

Centrální k celému řešení bude bude generické rozhraní, které bude příjimat textový vstup a bude vracet hodnoty různých typů, které byly parsovány ze vstupu.

Parsery z knihovny Parsec jsou monády a proto bych chtěl, aby moje parsery se chovaly podobně, včetně umožnění "do" notace. "Do" notace bych chtěl dosáhnout pomocí LINQ, kde výraz `from x in XParser` by byl podobný funkci bind a tedy měl ekvivalent v haskellovém `x <- XParser` a výraz `select` by byl podobný haskellovém výrazu `return`.

Ukázka možné syntaxe:

```csharp
var integerFromDouble = 
    from integerPart in Digits
    from decimalPoint in Char('.')
    from fractionalPart in Digits
    select Int32.Parse(integerPart);
```

Přičemž `Digits` je parser, který parsuje několik číslic za sebou a `Char('.')` vrátí parser, který naparsuje přesně znak '.'. Výsledný parser `integerFromDouble` by tedy byl parser, který naparsuje desetinná čísla, vezme pouze jejich celou část a tu vrátí jako již naparsovaný integer.

Mimochodem obdobný výraz by se v Parsecu napsal jako:

```haskell
integerFromDouble :: Parser Int
integerFromDouble = do
  integerPart <- digits
  decimalPoint <- char '.'
  fractionalPart <- digits
  return $ read integerPart
```

Tedy vydíme, že naše syntaxe je velice podobná "do" notaci.

Uživatel knihovny bude mít tedy k dispozici funkce, které kombinují jednotlivé parsery, a také LINQ, pomocí kterého lze sekvencovat parsery. Tyto dva způsoby tvoření nových parserů by měli být dostatečné k tomu, aby uživatel mohl vytvořit libovolně složitý parser relativně lehce.

Výsledkem parsování bude buď výsledek parsování, generického typu `T`, či zpráva, která signalizuje neúspěch parsování, která bude složena z chybové hlášky a pozice, kde parsování selhalo.

Parser by měl umět přijmout vstup jak z řetězců, tak ze streamů. U streamů je však nutné, aby bylo možné v rámci čtení použít operaci seek. Seek je nutný k implementaci `try` funkce. Pokud stream nepodporuje seek, tak uživatel může načíst celý vstup do paměti a přeměnit vstup na řetězec, či stream, který seek podporuje. Naše knihovna tedy nebude podporovat vstupy, které se nevejdou do paměti počítače a zároveň nelze v nich seekovat.

## Použité pokročilé vlastnosti

Jak bylo zmíněno dříve, tak z vlastností jazyka, které byly zmíněny na pokročilých přednáškách, bych využil generické třídy a metody.

Následně bych také využil syntaxe LINQ a napsal tedy potřebné extension methods pro mé třidy, které napodobují monády. V plánu mám naiplementovat metody `Select` a `SelectMany`.

Při implementaci extension metod budu samozřejmě muset využít delegátů a i v ostatních částech programu plánuji na časté použití delegátů a lambd. `Parser` samotný bude nejspíše delegát.

## Na co se nebudu zaměřovat

Zajímavým problémem, který se blízko týká samotného parsování, je načítání textu z streamů. Moje knihovna by měla podporovat jak vstup nacházející se v řetězcích, tak vstup nacházející se v streamech (souborech), avšak nebude zaměřením práce vytvořit co nejefektivnější čtení vstupu a implementovat s tím spojené optimalizace.
