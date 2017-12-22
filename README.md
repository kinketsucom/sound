# Link
+ [Version up info](#Version-up-info)
+ [メッシュ作成の記録](#メッシュ作成の記録)

# <u>Version up info</u>
+ ver1.0
適当なwave音源を距離に対して減衰させる。また、距離に応じて遅れて聞こえるようにもする。
+ ver1.1
距離に応じて遅れて聞こえるように変更。
+ ver 1.2
複数の音源にたいして距離に応じて遅延させるように変更。  
FIXIT:オブジェクトの衝突判定を用いて音を表現する方が良いかもしれないと考えたので、次回実装。
+ ver 1.3
衝突判定による音の発生に変更。FIXIT:衝突領域から外れた場合の処理が不安定。


# <u>メッシュ作成の記録</u>
1. ターミナル->module load patran->patran
1. File->New->ok
1. viweport->create->適当なviewportを選択or作成
1. viewport(青い画面)で右クリック->verify->element->normals
1. Action->Modify

# <u>各ファイルの説明(暫定)</u>
+ fort.100 (現在は)ノイマン条件下なので、ディリクレ条件のファイル

