import sys
import re


counter = 0
triangle_file = open('meshtriangle.d', 'w')#出力先
point_file = open('meshpoint.d', 'w')#出力先
parameter_file = open('meshhparam.d', 'w')


triangle_num = 0 #三角形の個数(nel)
node_point = 0 #三角形をつくるnodeの数(npo)

for line in open('fmm_b.dat', 'r'):
    if(counter==0):#1行目のデータを取っておく
        itemList = line[:-1]
        data = itemList.split()
        triangle_num = data[0]
        node_point = data[1]
        # data[2]何か不明なのでいまは保留

    elif(counter<= int(triangle_num)): #meshtriangle.dの書き込み
        itemList = line[7:-9]#三角形の番号はいらないので最初から7文字消去している。またケツの２つの数字がいらないのでまたいろいろしてる。
        triangle_file.write(itemList)
        triangle_file.write("        -1")#ディリクレ問題なので-1をついか
        triangle_file.write("\n")

    else: #meshpoint.dの書き込み
        itemList = line[10:-1]
        point_file.write(itemList)
        point_file.write("\n")
    counter += 1


#meshparam.dの書き込み
parameter_file.write("        ")
parameter_file.write(node_point)
parameter_file.write("        ")
parameter_file.write(triangle_num)
parameter_file.write("  0.100000000000000              50\n1.00000000000000     SC")

#クローズ
triangle_file.close()
point_file.close
parameter_file.close()
