using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public Image MainBg;
	public Image startGame;
	public Image backstory;
	public Image about;
	public Image exit;

	public Sprite startGame1;
	public Sprite startGame2;
	public Sprite backstory1;
	public Sprite backstory2;
	public Sprite about1;
	public Sprite about2;
	public Sprite exit1;
	public Sprite exit2;

	public GameObject uiClickSourceObj;
	private AudioSource uiClickSource;
	public GameObject bgMusicSourceObj;
	private AudioSource bgMusicSource;

	public AudioClip[] mAudioClips = new AudioClip[3];
	private float mainBgTime;

	public Image aboutTexts;
	public Image backAbout;
	public Text version;
	public Text musicNote1;
	public Text musicNote2;
	public Text musicNote3;
	public Sprite backAbout1;
	public Sprite backAbout2;

	private int textSize = (int)(Screen.height* 0.045f);
	private float aboutTextsMoveSpeed = Screen.height * 0.0005f;
	private bool showAboutTexts = false;
	private float aboutTextsMaxHeight = Screen.height * 1.4f;

	// Use this for initialization
	void Start () {
		Time.timeScale = 1.0f;

		uiClickSource = uiClickSourceObj.GetComponent<AudioSource> ();
		bgMusicSource = bgMusicSourceObj.GetComponent<AudioSource> ();
		mainBgTime = Time.time - 1f;

		MainBg.rectTransform.sizeDelta = new Vector2 (Screen.width, Screen.height);
		MainBg.rectTransform.position = new Vector3 (
			Screen.width * 0.5f, 
			Screen.height * 0.5f, 
			MainBg.rectTransform.position.z);

		backstory.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.5f, Screen.height * 0.15f);
		backstory.rectTransform.position = new Vector3 (
			Screen.width * 0.5f, 
			Screen.height * 0.5f + backstory.rectTransform.sizeDelta.y * 0.3f, 
			backstory.rectTransform.position.z);

		startGame.rectTransform.sizeDelta = new Vector2 (backstory.rectTransform.sizeDelta.x, backstory.rectTransform.sizeDelta.y);
		startGame.rectTransform.position = new Vector3 (
			Screen.width * 0.5f, 
			Screen.height * 0.5f + backstory.rectTransform.sizeDelta.y * 0.95f, 
			startGame.rectTransform.position.z);

		about.rectTransform.sizeDelta = new Vector2 (backstory.rectTransform.sizeDelta.x,  backstory.rectTransform.sizeDelta.y);
		about.rectTransform.position = new Vector3 (
			Screen.width * 0.5f, 
			Screen.height * 0.5f - backstory.rectTransform.sizeDelta.y * 0.3f, 
			about.rectTransform.position.z);

		exit.rectTransform.sizeDelta = new Vector2 (backstory.rectTransform.sizeDelta.x,  backstory.rectTransform.sizeDelta.y);
		exit.rectTransform.position = new Vector3 (
			Screen.width * 0.5f, 
			Screen.height * 0.5f - backstory.rectTransform.sizeDelta.y * 0.95f, 
			exit.rectTransform.position.z);

		InvokeRepeating ("musicManager", 0.5f, 0.5f);

		initAboutTexts ();

	}

	void initAboutTexts(){

		aboutTexts.rectTransform.sizeDelta = new Vector2 (Screen.width, Screen.height);
		aboutTexts.rectTransform.position = new Vector3 (
			Screen.width * 0.5f, 
			Screen.height * 0.5f, 
			aboutTexts.rectTransform.position.z);

		version.rectTransform.sizeDelta = new Vector2 (Screen.width, Screen.height * 0.3f);
		version.rectTransform.position = new Vector3 (
			Screen.width * 0.5f, 
			Screen.height * 0.5f, 
			version.rectTransform.position.z);
		version.fontSize = textSize;

		musicNote1.rectTransform.sizeDelta = new Vector2 (Screen.width, Screen.height * 0.3f);
		musicNote1.rectTransform.position = new Vector3 (
			Screen.width * 0.5f, 
			Screen.height * 0.5f - Screen.height * 0.3f, 
			musicNote1.rectTransform.position.z);
		musicNote1.fontSize = textSize;

		musicNote2.rectTransform.sizeDelta = new Vector2 (Screen.width, Screen.height * 0.3f);
		musicNote2.rectTransform.position = new Vector3 (
			Screen.width * 0.5f, 
			Screen.height * 0.5f - Screen.height * 0.6f, 
			musicNote2.rectTransform.position.z);
		musicNote2.fontSize = textSize;

		musicNote3.rectTransform.sizeDelta = new Vector2 (Screen.width, Screen.height * 0.3f);
		musicNote3.rectTransform.position = new Vector3 (
			Screen.width * 0.5f, 
			Screen.height * 0.5f - Screen.height * 0.9f, 
			musicNote3.rectTransform.position.z);
		musicNote3.fontSize = textSize;

		backAbout.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.11f, Screen.height * 0.095f);
		backAbout.rectTransform.position = new Vector3 (
			backAbout.rectTransform.sizeDelta.x*0.8f, 
			Screen.height - backAbout.rectTransform.sizeDelta.y*0.8f, 
			backAbout.rectTransform.position.z);
		aboutTexts.rectTransform.localScale = new Vector3 (0,0,0);
	}

	void Update () {
		if(showAboutTexts){
			moveAboutTexts ();
		}

	}

	void moveAboutTexts(){
		float tempY = version.rectTransform.position.y + aboutTextsMoveSpeed;
		if(tempY >= aboutTextsMaxHeight){
			tempY = -version.rectTransform.sizeDelta.y * 0.5f;
		}
		version.rectTransform.position = new Vector3 (
			version.rectTransform.position.x, 
			tempY, 
			version.rectTransform.position.z);

		tempY = getTempY (version.rectTransform.position.y, musicNote1.rectTransform.sizeDelta.y,
			musicNote1.rectTransform.position.y, aboutTextsMaxHeight, aboutTextsMoveSpeed);
		musicNote1.rectTransform.position = new Vector3 (
			musicNote1.rectTransform.position.x, 
			tempY, 
			musicNote1.rectTransform.position.z);

		tempY = getTempY (version.rectTransform.position.y, musicNote1.rectTransform.sizeDelta.y*2,
			musicNote2.rectTransform.position.y, aboutTextsMaxHeight, aboutTextsMoveSpeed);
		musicNote2.rectTransform.position = new Vector3 (
			musicNote2.rectTransform.position.x, 
			tempY, 
			musicNote2.rectTransform.position.z);

		tempY = getTempY (version.rectTransform.position.y, musicNote1.rectTransform.sizeDelta.y*3,
			musicNote3.rectTransform.position.y, aboutTextsMaxHeight, aboutTextsMoveSpeed);
		musicNote3.rectTransform.position = new Vector3 (
			musicNote3.rectTransform.position.x, 
			tempY, 
			musicNote3.rectTransform.position.z);
	}

	float getTempY(float target, float targetDs, float position, float maxHeight, float Movespeed){
		float tempY = target - targetDs;
		if(tempY < 0){
			tempY = position + Movespeed;
			if(tempY >= maxHeight ){
				tempY = target - targetDs;
			}
		}
		return tempY;
	}


	void musicManager(){
		if(showAboutTexts){
			playBgMusic(mAudioClips[3], true);
			return;
		}
		if((Time.time -  mainBgTime) > 1){
			int whichClip = Random.Range (1, 99)%3;
			playBgMusic (mAudioClips[whichClip], false);
			mainBgTime = Time.time;
		}
	}


	public void OnStartGameClick(){
		playUiClick ();
		SceneManager.LoadScene ("T4");
	}

	public void OnStartGameDown(){
		startGame.sprite = startGame2;
	}
		
	public void OnStartGameUp(){
		startGame.sprite = startGame1;
	}

	public void OnBackStoryClick(){
		playUiClick ();
	}

	public void OnBackStoryDown(){
		backstory.sprite = backstory2;
	}

	public void OnBackStoryUp(){
		backstory.sprite = backstory1;
	}

	public void OnAboutClick(){
		playUiClick ();
		showAboutTexts = true;
		aboutTexts.rectTransform.localScale = new Vector3 (1,1,1);
	}

	public void OnAboutDown(){
		about.sprite = about2;
	}

	public void OnAboutUp(){
		about.sprite = about1;
	}
		

	public void OnExitClick(){
		playUiClick ();
	}

	public void OnExitDown(){
		exit.sprite = exit2;
	}

	public void OnExitUp(){
		exit.sprite = exit1;
	}

	void playUiClick(){
		if(uiClickSource != null){
			uiClickSource.Play ();
		}
	}

	void stopPlayBgMusic(){
		if(bgMusicSource == null){
			return;
		}
		bgMusicSource.Pause();
	}

	void playBgMusic(AudioClip mAudioClip, bool isForced){
		if(bgMusicSource == null){
			return;
		}
		if (isForced) {
			if(bgMusicSource.clip != mAudioClip){
				bgMusicSource.clip = mAudioClip;
			}
			if(!bgMusicSource.isPlaying){
				bgMusicSource.Play ();	
			}
		} else {
			if(!bgMusicSource.isPlaying){
				bgMusicSource.clip = mAudioClip;
				bgMusicSource.Play ();	
			}
		}

	}

	public void OnBackAboutClick(){
		playUiClick ();
		showAboutTexts = false;
		aboutTexts.rectTransform.localScale = new Vector3 (0,0,0);
		stopPlayBgMusic ();
	}

	public void OnBackAboutDown(){
		backAbout.sprite = backAbout2;
	}

	public void OnBackAboutUp(){
		backAbout.sprite = backAbout1;
	}



}
