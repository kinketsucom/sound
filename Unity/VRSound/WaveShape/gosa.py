import os

original = []
oreno = []

for line in open('original_u.txt', 'r'):
    original.append(line)

for line in open('naiten_u.txt', 'r'):
    oreno.append(line)


gosadekita = open('koregagosa.txt', 'w')
for i in range(0,8000)):
    if(float(original[i])==0):
        fuga = 0
    else:
        fuga = abs(float(original[i])-float(oreno[i]))/float(original[i])
    print(i,fuga,float(original[i]),float(oreno[i]))
    gosadekita.write(str(fuga)+'\n')
