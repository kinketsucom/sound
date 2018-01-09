import numpy as np
import wave
import sys

name = "./music/dram.wav"
wavefile = wave.open(name, "r")
framerate = wavefile.getframerate()#サンプリングレート
ch = wavefile.getnchannels()#チャンネル
data = wavefile.readframes(wavefile.getnframes())
x = np.frombuffer(data, dtype="float32")
print("チャンネル:",ch)
print("サンプリングレート:",framerate,"Hz")
print("再生時間:",len(x)/framerate/ch,"sec")

#書き出し
f = open('./music/dram.dat', 'w') # 書き込みモードで開く
counter = 0
for dat in x:
    counter += 1
    f.write(str(dat)+"#")
    sys.stderr.write("\r"+str(int(counter*100/len(x))).rjust(3)+"%")
    sys.stderr.flush()
f.close() # ファイルを閉じる
print("\n")
