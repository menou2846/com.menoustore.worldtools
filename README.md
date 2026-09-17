# worldtools (Public)

menou-store が公開しているVRChat**ワールド**制作用のツール・ギミック集です(`com.vrchat.avatars`向けの `com.menoustore.tools`/`com.menoustore.menotool` とはSDKが異なるため別リポジトリ)。GitHubアカウント不要で誰でもインストールできます。

## 含まれるパッケージ

**インストールできること**と**実際に機能が使えること**は別です。🔒が付いているパッケージは、VCCでインストールしても、Fanboxで配布されるパスワードで認証するまではツールの本体機能が動きません。

| | パッケージID | 内容 |
|---|---|---|
| 🔓 無料 | `com.menoustore.stagecameratrigger` | プレイヤーがトリガーColliderに入っている間だけ舞台カメラを有効化する省エネ監視カメラギミック(UdonSharp) |
| 🔓 無料 | `com.menoustore.particlerangetrigger` | プレイヤーがトリガーColliderに入っている間だけParticleSystemを再生する省エネギミック(UdonSharp) |
| 🔒 **Fanbox限定(要パスワード認証)** | `com.menoustore.mantislodbatch` | Mantis LOD EditorのAPIを使いLOD1/LOD2メッシュとLODGroupを一括自動生成するエディタ拡張(要Mantis LOD Editor別途購入) |

必要なものだけ個別にインストールできます。

> 🔒が付いているFanbox限定ツールを使うには、パスワード認証用に以下の`com.menoustore.license`リポジトリ**も**VCCに追加してください(このリポジトリ単体はインストールしても何も機能しません。上記🔒ツールの依存パッケージとして自動で入ります):
> ```
> https://raw.githubusercontent.com/menou2846/com.menoustore.license/main/index.json
> ```
> パスワードは[Fanbox](https://kannazukimenou.fanbox.cc/)の支援者限定記事で配布しています。

## 依存関係

- Unity 2022.3以降
- VRChat SDK - Worlds (`com.vrchat.worlds`) 3.5.0以降(UdonSharpを含む)

## VCC(VRChat Creator Companion)での導入方法

GitHubアカウント不要で追加できます。

1. VCCを開く → `Settings` → `Packages` タブ → `Add Repository`
2. 以下のURLを入力して追加:

```
https://raw.githubusercontent.com/menou2846/com.menoustore.worldtools/master/index.json
```

3. 対象プロジェクトの `Manage Project` 画面に上記パッケージが個別に表示されるので、必要なものだけInstall

## 開発者向け: 新バージョンの出し方

1. 変更したいパッケージの `packages/<name>/package.json` の `version` を上げてcommit・push
2. バージョンタグを作成してpush(全パッケージまとめてリリースされます)

```bash
git tag v1.0.1
git push origin v1.0.1
```

3. GitHub Actions(`.github/workflows/publish.yml`)が全パッケージをzip化し、`index.json` を更新します
4. `Actions` タブでワークフローが成功していることを確認してください

## 使い方(Stage Camera Trigger)

1. `Prefabs/舞台カメラ.prefab` をシーンに配置(または個別にCamera・Collider・スクリプトを組む)
2. 舞台カメラのRenderTextureを表示するモニター付近に配置したColliderを `Is Trigger = ON` に
3. `StageCameraTrigger` の `Target Camera` に対象の舞台カメラをアサイン
4. カメラ本体の `Enabled` は最初からOFFにしておく(起動時は非表示、誰かがトリガーに入った時だけ有効化される)

## 使い方(Particle Range Trigger)

1. 演出したいエリアに空のGameObjectを作り、Box/Sphere ColliderをIs Trigger=ONで追加(範囲=そのColliderのSize)。`Prefabs/パーティクルコライダー.prefab` をベースにしてもよい
2. `ParticleRangeTrigger` の `Target Particles` に対象のParticleSystemをドラッグ(複数登録可)
3. 各ParticleSystem側の `Play On Awake` はOFFにしておく(起動時は非再生スタート)

## 使い方(Mantis LOD Batch Generator)

**要件: Mantis LOD Editor(有償, Unity Asset Store)を別途購入し、対象プロジェクトの `Assets` 配下にインポートしておくこと。**このパッケージにはMantis LOD Editor本体は含まれません。

1. Unityメニューの `Tools > 軽量化検証 > Mantis LODを一括生成` を開く
2. Hierarchyで対象オブジェクトを選択(MeshRendererが無ければ子階層を自動探索)
3. LOD1/LOD2の品質(%)と切替しきい値を設定し、「選択中のオブジェクトに生成」をクリック
4. `Assets/GeneratedLODs/` に簡略化メッシュが生成され、対象オブジェクトに標準の `LODGroup` が設定される

## 注意

開発は必ずこのリポジトリのクローン(`C:\_VPMDev\com.menoustore.worldtools`)で行ってください。VCCでInstallすると対象フォルダの中身がzipの内容で上書きされるため、gitの作業ディレクトリをVCCのインストール先と兼用しないでください。
