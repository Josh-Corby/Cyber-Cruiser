using System.Collections;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        public class AudioController : GameBehaviour<AudioController>
        {
            public bool debug;
            public AudioTrack[] Tracks;

            private Hashtable _audioTable; // relationship between audio types (key) and audio tracks (value)
            private Hashtable _jobTable; //relationship between audio types (key) and jobs (value) (Coroutine,Ienumerator)

            [System.Serializable]
            public class AudioObject
            {
                public AudioType Type;
                public AudioClip Clip;
            }

            [System.Serializable]
            public class AudioTrack
            {
                public AudioSource Source;
                public AudioObject[] Audio;
            }

            private class AudioJob
            {
                public AudioAction Action;
                public AudioType Type;
                public bool Fade;
                public float FadeTime;
                public float DelayTime;

                public AudioJob(AudioAction action, AudioType type, bool fade, float delayTime, float fadeTime)
                {
                    Action = action;
                    Type = type;
                    Fade = fade;
                    FadeTime = fadeTime;
                    DelayTime = delayTime;
                }
            }

            private enum AudioAction
            {
                START,
                STOP,
                RESTART
            }

            #region Public Functions
            public void PlayAudio(AudioType type, bool fade = false, float fadeTime = 0.0f, float delay = 0.0f)
            {
                AddJob(new AudioJob(AudioAction.START, type, fade, fadeTime, delay));
            }

            public void StopAudio(AudioType type, bool fade = false, float fadeTime = 0.0f, float delay = 0.0f)
            {
                AddJob(new AudioJob(AudioAction.STOP, type, fade, fadeTime, delay));
            }

            public void RestartAudio(AudioType type, bool fade = false, float fadeTime = 0.0f, float delay = 0.0f)
            {
                AddJob(new AudioJob(AudioAction.RESTART, type, fade, fadeTime, delay));
            }
            #endregion

            #region Private Functions
            protected override void Awake()
            {
                base.Awake();
                Configure();
            }

            private void OnDisable()
            {
                Dispose();
            }

            private void Configure()
            {
                _audioTable = new Hashtable();
                _jobTable = new Hashtable();
                GenerateAudioTable();
            }

            private void Dispose()
            {
                foreach (DictionaryEntry entry in _jobTable)
                {
                    IEnumerator job = (IEnumerator)entry.Value;
                    StopCoroutine(job);
                }
            }

            private void GenerateAudioTable()
            {
                foreach (AudioTrack track in Tracks)
                {
                    foreach (AudioObject obj in track.Audio)
                    {
                        // do not duplicate keys
                        if (_audioTable.ContainsKey(obj.Type))
                        {
                            LogWarning("You are trying to register audio [" + obj.Type + "] that has already been registered.");
                        }
                        else
                        {
                            _audioTable.Add(obj.Type, track);
                            Log("Registering audio [" + obj.Type + "].");
                        }
                    }
                }
            }

            private IEnumerator RunAudioJob(AudioJob job)
            {
                yield return new WaitForSeconds(job.DelayTime);

                AudioTrack track = (AudioTrack)_audioTable[job.Type];
                track.Source.clip = GetAudioClipFromAudioTrack(job.Type, track);

                switch(job.Action)
                {
                    case AudioAction.START:
                        track.Source.Play();
                        break;

                        case AudioAction.STOP:
                        if (!job.Fade)
                        {
                            track.Source.Stop();
                        }
                        break;

                        case AudioAction.RESTART:
                        track.Source.Stop();
                        track.Source.Play();
                        break;
                }

                if(job.Fade)
                {
                    float initial = job.Action == AudioAction.START || job.Action == AudioAction.RESTART ? 0.0f : 1.0f;
                    float target = initial == 0 ? 1 : 0;
                    float timer = 0.0f;

                    while (timer <= job.FadeTime) 
                    {
                        track.Source.volume = Mathf.Lerp(initial, target, timer / job.FadeTime);
                        timer += Time.deltaTime;
                        yield return null;
                    }

                    if(job.Action == AudioAction.STOP)
                    {
                        track.Source.Stop();
                    }
                }

                _jobTable.Remove(job.Type);
                Log("Job Count " + _jobTable.Count);

                yield return null;
            }

            private void AddJob(AudioJob job)
            {
                // remove conflicting jobs
                RemoveConflictingJobs(job.Type);

                // start job
                IEnumerator jobRunner = RunAudioJob(job);
                _jobTable.Add(job.Type, jobRunner);
                StartCoroutine(jobRunner);
                Log("Starting job on [" + job.Type + "] with operation:" + job.Action);
            }

            private void RemoveJob(AudioType type)
            {
                if(!_jobTable.ContainsKey(type))
                {
                    LogWarning("Trying to stop a job [" + type + "] that is not running");
                    return;
                }

                IEnumerator runningJob = (IEnumerator)_jobTable[type];
                StopCoroutine(runningJob);
                _jobTable.Remove(type);
            }

            private void RemoveConflictingJobs(AudioType type)
            {
                if (_jobTable.ContainsKey(type))
                {
                    RemoveJob(type);
                }

                AudioType conflictAudio = AudioType.None;
                foreach (DictionaryEntry entry in _jobTable)
                {
                    AudioType audioType = (AudioType)entry.Key;
                    AudioTrack audioTrackInUse = (AudioTrack)_audioTable[audioType];
                    AudioTrack audioTrackNeeded = (AudioTrack)_audioTable[type];

                    if(audioTrackNeeded.Source == audioTrackInUse.Source)
                    {
                        // conflict
                        conflictAudio = audioType;
                    }
                }

                if(conflictAudio != AudioType.None)
                {
                    RemoveJob(conflictAudio);
                }
            }

            public AudioClip GetAudioClipFromAudioTrack(AudioType type, AudioTrack track)
            {
                foreach (AudioObject obj in track.Audio)
                {
                    if (obj.Type == type)
                    {
                        return obj.Clip;
                    }
                }
                return null;
            }

            private void Log(string msg)
            {
                if (!debug)
                {
                    return;
                }
                Debug.Log("[Audio Controller]: "+msg);
            }

            private void LogWarning(string msg)
            {
                if (!debug)
                {
                    return;
                }
                Debug.LogWarning("[Audio Controller]: " + msg);
            }
            #endregion
        }
    }
}