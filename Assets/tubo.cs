using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tubo : MonoBehaviour
{
    public List<GameObject> palline;
    public GameObject palla;
    public Transform posFine;
    Vector3 posIniz;
    public int sali;
    manager manager;
    public bool tuboRisposta;
    public Button rispondiBot;
    public string risposta;

    private void Awake()
    {
        if (transform.childCount > 0 && transform.GetChild(0).childCount > 0)
        {
            Color[] colori = new Color[4];
            colori[0] = Color.blue;
            colori[1] = Color.yellow;
            colori[2] = new Color(238f / 255f, 160f / 255f, 30f / 255f, 1f);
            colori[3] = new Color(149f / 255f, 2f / 255f, 112f / 255f, 1f);
            transform.GetChild(0).GetChild(0).GetComponent<Text>().color = colori[Random.Range(0, 4)];
        }
    }

    public void Start()
    {
        Application.targetFrameRate = 60;
        sali = 0;
        palline.Clear();
        if (transform.childCount != 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                palline.Add(transform.GetChild(i).gameObject);
            }
            palla = palline[palline.Count - 1];
            posIniz = palla.transform.position;
        }
        else
        {
            palla = null;
            posIniz = Vector3.zero;
        }
        manager = transform.parent.GetComponent<manager>();
    }

    void Update()
    {
        if (transform.childCount != 0)
        {
            if (sali == 1)
            {
                palla.transform.position = new Vector3(palla.transform.position.x, Mathf.Lerp(palla.transform.position.y, posFine.position.y, Time.deltaTime * 10), 0);
            }

            if (sali == 3)
            {
                palla.transform.position = new Vector3(palla.transform.position.x, Mathf.Lerp(palla.transform.position.y, posIniz.y, Time.deltaTime * 10), 0);
            }

            if (sali == 4)
            {
                palla.transform.position = Vector2.Lerp(palla.transform.position, new Vector2(transform.position.x, manager.posX[transform.childCount - 1].position.y), Time.deltaTime * 10);
            }
        }
        else
        {
            if (tuboRisposta)
            {
                if (rispondiBot.interactable)
                {
                    rispondiBot.interactable = false;
                }
            }
        }
    }

    public void Evidenzia()
    {
        manager.tocchi++;
        if (sali == 0)
        {
            int i = 2;
            sali = 1;
            if (manager.palla != palla)
            {
                if (manager.palla != null)
                {
                    if (palla != null)
                    {
                        if (manager.palla.GetComponent<Image>().sprite != palla.GetComponent<Image>().sprite)
                        {
                            manager.palla.transform.parent.GetComponent<tubo>().Evidenzia();
                        }
                        else
                        {
                            if (manager.palla.transform.childCount == 0)// && manager.palla.transform.GetChild(0).GetComponent<Text>() == null)
                            {
                                if (transform.childCount == 4)
                                {
                                    manager.palla.transform.parent.GetComponent<tubo>().Evidenzia();
                                }
                                else
                                {
                                    GameObject a = manager.palla.transform.parent.gameObject;
                                    manager.palla.transform.SetParent(transform);
                                    manager.palla.name = "pallina " + transform.childCount.ToString();
                                    Start();
                                    a.GetComponent<tubo>().Start();
                                    sali = 4;
                                    manager.mov = true;
                                    manager.Vinto();
                                }
                            }
                            else
                            {
                                manager.palla.transform.parent.GetComponent<tubo>().Evidenzia();
                            }
                        }
                    }
                    else
                    {
                        GameObject a = manager.palla.transform.parent.gameObject;
                        manager.palla.transform.SetParent(transform);
                        manager.palla.name = "pallina " + transform.childCount.ToString();
                        Start();
                        a.GetComponent<tubo>().Start();
                        sali = 4;
                        manager.mov = true;
                        if (tuboRisposta)
                        {
                            rispondiBot.interactable = true;
                        }
                    }
                }
                if (!manager.mov)
                {
                    manager.palla = palla;
                }
            }

            if (sali != 4)
            {
                StartCoroutine(Aspetta(0.5f, i));
            }
            else
            {
                StartCoroutine(Aspetta(0.5f, 0));
            }
        }
        else
        {
            StopAllCoroutines();
            sali = 3;
            manager.palla = null;
            StartCoroutine(Aspetta(0.5f, 0));
        }
    }

    IEnumerator Aspetta(float secondi, int i)
    {
        for (int o = 2; o < transform.parent.childCount; o++)
        {
            transform.parent.GetChild(o).GetComponent<Button>().interactable = false;
        }
        yield return new WaitForSeconds(secondi);
        sali = i;
        for (int o = 2; o < transform.parent.childCount; o++)
        {
            transform.parent.GetChild(o).GetComponent<Button>().interactable = true;
        }
        if (manager.mov)
        {
            GameObject a = manager.palla.transform.parent.gameObject;
            manager.mov = false;
            manager.palla = null;
            posIniz = palla.transform.position;
        }
    }

    public void Rispondi()
    {
        if (risposta == palla.GetComponentInChildren<Text>().text)
        {
            manager.vittoria = true;
            StartCoroutine(Risultato(new Color(0, 1, 0, GetComponent<Image>().color.a)));
        }
        else
        {
            manager.fine = true;
            StartCoroutine(Risultato(new Color(1, 0.5f, 0, GetComponent<Image>().color.a)));
        }
        rispondiBot.interactable = false;
    }

    IEnumerator Risultato(Color colore)
    {
        for (int o = 2; o < transform.parent.childCount; o++)
        {
            transform.parent.GetChild(o).GetComponent<Button>().interactable = false;
        }
        GetComponent<Animator>().SetTrigger("vai");
        manager.StopCoroutine(manager.Timer());
        yield return new WaitForSeconds(3.5f);
        GetComponent<Image>().color = colore;
        yield return new WaitForSeconds(1);
        manager.battito.SetTrigger("fine");
        yield return new WaitForSeconds(2);
        RivelaRisposta();
    }

    public void RivelaRisposta()
    {
        manager.completato.gameObject.SetActive(true);
        if (manager.vittoria)
        {
            manager.completato.transform.GetChild(0).Find("risposta").GetComponent<Text>().text = "Congratulations!!!\nYou gave the right answer.";
            manager.completato.transform.GetChild(0).Find("risposta").GetComponent<Text>().color = Color.green;
            if (PlayerPrefs.HasKey("livelli completati"))
            {
                if (manager.numLivello == PlayerPrefs.GetInt("livelli completati") + 1)
                {
                    int livelliComp = PlayerPrefs.GetInt("livelli completati") + 1;
                    PlayerPrefs.DeleteKey("livelli completati");
                    PlayerPrefs.SetInt("livelli completati", livelliComp);
                }
            }
            else
            {
                PlayerPrefs.SetInt("livelli completati", 1);
            }
        }
        else
        {
            manager.completato.transform.GetChild(0).Find("risposta").GetComponent<Text>().text = "Maybe Next Time.\nThe right answer was <color=#FF8000>(" + risposta + ")</color>.";
            manager.completato.transform.GetChild(0).Find("risposta").GetComponent<Text>().color = Color.black;
        }
    }
}
