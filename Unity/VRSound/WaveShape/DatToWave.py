import wave
from scipy import fromstring, int16

# 90秒分に相当するフレーム数を算出
ch = 2
fr = 44100
width = 2

file = open('original.txt', 'r')  #読み込みモードでオープン
string = file.read()      #readですべて読み込む
# print(string)
X = fromstring(string, dtype=int16)

print(X)
# 出力データを生成
outf = './test_original.wav'
w = wave.Wave_write(outf)
w.setnchannels(2)
w.setsampwidth(4)
w.setframerate(44100)
w.setnframes(fr*ch)
# w.writeframes(x)
w.close()
