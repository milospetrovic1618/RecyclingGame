using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using TMPro;
using UnityEngine;
public static class Quiz
{
    public static Dictionary<string, (string[] answers, string correctAnswer, string explanation)> quizData =
    new Dictionary<string, (string[] answers, string correctAnswer, string explanation)>()
{
    {
        "How long can it take for a glass battle to break down in a landfill?",
        (
            new[] { "1 year", "100 years", "Over 1,000 years" },
            "Over 1,000 years",
            "Glass takes a really long time to break down — over a thousand years! That’s why recycling it is so important."
        )
    },
    {
        "What kind of recycling involves food and garden waste?",
        (
            new[] { "Sorting", "Composting", "Filtering" },
            "Composting",
            "Composting turns leftovers, fruit peels, and garden clippings into nutrient-rich soil."
        )
    },
    {
        "Which of these items is the best to put in a compost bin?",
        (
            new[] { "Banana peel", "Plastic spoon", "Glass jar" },
            "Banana peel",
            "Banana peels break down easily and help make healthy compost. The others belong in the trash or recycling."
        )
    },
    {
        "Which of these is made from trees?",
        (
            new[] { "Plastic bottle", "Cardboard box", "Glass cup" },
            "Cardboard box",
            "Cardboard is made from paper, which comes from trees. Recycling cardboard helps save forests!"
        )
    },
    {
        "What symbol shows something is recyclable?",
        (
            new[] { "Heart", "Triangle", "Star" },
            "Triangle",
            "The triangle with arrows is called the recycling symbol. If you see it, it usually means the item can be recycled."
        )
    },
    {
        "What does compost turn into?",
        (
            new[] { "Dirt", "Water", "Plastic" },
            "Dirt",
            "Compost becomes dark, crumbly soil that’s great for plants. It’s like nature’s way of recycling food scraps!"
        )
    },
    {
        "If you can’t recycle an item, what’s the best thing to do?",
        (
            new[] { "Throw it in the trash", "Reuse or repurpose it", "Bury it" },
            "Reuse or repurpose it",
            "Instead of throwing things away, reusing or repurposing them gives items a second life and reduces trash."
        )
    },
    {
        "True or False: Glass can be recycled forever without losing quality.",
        (
            new[] { "True", "False" },
            "True",
            "Glass doesn’t wear out — it can be melted and reused again and again without changing how strong or clear it is!"
        )
    },
    {
        "True or False: You can put food-covered containers directly into the recycling bin.",
        (
            new[] { "True", "False" },
            "False",
            "Food can spoil the recycling process. Containers need to be clean so they don’t contaminate other items."
        )
    },
    {
        "True or False: You should rinse out containers before putting them in the recycling bin.",
        (
            new[] { "True", "False" },
            "True",
            "A quick rinse helps keep the recycling clean and makes it easier to turn it into something new."
        )
    },
    {
        "True or False: You should flatten cardboard boxes before recycling them.",
        (
            new[] { "True", "False" },
            "True",
            "Flattening boxes saves space in the bin and makes it easier to collect and process the cardboard."
        )
    },
    {
        "True or False: Composting helps make healthy soil.",
        (
            new[] { "True", "False" },
            "True",
            "Composting turns food scraps and leaves into rich, healthy soil that helps plants grow!"
        )
    }
};
}
public class ButtonsGameplay : MonoBehaviour
{
    

