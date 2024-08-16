using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
namespace AduioDesign
{
    /// <summary>
    /// 用于管理全局音量
    /// </summary>
    public class AudioVolumeManager : MonoBehaviour
    {
        public Slider BgmSilider;

        public Slider SfxSilider;

        [SerializeField] private AudioMixer mixer;
        // Start is called before the first frame update
        void Start()
        {
            BgmSilider.onValueChanged.AddListener((t) =>
            {
                SetBGM(t);
            });
            SfxSilider.onValueChanged.AddListener((t) =>
            {
                SetSFX(t);
            });
            if (PlayerPrefs.GetString("IsFirstEnterGame") != "false")
            {
                PlayerPrefs.SetFloat("BGM", 0);
                PlayerPrefs.SetFloat("BGS", 0);
                PlayerPrefs.SetString("IsFirstEnterGame", "false");
            }
            else
            {
                mixer.SetFloat("BGM", PlayerPrefs.GetFloat("BGM"));
                BgmSilider.value = 1 - PlayerPrefs.GetFloat("BGM") / -40f;
                mixer.SetFloat("SFX", PlayerPrefs.GetFloat("BGS"));
                SfxSilider.value = 1 - PlayerPrefs.GetFloat("SFX") / -40f;
            }
        }

        public void SetBGM(float value)
        {
            if (value < -40)
                value = -80;
            mixer.SetFloat("BGM", -40 + 40 * value);
            PlayerPrefs.SetFloat("BGM", -40 + 40 * value);
        }

        public void SetSFX(float value)
        {
            if (value < -40)
                value = -80;
            mixer.SetFloat("SFX", -40 + 40 * value);
            PlayerPrefs.SetFloat("SFX", -40 + 40 * value);
        }
    }
}
