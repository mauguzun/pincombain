using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartNewMakeAccount
{
    public class ImageRepository
    {
        public string DirectoryPath { get; private set; } = @"C:\Users\maugu\Desktop\girls\";

        private List<String> _db;

        public ImageRepository()
        {

        }
        public ImageRepository(string path)
        {
            this.DirectoryPath = path;
        }
        public int LoadImages()
        {
            _db = Directory.GetFiles(this.DirectoryPath).ToList();
            return _db.Count();
        }
        public string Random()
        {
            if (_db != null && _db.Count() > 0)
            {
                return _db[new Random().Next(0, _db.Count)];
            }

            return null;

        }
        public void Delete(string delete)
        {
            try
            {
                File.Delete(delete);
                this.LoadImages();
            }
            catch
            {

            }
        }

    }
}
