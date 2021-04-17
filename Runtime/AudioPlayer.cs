using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.gb.statemachine_toolkit
{
    public class AudioPlayer : MonoBehaviour
    {
        public void Play(AudioClip clip)
        {
            var newSource = new GameObject().AddComponent<AudioSource>();
            newSource.transform.SetParent(this.transform);
            newSource.clip = clip;
            newSource.Play();
            StartCoroutine(WaitThenDestroy(clip.length, newSource));
        }

        IEnumerator WaitThenDestroy(float wait, AudioSource toDestroy)
        {
            yield return new WaitForSeconds(wait);
            toDestroy.Stop();
            Destroy(toDestroy.gameObject);
        }
    }
}
