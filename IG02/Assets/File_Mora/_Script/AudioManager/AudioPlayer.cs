using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace AduioDesign
{
    /// <summary>
    /// 用于音频播放与管理
    /// </summary>
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup audioMixer;
        [Header("可选-本物体附着的物体")]
        [SerializeField] private Transform parent;
        private AudioSource source;
        [Header("音频素材")]
        [SerializeField] private List<AudioClip> clips = new();

        // Start is called before the first frame update
        void Start()
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = audioMixer;
        }

        private void Update()
        {
            if (parent != null && transform.parent != parent)
            {
                transform.SetParent(parent.transform);
                transform.position = parent.transform.position;
            }

            //switch (type)
            //{
            //    case AudioType.Bgm:
            //        audioMixer.GetFloat("BGM", out var value);
            //        source.volume = value;
            //        break;
            //    case AudioType.Sfx:
            //        audioMixer.GetFloat("SFX", out var value2);
            //        source.volume = value2;
            //        break;
            //}
        }

        public void PlayAudioClip(int targetIndex)
        {
            if (targetIndex >= clips.Count || clips[targetIndex] == null)
            {
                Debug.LogError("不存在此音效");
                return;
            }

            if (source.isPlaying)
                return;

            source.clip = clips[targetIndex];
            source.Play();
        }

        public void StopPlayAudioClip()
        {
            source.Stop();
        }
    }
    public enum AudioType
    {
        Bgm, Sfx
    }
}