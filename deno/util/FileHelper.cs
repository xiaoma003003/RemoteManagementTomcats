using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace deno
{
    class FileHelper
    {
        //从文件读取信息，每行为一个单位
        public List<string> getListFromFile(string path) {
            var file = File.Open(path, FileMode.Open);
            List<string> txt = new List<string>();
            using (var stream = new StreamReader(file))
            {
                while (!stream.EndOfStream)
                {
                    txt.Add(stream.ReadLine());
                }
            }
            return txt;
        }
        //覆盖文件数据
        public void writeFileFromList(List<string> txt, String path)
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
                
                try
                {
                    foreach (string str in txt)
                    {
                        sw.WriteLine(str);
                    }
                }
                finally
                {
                    if (sw != null) { sw.Close(); }
                }
            
        }
        //写日志文件
        public void writeLog(List<string> txt, String path) {
            FileStream fs = null;
            if (!File.Exists(path)) {
                FileInfo fi = new FileInfo(path);
                fs= fi.Create();
                fs.Close();
            }
            fs = new FileStream(path, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                foreach (string str in txt)
                {
                    sw.WriteLine(str);
                }
            }
            finally
            {
                if (sw != null) { sw.Close(); }
            }

        }
    }
 }
