using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// パーティクル省エネスクリプト。
// トリガーColliderにプレイヤーが「誰か1人でも」入っている間だけ
// 登録したParticleSystemを再生し、範囲外では停止してCPU/描画コストを避ける。
//
// 使い方:
//  1. 演出したいエリアに空のGameObjectを作り、Box/Sphere ColliderをIs Trigger=ONで追加
//     （範囲＝そのColliderのSize）
//  2. このスクリプトをそのGameObjectにアタッチ
//  3. Inspectorの Target Particles に対象のParticleSystemをドラッグ（複数まとめて登録可）
//  4. 各ParticleSystem側の Play On Awake はOFFにしておく（起動時は非再生スタートにするため）
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ParticleRangeTrigger : UdonSharpBehaviour
{
    [Tooltip("誰かがこのColliderに入っている間だけ再生するParticleSystem（複数可）")]
    public ParticleSystem[] targetParticles;

    private int playersInside = 0;

    void Start()
    {
        StopAll();
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        playersInside++;
        if (playersInside == 1)
        {
            PlayAll();
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        playersInside--;
        if (playersInside <= 0)
        {
            playersInside = 0;
            StopAll();
        }
    }

    void PlayAll()
    {
        if (targetParticles == null) return;
        for (int i = 0; i < targetParticles.Length; i++)
        {
            if (targetParticles[i] != null) targetParticles[i].Play();
        }
    }

    void StopAll()
    {
        if (targetParticles == null) return;
        for (int i = 0; i < targetParticles.Length; i++)
        {
            if (targetParticles[i] != null)
                targetParticles[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
