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

        private string path = Environment.CurrentDirectory + "//CheckersLog.txt"; //Задание пути файла

        //Создание файла
        private void FileCreation()
        {
                //var path = Environment.CurrentDirectory + "//CheckersLog.txt";
                var LogFile = File.Create(path);
                LogFile.Close();
        }

        //Запись в файл текстовой строки
        public void WriteToLog (string _stringLog)
        {
            if (File.Exists(path))
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(_stringLog);
                }
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            //Создание файла в режиме записи партии
            if (isRecorded == true) FileCreation();
        }
    }
}