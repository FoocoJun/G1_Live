using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
	private AudioSource[] _audioSources = new AudioSource[(int)Define.ESound.Max];
	private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
	private GameObject _soundRoot = null;

	public void Init() {
		if (_soundRoot == null) {
			_soundRoot = GameObject.Find("@SoundRoot");

			if (_soundRoot == null) {
                // 늘 그렇듯 담아둘 루트 경로 게임오브젝트 생성 
				_soundRoot = new GameObject { name = "@SoundRoot" };
				UnityEngine.Object.DontDestroyOnLoad(_soundRoot);

                // Enum에 정의된 사운드 이름들 추출
				string[] soundTypeNames = System.Enum.GetNames(typeof(Define.ESound));
				for (int count = 0; count < soundTypeNames.Length - 1; count++) {
                    // 순서대로 사운드 이름대로 게임 오브젝트 생성
					GameObject go = new GameObject { name = soundTypeNames[count] };
                    // 사운드 게임 오브젝트에 오디오 컴포넌트 생성
					_audioSources[count] = go.AddComponent<AudioSource>();
                    // 게임오브젝트의 부모를 루트로 설정
					go.transform.parent = _soundRoot.transform;
				}

                // BGM 시작
				_audioSources[(int)Define.ESound.Bgm].loop = true;
			}
		}
	}

	public void Clear() {
		foreach (AudioSource audioSource in _audioSources) {
			audioSource.Stop();
        }

		_audioClips.Clear();
	}

	public void Play(Define.ESound type) {
		AudioSource audioSource = _audioSources[(int)type];
		audioSource.Play();
	}

	public void Play(Define.ESound type, string key, float pitch = 1.0f) {
        // 실행할 오디오소스 컴포넌트의 오디오 소스를 찾기
		AudioSource audioSource = _audioSources[(int)type];

        // BGM 일때는 무한 / 아닐때는 한 번
		if (type == Define.ESound.Bgm) {
			LoadAudioClip(key, (audioClip) => {
				if (audioSource.isPlaying)
					audioSource.Stop();

				audioSource.clip = audioClip;
				audioSource.Play();
			});
		} else {
			LoadAudioClip(key, (audioClip) => {
				audioSource.pitch = pitch;
				audioSource.PlayOneShot(audioClip);
			});
		}
	}

	public void Play(Define.ESound type, AudioClip audioClip, float pitch = 1.0f) {
		AudioSource audioSource = _audioSources[(int)type];

		if (type == Define.ESound.Bgm) {
			if (audioSource.isPlaying) {
				audioSource.Stop();
            }

			audioSource.clip = audioClip;
			audioSource.Play();
		} else {
			audioSource.pitch = pitch;
			audioSource.PlayOneShot(audioClip);
		}
	}

	public void Stop(Define.ESound type) {
		AudioSource audioSource = _audioSources[(int)type];
		audioSource.Stop();
	}

    // key로 오디오 소스 불러온 후 콜백 실행하기.
	private void LoadAudioClip(string key, Action<AudioClip> callback) {
        // 빈 오디오 클립(리소스)
		AudioClip audioClip = null;
        // 혹 미리 불러둔 오디오클립들 중에 있으면 오디오클립에 할당 후 콜백 실행
		if (_audioClips.TryGetValue(key, out audioClip)) {
			callback?.Invoke(audioClip);
			return;
		}

        // 없으면 리소스 메니저로 로드하기
		audioClip = Managers.Resource.Load<AudioClip>(key);

        // 불러온 시점에 오디오 클립들에 불러온 오디오 클립(리소스) 없으면 담기
		if (_audioClips.ContainsKey(key) == false) {
			_audioClips.Add(key, audioClip);
        }

        // 콜백 실행
		callback?.Invoke(audioClip);
	}
}
