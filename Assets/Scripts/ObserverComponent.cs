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
                    sw.WriteLine(_stringLog + "2nd");
                }
                /*
                using(var stream = File.OpenWrite(path))
                {
                    
                    using (StreamWriter sw = new StreamWriter(path,true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine(_stringLog);
                    }
                    
                    
                    //using(var writer = new BinaryWriter(stream, Encoding.Default, false))
                    using (var writer = new StreamWriter(stream))
                    {
                        //writer.Write("/n" +_stringLog);
                        //writer.Seek(0, SeekOrigin.End);
                        writer.WriteLine(_stringLog);
                        writer.WriteLine(_stringLog + "2nd");
                    }
                }
                */
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            //Создание файла в режиме записи партии
            if (isRecorded == true) FileCreation();
            WriteToLog("test string1"); //тестирование записи в файл
            WriteToLog("test string2"); //тестирование записи в файл
        }
    }
}