import os

original = []
oreno = []

for line in open('AAAu_original.txt', 'r'):
    original.append(line)

for line in open('u_array_late.txt', 'r'):
    oreno.append(line)


gosadekita = open('gosa.txt', 'w')
for i in range(0,1000):
    if(float(original[i])!=0):
        fuga = (float(original[i])-float(oreno[i]))/float(original[i])
        gosadekita.write(str(fuga)+'\n')
