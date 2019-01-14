using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceLoader : MonoBehaviour {
    public static float SoundPlay(string path, Transform origin, bool overrideOther = false, float volume = 0.1f){
        // if sound already playing
        AudioSource audioPlaying = origin.GetComponentInChildren<AudioSource>();

        if (overrideOther && audioPlaying != null)
                Destroy(audioPlaying.gameObject);
            
        GameObject sfx = Resources.Load<GameObject>("Sound/" + path);
        if (soundMissing(sfx, path)){
            return 0;
        }

        sfx = Instantiate(sfx, origin.position, origin.rotation, origin);
        AudioSource sound = sfx.GetComponent<AudioSource>();
        sound.volume = volume;
        Destroy(sfx, sound.clip.length);
        return sound.clip.length;
    }
        
    public static void SoundMenu(string path){
        // if sound already playing
        Transform origin = Camera.main.transform;

        AudioSource audioPlaying = origin.GetComponentInChildren<AudioSource>();

        GameObject sfx = Resources.Load<GameObject>("Sound/" + path);
        if (soundMissing(sfx, path)){
            return;
        }

        if (audioPlaying != null)
        {
            if (audioPlaying.clip == sfx.GetComponent<AudioSource>().clip)
                return;
            else
                Destroy(audioPlaying.gameObject);
        }

        sfx = Instantiate(sfx, origin.position, origin.rotation, origin);
    }

    private static bool soundMissing(GameObject sfx, string loc){
        loc = "Resources/Sound/" + loc;
        if (sfx == null)
        {
            Debug.Log("Object missing: " + loc);
            return true;
        }

        if (sfx.GetComponent<AudioSource>().clip == null){
            Debug.Log("Prefab exists, missing audio: " + loc);
            return true;
        }
        return false;
    }

    public static void LoadImageObject(string path, Transform origin){
        GameObject image = Resources.Load<GameObject>(path);
        if (image != null)
            Instantiate(image, origin);
        else
            Debug.Log("Image at path missing: " + path);
    }

    public static void LoadImageObjectForTime(string path, Transform origin, float time){
        GameObject image = Resources.Load<GameObject>(path);
        if (image != null)
            Destroy(Instantiate(image, origin), time);
        else
            Debug.Log("Image at path missing: " + path);
    }

    public static Image LoadImage(string path)
    {
        Image theImage = Resources.Load<Image>(path);
        if (theImage == null)
        {
            Debug.Log("Image at path missing: " + path);
            return null;
        }
        return theImage;
    }

    public static void LoadImage(string path, Transform replacePath)
    {
        Sprite theImage = Resources.Load<Sprite>(path);
        if (theImage == null)
        {
            Debug.Log("Image at path missing: " + path);
        }
        replacePath.GetComponent<Image>().sprite = theImage;
    }

    public static void ImageUnloadAll(Transform origin){
        foreach (RawImage childImage in origin.GetComponentsInChildren<RawImage>(true))
        {
            Destroy(childImage.gameObject);
        }

        foreach (Image childImage in origin.GetComponentsInChildren<Image>(true))
        {
            Destroy(childImage.gameObject);
        }
    }
}
