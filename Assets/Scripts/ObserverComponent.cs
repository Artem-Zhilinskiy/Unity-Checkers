using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;

namespace Checkers
{

    public class ObserverComponent : MonoBehaviour
    {
        public bool isRecorded; //Ведётся ли запись партии
        public bool PlayMode; //Включить или выключить воспроизведение

        //Создание файла
        private void FileCreation()
        {
                var path = Environment.CurrentDirectory + "//CheckersLog.txt";
                var LogFile = File.Create(path);
                LogFile.Close();
        }


        // Start is called before the first frame update
        void Start()
        {
            //Создание файла в режиме записи партии
            if (isRecorded == true) FileCreation(); 
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}