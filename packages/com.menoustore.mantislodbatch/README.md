# Mantis LOD Batch Generator (Private)

選択したオブジェクト群に対して、[Mantis LOD Editor](http://www.mesh-online.net/mantis.html) のAPIを直接呼び出し、
LOD1/LOD2メッシュの生成とLODGroupのセットアップを一括自動化するUnityエディタ拡張です。

VRChatワールド制作でのポリゴン数最適化を想定していますが、VRChat SDKへの依存はなく、
Unity汎用のLODGroupコンポーネントのみを使用します。

## 必須要件

- **Mantis LOD Editor(有償, Unity Asset Store)** を別途購入し、対象プロジェクトの `Assets` 配下にインポートしておく必要があります。
  このパッケージにはMantis LOD Editor本体のファイルは一切含まれていません。
- Unity 2022.3以降

## 使い方

1. Unityメニューの `Tools > 軽量化検証 > Mantis LODを一括生成` を開く
2. Hierarchyで対象オブジェクトを選択
   - MeshRendererを持つオブジェクトを直接選択、または
   - 親オブジェクトを選択(子階層のMeshRendererを自動探索して全て処理)
3. ウィンドウでLOD1/LOD2の品質(%)と切替しきい値(画面占有率)を設定
4. 「選択中のオブジェクトに生成」をクリック

## 生成されるもの

- `Assets/GeneratedLODs/` に、簡略化されたメッシュアセット(`<オブジェクト名>_LOD1.asset` / `_LOD2.asset`)
- 対象オブジェクトの子として `LOD1` / `LOD2` GameObject(元のマテリアルを使用)
- 対象オブジェクトに標準の `LODGroup` コンポーネント(LOD0=元のメッシュ、LOD1/LOD2=簡略化メッシュ)

既に `LODGroup` が付いているオブジェクトは自動的にスキップされます。

## 注意

- 元のメッシュアセットは変更しません(複製した上で処理します)。
- 彫刻装飾など細部の多いメッシュは、デフォルト設定(LOD1=50%, LOD2=20%)だと見た目が崩れることがあります。1つ試してから他のオブジェクトに適用することを推奨します。
