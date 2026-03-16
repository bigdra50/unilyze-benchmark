# unilyze-benchmark

[unilyze](https://github.com/bigdra50/unilyze) のベンチマーク用リポジトリ。
サンプルのUnityプロジェクトに対して解析を実行し、解析モードごとの結果を比較する。

## プレイ動画

https://github.com/user-attachments/assets/b86bcd5f-626e-47ef-947f-61f40ec37cc7

## プロジェクト構成

```
.
├── Project/          # Unity 6 サンプルプロジェクト（ローグライク風ゲーム）
│   ├── Assets/App/
│   │   ├── Core/     # ドメインロジック（AI, Combat, Dungeon, Inventory, Turn等）
│   │   ├── Editor/   # エディタ拡張
│   │   └── Game/     # MonoBehaviour層（Bootstrap, Camera, UI, Views等）
│   ├── Packages/
│   └── ProjectSettings/
└── results/          # unilyze 解析結果
    ├── with-semantic.json        # セマンティック解析あり
    ├── syntax-only.json          # 構文解析のみ
    └── semantic-vs-syntax.json   # 両モードの差分比較
```

## 計測メトリクス

| カテゴリ | メトリクス |
|---------|-----------|
| 健全性 | CodeHealth, MaintainabilityIndex |
| 複雑度 | CognitiveComplexity, CyclomaticComplexity, NestingDepth |
| 結合度 | CBO, AfferentCoupling, EfferentCoupling |
| アセンブリ | Abstractness, Instability, DistanceFromMainSequence, RelationalCohesion |

## 環境

- Unity 6000.3.5f2
- VContainer 1.17.0
- UniTask 2.5.10
