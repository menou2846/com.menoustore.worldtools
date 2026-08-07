using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// 監視用カメラの省エネスクリプト。
// トリガーColliderにプレイヤーが「誰か1人でも」入っている間だけ
// 対象Cameraを有効化し、RenderTextureへの描画コストを避ける。
//
// 使い方:
//  1. 舞台カメラのRenderTextureを表示するモニター（Screen等）付近に
//     空のGameObjectを作り、Box/Sphere ColliderをIs Trigger=ONで追加
//  2. このスクリプトをそのGameObjectにアタッチ
//  3. Inspectorの Target Camera に対象の舞台カメラをドラッグ
//  4. Camera側の m_Enabled は最初からOFFにしておく（起動時は非表示）
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class StageCameraTrigger : UdonSharpBehaviour
{
    [Tooltip("誰かがこのColliderに入っている間だけ有効化するカメラ")]
    public Camera targetCamera;

    private int playersInside = 0;

    void Start()
    {
        if (targetCamera != null)
        {
            targetCamera.enabled = false;
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        playersInside++;
        if (targetCamera != null && !targetCamera.enabled)
        {
            targetCamera.enabled = true;
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        playersInside--;
        if (playersInside <= 0)
        {
            playersInside = 0;
            if (targetCamera != null)
            {
                targetCamera.enabled = false;
            }
        }
    }
}
