using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class BHBAudio
{
    //private const string gamePlayBusPath = "bus:/gameplay_sfx";
    //private const string stingersBusPath = "bus:/stingers";

    public static void PlayOneShot(string eventName, Vector3 sourcePosition)
    {
        RuntimeManager.PlayOneShot(eventName, sourcePosition);
    }

    public static void PlayOneShotAttached(string eventName, GameObject callerObject)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventName); ;

        if (callerObject.GetComponent<Rigidbody2D>() != null)
        {
            RuntimeManager.AttachInstanceToGameObject(instance, callerObject, callerObject.GetComponent<Rigidbody2D>());
        }
        else
        {
            RuntimeManager.AttachInstanceToGameObject(instance, callerObject);
        }

        instance.start();
        instance.release();
    }

    public static EventInstance CreateClipInstance(string eventString)
    {
        return RuntimeManager.CreateInstance(eventString);
    }

    public static void AttachInstanceToGameObject(EventInstance instance, GameObject gameObject)
    {
        RuntimeManager.AttachInstanceToGameObject(instance, gameObject, gameObject.GetComponent<Rigidbody2D>());
    }

    public static void DetachInstanceFromGameObject(EventInstance instance)
    {
        RuntimeManager.DetachInstanceFromGameObject(instance);
    }

    public static void AttachInstanceAndStartNotOneShot(EventInstance instance, GameObject targetGameObject)
    {
        RuntimeManager.AttachInstanceToGameObject(instance, targetGameObject, targetGameObject.GetComponent<Rigidbody2D>());
        instance.start();
    }

    public static void StopAndReleaseInstanceImmediately(EventInstance targetInstance) //if single instance is active at the time of OnDisable
    {
        targetInstance.release();
        RuntimeManager.DetachInstanceFromGameObject(targetInstance);
        targetInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public static void StopAndReleaseInstanceImmediately(EventInstance[] targetInstances) //if multiple instances are active at the time of OnDisable
    {
        for (int i=0; i < targetInstances.Length; i++)
        {
            targetInstances[i].release();
            RuntimeManager.DetachInstanceFromGameObject(targetInstances[i]);
            targetInstances[i].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    public static void StopAndReleaseInstanceFadeOut(EventInstance targetInstance)
    {
        targetInstance.release();
        RuntimeManager.DetachInstanceFromGameObject(targetInstance);
        targetInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    public static void StopAndReleaseInstanceFadeOut(EventInstance[] targetInstances)
    {
        for (int i = 0; i < targetInstances.Length; i++)
        {
            targetInstances[i].release();
            RuntimeManager.DetachInstanceFromGameObject(targetInstances[i]);
            targetInstances[i].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
    
    public static void StopClipInstanceImmediate(EventInstance instance)
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public static void StopClipInstanceFadeOut(EventInstance instance)
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public static EventInstance CreateInstanceLoadedSampleData(EventDescription eventDescription)
    {
        EventInstance instance;
        eventDescription.createInstance(out instance);
        return instance;
    }

    // public static void PauseSFXBusEvents()
    // {
    //     Bank sfxBank;
    //     RuntimeManager.StudioSystem.getBank("bank:/Master", out sfxBank);

    //     int count;
    //     sfxBank.getBusCount(out count);

    //     Bus[] buses = new Bus[count];
    //     sfxBank.getBusList(out buses);
        
    //     for (int i=0; i < buses.Length; i++)
    //     {
    //         string path;
    //         buses[i].getPath(out path);

    //         if (path == gamePlayBusPath || path == stingersBusPath)
    //         {
    //             buses[i].setPaused(true); ;
    //         }
    //     }       
    // }

    // public static void ResumeSFXBusEvents()
    // {
    //     Bank sfxBank;
    //     RuntimeManager.StudioSystem.getBank("bank:/Master", out sfxBank);

    //     int count;
    //     sfxBank.getBusCount(out count);

    //     Bus[] buses = new Bus[count];
    //     sfxBank.getBusList(out buses);

    //     for (int i = 0; i < buses.Length; i++)
    //     {
    //         string path;
    //         buses[i].getPath(out path);

    //         if (path == gamePlayBusPath || path == stingersBusPath)
    //         {
    //             buses[i].setPaused(false);
    //         }
    //     }
    // }

    public static void PlayOneShotFromLoadedSampleData(EventDescription eventDescription, Vector3 position)
    {
        EventInstance instance;
        eventDescription.createInstance(out instance);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.start();
        instance.release();
    }
    public static void PlayOneShotFromLoadedSampleData(EventDescription eventDescription, Vector3 position, float volume)
    {
        EventInstance instance;
        eventDescription.createInstance(out instance);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.setVolume(volume);

        instance.start();
        instance.release();
    }
    
    public static void PlayOneShotAttachedFromLoadedSampleData(EventDescription eventDescription, GameObject targetGameObject)
    {
        EventInstance instance;
        eventDescription.createInstance(out instance);

        if (targetGameObject.GetComponent<Rigidbody2D>() != null)
        {
            RuntimeManager.AttachInstanceToGameObject(instance, targetGameObject, targetGameObject.GetComponent<Rigidbody2D>());
        }
        else
        {
            RuntimeManager.AttachInstanceToGameObject(instance, targetGameObject);
        }

        instance.start();
        instance.release();
    }

    public static void PlayOneShotAttachedFromLoadedSampleData(EventDescription eventDescription, GameObject targetGameObject, float volume)
    {
        EventInstance instance;
        eventDescription.createInstance(out instance);

        if (targetGameObject.GetComponent<Rigidbody2D>() != null)
        {
            RuntimeManager.AttachInstanceToGameObject(instance, targetGameObject, targetGameObject.GetComponent<Rigidbody2D>());
        }
        else
        {
            RuntimeManager.AttachInstanceToGameObject(instance, targetGameObject);
        }

        instance.setVolume(volume);

        instance.start();
        instance.release();
    }

    public static EventDescription EventDescriptionWithLoadedSampleData(string eventPath)
    {
        EventDescription loadedSampleData = RuntimeManager.GetEventDescription(eventPath);
        loadedSampleData.loadSampleData();

        return loadedSampleData;
    }
    
    public static void UnloadSampleData(EventDescription eventDescription)
    {
        eventDescription.unloadSampleData();
    }
    
    public static void UnloadSampleData(EventDescription[] eventDescription)
    {
        for (int i=0; i < eventDescription.Length; i++)
        {
            eventDescription[i].unloadSampleData();
        }
    }
}
