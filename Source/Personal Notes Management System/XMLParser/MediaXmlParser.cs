using Personal_Notes_Management_System.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Personal_Notes_Management_System.XMLParser
{
    public class MediaXmlParser
    {
        public static List<MediaElement> Parse(string xmlstring)
        {
            if (xmlstring == null || xmlstring.Length <= 0)
                return null;
            List<MediaElement> result = new List<MediaElement>();
            XDocument hehe = XDocument.Parse(xmlstring);
            foreach (XElement path in hehe.Root.Elements("path"))
            {
                result.Add(new MediaElement(path.Value));
            }
            return result;
        }

        public static string CreateXmlString(List<MediaElement> data, string rootname)
        {
            var root = new XElement(rootname);
            foreach (MediaElement melement in data)
            {
                XElement element = new XElement("path");
                element.SetValue(melement.FullPath);
                root.Add(element);
            }
            return root.ToString();
        }
    }

    public class MediaElement
    {
        private string _path;
        public string FullPath
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    NotifyPropertyChanging("FullPath");
                    _path = value;
                    NotifyPropertyChanged("FullPath");
                }
            }
        }

        private string _name;
        public string FileName
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    NotifyPropertyChanging("FileName");
                    _name = value;
                    NotifyPropertyChanged("FileName");
                }
            }
        }

        private ImageSource _image;
        public ImageSource ImageSource
        {
            get
            {
                return StorageManager.GetImage(FullPath);
            }
        }

        public MediaElement(string path)
        {
            FullPath = path;
            FileInfo file = new FileInfo(path);
            FileName = file.Name;
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion

    }
}
