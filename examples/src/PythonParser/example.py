vstup = []
vstup = input().split()

i = 0
pocet = 0
while '.' not in vstup[i]:
    pocet = pocet + 1
    i = i + 1

if vstup[i] != '.':
    pocet = pocet + 1

print(pocet)
