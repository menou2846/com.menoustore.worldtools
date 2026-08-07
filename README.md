# menotool World Tools (Private)

menou-store が使用しているVRChat**ワールド**制作用のツール・ギミック集です(`com.vrchat.avatars`向けの `com.menoustore.tools` とはSDKが異なるため別リポジトリ)。Privateリポジトリのため、ここからインストールできるのは許可された本人・関係者のみです。

## 含まれるパッケージ

| パッケージID | 内容 |
|---|---|
| `com.menoustore.stagecameratrigger` | プレイヤーがトリガーColliderに入っている間だけ舞台カメラを有効化する省エネ監視カメラギミック(UdonSharp) |
| `com.menoustore.particlerangetrigger` | プレイヤーがトリガーColliderに入っている間だけParticleSystemを再生する省エネギミック(UdonSharp) |

必要なものだけ個別にインストールできます。

## 依存関係

- Unity 2022.3以降
- VRChat SDK - Worlds (`com.vrchat.worlds`) 3.5.0以降(UdonSharpを含む)

## VCC(VRChat Creator Companion)での導入方法

このリポジトリはPrivateのため、認証ヘッダー(Personal Access Token)の設定が必要です。classic tokenを使用してください(`repo`スコープ)。

1. GitHubで classic personal access token を発行
   - https://github.com/settings/tokens/new
   - Scopes: `repo` にチェック
2. VCC(ALCOM等) → `Settings` → `Packages` タブ → `Add Repository`
3. URL欄に以下を入力(`api.github.com` の Contents API エンドポイント):

```
https://api.github.com/repos/menou2846/com.menoustore.worldtools/contents/index.json
```

4. ヘッダーを2つ追加:
   - `Authorization` = `token <発行したPAT>`
   - `Accept` = `application/vnd.github.raw+json`
5. 追加後、対象プロジェクトの `Manage Project` 画面に上記パッケージが個別に表示されるので、必要なものだけInstall

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

## 注意

開発は必ずこのリポジトリのクローン(`C:\_VPMDev\com.menoustore.worldtools`)で行ってください。VCCでInstallすると対象フォルダの中身がzipの内容で上書きされるため、gitの作業ディレクトリをVCCのインストール先と兼用しないでください。
