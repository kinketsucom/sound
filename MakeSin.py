# -*- coding: utf-8 -*-
import sys
import numpy as np


args = sys.argv
# 周波数
freq = float(args[1])
# サンプリング周波数
sample_freq = 8000
#時間
time = 1
#必要な定数
pi = np.arccos(-1)

f = open('./1000.dat', 'w') # 書き込みモードで開く
for i in range(0,sample_freq*time):
    data = np.sin(2*pi*freq*i/sample_freq)
    string = str(data)+"\n" #２チャンネルにしておく
    # string = str(i)+" "+str(data)+"\n" #gnuplot確認用
    # print(string)
    f.write(string)
f.close()
