using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotBehavior : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI _text;
	[SerializeField] private AudioClip _voice1;
	[SerializeField] private AudioClip _voice2;

	private int _currentScore = 0;
	private string[] _introText = {"1Hey! You!",				//textgreen <color=#479F00> textbrown <color=#996633>
								   "2Bring me some <color=#996633>coffee<color=#479F00>!",
								   "1There's a few <color=#996633>coffee cups<color=#479F00> on the table to your left!",
								   "1Make a fist with both hands and jog to the table! Swing your fists up and down!",
								   "2Use the <color=#FF0000>grip buttons <color=#479F00>to grab the <color=#996633>cups <color=#479F00> with your hands!",
								   "1You can hold the <color=#996633>coffee cups <color=#479F00>in your <color=#0000BB>spooky blue pouches<color=#479F00> on your waist.",
								   "1Do you see that <color=#000000>black pipe <color=#479F00>in front of me?",
								   "1Use the <color=#FF0000>stick <color=#479F00FF>to teleport to it.",
								   "2Throw the cups in there!"
									};
	private string[] _firstWorldText = {"2yeeeeeeee.... that hit the spot...",
										"2As you can see, the screen shows your current <color=#996633>coffee points<color=#479F00>.",
										"1The nice cups give you <color=#0000FF>two points<color=#479F00>, the others give you <color=#0000FF>one point<color=#479F00>.",
										"1I will give you a keypad code that opens the way to the <color=#FFB600>desert town<color=#479F00>!",
										"2I'm doing this because I want you to bring me more <color=#996633>coffee<color=#479F00>!",
										"1Before I do this, let me tell you what you can do!",
										"1You see that <color=#00FF00>green pouch<color=#479F00> on your <color=#0000FF>waist<color=#479F00>?",
										"2That's your map, it'll show you where you are and in what direction you're looking at.",
										"1Your teleporter is pretty cool too!",
										"1If you rotate the <color=#FF0000>stick<color=#479F00>, you can change your orientation.",
										"1You see that <color=#595959>watch <color=#479F00>on your left wrist?",
										"1Tap the watchface!",
										"2Use the sliders to change the volume, belt height, and your speed.",
										"1You can also tap the buttons to change your orientation!",
										"2Pretty cool huh?",
										//"1If you forget how to do something, check out the panel to my right.",
										"2You'll be typing the code in the <color=#595959>keypad <color=#479F00>right in front of me!",
										"1Here's the code: <color=#0000FF>1234<color=#479F00>! Type the number on the keypad and jog through the portal!"
										};
	private string[] _secondWorldText = {//textgreen <color=#479F00> textbrown <color=#996633>
									"1Oh boy... <color=#996633>coffee <color=#479F00>is GREAT!",
									"1I have some bad news for you...",
									"1I actually don't know what the other codes are...",
									"2Don't look at me like that! I can still help!",
									"1I know how to get the code to unlock the <color=#00BB00>forest  <color=#479F00>portal!",
									"2The hint of for the code is this: The number of rocks behind the oasis, the number of big houses, the number of small houses, and the number of trees at the oasis!"
									};
	private string[] _lastWorldText = {
									"1The last world doesn't exist",
									"2Maybe I'll build something someday, maybe a <color=#996633>coffee world<color=#479F00>.",
									"1With <color=#996633>coffee lakes <color=#479F00>and <color=#996633>coffee trees <color=#479F00>and <color=#996633>coffee everywhere...",
									"1If you still want to open the door, I'll tell you the code hint: ",
									"In order, the number of tables in the forest, the number of spiky rocks inside a cave, the number of caves, the sum of the previous three numbers"
									};

	private string[] _memeString = { };
	private int _storedIndex = 0;

	private bool noMemes = true;
	private bool isMemeing = false;

	private bool isStart = true;
	private bool clearText = true;
	private bool playIntro = false;
	private bool firstWorld = false;
	private bool secondWorld = false;
	private bool lastWorld = false;
	private bool isDone = false;

	private bool got15 = false;
	private bool got10 = false;
	private bool got1 = false;

	private int curIndex = 0;
	private int curCharacter = 0;
	private float time = 0f;
	
	// Update is called once per frame
	void Update () {
		if (isStart) StartCoroutine(begin());
		if (clearText) {
			_text.text = "";
			clearText = false;
		}

		if (noMemes) {
			if (playIntro) speak(_introText);
			else if (firstWorld) speak(_firstWorldText);
			else if (secondWorld) speak(_secondWorldText);
			else if (lastWorld) speak(_lastWorldText);
		}
		else {
			if(isMemeing) speak(_memeString);
		}
	}

	private void speak(string[] dialogue) {
		time += Time.deltaTime;
		if (time >= 0.05f) {
			//Audio
			if (curCharacter == 0) {
				if (dialogue[curIndex][curCharacter] == '1')
					GetComponent<AudioSource>().clip = _voice1;
				else if (dialogue[curIndex][curCharacter] == '2')
					GetComponent<AudioSource>().clip = _voice2;
				GetComponent<AudioSource>().Play();
				curCharacter++;
			}

			time = 0;
			char nextChar = dialogue[curIndex][curCharacter];

			//Coloring
			if (nextChar == '<') {
				while (nextChar != '>') {
					_text.text += nextChar;
					curCharacter++;
					nextChar = dialogue[curIndex][curCharacter];
				}
				_text.text += nextChar;
				curCharacter++;
				nextChar = dialogue[curIndex][curCharacter];
			}
			
			_text.text += nextChar;
			curCharacter++;

			//End of string
			if (curCharacter >= dialogue[curIndex].Length) {
				curCharacter = 0;
				curIndex++;

				checkForEndOfString(dialogue.Length);
			}
		}
	}

	private void checkForEndOfString(int length) {
		if (isMemeing) {
			if (curIndex >= length)
				StartCoroutine(nextDialogue4());
		}
		else if (playIntro) {
			if (curIndex >= length)
				playIntro = false;
			else
				StartCoroutine(nextDialogue0());
		}
		else if (firstWorld) {
			if (curIndex >= length)
				firstWorld = false;
			else
				StartCoroutine(nextDialogue1());
		}
		else if (secondWorld) {
			if (curIndex >= length)
				secondWorld = false;
			else
				StartCoroutine(nextDialogue2());
		}
		else if (lastWorld) {
			if (curIndex >= length)
				lastWorld = false;
			else
				StartCoroutine(nextDialogue3());
		}
		
	}

	private IEnumerator begin() {
		isStart = false;
		yield return new WaitForSeconds(4f);
		playIntro = true;
	}

	private IEnumerator nextDialogue0() {
		playIntro = false;
		yield return new WaitForSeconds(2.0f);
		if (!firstWorld) {
			playIntro = true;
			clearText = true;
		}
	}

	private IEnumerator nextDialogue1() {
		firstWorld = false;
		yield return new WaitForSeconds(2.0f);
		if (!secondWorld) {
			firstWorld = true;
			clearText = true;
		}
	}

	private IEnumerator nextDialogue2() {
		secondWorld = false;
		yield return new WaitForSeconds(2.0f);
		if (!lastWorld) {
			secondWorld = true;
			clearText = true;
		}
	}

	private IEnumerator nextDialogue3() {
		lastWorld = false;
		yield return new WaitForSeconds(2.0f);
		lastWorld = true;
		clearText = true;
	}

	private IEnumerator nextDialogue4() {
		isMemeing = false;
		yield return new WaitForSeconds(2.0f);
		noMemes = true;
		clearText = true;
		curIndex = _storedIndex;
		curCharacter = 0;
	}

	public void updateScore(int i) {
		_currentScore = i;

		if (!got15 && _currentScore > 99) {
			got15 = true;
			secondWorld = false;
			lastWorld = true;
			clearText = true;
			curCharacter = 0;
			curIndex = 0;
		}
		else if (!got10 && _currentScore > 9) {
			got10 = true;
			firstWorld = false;
			secondWorld = true;
			clearText = true;
			curCharacter = 0;
			curIndex = 0;
		}
		else if (!got1 && _currentScore > 0) {
			got1 = true;
			playIntro = false;
			firstWorld = true;
			clearText = true;
			curCharacter = 0;
			curIndex = 0;
		}
	}

	public void codes(int code) {
		if (noMemes) {
			if (code == 8008 || code == 6900 || code == 0069) {
				_memeString = new string[] { "2Very funny..." };
				startMemeText();
			}
			else if (code == 0420 || code == 4200) {
				_memeString = new string[] { "1That's not coffee!" };
				startMemeText();
			}
			else if (code == 1337) {
				_memeString = new string[] { "1I haven't installed my 1337 module yet." };
				startMemeText();
			}
			else if (code == 2125) {
				_memeString = new string[] { "1I hope he enjoys this project." };
				startMemeText();
			}
			else if (code == 0210 || code == 8210) {
				_memeString = new string[] { "1That's the VR lab isn't it?" };
				startMemeText();
			}
			else if (code == 0066 || code == 6600) {
				_memeString = new string[] { "2<color=#RR0000>Excecute order 66<color=#996633>  " };
				startMemeText();
			}
			else if (code == 6666 || code == 0666 || code == 6660) {
				_memeString = new string[] { "1<color=#RR0000>spooky<color=#996633>  " };
				startMemeText();
			}
			else if (code == 7777) {
				_memeString = new string[] { "2You <color=#FFB600>win<color=#479F00>!" };
				startMemeText();
			}
			else if (code == 4300 || code == 0043) {
				_memeString = new string[] { "2<color=#9933FF>The answer<color=#479F00>  " };
				startMemeText();
			}
			else if (code == 3141 || code == 6283) {
				_memeString = new string[] { "2Pi or Tau? What do you think?" };
				startMemeText();
			}
		}
	}

	private void startMemeText() {
		_storedIndex = curIndex;
		noMemes = false;
		isMemeing = true;
		clearText = true;
		curCharacter = 0;
		curIndex = 0;
	}
}
