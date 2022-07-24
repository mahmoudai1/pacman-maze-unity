using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using System.IO;
using System;
using UnityEngine.UI;

public class HandDetector : MonoBehaviour
{
    // Start is called before the first frame update
    WebCamTexture _webCamTexture;
    public Camera cam;
    public Text text;
    CascadeClassifier cascade1;
    OpenCvSharp.Rect hand1;

    bool fistExists = false;

    public Transform heroObj;
    public GameObject maze;

    float timeLeft = 10.0f;
    bool gameEnded = false;

    int moveX = 0;
    int moveY = 0;

    void Start()
    {
        text.gameObject.SetActive(false);

        WebCamDevice[] devices = WebCamTexture.devices;
        _webCamTexture = new WebCamTexture(devices[0].name);
        _webCamTexture.requestedFPS = 60;
        _webCamTexture.Play();

        cascade1 = new CascadeClassifier("Assets/fist.xml");
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        //Debug.Log(timeLeft);
        if(timeLeft > 0.0)
        {
            if(cam.transform.position.z < -2.3f)
            {
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z + 0.03f);
            }
        }
        //GetComponent<Renderer>().material.mainTexture = _webCamTexture;
        Mat frame = OpenCvSharp.Unity.TextureToMat(_webCamTexture);

        findNewHand(frame);
        display(frame);

        if (!gameEnded)
        {
            if (fistExists)
            {
                moveTheHeroHorizontal();
                moveTheHeroVertical();
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, moveY);
                GetComponent<Rigidbody2D>().velocity = new Vector2(moveX, 0);
            }
        }

        if (gameEnded)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -12.0f);
            text.gameObject.SetActive(true);
            maze.transform.position = new Vector3(-0.01f, -0.01f, 0.0f);
        }
        
        
         //heroObj.transform.eulerAngles = new Vector3(0, 0, 90);
    }

    void findNewHand(Mat frame)
    {
        var handc1 = cascade1.DetectMultiScale(frame, 1.1, 3, HaarDetectionType.ScaleImage, new Size(100, 100));

        if (handc1.Length >= 1)
        {
            //Debug.Log(handc1[0].Location);
            hand1 = handc1[0];
            fistExists = true;
        }
        else
        {
            fistExists = false;
            moveX = 0;
            moveY = 0;
            //GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }

    void display(Mat frame)
    {
        if(hand1 != null && fistExists)
        {
            frame.Rectangle(hand1, new Scalar(0, 255, 0), 2);
        }

        Texture newTexture = OpenCvSharp.Unity.MatToTexture(frame);
        GetComponent<Renderer>().material.mainTexture = newTexture;
    }

    void moveTheHeroHorizontal()
    {
        if (fistExists && !gameEnded)
        {
            //Debug.Log(hand1.Location.X);

            if (hand1.Location.X < 225)
            {
                heroObj.transform.localScale = new Vector3(0.07f, 0.07f, 1.0f);

                if (hand1.Location.X > 175)
                {
                    moveX = -1;
                }
                else if (hand1.Location.X > 100)
                {
                    moveX = -2;
                }
                else if (hand1.Location.X > 1)
                {
                    moveX = -3;
                }
                else
                {
                    moveX = 0;
                }
                GetComponent<Rigidbody2D>().velocity = new Vector2(moveX, moveY);
            }
            else
            {
                heroObj.transform.localScale = new Vector3(-0.07f, 0.07f, 1.0f);

                if (hand1.Location.X < 275)
                {
                    moveX = 1;
                }
                else if (hand1.Location.X < 350)
                {
                    moveX = 2;
                }
                else if (hand1.Location.X < 450)
                {
                    moveX = 3;
                }
                else
                {
                    moveX = 0;
                }
            }
            GetComponent<Rigidbody2D>().velocity = new Vector2(moveX, moveY);
        }

    }

    void moveTheHeroVertical()
    {
        if (fistExists && !gameEnded)
        {
            //Debug.Log(hand1.Location.Y);

            if (hand1.Location.Y < 125)
            {
                heroObj.transform.localScale = new Vector3(0.07f, 0.07f, 1.0f);

                if (hand1.Location.Y > 100)
                {
                    moveY = -1;
                }
                else if (hand1.Location.Y > 50)
                {
                    moveY = -2;
                }
                else if (hand1.Location.Y > 1)
                {
                    moveY = -3;
                }
                else
                {
                    moveY = 0;
                }
            }
            else
            {
                heroObj.transform.localScale = new Vector3(-0.07f, 0.07f, 1.0f);

                if (hand1.Location.Y < 150)
                {
                    moveY = 1;
                }
                else if (hand1.Location.Y < 200)
                {
                    moveY = 2;
                }
                else if (hand1.Location.Y < 250)
                {
                    moveY = 3;
                }
                else
                {
                    moveY = 0;
                }
            }
            GetComponent<Rigidbody2D>().velocity = new Vector2(moveX, moveY);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "pacman" && !gameEnded)
        {
            gameEnded = true;
        }
    }
}