    public static string[] ShuffleArray(string[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            string temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }
    public string chosenQuestionKey;
    public void ContinueTry()
    {

        SoundManager.Instance.Button();
        int randomIndex = UnityEngine.Random.Range(0, Quiz.quizData.Count);
        chosenQuestionKey = Quiz.quizData.Keys.ElementAt(randomIndex);

        //


        BootGameplay.Instance.PauseUI.SetActive(false);
        BootGameplay.Instance.GameOverUI.SetActive(false);
        BootGameplay.Instance.Quiz_Question.text = chosenQuestionKey;
        BootGameplay.Instance.scoreHolder.SetActive(false);
        Debug.Log(BootGameplay.Instance.scoreHolder.activeSelf.ToString()+" ");
        BootGameplay.Instance.Quiz.SetActive(true);
        Time.timeScale = 0f;
        if (Quiz.quizData[chosenQuestionKey].answers.Length == 3)
        {
            BootGameplay.Instance.Quiz1.SetActive(true);
            BootGameplay.Instance.Quiz2.SetActive(false);


            
            //assign shuffledArray, ovo nije potrebno jer moze i direktno random na text na buttonu ali ok je
            Quiz.quizData[chosenQuestionKey] = (ShuffleArray(Quiz.quizData[chosenQuestionKey].answers.ToArray()), Quiz.quizData[chosenQuestionKey].correctAnswer, Quiz.quizData[chosenQuestionKey].explanation);

            BootGameplay.Instance.Quiz1_Button1.text = Quiz.quizData[chosenQuestionKey].answers[0];
            BootGameplay.Instance.Quiz1_Button2.text = Quiz.quizData[chosenQuestionKey].answers[1];
            BootGameplay.Instance.Quiz1_Button3.text = Quiz.quizData[chosenQuestionKey].answers[2];
        }
        else//==2
        {
            BootGameplay.Instance.Quiz1.SetActive(false);
            BootGameplay.Instance.Quiz2.SetActive(true);

            //doesnt shuffle
            BootGameplay.Instance.Quiz2_Button1.text =  Quiz.quizData[chosenQuestionKey].answers[0];
            BootGameplay.Instance.Quiz2_Button2.text = Quiz.quizData[chosenQuestionKey].answers[1];
        }

        //Destroy(BootGameplay.Instance.ContinueButton.gameObject);
        Debug.Log(BootGameplay.Instance.scoreHolder.activeSelf.ToString() + " ");
    }
    public void QuizAnswer(TextMeshProUGUI TMP)
    {
        Debug.Log("QuizAnswer");
        SoundManager.Instance.Button();
        string answer = TMP.text;
        bool correct = Quiz.quizData[chosenQuestionKey].correctAnswer == answer;

        GameObject addQuizIndicator = new GameObject();
        QuizIndicator quizIndicator = addQuizIndicator.AddComponent<QuizIndicator>();
        Time.timeScale = 1f;//zbog animacije correct incorect quizIndicator
        TrashManager.Instance.blockSpawn = true;//zbog animacije correct incorect quizIndicator
        if (correct)
        {
            SoundManager.Instance.QuizCorrect();
            SaveSystem.Instance.Player.AddHashMapTrivia(chosenQuestionKey);

            quizIndicator.Initialize(true, TMP.rectTransform.position);

            StartCoroutine(WaitAndDo(1,() => { Continue(); TrashManager.Instance.blockSpawn = false; }));
            //1 sec wait
            //Continue();
        }
        else
        {
            SoundManager.Instance.QuizIncorrect();
            quizIndicator.Initialize(false, TMP.rectTransform.position);
            StartCoroutine(WaitAndDo(1, () => { GameOver(); TrashManager.Instance.blockSpawn = false; }));
            
        }

        if (Quiz.quizData[chosenQuestionKey].answers.Length == 3)
        {
            if (BootGameplay.Instance.Quiz1_Button1.text != TMP.text) BootGameplay.Instance.Quiz1_Image1.sprite = Resources.Load<Sprite>("ButtonGrayBackground");
            if (BootGameplay.Instance.Quiz1_Button2.text != TMP.text) BootGameplay.Instance.Quiz1_Image2.sprite = Resources.Load<Sprite>("ButtonGrayBackground");
            if (BootGameplay.Instance.Quiz1_Button3.text != TMP.text) BootGameplay.Instance.Quiz1_Image3.sprite = Resources.Load<Sprite>("ButtonGrayBackground");

            if (!correct)
            {
                if (BootGameplay.Instance.Quiz1_Button1.text == TMP.text) BootGameplay.Instance.Quiz1_Image1.sprite = Resources.Load<Sprite>("ButtonRedBackground");
                if (BootGameplay.Instance.Quiz1_Button2.text == TMP.text) BootGameplay.Instance.Quiz1_Image2.sprite = Resources.Load<Sprite>("ButtonRedBackground");
                if (BootGameplay.Instance.Quiz1_Button3.text == TMP.text) BootGameplay.Instance.Quiz1_Image3.sprite = Resources.Load<Sprite>("ButtonRedBackground");
            }
        }
        else//==2
        {
            if (BootGameplay.Instance.Quiz2_Button1.text != TMP.text) BootGameplay.Instance.Quiz2_Image1.sprite = Resources.Load<Sprite>("ButtonGrayBackground");
            if (BootGameplay.Instance.Quiz2_Button2.text != TMP.text) BootGameplay.Instance.Quiz2_Image2.sprite = Resources.Load<Sprite>("ButtonGrayBackground");

            if (!correct)
            {
                if (BootGameplay.Instance.Quiz2_Button1.text == TMP.text) BootGameplay.Instance.Quiz1_Image1.sprite = Resources.Load<Sprite>("ButtonRedBackground");
                if (BootGameplay.Instance.Quiz2_Button2.text == TMP.text) BootGameplay.Instance.Quiz1_Image2.sprite = Resources.Load<Sprite>("ButtonGreenBackground");
            }

        }
        Destroy(BootGameplay.Instance.ContinueButton); 
        BootGameplay.Instance.ContinueButtonImage.sprite = Resources.Load<Sprite>("ButtonGrayBackground");

        Debug.Log(correct);
    }
    public void GameOver()
    {
        TrashManager.Instance.GameOverUI();
    }
    private IEnumerator WaitAndDo(float duration, Action callback)
    {
        yield return new WaitForSeconds(duration);
        callback.Invoke();
    }
    public void Continue()
    {
        Debug.Log("Continue1");
        SoundManager.Instance.Button();
        Time.timeScale = 1f;
        BootGameplay.Instance.Quiz.SetActive(false);
        BootGameplay.Instance.PauseButton.SetActive(true);
        BootGameplay.Instance.scoreHolder.SetActive(true);

        if (GraduallyReturnSpawnRateCoroutine != null)
        {
            StopCoroutine(GraduallyReturnSpawnRateCoroutine);
        }
        GraduallyReturnSpawnRateCoroutine = StartCoroutine(GraduallyReturnSpawnRate());
    }
    Coroutine GraduallyReturnSpawnRateCoroutine;
    public IEnumerator GraduallyReturnSpawnRate()//USPORAVA BRZINU SPAWNOVANJA NA POCETKU
    {
    
        float maxTimeToReturnToNormal = 25f;//za ovoliko ide od maks do min intervala, ali ako nije min interval moze i brze
        float minInterval = TrashManager.Instance.minInterval;
        float maxInterval = TrashManager.Instance.maxInterval;
        float refreshInterval = minInterval - 0.05f;//malo manje nego min da se ne bi poklapalo
        float subtractEveryInterval = (maxInterval - minInterval) / (maxTimeToReturnToNormal / refreshInterval);
        float curRateBeforeGameOver = maxInterval;// TrashManager.Instance.actualSpawnInterval;
        Debug.Log($"kkkk  curRateBeforeGameOver {curRateBeforeGameOver} referenceSpawnInterval {TrashManager.Instance.referenceSpawnInterval} actualSpawnInterval {TrashManager.Instance.actualSpawnInterval} subtractEveryInterval {subtractEveryInterval}");
        while (TrashManager.Instance.referenceSpawnInterval < curRateBeforeGameOver)
        {
            Debug.Log($" curRateBeforeGameOver {curRateBeforeGameOver} referenceSpawnInterval {TrashManager.Instance.referenceSpawnInterval} actualSpawnInterval {TrashManager.Instance.actualSpawnInterval} subtractEveryInterval {subtractEveryInterval}");
            TrashManager.Instance.actualSpawnInterval = curRateBeforeGameOver;

            curRateBeforeGameOver -= subtractEveryInterval;
            yield return new WaitForSeconds(refreshInterval) ;
        }
        //na interval od min,  oduzimaj od max po malo sve dok ne bude manji od curRateBeforeGameOver, tad nemoj da postavljas nego prestani
        //na interval od min zato sto na toliko najbrze moze da se updatuje interval u trashManageru, pa nema potrebe u svakom frame-u
    }
    public void TogglePause()
    {
        SoundManager.Instance.Button();
        bool pause = !BootGameplay.Instance.PauseUI.activeSelf;
        if (!SaveSystem.Instance.Player.TutorialFinished)
        {
            TutorialTap.Instance?.gameObject?.SetActive(!pause);
            if (!pause)
            {
                TutorialTap.Instance?.Disable();
            }
        }
        BootGameplay.Instance.PauseUI.SetActive(pause);
        Time.timeScale = pause ? 0f : 1f;
    }
    public void Home()
    {
        SoundManager.Instance.Button();
        BootMain.Instance.LoadSceneFromBoot(Scenes.Menu);
    }
    public void Retry()
    {
        SoundManager.Instance.Button();
        BootMain.Instance.LoadSceneFromBoot(Scenes.Gameplay);//ovo je najbolje resenje jer treba i za svaku kantu da vracam da nema moc itd... n eisplati mi se druga opcija bez pokretanja scene
        //TrashManager.Instance.GameOverUIScreenActivate(false);
    }
    public void Options()
    {
        SoundManager.Instance.Button();
        BootMain.Instance.LoadOptions();

    }
}
