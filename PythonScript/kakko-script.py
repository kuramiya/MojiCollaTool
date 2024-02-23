# テキストファイルのパス
file_path = 'kakko-text.txt'

# 結果を格納するリスト
results = []

# ファイルを開いて各行の最初の1文字目と3文字目を抽出して連結する
with open(file_path, 'r', encoding='utf-8', errors='ignore') as file:
    for line in file:
        # 行の先頭が空白文字でない場合のみ処理を行う
        if line.strip():
            # 行の長さが3文字以上であることを確認してから1文字目と3文字目を抽出して連結
            if len(line) >= 3:
                result = line[0] + line[2]
                results.append(result)

# 結果を1行に連結して表示
print(''.join(results))
